using UnityEngine;

public class CameraSequenceController : MonoBehaviour
{
    [Header("Refs")]
    public BezierCameraFly fly;        // sur CameraRig
    public Transform switchTarget;     // généralement: Player (ou un Empty "CameraAnchor" sur le Player)
    public FollowTarget follow;        // sur CameraRig (désactivé au départ)

    [Header("Conditions de bascule")]
    public float switchDistance = 3f;  // distance CameraRig → joueur pour basculer
    public float maxFlyTime = 12f;     // garde-fou (en secondes)

    [Header("Composants à (dé)activer")]
    public Behaviour[] disableDuringFly; // ex: scripts de camera perso / Cinemachine Brain
    public Behaviour[] enableAfter;      // ex: scripts de camera perso / Cinemachine Brain

    float timer;
    bool switched;

    void Reset()
    {
        fly = GetComponent<BezierCameraFly>();
        follow = GetComponent<FollowTarget>();
    }

    void OnEnable()
    {
        // intro: activer fly, désactiver follow
        if (fly) fly.enabled = true;
        if (follow) follow.enabled = false;

        // couper les autres contrôleurs de cam pendant le fly
        foreach (var b in disableDuringFly) if (b) b.enabled = false;
        foreach (var b in enableAfter) if (b) b.enabled = false;

        timer = 0f;
        switched = false;
    }

    void Update()
    {
        if (switched || !switchTarget) return;

        timer += Time.deltaTime;

        bool closeEnough = Vector3.Distance(transform.position, switchTarget.position) <= switchDistance;
        bool timeout = timer >= maxFlyTime;

        if (closeEnough || timeout)
            SwitchToFollow();
    }

    void SwitchToFollow()
    {
        switched = true;

        // éteindre le vol sur courbe
        if (fly) fly.enabled = false;

        // brancher le follow
        if (follow)
        {
            follow.target = switchTarget;
            follow.enabled = true;
        }

        // remettre les autres contrôleurs (ex: Cinemachine Brain, scripts perso)
        foreach (var b in enableAfter) if (b) b.enabled = true;
    }
}
