using System.Collections;
using UnityEngine;
using Rewired;

public class FirstPersonInputManager : InputManager
{
    private PlayerController playerCharacterController;
    private FirstPersonCameraController playerCameraController;

    private Player playerInput;

    [Header("Player Input Values")]
    public float moveXAxis;
    public float moveYAxis;
    [Space(10)]
    public float lookXAxis;
    public float lookYAxis;
    [Space(10)]
    public float lookXAxisSensitivity = 1f;
    public float lookYAxisSensitivity = 1f;
    [Space(10)]
    public Vector2 moveAxisVector;
    [Space(10)]
    public Vector2 lookAxisVector;

    private bool isInputInitialized = false;

    private void Start()
    {
        IntializeInput();
    }

    private void Update()
    {
        GetInput();
    }

    private void IntializeInput()
    {
        playerCharacterController = GetComponent<PlayerController>();

        playerCameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<FirstPersonCameraController>();

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

            lookXAxis = playerInput.GetAxis("HeadHorizontalAxis") / lookXAxisSensitivity;
            lookYAxis = playerInput.GetAxis("HeadVerticalAxis") / lookYAxisSensitivity;

            lookAxisVector.Set(lookXAxis, lookYAxis);

            playerCharacterController.ReceiveInputVectors(moveAxisVector, lookAxisVector);

            playerCameraController.ReceiveInputVectors(lookAxisVector);

            if (playerInput.GetButtonDown("Interact"))
            {
                playerCharacterController.Interact();
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
        }
    }

}
