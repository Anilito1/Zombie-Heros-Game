using UnityEngine;
using System.Collections.Generic;

[DisallowMultipleComponent]
[AddComponentMenu("TP3/Bezier Recursive (De Casteljau)")]
[RequireComponent(typeof(LineRenderer))]
public class BezierRecursive : MonoBehaviour
{
    [SerializeField] private Transform controlParent;
    [SerializeField, Min(2)] private int resolution = 100;

    private LineRenderer lr;

    void Awake() => lr = GetComponent<LineRenderer>();
    void OnValidate() { lr = GetComponent<LineRenderer>(); UpdateCurve(); }
    void Update() => UpdateCurve();

    void UpdateCurve()
    {
        if (!lr || !controlParent) return;
        var cps = GetControlPoints();
        if (cps.Count < 2) return;

        Vector3[] points = new Vector3[resolution];
        for (int i = 0; i < resolution; i++)
        {
            float t = i / (resolution - 1f);
            points[i] = DeCasteljau(cps, t);
        }
        lr.positionCount = points.Length;
        lr.SetPositions(points);
    }

    List<Vector3> GetControlPoints()
    {
        var list = new List<Vector3>();
        foreach (Transform c in controlParent) list.Add(c.position);
        return list;
    }

    static Vector3 DeCasteljau(List<Vector3> pts, float t)
    {
        // copie
        var tmp = new List<Vector3>(pts);
        int n = tmp.Count;
        for (int r = 1; r < n; r++)
        {
            for (int i = 0; i < n - r; i++)
            {
                tmp[i] = Vector3.Lerp(tmp[i], tmp[i + 1], t);
            }
        }
        return tmp[0];
    }
}
