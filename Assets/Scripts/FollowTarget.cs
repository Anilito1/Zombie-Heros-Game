using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [Header("Cible")]
    public Transform target;                        // Player ou un Empty "CameraAnchor" sur le player
    public Vector3 targetOffset = new Vector3(0f, 1.7f, 0f);

    [Header("Distance / Zoom")]
    public float distance = 3f;
    public float minDistance = 1.4f;
    public float maxDistance = 5.5f;
    public float zoomStep = 2f;                     // sensibilité molette

    [Header("Souris (doux, pas trop rapide)")]
    public float mouseSensitivityX = 90f;           // degrés/seconde (horizontal)
    public float mouseSensitivityY = 80f;           // degrés/seconde (vertical)
    [Range(0f, 30f)] public float smoothing = 12f;  // 0 = brut, 12 = doux
    public float minPitch = -35f;                   // limite regard bas
    public float maxPitch = 70f;                    // limite regard haut
    public bool invertY = false;
    public bool requireRightMouse = false;          // ne tourne que clic droit enfoncé
    public bool lockCursor = true;

    [Header("Collision caméra")]
    public LayerMask collisionMask;                 // mets Ground + Environment
    public float sphereRadius = 0.25f;

    float yaw, pitch, desiredDistance;

    void OnEnable()
    {
        desiredDistance = Mathf.Clamp(distance, minDistance, maxDistance);

        // initialise yaw/pitch depuis la rotation actuelle
        var e = transform.rotation.eulerAngles;
        yaw = e.y;
        pitch = NormalizePitch(e.x);

        if (lockCursor) { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
    }

    void OnDisable()
    {
        if (lockCursor) { Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
    }

    void Update()
    {
        if (!target) return;

        // Rotation à la souris
        bool rotate = !requireRightMouse || Input.GetMouseButton(1);
        if (rotate)
        {
            float mx = Input.GetAxis("Mouse X");
            float my = Input.GetAxis("Mouse Y");
            yaw   += mx * mouseSensitivityX * Time.deltaTime;
            pitch += (invertY ? 1f : -1f) * my * mouseSensitivityY * Time.deltaTime;
            pitch  = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        // Zoom molette
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.0001f)
            desiredDistance = Mathf.Clamp(desiredDistance - scroll * zoomStep, minDistance, maxDistance);
    }

    void LateUpdate()
    {
        if (!target) return;

        Quaternion wantedRot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 targetCenter = target.position + target.TransformVector(targetOffset);
        Vector3 wantedPos = targetCenter - wantedRot * Vector3.forward * desiredDistance;

        // Collision (évite murs/arbres)
        if (collisionMask.value != 0)
        {
            Vector3 dir = (wantedPos - targetCenter).normalized;
            float maxDist = Vector3.Distance(targetCenter, wantedPos);
            if (Physics.SphereCast(targetCenter, sphereRadius, dir, out var hit, maxDist, collisionMask, QueryTriggerInteraction.Ignore))
            {
                float d = Mathf.Max(hit.distance - 0.05f, minDistance * 0.3f);
                wantedPos = targetCenter + dir * d;
            }
        }

        // Lissage (exponentiel, très doux)
        float t = 1f - Mathf.Exp(-smoothing * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, wantedPos, t);
        transform.rotation = Quaternion.Slerp(transform.rotation, wantedRot, t);
    }

    static float NormalizePitch(float x)
    {
        x = (x > 180f) ? x - 360f : x; // 350° -> -10°
        return x;
    }
}
