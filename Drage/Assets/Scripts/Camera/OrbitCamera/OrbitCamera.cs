using System.Collections;
using UnityEngine;

public class OrbitCamera : CameraController
{
    [Header("Camera Attributes")]
    public Transform targetOrbitPoint;

    private float inputX;
    private float inputY;

    [Header("Orbit Attributes")]
    public Vector2 orbitAngleMinMax;

    [Header("Camera Position Attributes")]
    public float cameraDistanceFromTarget = 3f;

    [Space(10)]
    public float cameraMovementSmoothTime = 0.12f;

    private Vector3 cameraCurrentPosition;
    private Vector3 cameraMoveSmoothVelocity;

    [Header("Camera Rotation Attributes")]
    public float cameraRotationSmoothTime = 0.12f;

    private Vector3 cameraRotationSmoothVelocity;
    private Vector3 cameraCurrentRotation;

    private float cameraYaw;
    private float cameraPitch;

    private void LateUpdate()
    {
        ManageCameraMovement();
        ManageCameraRotation();
    }

    private void ManageCameraMovement()
    {
        Vector3 cameraDesiredPosition = targetOrbitPoint.transform.position - transform.forward * cameraDistanceFromTarget;

        cameraCurrentPosition = Vector3.SmoothDamp(cameraCurrentPosition, cameraDesiredPosition, ref cameraMoveSmoothVelocity, cameraMovementSmoothTime);

        transform.position = cameraCurrentPosition;
    }

    private void ManageCameraRotation()
    {
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");

        cameraYaw += inputX;
        cameraPitch -= inputY;

        cameraPitch = Mathf.Clamp(cameraPitch, orbitAngleMinMax.x, orbitAngleMinMax.y);

        Vector3 newCameraRotation = new Vector3(cameraPitch, cameraYaw);

        cameraCurrentRotation = Vector3.SmoothDamp(cameraCurrentRotation, newCameraRotation, ref cameraRotationSmoothVelocity, cameraRotationSmoothTime);

        transform.eulerAngles = cameraCurrentRotation;
    }
}
