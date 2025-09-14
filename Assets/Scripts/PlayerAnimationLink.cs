using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("TP3/Player Animation Link")]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerAnimationLink : MonoBehaviour
{
    [Header("Ground Check (same as PlayerController)")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundMask = ~0;

    [Header("Params")]
    [SerializeField] private string speedParam = "Speed";
    [SerializeField] private string groundedParam = "IsGrounded";
    [SerializeField] private string attackTrigger = "Attack";
    [SerializeField] private KeyCode attackKey = KeyCode.E;

    private Animator anim;
    private Rigidbody rb;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Speed (planar)
        Vector3 v = rb.linearVelocity; v.y = 0f;
        anim.SetFloat(speedParam, v.magnitude);

        // Grounded
        bool grounded = groundCheck && Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask, QueryTriggerInteraction.Ignore);
        anim.SetBool(groundedParam, grounded);

        // Attack
        if (Input.GetKeyDown(attackKey))
            anim.SetTrigger(attackTrigger);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
