using System.Collections;
using System.Collections.Generic;
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

    [Space(10)]
    public bool canPlayerMove = true;
    public bool canPlayerSprint = true;

    private bool isPlayerSprinting = false;

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

    private Vector2 playerVelocityBeforeDash;

    private DIRECTION playerDashDirection;

    private Vector2 playerDashVelocity;

    private float playerDashEndTime;

    private bool hasAirDashed = true;

    [Space(10)]
    public bool isDashing = false;

    [Space(10)]
    public bool canPlayerDash = true;

    [Header("Player Attack Attributes")]
    public PlayerAttackController2D playerAttackController;

    [Header("Player Collision Attributes")]
    public PlayerCollisionManager2D playerCollision;

    [Header("Player Animation Attributes")]
    public PlayerAnimationController2D playerAnimation;

    [Header("Player Audio Attributes")]
    public PlayerAudioController2D playerAudio;

    private void Start()
    {
        ObjectSetup();
    }

    private void Update()
    {
        ManagePlayerVelocity();
    }

    public override void ObjectSetup()
    {
        base.ObjectSetup();

        playerCollision = GetComponent<PlayerCollisionManager2D>();
        playerAttackController = GetComponent<PlayerAttackController2D>();
        playerAnimation = GetComponent<PlayerAnimationController2D>();
        playerAudio = GetComponent<PlayerAudioController2D>();

        EventSetup();

        CalculateGravity();
    }

    private void EventSetup()
    {
        PlayerHealthController2D.OnPlayerDeath += ResetController;
        PlayerHealthController2D.OnPlayerGameOver += DisableController;
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

        HandleSpriteFaceDirection(playerCollision.collisionData.faceDirection);

        MoveController(playerVelocity * Time.deltaTime, false);

        playerAnimation.SetBool("isGrounded", playerCollision.collisionData.isCollidingBelow);

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
        if (canPlayerMove)
        {
            playerMovementTargetSpeed = ReturnPlayerTargetSpeed(rawInput);

            playerVelocity.x = Mathf.SmoothDamp(playerVelocity.x, playerMovementTargetSpeed, ref playerMovementSmoothVelocity, ReturnPlayerSmoothMovementTime());
            playerVelocity.y += playerGravity * Time.deltaTime;
        }
    }

    public override void MoveController(Vector2 finalPlayerVelocity, bool isOnPlatform = false)
    {
        playerCollision.ManageObjectCollisions(ref finalPlayerVelocity, playerInputDirection);

        transform.Translate(finalPlayerVelocity);

        playerAnimation.SetFloat("moveSpeed", Mathf.Abs(finalPlayerVelocity.x));

        if (isOnPlatform)
        {
            playerCollision.collisionData.isCollidingBelow = true;
        }
    }

    public override void ResetController()
    {
        playerAudio.RequestAudioEvent("DeathSFX");

        Vector2 currentCheckpoint = CheckpointManager.Instance.currentCheckpoint;

        playerVelocity = Vector2.zero;

        transform.position = currentCheckpoint;
    }

    public void StartSprint()
    {
        if (!isPlayerSprinting)
        {
            if (canPlayerSprint)
            {
                isPlayerSprinting = true;
            }
        }
    }

    public void StopSprint()
    {
        if (isPlayerSprinting)
        {
            isPlayerSprinting = false;
        }
    }

    private bool CanPlayerMove()
    {
        if (isDashing)
        {
            return false;
        }
        else if (playerAttackController.isActivelyAttacking)
        {
            return false;
        }

        return true;
    }

    private float ReturnPlayerSmoothMovementTime()
    {
        if (playerCollision.collisionData.isCollidingBelow)
        {
            return playerMovementAttributes.playerMovementSmoothingGround;
        }
        else
        {
            return playerMovementAttributes.playerMovementSmoothingAir;
        }
    }

    private float ReturnPlayerTargetSpeed(Vector2 rawInput)
    {
        if (isDashing || playerAttackController.isActivelyAttacking)
        {
            return 0;
        }

        if (isPlayerSprinting && playerCollision.collisionData.isCollidingBelow)
        {
            return playerInputDirection.x * playerMovementAttributes.playerSprintMovementSpeed; //Instead of grabbing the raw input, we grab the direction so that sprinting is always in normalized direction steps.
        }
        else
        {
            return rawInput.x * playerMovementAttributes.playerBaseMovementSpeed;
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

                playerAnimation.SetTrigger("isWallJump");
            }
            else if (playerInputDirection.x == 0)
            {
                Vector2 wallClimb = Vector2.zero;

                wallClimb.x = -wallDirectionX * playerWallClimbAttributes.wallJumpOff.x;
                wallClimb.y = playerWallClimbAttributes.wallJumpOff.y;

                playerVelocity.x = -wallDirectionX * playerWallClimbAttributes.wallJumpOff.x;
                playerVelocity.y = playerWallClimbAttributes.wallJumpOff.y;

                playerAnimation.SetTrigger("isWallJump");
            }
            else
            {
                Vector2 wallClimb = Vector2.zero;

                wallClimb.x = -wallDirectionX * playerWallClimbAttributes.wallJumpLeap.x;
                wallClimb.y = playerWallClimbAttributes.wallJumpLeap.y;

                playerVelocity.x = -wallDirectionX * playerWallClimbAttributes.wallJumpLeap.x;
                playerVelocity.y = playerWallClimbAttributes.wallJumpLeap.y;

                playerAnimation.SetTrigger("isWallJump");
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

                    playerAnimation.SetTrigger("isJump");
                }
            }
            else
            {
                CalculateJumpVelocity();

                playerVelocity.y = playerMaxJumpVelocity.y;

                playerAnimation.SetTrigger("isJump");

                playerAudio.RequestAudioEvent("jumpSFX");
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

    private void CalculateJumpVelocity()
    {
        playerMinJumpVelocity.y = Mathf.Sqrt(2 * Mathf.Abs(playerGravity) * playerJumpAttributes.playerMinJumpHeight);
        playerMaxJumpVelocity.y = Mathf.Abs(playerGravity) * playerJumpAttributes.playerJumpTime;

        CalculateGravity();
    }

    private void CalculateGravity()
    {
        playerGravity = -(2 * playerJumpAttributes.playerMaxJumpHeight / Mathf.Pow(playerJumpAttributes.playerJumpTime, 2));
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

        playerAnimation.SetBool("isWallSliding", isWallSliding);

    }

    private Vector2 CalculateDashAcceleration(float dashDistance, float dashTime)
    {
        Vector2 newDashVelocity = Vector2.zero;
        newDashVelocity.x = (2 * dashDistance / Mathf.Pow(dashTime, 2));

        return newDashVelocity;
    }

    public void PlayerDash()
    {
        if (canPlayerDash)
        {
            if (!isDashing)
            {
                if (playerInputDirection.x != 0)
                {
                    float dashDistance = 0;
                    float dashTime = 0;

                    playerDashDirection = (playerInputDirection.x == -1) ? DIRECTION.LEFT : DIRECTION.RIGHT;

                    if (playerCollision.collisionData.isCollidingBelow)
                    {
                        dashDistance = (playerDashDirection == DIRECTION.LEFT) ? -playerDashingAttributes.playerGroundDashDistance : playerDashingAttributes.playerGroundDashDistance;
                        dashTime = playerDashingAttributes.playerGroundDashTime;

                        hasAirDashed = false;
                    }
                    else if (!hasAirDashed)
                    {
                        dashDistance = (playerDashDirection == DIRECTION.LEFT) ? -playerDashingAttributes.playerAirDashDistance : playerDashingAttributes.playerAirDashDistance;
                        dashTime = playerDashingAttributes.playerGroundDashTime;

                        hasAirDashed = true;
                    }
                    else
                    {
                        return;
                    }

                    playerDashVelocity = CalculateDashAcceleration(dashDistance, dashTime);

                    playerDashEndTime = Time.time + dashTime;

                    playerVelocityBeforeDash = playerVelocity;

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
                if (playerDashDirection == DIRECTION.LEFT ? playerCollision.collisionData.isCollidingLeft : playerCollision.collisionData.isCollidingRight)
                {
                    foreach (KeyValuePair<int, CollisionInfo2D> collisionDataPair in playerCollision.collisionData.collidedObjects)
                    {
                        if (collisionDataPair.Value.collisionDirection == playerDashDirection)
                        {
                            if (Helper.LayerMaskContainsLayer(playerDashingAttributes.dashCollisionLayer, collisionDataPair.Value.collidedObject.layer))
                            {
                                EndDash();
                            }
                        }
                    }
                }

                playerVelocity.x += playerDashVelocity.x * Time.deltaTime;
                playerVelocity.y = 0;
            }
            else
            {
                EndDash();
            }
        }
        else
        {
            if (hasAirDashed)
            {
                if (playerCollision.collisionData.isCollidingBelow || playerCollision.playerWallCollisionData.isAdjacentToClimbableWall)
                {
                    hasAirDashed = false;
                }
            }
        }   
    }

    private void EndDash()
    {
        playerVelocity = playerVelocityBeforeDash;

        playerDashDirection = 0;

        playerDashVelocity = Vector2.zero;
        playerVelocityBeforeDash = Vector2.zero;

        playerDashEndTime = 0;

        playerVelocity.x = 0;
        playerVelocity.y = 0;

        playerMovementSmoothVelocity = 0;

        isDashing = false;
    }

    public void PlayerAttack()
    {
        playerAttackController.RequestAttack(playerCollision.collisionData.faceDirection);
    }
}

[System.Serializable]
public struct PlayerMovementAttributes2D
{
    [Header("Player Movement Values")]
    public float playerBaseMovementSpeed;
    public float playerSprintMovementSpeed;

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
    public float playerGroundDashDistance;
    public float playerGroundDashTime;

    [Space(10)]
    public float playerAirDashDistance;
    public float playerAirDashTime;

    [Space(20)]
    public LayerMask dashCollisionLayer;
}
