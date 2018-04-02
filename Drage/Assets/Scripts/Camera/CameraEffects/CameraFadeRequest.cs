using System.Collections;
using UnityEngine;

public class CameraFadeRequest : EventAction
{
    private CameraFadeController cameraFadeController;

    [Header("Camera Fade Attributes")]
    public CameraFadeAttributes cameraFadeAttributes;

    public override void ActivateEvent()
    {
        cameraFadeController = Camera.main.GetComponent<CameraFadeController>();

        cameraFadeController.StartFade(cameraFadeAttributes);
    }

    public override void DisableEvent()
    {

    }
}
