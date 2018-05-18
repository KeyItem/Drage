using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class InputManager2D : MonoBehaviour
{
    private Player playerInput;

    private PlayerController2D playerController;

    [Header("Player Input Attributes")]
    public PlayerInputSettings playerInputSettings;

    [Space(10)]
    public float xAxisInput;
    public float yAxisInput;

    private Vector2 inputVector;

    [Space(10)]
    public float xAxisRawInput;
    public float yAxisRawInput;

    [Space(10)]
    private Vector2 inputDirectionVector;

    private bool canReceiveInput = true;

    private bool isInputInitialized = false;

    [Header("Player Captured Input")]
    public PlayerCapturedInput playerCapturedInput = new PlayerCapturedInput();

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

                xAxisRawInput = playerInput.GetAxisRaw("HorizontalAxis");
                yAxisRawInput = playerInput.GetAxisRaw("VerticalAxis");

                inputDirectionVector = ReturnInputDirectionVector(xAxisRawInput, yAxisRawInput);

                if (playerInput.GetButtonDown("Jump"))
                {
                    playerCapturedInput.playerActions.Add(PlayerActions.JUMP);

                    playerController.PlayerJump();
                }
                if (playerInput.GetButtonUp("Jump"))
                {
                    playerCapturedInput.playerActions.Add(PlayerActions.JUMP);

                    playerController.PlayerJumpEarlyRelease();
                }

                if (playerInput.GetButtonDown("Dash"))
                {
                    playerCapturedInput.playerActions.Add(PlayerActions.DASH);

                    playerController.PlayerDash();
                }

                if (playerInput.GetButtonDown("Attack"))
                {
                    playerCapturedInput.playerActions.Add(PlayerActions.ATTACK);

                    playerController.PlayerAttack();
                }

                if (playerInput.GetButtonDown("SelectWeapon1"))
                {
                    playerCapturedInput.playerActions.Add(PlayerActions.SWITCH_WEAPONS);

                    playerController.SwitchWeapons(0);
                }
                else if (playerInput.GetButtonDown("SelectWeapon2"))
                {
                    playerCapturedInput.playerActions.Add(PlayerActions.SWITCH_WEAPONS);

                    playerController.SwitchWeapons(1);
                }
                else if (playerInput.GetButtonDown("SelectWeapon3"))
                {
                    playerCapturedInput.playerActions.Add(PlayerActions.SWITCH_WEAPONS);

                    playerController.SwitchWeapons(2);
                }
                else if (playerInput.GetButtonDown("SelectWeapon4"))
                {
                    playerCapturedInput.playerActions.Add(PlayerActions.SWITCH_WEAPONS);

                    playerController.SwitchWeapons(3);
                }

                if (playerInput.GetButtonDown("Reset"))
                {
                    playerController.ResetController();
                }

                playerController.ReceiveInputData(inputVector, inputDirectionVector, playerCapturedInput);

                playerCapturedInput.Reset();
            }
        }
    }

    private Vector2 ReturnInputDirectionVector(float inputXAxis, float inputYAxis)
    {
        Vector2 modifiedInputDirectionVector = Vector2.zero;

        if (inputXAxis != 0 && Mathf.Abs(inputXAxis) > playerInputSettings.playerXInputDirectionDeadzone)
        {
            modifiedInputDirectionVector.x = Mathf.Sign(inputXAxis);
        }
        else
        {
            modifiedInputDirectionVector.x = 0;
        }

        if (inputYAxis != 0 && Mathf.Abs(inputYAxis) > playerInputSettings.playerYInputDirectionDeadzone)
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

[System.Serializable]
public struct PlayerInputSettings
{
    [Header("Player Input Settings")]
    public float playerXInputDirectionDeadzone;
    public float playerYInputDirectionDeadzone;
}

[System.Serializable]
public struct PlayerCapturedInput
{
    [Header("Captured Input Attributes")]
    public DIRECTION inputDirection;

    [Space(10)]
    public List<PlayerActions> playerActions;

    public void Reset()
    {
        inputDirection = DIRECTION.NONE;

        playerActions.Clear();
    }
}

public enum PlayerActions
{
    NONE,
    ATTACK,
    JUMP,
    SWITCH_WEAPONS,
    DASH
}
