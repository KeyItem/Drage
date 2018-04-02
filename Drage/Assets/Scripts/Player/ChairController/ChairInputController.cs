using System.Collections;
using UnityEngine;
using Rewired;

public class ChairInputController : InputManager
{
    private Player playerInput;

    private ChairPlayerController chairPlayerController;

    [Header("Player Input Values")]
    public float topLeftLegAxis;
    public float topRightLegAxis;
    public float bottomLeftLegAxis;
    public float bottomRightLegAxis;

    private bool isInitialized = false;

    private void Start()
    {
        InitializeInput();
    }

    private void Update()
    {
        SearchForInput();
    }

    private void InitializeInput()
    {
        chairPlayerController = GetComponent<ChairPlayerController>();

        switch (playerNumber)
        {
            case PLAYER_NUMBER.PLAYER_1:
                playerInput = ReInput.players.GetPlayer(0);
                break;

            case PLAYER_NUMBER.PLAYER_2:
                playerInput = ReInput.players.GetPlayer(1);
                break;

            case PLAYER_NUMBER.PLAYER_3:
                playerInput = ReInput.players.GetPlayer(2);
                break;

            case PLAYER_NUMBER.PLAYER_4:
                playerInput = ReInput.players.GetPlayer(3);
                break;

            default:
                Debug.Log("An Error has occured while setting up Input!");
                break;
        }

        isInitialized = true;
    }

    private void SearchForInput()
    {
        if (isInitialized)
        {
            topLeftLegAxis = playerInput.GetAxis("TopLeftLegAxis");
            topRightLegAxis = playerInput.GetAxis("TopRightLegAxis");

            bottomLeftLegAxis = playerInput.GetAxis("BottomLeftLegAxis");
            bottomRightLegAxis = playerInput.GetAxis("BottomRightLegAxis");
        }
    }
}
