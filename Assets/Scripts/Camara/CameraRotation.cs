using UnityEngine;

public class LockCameraRotation : MonoBehaviour
{
    private Quaternion initialLocalRotation;

    void Start()
    {
        // Guarda la rotaci�n local inicial de la c�mara
        initialLocalRotation = transform.localRotation;
    }

    void LateUpdate()
    {
        // Fuerza la c�mara a mantener su rotaci�n local original
        transform.localRotation = initialLocalRotation;
    }
}
