using UnityEngine;

[ExecuteAlways]
[DisallowMultipleComponent]
[RequireComponent(typeof(LineRenderer))]
public class BezierCurve : MonoBehaviour
{
    public enum Degree { Quadratic, Cubic }
    public Degree degree = Degree.Cubic;

    [Header("Points de contrôle")]
    public Transform p0, p1, p2, p3;   // p3 utilisé seulement en cubique

    [Header("Affichage")]
    [Range(2, 200)] public int segments = 120;
    public float width = 0.04f;

    LineRenderer lr;

    void Reset()
    {
        lr = GetComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.alignment = LineAlignment.View;
        lr.widthMultiplier = width;
        lr.positionCount = segments + 1;
    }

    void OnValidate()
    {
        if (!lr) lr = GetComponent<LineRenderer>();
        segments = Mathf.Max(2, segments);
        lr.widthMultiplier = width;
        UpdateCurve();
    }

    void Update() => UpdateCurve();

    void UpdateCurve()
    {
        if (!lr || !p0 || !p1 || !p2) return;
        lr.positionCount = segments + 1;
        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            Vector3 pos = (degree == Degree.Quadratic)
                ? EvalQuadratic(p0.position, p1.position, p2.position, t)
                : EvalCubic   (p0.position, p1.position, p2.position, p3 ? p3.position : p2.position, t);
            lr.SetPosition(i, pos);
        }
    }

    public static Vector3 EvalQuadratic(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        float u = 1f - t;
        return u*u*a + 2f*u*t*b + t*t*c;
    }

    public static Vector3 EvalCubic(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        float u = 1f - t;
        return u*u*u*a + 3f*u*u*t*b + 3f*u*t*t*c + t*t*t*d;
    }
}
