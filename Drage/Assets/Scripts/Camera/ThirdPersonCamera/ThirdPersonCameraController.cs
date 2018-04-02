using System.Collections;
using UnityEngine;

public class ThirdPersonCameraController : CameraController
{
    [Header("Camera Attributes")]
    public CAMERA_STATE cameraState;

    [Space(10)]
    public Camera thirdPersonCamera;

    [Space(10)]
    public Transform targetPlayer;
    public Transform currentLockOnTarget;

    [Space(10)]
    public Vector3 cameraDesiredPosition;

    private Vector3 cameraCurrentPosition;

    [Space(10)]
    public float cameraCurrentDistanceFromPlayer = 0f;

    [Space(10)]
    public bool canCameraMove = true;
    public bool canCameraRotate = true;

    [Space(10)]
    public bool isPlayerInCover = false;

    [Header("Camera Movement Attributes")]
    public ThirdPersonCameraMovementAttributes thirdPersonCameraMovementAttributes;

    private Vector3 cameraMoveSmoothVelocity;

    [Header("Camera Rotation Attributes")]
    public ThirdPersonCameraRotationAttributes thirdPersonCameraRotationAttributes;

    private Vector2 cameraInputVector;

    [Space(10)]
    private float cameraYaw;
    private float cameraPitch;

    private Vector3 cameraRotationSmoothVelocity;
    private Vector3 cameraCurrentRotation;

    private Quaternion cameraTargetRotation;

    [Header("Camera Collision Attributes")]
    public ThirdPersonCameraCollisionAttributes thirdPersonCameraCollisionAttributes;

    private ThirdPersonCameraCollisionController thirdPersonCameraCollisionController;

    [Header("Camera LockOn Attributes")]
    public ThirdPersonCameraLockOnAttributes thirdPersonCameraLockOnAttributes;

    private ThirdPersonCameraLockOnController thirdPersonCameraLockOnController;

    private bool canInputLockOnSwitch = true;

    [Space(10)]
    public bool canCameraReset = true;

    [Header("DEBUG")]
    public bool canShowDebug = true;

    private void Start()
    {
        InitializeCamera();
    }

    private void InitializeCamera()
    {
        thirdPersonCamera = GetComponent<Camera>();

        thirdPersonCameraCollisionController = GetComponent<ThirdPersonCameraCollisionController>();
        thirdPersonCameraLockOnController = GetComponent<ThirdPersonCameraLockOnController>();

        cameraCurrentPosition = transform.position;

        cameraCurrentDistanceFromPlayer = thirdPersonCameraCollisionAttributes.cameraBaseDistanceFromTarget;

        cameraPitch = thirdPersonCameraRotationAttributes.basePitch;
        cameraYaw = thirdPersonCameraRotationAttributes.baseYaw;
    }

    private void LateUpdate()
    {
        ManageCameraMovement();
        ManageCameraRotation();
    }

    private void ManageCameraMovement()
    {
        if (canCameraMove)
        {
            if (cameraState == CAMERA_STATE.FOLLOWING_PLAYER)
            {
                cameraDesiredPosition = targetPlayer.position - transform.forward * cameraCurrentDistanceFromPlayer;

                cameraCurrentPosition = Vector3.SmoothDamp(cameraCurrentPosition, cameraDesiredPosition, ref cameraMoveSmoothVelocity, ReturnCameraSmoothTime());

                transform.position = cameraCurrentPosition;
            }
            else if (cameraState == CAMERA_STATE.LOCKED_ON)
            {
                cameraDesiredPosition = (targetPlayer.position - transform.forward * cameraCurrentDistanceFromPlayer) + (Vector3.up * thirdPersonCameraLockOnController.ReturnCameraLockOnHeight());

                cameraCurrentPosition = Vector3.SmoothDamp(cameraCurrentPosition, cameraDesiredPosition, ref cameraMoveSmoothVelocity, ReturnCameraSmoothTime());

                transform.position = cameraCurrentPosition;
            }        
        }
    }

    private void ManageCameraRotation()
    {
        if (canCameraRotate)
        {
            if (cameraState == CAMERA_STATE.FOLLOWING_PLAYER)
            {
                cameraPitch = Mathf.Clamp(cameraPitch, thirdPersonCameraRotationAttributes.pitchMinMax.x, thirdPersonCameraRotationAttributes.pitchMinMax.y);

                Vector3 targetCameraRotation = new Vector3(cameraPitch, cameraYaw);

                cameraCurrentRotation = Vector3.SmoothDamp(cameraCurrentRotation, targetCameraRotation, ref cameraRotationSmoothVelocity, thirdPersonCameraRotationAttributes.cameraRotationSmoothTime);

                cameraTargetRotation = Quaternion.Euler(cameraCurrentRotation);

                transform.rotation = cameraTargetRotation;
            }
            else if (cameraState == CAMERA_STATE.LOCKED_ON)
            {
                Vector3 targetLookDirection = currentLockOnTarget.position - transform.position;

                Quaternion newLookRotation = Quaternion.LookRotation(targetLookDirection);

                transform.rotation = Quaternion.Lerp(transform.rotation, newLookRotation, thirdPersonCameraLockOnAttributes.cameraLockOnRotateSmoothTime * Time.deltaTime);
            }
        }
    }

