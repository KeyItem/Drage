using System.Collections;
using UnityEngine;
using Rewired;

public class InputManager2D : MonoBehaviour
{
    private Player playerInput;

    private PlayerController2D playerController;

    [Header("Player Input Attributes")]
    public float xAxisInput;
    public float yAxisInput;

    private Vector2 inputVector;

    [Space(10)]
    private Vector2 inputDirectionVector;

    private bool canReceiveInput = true;

    private bool isInputInitialized = false;

    private void Start()
    {
        InputSetup();
    }

    private void Update()
    {
        GetInput();
    }

    private void InputSetup()
    {
        playerInput = ReInput.players.GetPlayer(0);

        playerController = GetComponent<PlayerController2D>();

        isInputInitialized = true;
    }

    private void GetInput()
    {
        if (isInputInitialized)
        {
            if (canReceiveInput)
            {
                xAxisInput = playerInput.GetAxis("HorizontalAxis");
                yAxisInput = playerInput.GetAxis("VerticalAxis");

                inputVector.Set(xAxisInput, yAxisInput);

                inputDirectionVector = ReturnInputDirectionVector(xAxisInput, yAxisInput);

                if (playerInput.GetButtonDown("Jump"))
                {
                    playerController.PlayerJump();
                }
                if (playerInput.GetButtonUp("Jump"))
                {
                    playerController.PlayerJumpEarlyRelease();
                }

                if (playerInput.GetButtonDown("Dash"))
                {
                    playerController.PlayerDash();
                }

                playerController.ReceiveInputData(inputVector, inputDirectionVector);
            }
        }
    }

    private Vector2 ReturnInputDirectionVector(float inputXAxis, float inputYAxis)
    {
        Vector2 modifiedInputDirectionVector = Vector2.zero;

        if (inputXAxis != 0)
        {
            modifiedInputDirectionVector.x = Mathf.Sign(inputXAxis);
        }
        else
        {
            modifiedInputDirectionVector.x = 0;
        }

        if (inputYAxis != 0)
        {
            modifiedInputDirectionVector.y = Mathf.Sign(inputYAxis);
        }
        else
        {
            modifiedInputDirectionVector.y = 0;
        }

        return modifiedInputDirectionVector;
    }
}
