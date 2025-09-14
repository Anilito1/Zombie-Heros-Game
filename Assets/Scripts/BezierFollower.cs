using UnityEngine;
using System.Collections.Generic;

[DisallowMultipleComponent]
[AddComponentMenu("TP3/Bezier Follower (Constant Speed)")]
public class BezierFollower : MonoBehaviour
{
    [SerializeField] private BezierDrawer path;
    [SerializeField, Min(0.01f)] private float speed = 5f;
    [SerializeField] private bool loop = true;
    [SerializeField] private bool lookForward = true;

    private List<Vector3> samples = new List<Vector3>();
    private int currentIndex = 0;
    private float leftover = 0f;

    void Start() => RebuildCache();
    void Update()
    {
        if (!path) return;

        // Rebuild if resolution changed
        if (samples.Count != path.GetPoints().Length) RebuildCache();
        if (samples.Count < 2) return;

        float distToTravel = speed * Time.deltaTime + leftover;
        leftover = 0f;

        while (distToTravel > 0f)
        {
            Vector3 a = samples[currentIndex];
            Vector3 b = samples[(currentIndex + 1) % samples.Count];
            float segLen = Vector3.Distance(a, b);
            if (distToTravel <= segLen)
            {
                float t = segLen < 1e-5f ? 0f : distToTravel / segLen;
                Vector3 pos = Vector3.Lerp(a, b, t);
                if (lookForward) transform.rotation = Quaternion.LookRotation((b - a).normalized, Vector3.up);
                transform.position = pos;
                distToTravel = 0f;
            }
            else
            {
                transform.position = b;
                if (lookForward) transform.rotation = Quaternion.LookRotation((b - a).normalized, Vector3.up);
                currentIndex++;
                if (currentIndex >= samples.Count - 1)
                {
                    if (loop) currentIndex = 0;
                    else { leftover = 0f; return; }
                }
                distToTravel -= segLen;
            }
        }
    }

    public void RebuildCache()
    {
        samples.Clear();
        if (!path) return;
        var pts = path.GetPoints();
        if (pts.Length < 2) return;

        // Option: densifier la polyline pour plus de constance
        samples.AddRange(pts);
        currentIndex = 0;
        leftover = 0f;
        transform.position = samples[0];
        if (samples.Count > 1 && lookForward)
            transform.rotation = Quaternion.LookRotation((samples[1] - samples[0]).normalized, Vector3.up);
    }
}
