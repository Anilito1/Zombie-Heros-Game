using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("TP3/Player Controller (Rigidbody)")]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField, Min(0f)] private float moveSpeed = 6f;
    [SerializeField, Min(0f)] private float turnSpeed = 12f;

    [Header("Jump")]
    [SerializeField, Min(0f)] private float jumpForce = 7f;
    [SerializeField] private Transform groundCheck;
    [SerializeField, Min(0f)] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundMask = ~0;

    [Header("Camera Reference")]
    [SerializeField] private Transform cameraTransform;

    public bool IsGrounded { get; private set; }
    public float PlanarSpeed { get; private set; }

    private Rigidbody rb;
    private Vector3 inputDir;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // sécurité supplémentaire
    }

    void Update()
    {
        // Inputs
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        inputDir = new Vector3(h, 0f, v).normalized;

        // Ground check
        if (groundCheck != null)
            IsGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask, QueryTriggerInteraction.Ignore);

        // Jump
        if (Input.GetButtonDown("Jump") && IsGrounded)
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        // Face movement direction if moving
        if (inputDir.sqrMagnitude > 0.001f)
        {
            Vector3 camFwd = cameraTransform ? Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized : Vector3.forward;
            Vector3 camRight = cameraTransform ? cameraTransform.right : Vector3.right;
            camRight.y = 0f; camRight.Normalize();

            Vector3 moveDir = camFwd * inputDir.z + camRight * inputDir.x;
            if (moveDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
            }
        }

        // Planar speed for animations
        Vector3 planarVel = rb.linearVelocity; planarVel.y = 0;
        PlanarSpeed = planarVel.magnitude;
    }

    void FixedUpdate()
    {
        Vector3 camFwd = cameraTransform ? Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized : Vector3.forward;
        Vector3 camRight = cameraTransform ? cameraTransform.right : Vector3.right;
        camRight.y = 0f; camRight.Normalize();

        Vector3 moveDir = camFwd * inputDir.z + camRight * inputDir.x;
        Vector3 targetVel = new Vector3(moveDir.x * moveSpeed, rb.linearVelocity.y, moveDir.z * moveSpeed);
        rb.linearVelocity = targetVel;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
