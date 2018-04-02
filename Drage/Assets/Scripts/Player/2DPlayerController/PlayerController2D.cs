using System.Collections;
using UnityEngine;

public class PlayerController2D : Controller2D
{
    [Header("Player Input Attributes")]
    public Vector2 playerInput;
    public Vector2 playerInputDirection;

    [Header("Player Movement Attributes")]
    public PlayerMovementAttributes2D playerMovementAttributes;

    private float playerMovementTargetSpeed;

    private float playerMovementSmoothVelocity;

    [Space(10)]
    public Vector2 playerVelocity;

    [Header("Player Jump Attributes")]
    public PlayerJumpingAttributes2D playerJumpAttributes;

    [Space(10)]
    [SerializeField]
    private Vector2 playerMinJumpVelocity;
    [SerializeField]
    private Vector2 playerMaxJumpVelocity;

    [Space(10)]
    public float playerGravity;

    [Header("Player Wall Climbing Attributes")]
    public PlayerWallClimbingAttributes2D playerWallClimbAttributes;

    [Space(10)]
    private int wallDirectionX;

    private float wallStickTime;

    [Space(10)]
    public bool isWallSliding = false;

    [Header("Player Dashing Attributes")]
    public PlayerDashingAttributes2D playerDashingAttributes;

    private Vector2 playerDashVelocity;

    private int playerDashDirection;

    private float playerDashEndTime;

    [Space(10)]
    public bool isDashing = false;

    [Header("Player Collision Attributes")]
    public PlayerCollisionManager2D playerCollision;

    private void Start()
    {
        PlayerSetup();
    }

    private void Update()
    {
        ManagePlayerVelocity();
    }

    private void PlayerSetup()
    {
        playerCollision = GetComponent<PlayerCollisionManager2D>();

        CalculateGravity();
        CalculateJumpVelocity();
    }

    public void ReceiveInputData(Vector2 rawInput, Vector2 inputDirection)
    {
        playerInput = rawInput;
        playerInputDirection = inputDirection;
    }

    private void ManagePlayerVelocity()
    {
        CalculatePlayerVelocity(playerInput);

        HandlePlayerWallSliding();
        HandlePlayerDash();

        MoveController(playerVelocity * Time.deltaTime, false);

        if (playerCollision.collisionData.isCollidingAbove || playerCollision.collisionData.isCollidingBelow)
        {
            if (playerCollision.collisionData.isSlidingDownSlope)
            {
                playerVelocity.y += playerCollision.collisionData.slopeNormal.y * -playerGravity * Time.deltaTime;
            }
            else
            {
                playerVelocity.y = 0f;
            }
        }
    }

    private void CalculatePlayerVelocity(Vector2 rawInput)
    {
        playerMovementTargetSpeed = rawInput.x * playerMovementAttributes.playerBaseMovementSpeed;

        playerVelocity.x = Mathf.SmoothDamp(playerVelocity.x, playerMovementTargetSpeed, ref playerMovementSmoothVelocity, ReturnPlayerSmoothMovementTime());

        playerVelocity.y += playerGravity * Time.deltaTime;
    }

    public override void MoveController(Vector2 finalPlayerVelocity, bool isOnPlatform = false)
    {
        playerCollision.ManageObjectCollisions(ref finalPlayerVelocity);

        transform.Translate(finalPlayerVelocity);

        if (isOnPlatform)
        {
            playerCollision.collisionData.isCollidingBelow = true;
        }
    }

    private float ReturnPlayerSmoothMovementTime()
    {
        if (isDashing)
        {
            return float.MaxValue;
        }
        if (playerCollision.collisionData.isCollidingBelow)
        {
            return playerMovementAttributes.playerMovementSmoothingGround;
        }
        else if (!playerCollision.collisionData.isCollidingBelow)
        {
            return playerMovementAttributes.playerMovementSmoothingAir;
        }
        else
        {
            return 0;
        }
    }

    public void PlayerJump()
    {
        if (isWallSliding)
        {
            if (wallDirectionX == playerInputDirection.x)
            {
                Vector2 wallClimb = Vector2.zero;

                wallClimb.x = -wallDirectionX * playerWallClimbAttributes.wallJumpClimb.x;
                wallClimb.y = playerWallClimbAttributes.wallJumpClimb.y;

                playerVelocity.x = -wallDirectionX * playerWallClimbAttributes.wallJumpClimb.x;
                playerVelocity.y = playerWallClimbAttributes.wallJumpClimb.y;
            }
            else if (playerInputDirection.x == 0)
            {
                Vector2 wallClimb = Vector2.zero;

                wallClimb.x = -wallDirectionX * playerWallClimbAttributes.wallJumpOff.x;
                wallClimb.y = playerWallClimbAttributes.wallJumpOff.y;

                playerVelocity.x = -wallDirectionX * playerWallClimbAttributes.wallJumpOff.x;
                playerVelocity.y = playerWallClimbAttributes.wallJumpOff.y;
            }
            else
            {
                Vector2 wallClimb = Vector2.zero;

                wallClimb.x = -wallDirectionX * playerWallClimbAttributes.wallJumpLeap.x;
                wallClimb.y = playerWallClimbAttributes.wallJumpLeap.y;

                playerVelocity.x = -wallDirectionX * playerWallClimbAttributes.wallJumpLeap.x;
                playerVelocity.y = playerWallClimbAttributes.wallJumpLeap.y;
            }
        }
        else if (playerCollision.collisionData.isCollidingBelow)
        {
            if (playerCollision.collisionData.isSlidingDownSlope)
            {
                if (playerInputDirection.x != -Mathf.Sign(playerCollision.collisionData.slopeNormal.x))
                {
                    playerVelocity.y = playerMaxJumpVelocity.y * playerCollision.collisionData.slopeNormal.y;
                    playerVelocity.x = playerMaxJumpVelocity.y * playerCollision.collisionData.slopeNormal.x;
                }
            }
            else
            {
                playerVelocity.y = playerMaxJumpVelocity.y;
            }
        }
    }

