using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("TP3/Monster Aggro")]
[RequireComponent(typeof(Animator))]
public class MonsterAggro : MonoBehaviour
{
    [Header("Aggro Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField, Min(0f)] private float attackRange = 2f;
    [SerializeField, Min(0f)] private float attackCooldown = 1.2f;
    [SerializeField, Min(0f)] private float moveSpeed = 0f; // 0 = statique (pas de poursuite)

    private Animator anim;
    private Stats selfStats;
    private Stats targetStats;
    private Transform target;
    private float nextAttackTime;

    void Awake()
    {
        anim = GetComponent<Animator>();
        selfStats = GetComponent<Stats>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            target = other.transform;
            targetStats = other.GetComponent<Stats>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform == target)
        {
            target = null;
            targetStats = null;
        }
    }

    void Update()
    {
        if (!target) return;

        // Look at player (plan)
        Vector3 to = target.position - transform.position; to.y = 0f;
        if (to.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(to.normalized, Vector3.up), 8f * Time.deltaTime);

        // Optional: move towards
        if (moveSpeed > 0f && to.magnitude > attackRange * 0.9f)
            transform.position += to.normalized * moveSpeed * Time.deltaTime;

        // Attack
        if (Time.time >= nextAttackTime && to.magnitude <= attackRange)
        {
            nextAttackTime = Time.time + attackCooldown;
            if (anim) anim.SetTrigger("Attack");
            if (targetStats && selfStats) targetStats.TakeDamage(selfStats.AttackPower);
        }
    }
}
