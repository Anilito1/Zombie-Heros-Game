using UnityEngine;
using System.Collections.Generic;

[DisallowMultipleComponent]
[AddComponentMenu("TP3/Bezier Drawer (Quad/Cubic)")]
[RequireComponent(typeof(LineRenderer))]
public class BezierDrawer : MonoBehaviour
{
    [Header("Control Points")]
    public Transform P0;
    public Transform P1;
    public Transform P2;
    public Transform P3; // optional for cubic

    [Header("Settings")]
    public bool useCubic = false;
    [Min(2)] public int resolution = 100;

    private LineRenderer lr;
    private readonly List<Vector3> pts = new List<Vector3>(2048);

    void Awake() => lr = GetComponent<LineRenderer>();
    void OnValidate() { lr = GetComponent<LineRenderer>(); UpdateCurve(); }
    void Update() => UpdateCurve();

    public void UpdateCurve()
    {
        if (!lr || !P0 || !P1 || !P2) return;
        pts.Clear();

        for (int i = 0; i < resolution; i++)
        {
            float t = i / (resolution - 1f);
            Vector3 p = useCubic && P3
                ? Cubic(P0.position, P1.position, P2.position, P3.position, t)
                : Quadratic(P0.position, P1.position, P2.position, t);
            pts.Add(p);
        }
        lr.positionCount = pts.Count;
        lr.SetPositions(pts.ToArray());
    }

    public Vector3[] GetPoints()
    {
        if (lr == null) return new Vector3[0];
        Vector3[] a = new Vector3[lr.positionCount];
        lr.GetPositions(a);
        return a;
    }

    static Vector3 Quadratic(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1f - t;
        return u * u * p0 + 2f * u * t * p1 + t * t * p2;
    }

    static Vector3 Cubic(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u = 1f - t;
        return u * u * u * p0 + 3f * u * u * t * p1 + 3f * u * t * t * p2 + t * t * t * p3;
    }
}
