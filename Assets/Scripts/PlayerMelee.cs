using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
public class PlayerMelee : MonoBehaviour
{
    [Header("Hit detection (sphere in front of player)")]
    public Transform hitOrigin;          // Laisse vide -> prendra ce GameObject
    [Min(0f)] public float range = 1.6f; // distance centrée devant
    [Min(0f)] public float radius = 0.6f;// épaisseur de la sphère
    public LayerMask damageMask = ~0;    // que peut-on toucher ?
    public bool useTagFilter = true;
    public string targetTag = "Enemy";   // on frappe les objets taggés Enemy

    private Stats selfStats;
    private Transform _origin;

    void Awake()
    {
        selfStats = GetComponent<Stats>();
        _origin = hitOrigin ? hitOrigin : transform;
    }

    // --- Appelée par un Animation Event sur Attack01 ---
    public void OnMeleeHit()
{
    Vector3 center = _origin.position + _origin.forward * (range * 0.6f) + Vector3.up * 0.9f;
    var hits = Physics.OverlapSphere(center, radius, damageMask, QueryTriggerInteraction.Ignore);

    foreach (var h in hits)
    {
        if (h.transform.root == transform) continue;
        var st = h.GetComponentInParent<Stats>();
        if (!st || st == selfStats) continue;
        if (useTagFilter && !h.transform.root.CompareTag(targetTag)) continue;

        // >>> convertir en int pour correspondre à Stats.TakeDamage(int)
        int dmg = selfStats ? Mathf.RoundToInt(selfStats.AttackPower) : 10;
        st.TakeDamage(dmg);
    }
}


    void OnDrawGizmosSelected()
    {
        Transform o = hitOrigin ? hitOrigin : transform;
        Vector3 center = o.position + o.forward * (range * 0.6f) + Vector3.up * 0.9f;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, radius);
    }
}
