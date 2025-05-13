using UnityEngine;

public class LockCameraRotation : MonoBehaviour
{
    public float sensitivity = 2f;  
    private float rotationX = 0f;   
    private float rotationY = 0f;   

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");


        rotationX -= mouseY * sensitivity;
        rotationY += mouseX * sensitivity;


        rotationX = Mathf.Clamp(rotationX, -90f, 90f);


        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }
}
