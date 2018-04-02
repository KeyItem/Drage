using System.Collections;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Camera Attributes")]
    public Camera currentCam;
    private Camera lastCam;

    [Space(10)]
    public int currentCameraIndex = 0;

    [Space(10)]
    public Camera[] cameraArray;

    private void Start()
    {
        SetupCameras();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchToNextCamera();
        }
    }

    private void SetupCameras()
    {
        currentCam = cameraArray[0];
        currentCam.enabled = true;
        currentCameraIndex = 0;

        for (int i = 1; i < cameraArray.Length; i++)
        {
            cameraArray[i].enabled = false;
        }
    }

    public void SwitchToNextCamera()
    {
        if (++currentCameraIndex > cameraArray.Length - 1)
        {
            currentCameraIndex = 0;
        }

        lastCam = currentCam;

        currentCam = cameraArray[currentCameraIndex];

        lastCam.enabled = false;
        currentCam.enabled = true;
    }

    public void SwitchToTargetCamera(int targetCameraIndex)
    {
        lastCam = currentCam;

        currentCam = cameraArray[targetCameraIndex];
        currentCameraIndex = targetCameraIndex;

        lastCam.enabled = false;
        currentCam.enabled = true;
    }
}
