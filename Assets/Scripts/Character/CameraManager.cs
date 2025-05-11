using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private List<CinemachineVirtualCamera> cameras = new List<CinemachineVirtualCamera>();
    private void Start()
    {
        cameras = new List<CinemachineVirtualCamera>(FindObjectsOfType<CinemachineVirtualCamera>());
    }

    public void SwitchCamera(string camera)
    {
        foreach (CinemachineVirtualCamera cam in cameras)
        {
            if (cam.gameObject.name.Contains(camera))
            {
                cam.enabled = true;
            }
            else
            {
                cam.enabled = false;
            }
        }

    }
}
