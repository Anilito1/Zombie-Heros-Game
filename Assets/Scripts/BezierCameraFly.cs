using UnityEngine;

public class BezierCameraFly : MonoBehaviour
{
    [Header("Chemin")]
    public BezierCurve curve;
    [Tooltip("m/s le long de la courbe")]
    public float speed = 20f;
    public bool loop = true;

    [Header("Orientation")]
    [Range(0f, 0.2f)] public float lookAhead = 0.03f;
    public bool alignToTangent = true;   // sinon, mettez un LookAt
    public Transform lookAt;             // optionnel

    [Header("Sol (éviter de percuter)")]
    public float groundOffset = 3f;
    public LayerMask groundMask = ~0;    // mettez 'Ground' pour votre Terrain

    const int LUT_RES = 300;
    float[] arcT = new float[LUT_RES + 1];
    float[] arcLen = new float[LUT_RES + 1];
    float totalLen, travelled;

    void OnEnable()   { if (curve) RebuildLUT(); }
    void OnValidate() { if (curve) RebuildLUT(); }

    void Update()
    {
        if (!curve || totalLen <= 0f) return;

        travelled += speed * Time.deltaTime;
        travelled = loop ? Mathf.Repeat(travelled, totalLen) : Mathf.Min(travelled, totalLen);

        float t = GetTForDistance(travelled);
        Vector3 pos = Eval(t);

        // garder une hauteur mini au-dessus du terrain
        if (Physics.Raycast(pos + Vector3.up * 200f, Vector3.down, out var hit, 400f, groundMask))
            pos.y = Mathf.Max(pos.y, hit.point.y + groundOffset);

        transform.position = pos;

        if (alignToTangent)
        {
            float ta = Mathf.Clamp01(t + lookAhead);
            Vector3 fwd = (Eval(ta) - pos).normalized;
            if (fwd.sqrMagnitude > 1e-6f)
                transform.rotation = Quaternion.LookRotation(fwd, Vector3.up);
        }
        else if (lookAt)
        {
            transform.LookAt(lookAt.position, Vector3.up);
        }
    }

    Vector3 Eval(float t)
    {
        return (curve.degree == BezierCurve.Degree.Quadratic)
            ? BezierCurve.EvalQuadratic(curve.p0.position, curve.p1.position, curve.p2.position, t)
            : BezierCurve.EvalCubic   (curve.p0.position, curve.p1.position, curve.p2.position, curve.p3.position, t);
    }

    // === LUT de longueur d'arc (vitesse constante) ===
    void RebuildLUT()
    {
        if (!curve || !curve.p0 || !curve.p1 || !curve.p2) return;

        totalLen = 0f;
        Vector3 prev = Eval(0f);
        arcT[0] = 0f; arcLen[0] = 0f;

        for (int i = 1; i <= LUT_RES; i++)
        {
            float t = i / (float)LUT_RES;
            Vector3 p = Eval(t);
            totalLen += Vector3.Distance(prev, p);
            arcT[i] = t;
            arcLen[i] = totalLen;
            prev = p;
        }
        travelled = 0f;
    }

    float GetTForDistance(float d)
    {
        if (d <= 0f) return 0f;
        if (d >= totalLen) return 1f;
        int lo = 0, hi = LUT_RES;
        while (lo < hi)
        {
            int mid = (lo + hi) / 2;
            if (arcLen[mid] < d) lo = mid + 1; else hi = mid;
        }
        float t1 = arcT[lo - 1], t2 = arcT[lo];
        float l1 = arcLen[lo - 1], l2 = arcLen[lo];
        float f = (d - l1) / Mathf.Max(1e-5f, (l2 - l1));
        return Mathf.Lerp(t1, t2, f);
    }
}
