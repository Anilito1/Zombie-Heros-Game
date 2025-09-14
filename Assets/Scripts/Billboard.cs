using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("TP3/Billboard (Face Camera)")]
public class Billboard : MonoBehaviour
{
    private Camera cam;

    void LateUpdate()
    {
        if (!cam)
        {
            if (Camera.main) cam = Camera.main;
            else return;
        }
        // Regarde dans la même direction que la caméra
        transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position, cam.transform.up);
    }
}