    public void PlayerJumpEarlyRelease()
    {
        if (playerVelocity.y > playerMinJumpVelocity.y)
        {
            playerVelocity.y = playerMinJumpVelocity.y;
        }
    }

    private void CalculateGravity()
    {
        playerGravity = -(2 * playerJumpAttributes.playerMaxJumpHeight / Mathf.Pow(playerJumpAttributes.playerJumpTime, 2));
    }

    private void CalculateJumpVelocity()
    {
        playerMinJumpVelocity.y = Mathf.Sqrt(2 * Mathf.Abs(playerGravity) * playerJumpAttributes.playerMinJumpHeight);
        playerMaxJumpVelocity.y = Mathf.Abs(playerGravity) * playerJumpAttributes.playerJumpTime;
    }

    private void HandlePlayerWallSliding()
    {
        wallDirectionX = (playerCollision.collisionData.isCollidingLeft) ? -1 : 1;

        isWallSliding = false;

        if ((playerCollision.playerWallCollisionData.isAdjacentToClimbableWall) && !playerCollision.collisionData.isCollidingBelow && playerVelocity.y < 0)
        {
            isWallSliding = true;

            if (playerVelocity.y < -playerWallClimbAttributes.wallSlideSpeedMax)
            {
                playerVelocity.y = -playerWallClimbAttributes.wallSlideSpeedMax;
            }

            if (wallStickTime > 0)
            {
                playerVelocity.x = 0;
                playerMovementSmoothVelocity = 0;

                if (playerInputDirection.x != wallDirectionX && playerInputDirection.x != 0)
                {
                    wallStickTime -= Time.deltaTime;
                }
                else
                {
                    wallStickTime = playerWallClimbAttributes.wallStickToTime;
                }
            }
            else
            {
                wallStickTime = playerWallClimbAttributes.wallStickToTime;
            }
        }
    }

    private Vector2 CalculateDashAcceleration(float dashDistance, float dashTime)
    {
        Vector2 newDashVelocity = Vector2.zero;
        newDashVelocity.x = (2 * dashDistance / Mathf.Pow(dashTime, 2));

        return newDashVelocity;
    }

    public void PlayerDash()
    {
        if (!isDashing)
        {
            if (playerCollision.collisionData.isCollidingBelow)
            {
                if (playerInputDirection.x != 0)
                {
                    playerDashDirection = (int)playerInputDirection.x;
       
                    float dashDistance = (playerDashDirection == -1) ? playerDashingAttributes.playerBackwardDashDistance : playerDashingAttributes.playerForwardDashDistance;
                    float dashTime = (playerDashDirection == -1) ? playerDashingAttributes.playerBackwardDashTime : playerDashingAttributes.playerForwardDashTime;

                    playerDashVelocity = CalculateDashAcceleration(dashDistance, dashTime);

                    playerDashEndTime = Time.time + dashTime;

                    playerVelocity.x = 0;
                    playerMovementSmoothVelocity = 0;

                    isDashing = true;
                }
            }
        }
    }

    private void HandlePlayerDash()
    {
        if (isDashing)
        {
            if (Time.time < playerDashEndTime)
            {
                if (playerCollision.collisionData.isCollidingLeft || playerCollision.collisionData.isCollidingRight)
                {
                    for (int i = 0; i < playerCollision.collisionData.collidedObjects.Count; i++)
                    {
                        GameObject hitObject = playerCollision.collisionData.collidedObjects[i];

                        if (Helper.LayerMaskContainsLayer(playerDashingAttributes.dashCollisionLayer, hitObject.layer))
                        {
                            Debug.Log("Hit Something While Dashing!");
                        }
                    }
                }

                playerVelocity.x += playerDashVelocity.x * Time.deltaTime;
            }
            else
            {
                EndDash();
            }
        }
    }

    private void EndDash()
    {
        playerDashDirection = 0;

        playerDashVelocity = Vector2.zero;

        playerDashEndTime = 0;

        playerVelocity.x = 0;
        playerMovementSmoothVelocity = 0;

        isDashing = false;
    }
}

[System.Serializable]
public struct PlayerMovementAttributes2D
{
    [Header("Player Movement Values")]
    public float playerBaseMovementSpeed;

    [Space(10)]
    public float playerMovementSmoothingGround;
    public float playerMovementSmoothingAir;
}

[System.Serializable]
public struct PlayerJumpingAttributes2D
{
    [Header("Player Jumping Values")]
    public float playerMinJumpHeight;
    public float playerMaxJumpHeight;

    [Space(10)]
    public float playerJumpTime;
}

[System.Serializable]
public struct PlayerWallClimbingAttributes2D
{
    [Header("Player Wall Climbing Attributes")]
    public Vector2 wallJumpOff;

    [Space(10)]
    public Vector2 wallJumpClimb;

    [Space(10)]
    public Vector2 wallJumpLeap;

    [Header("Player Wall Sliding Attributes")]
    public float wallSlideSpeedMax;

    [Space(10)]
    public float wallStickToTime;
}

[System.Serializable]
public struct PlayerDashingAttributes2D
{
    [Header("Player Dashing Attributes")]
    public float playerForwardDashDistance;
    public float playerForwardDashTime;

    [Space(10)]
    public float playerBackwardDashDistance;
    public float playerBackwardDashTime;

    [Space(20)]
    public LayerMask dashCollisionLayer;
}
