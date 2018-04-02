using System.Collections;
using UnityEngine;

public class CoverMovementTest : MonoBehaviour
{
    [Header("Cover Attributes")]
    public BezierSpline targetSpline;

    [Space(10)]
    public int playerStartingPoint = 0;

    [Space(10)]
    public float playerSplineProgress = 0f;

    [Header("Cover Movement Attributes")]
    public float playerSpeed = 1f;

    [Space(10)]
    public int playerDirection = 0;

    private void Start()
    {
        PlayerSplineSetup();
    }

    private void Update()
    {
        GetInput();

        MovePlayerAlongSpline();
    }

    private void PlayerSplineSetup()
    {
        float startPointPercent = 1f / 8f;
        startPointPercent *= playerStartingPoint;

        playerSplineProgress = startPointPercent;
        transform.position = targetSpline.GetPoint(startPointPercent);
    }

    private void GetInput()
    {
        if (Input.GetKey(KeyCode.A))
        {
            playerDirection = -1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            playerDirection = 1;
        }
        else
        {
            playerDirection = 0;
        }
    }

    private void MovePlayerAlongSpline()
    {
        if (playerDirection != 0)
        {
            playerSplineProgress += Time.deltaTime * playerSpeed * playerDirection;

            if (playerSplineProgress > 1)
            {
                playerSplineProgress = 0;
            }
            else if (playerSplineProgress < 0)
            {
                playerSplineProgress = 1;
            }

            Vector3 newPlayerPosition = targetSpline.GetPoint(playerSplineProgress);
            transform.position = newPlayerPosition;
        }
    }
}
