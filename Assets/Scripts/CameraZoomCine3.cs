using UnityEngine;
using Unity.Cinemachine;   // <— namespace Cinemachine 3.x

[DisallowMultipleComponent]
[AddComponentMenu("TP3/Camera Zoom (Cinemachine 3.x)")]
public class CameraZoomCine3 : MonoBehaviour
{
    [SerializeField] private CinemachineCamera vcam; // Caméra virtuelle 3.x
    [SerializeField, Min(0.1f)] private float zoomSpeed = 3f;
    [SerializeField] private float minFOV = 35f;
    [SerializeField] private float maxFOV = 65f;

    void Reset()
    {
        if (!vcam) vcam = GetComponent<CinemachineCamera>();
    }

    void Update()
    {
        if (!vcam) return;
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.0001f)
        {
            // En 3.x, Lens est une struct: on lit, on modifie, on réassigne
            var lens = vcam.Lens;
            lens.FieldOfView = Mathf.Clamp(lens.FieldOfView - scroll * zoomSpeed * 10f, minFOV, maxFOV);
            vcam.Lens = lens;
        }
    }
}
