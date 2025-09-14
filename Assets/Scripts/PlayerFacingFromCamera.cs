using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("TP3/Player Facing From Camera (RMB)")]
public class PlayerFacingFromCamera : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField, Min(0f)] private float rotateSpeed = 20f;
    [SerializeField] private int mouseButton = 1; // 1 = RMB

    void Update()
    {
        if (!cameraTransform) return;
        if (Input.GetMouseButton(mouseButton))
        {
            Vector3 fwd = cameraTransform.forward;
            fwd.y = 0f;
            if (fwd.sqrMagnitude < 0.0001f) return;
            Quaternion target = Quaternion.LookRotation(fwd.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, rotateSpeed * Time.deltaTime);
        }
    }
}