    public override void ResetCamera()
    {
        if (cameraState == CAMERA_STATE.LOCKED_ON)
        {
            StopLock();
        }
        else if (cameraState == CAMERA_STATE.FOLLOWING_PLAYER)
        {
            if (thirdPersonCameraLockOnController.CheckForEnemiesInRange())
            {
                RequestLockOn();
            }
            else
            {
                ResetPlayerCamera();
            }
        }  
    }

    private void RequestLockOn()
    {
        thirdPersonCameraLockOnController.RequestLockOn();
    }

    private void SwitchLockOnTarget(Vector2 inputVector)
    {
        DIRECTION newInputDirection = ReturnInputDirection(inputVector);

        thirdPersonCameraLockOnController.RequestNewLockTarget(newInputDirection);
    }

    public void SetLockOnTarget(Transform newLockOnTarget)
    {
        currentLockOnTarget = newLockOnTarget;

        cameraState = CAMERA_STATE.LOCKED_ON;
    }

    public void StopLock()
    {
        thirdPersonCameraLockOnController.RequestStopLockOn();

        currentLockOnTarget = null;

        ResetPlayerCamera();

        cameraState = CAMERA_STATE.FOLLOWING_PLAYER;
    }

    private void ResetPlayerCamera()
    {
        cameraYaw = targetPlayer.transform.eulerAngles.y;
        cameraPitch = thirdPersonCameraRotationAttributes.basePitch;
    }

    public override void ReceiveInputVectors(Vector2 inputVector)
    {
        cameraInputVector = inputVector;

        if (cameraState == CAMERA_STATE.FOLLOWING_PLAYER)
        {
            cameraYaw += cameraInputVector.x;
            cameraPitch -= cameraInputVector.y;
        }
        else if (cameraState == CAMERA_STATE.LOCKED_ON)
        {
            if (canInputLockOnSwitch)
            {
                if (cameraInputVector != Vector2.zero)
                {
                    if (cameraInputVector.sqrMagnitude > 0.9f)
                    {
                        SwitchLockOnTarget(cameraInputVector);

                        canInputLockOnSwitch = false;
                    }
                }
            }
            else
            {
                if (cameraInputVector == Vector2.zero)
                {
                    canInputLockOnSwitch = true;
                }
            }
        }       
    }

    private DIRECTION ReturnInputDirection(Vector2 inputVector)
    {
        if (inputVector.x > 0.9f)
        {
            return DIRECTION.RIGHT;
        }
        else if (inputVector.x < -0.9f)
        {
            return DIRECTION.LEFT;
        }
        else if (inputVector.y > 0.9f)
        {
            return DIRECTION.FORWARD;
        }
        else if (inputVector.y < -0.9f)
        {
            return DIRECTION.BACK;
        }

        return DIRECTION.NONE;
    }

    private float ReturnCameraSmoothTime()
    {
        if (thirdPersonCameraCollisionController.isCameraColliding)
        {
            return thirdPersonCameraCollisionAttributes.cameraCollisionSmoothTime;
        }
        else if (thirdPersonCameraLockOnController.isCameraLockedOn)
        {
            return thirdPersonCameraLockOnAttributes.cameraLockOnMoveSmoothTime;
        }

        return thirdPersonCameraMovementAttributes.cameraMoveSmoothTime;
    }

    private void OnDrawGizmosSelected()
    {
        if (canShowDebug)
        {
            Gizmos.color = Colors.Red; //LockDistance
            Gizmos.DrawWireSphere(targetPlayer.position, thirdPersonCameraLockOnAttributes.cameraLockOnRadius);
        }
    }
}

[System.Serializable]
public class ThirdPersonCameraMovementAttributes
{
    [Header("Camera Movement Attributes")]
    public float cameraMoveSmoothTime = 0.12f;

    [Space(10)]
    public float cameraResetTime = 3f;
}

[System.Serializable]
public class ThirdPersonCameraRotationAttributes
{
    [Header("Camera Rotation Attributes")]
    public float cameraRotationSmoothTime = 0.12f;

    [Header("Camera Base Position Attributes")]
    public float basePitch = 20f;
    public float baseYaw = 0f;

    [Space(10)]
    public Vector2 pitchMinMax = new Vector2(-40, 85);
}