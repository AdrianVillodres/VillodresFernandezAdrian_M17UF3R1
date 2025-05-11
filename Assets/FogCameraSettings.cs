using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogCameraSettings : MonoBehaviour
{
    [SerializeField] private bool FogEnabled = true;

    bool previousFogState;

    private void OnPreRender()
    {
        previousFogState = RenderSettings.fog;
        RenderSettings.fog = FogEnabled;
    }

    private void OnPostRender()
    {
        RenderSettings.fog = previousFogState;
    }
}
