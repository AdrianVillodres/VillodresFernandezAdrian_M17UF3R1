using UnityEngine;

public class LockCameraRotation : MonoBehaviour
{
    private Quaternion initialLocalRotation;

    void Start()
    {
        // Guarda la rotación local inicial de la cámara
        initialLocalRotation = transform.localRotation;
    }

    void LateUpdate()
    {
        // Fuerza la cámara a mantener su rotación local original
        transform.localRotation = initialLocalRotation;
    }
}
