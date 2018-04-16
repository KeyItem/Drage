using System.Collections;
using UnityEngine;

public class RequestCameraFade : MonoBehaviour
{
    public CameraFadeController cameraFadeController;
    public CameraFadeAttributes cameraFadeAttributes;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            cameraFadeController.StartFade(cameraFadeAttributes);
        }
    }
}
