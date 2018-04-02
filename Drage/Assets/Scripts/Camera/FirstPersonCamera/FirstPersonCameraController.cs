using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//http://wiki.unity3d.com/index.php/SmoothMouseLook

public class FirstPersonCameraController : CameraController
{
    [Header("Player Values")]
    public float xAxisSensitivityMultiplier = 1;
    public float yAxisSensitivityMultiplier = 1;

    private GameObject playerCharacter;

    [Header("Camera Look Values")]
    public Vector3 cameraForward;

    [Space(10)]
    public float minimumX = -360F; //Clamped Values for Player Y Rotation
    public float maximumX = 360F;

    [Space(10)]
    public float minimumY = -60F; //Clamped Values for Camera X Rotation
    public float maximumY = 60F;

    [Space(10)]
    private float rotationX = 0F;
    private float rotationY = 0F;

    private float rotAverageX = 0F;
    private float rotAverageY = 0F;

    private List<float> rotArrayX = new List<float>(); //List holding previous values of input, to find smoothed average
    private List<float> rotArrayY = new List<float>();

    [Space(10)]
    public float xAxisSmoothingLength = 20; //How many values are stored in the list before being recycled, higher the value -> More smoothing
    public float yAxisSmoothingLength = 20; //How many values are stored in the list before being recycled, higher the value -> More smoothing

    [Space(10)]
    public bool canLook = true;

    private Quaternion originalRotation;

    private Vector2 inputLookVector;

    [Header("Mouse Cursor Attributes")]
    public CursorLockMode currentLockMode = CursorLockMode.None;

    public bool isMouseLocked = false;

    private void Start()
    {
        InitializeCamera();
    }

    private void Update()
    {
        LookForInput();
    }

    private void LateUpdate()
    {
        ManageCamera();
    }

    private void InitializeCamera()
    {
        playerCharacter = GameObject.FindGameObjectWithTag("Player");

        originalRotation = transform.localRotation;

        LockMouse();
    }

    private void ManageCamera()
    {
        if (canLook)
        {
            rotAverageX = 0f;
            rotAverageY = 0f;

            rotationX += inputLookVector.x * xAxisSensitivityMultiplier;
            rotationY += inputLookVector.y * yAxisSensitivityMultiplier;

            rotArrayX.Add(rotationX);
            rotArrayY.Add(rotationY);

            if (rotArrayY.Count >= yAxisSmoothingLength)
            {
                rotArrayY.RemoveAt(0);
            }
            if (rotArrayX.Count >= xAxisSmoothingLength)
            {
                rotArrayX.RemoveAt(0);
            }

            for (int i = 0; i < rotArrayX.Count; i++)
            {
                rotAverageX += rotArrayX[i];
            }

            for (int o = 0; o < rotArrayY.Count; o++)
            {
                rotAverageY += rotArrayY[o];
            }

            rotAverageY /= rotArrayY.Count;
            rotAverageX /= rotArrayX.Count;

            rotAverageY = ClampAngle(rotAverageY, minimumY, maximumY);
            rotAverageX = ClampAngle(rotAverageX, minimumX, maximumX);

            Quaternion xRotation = Quaternion.AngleAxis(rotAverageX, Vector3.up);
            Quaternion yRotation = Quaternion.AngleAxis(-rotAverageY, Vector3.right);

            transform.localRotation = originalRotation * yRotation; //Rotate the Camera

            playerCharacter.transform.localRotation = xRotation; //Rotate the Player

            cameraForward = transform.forward;

            Debug.DrawRay(transform.position, cameraForward, Color.yellow);
        }      
    }

    public void SetCameraWallRunAngle(DIRECTION wallRunDirection)
    {
        switch (wallRunDirection)
        {
            case DIRECTION.LEFT:
                originalRotation = Quaternion.Euler(new Vector3(originalRotation.eulerAngles.x, originalRotation.eulerAngles.y, 15f));
                break;

            case DIRECTION.RIGHT:
                originalRotation = Quaternion.Euler(new Vector3(originalRotation.eulerAngles.x, originalRotation.eulerAngles.y, -15f));
                break;

            case DIRECTION.DOWN:
                break;

            case DIRECTION.UP:
                break;
        }
    }

    public void ResetCameraWallRunAngle()
    {
        originalRotation = Quaternion.Euler(new Vector3(originalRotation.eulerAngles.x, originalRotation.eulerAngles.y, 0));
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        angle = angle % 360;
        if ((angle >= -360F) && (angle <= 360F))
        {
            if (angle < -360F)
            {
                angle += 360F;
            }
            if (angle > 360F)
            {
                angle -= 360F;
            }
        }
        return Mathf.Clamp(angle, min, max);
    }

    public override void ReceiveInputVectors(Vector2 receivedInputVector)
    {
        inputLookVector = receivedInputVector;
    }

    private void LookForInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ManageMouseLockStatus();
        }
    }

    public void ManageMouseLockStatus()
    {
        if (isMouseLocked)
        {
            UnlockMouse();
        }
        else
        {
            LockMouse();
        }
    }

    public void LockMouse()
    {
        currentLockMode = CursorLockMode.Locked;

        Cursor.lockState = CursorLockMode.Locked;

        Cursor.visible = false;

        isMouseLocked = true;

        canLook = true;
    }

    public void UnlockMouse()
    {
        currentLockMode = CursorLockMode.None;

        Cursor.lockState = CursorLockMode.None;

        Cursor.visible = true;

        isMouseLocked = false;

        canLook = false;
    }
}
