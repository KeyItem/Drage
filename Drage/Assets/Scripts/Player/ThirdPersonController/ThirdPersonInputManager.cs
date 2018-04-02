using System.Collections;
using UnityEngine;
using Rewired;

public class ThirdPersonInputManager : InputManager
{
    private ThirdPersonPlayerController playerCharacterController;
    private CameraController playerCameraController;

    private Player playerInput;

    [Header("Player Input Values")]
    public float moveXAxis;
    public float moveYAxis;

    [Space(10)]
    public float cameraXAxis;
    public float cameraYAxis;

    [Space(10)]
    public Vector2 moveAxisVector;
    public Vector2 cameraAxisVector;

    private bool isInputInitialized = false;

    [Header("Input Sensitivity Values")]
    public float cameraXAxisSensitivity = 1f;
    public float cameraYAxisSensitivity = 1f;

    private void Start()
    {
        InitializeInput();
    }

    private void Update()
    {
        GetInput();
    }

    private void InitializeInput()
    {
        playerCharacterController = GetComponent<ThirdPersonPlayerController>();

        playerCameraController = Camera.main.GetComponent<CameraController>();

        playerInput = ReInput.players.GetPlayer(0);

        isInputInitialized = true;
    }

    private void GetInput()
    {
        if (isInputInitialized)
        {
            moveXAxis = playerInput.GetAxis("MoveHorizontalAxis");
            moveYAxis = playerInput.GetAxis("MoveVerticalAxis");

            moveAxisVector.Set(moveXAxis, moveYAxis);

            cameraXAxis = playerInput.GetAxis("LookHorizontalAxis") * cameraXAxisSensitivity;
            cameraYAxis = playerInput.GetAxis("LookVerticalAxis") * cameraYAxisSensitivity;

            cameraAxisVector.Set(cameraXAxis, cameraYAxis);

            playerCharacterController.ReceiveInputVectors(moveAxisVector, Vector2.zero);
            playerCameraController.ReceiveInputVectors(cameraAxisVector);

            if (playerInput.GetButtonDown("Interact"))
            {
                playerCharacterController.Interact();
            }

            if (playerInput.GetButtonLongPressDown("Interact"))
            {
                playerCharacterController.PlayerDropItem();
            }

            if (playerInput.GetButtonDown("Jump"))
            {
                playerCharacterController.Jump();
            }

            if (playerInput.GetButtonDown("Sprint"))
            {
                playerCharacterController.StartSprint();
            }

            if (playerInput.GetButtonUp("Sprint"))
            {
                playerCharacterController.StopSprint();
            }

            if (playerInput.GetButtonDown("Crouch"))
            {
                playerCharacterController.InitiateCrouch();
            }

            if (playerInput.GetButtonDown("TakeCover"))
            {
                playerCharacterController.TakeCover();
            }

            if (playerInput.GetButtonDown("ResetCamera"))
            {
                playerCameraController.ResetCamera();
            }
        }
    }
}
