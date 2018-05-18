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

    private bool isPlayerGrounded = true;

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

    [Space(10)]
    public bool canPlayerJump = true;
    public bool canPlayerWallJump = true;

    [Header("Player Wall Climbing Attributes")]
    public PlayerWallClimbingAttributes2D playerWallClimbAttributes;

    [Space(10)]
    private int wallDirectionX;

    private float wallStickTime;

    [Space(10)]
    public bool canPlayerWallSlide = true;

    [Space(10)]
    public bool isWallSliding = false;

    [Header("Player Static Velocity Attributes")]
    private Vector2 staticVelocity;

    private float staticVelocityTime;
    private float staticVelocityEndTime;

    private bool isPlayerBeingMovedByStaticVelocity = false;

    [Header("Player Dashing Attributes")]
    public PlayerDashingAttributes2D playerDashingAttributes;

    private DIRECTION playerDashDirection;

    private Vector2 playerDashVelocity;

    private float playerDashTime;
    private float playerDashEndTime;

    private bool hasAirDashed = false;

    [Space(10)]
    public bool canPlayerDash = true;

    [Space(10)]
    public bool isDashing = false;

    [Header("Player Attack Attributes")]
    public PlayerAttackController2D playerAttackController;

    [Space(10)]
    public bool canPlayerAttack = true;

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
        ManagePlayerMovement();
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

    public override void HandleSpriteFaceDirection(int faceDirection)
    {
        base.HandleSpriteFaceDirection(faceDirection);
    }

    public void ReceiveInputData(Vector2 rawInput, Vector2 inputDirection, PlayerCapturedInput playerCapturedInput)
    {
        playerInput = rawInput;
        playerInputDirection = inputDirection;
    }

    private void ManagePlayerMovement()
    {
        if (isDashing || isPlayerBeingMovedByStaticVelocity)
        {
            HandlePlayerDash();
            HandleControllerStaticVelocity();

            HandlePlayerWallSliding();

            MoveControllerStaticVelocity(playerVelocity * Time.deltaTime, false);
        }
        else
        {
            CalculatePlayerVelocity(playerInput);

            HandlePlayerWallSliding();

            MoveController(playerVelocity * Time.deltaTime, false);

            playerAnimation.SetBool("isGrounded", playerCollision.collisionData.isCollidingBelow);

            if (playerCollision.collisionData.isCollidingAbove || playerCollision.collisionData.isCollidingBelow)
            {
                if (!isDashing || !isPlayerBeingMovedByStaticVelocity)
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
        }

        ManagePlayerGrounding();

        HandleSpriteFaceDirection(playerCollision.collisionData.faceDirection);
    }

    private void CalculatePlayerVelocity(Vector2 rawInput)
    {
        if (canPlayerMove)
        {
            playerMovementTargetSpeed = ReturnPlayerTargetSpeed(rawInput);

            playerVelocity.x = Mathf.SmoothDamp(playerVelocity.x, playerMovementTargetSpeed, ref playerMovementSmoothVelocity, ReturnPlayerSmoothMovementTime());

            if (!isDashing || !isPlayerBeingMovedByStaticVelocity)
            {
                playerVelocity.y += playerGravity * Time.deltaTime;
            }
        }
    }

    public override void MoveController(Vector2 finalPlayerVelocity, bool isOnPlatform = false)
    {
        playerCollision.ManageObjectCollisions(ref finalPlayerVelocity, playerInputDirection);

        transform.Translate(finalPlayerVelocity);

        playerAnimation.SetFloat("moveSpeed", finalPlayerVelocity.x * playerMovementTargetSpeed);

        if (isOnPlatform)
        {
            playerCollision.collisionData.isCollidingBelow = true;
        }
    }

    public override void MoveControllerStaticVelocity(Vector2 finalPlayerVelocity, bool isOnPlatform)
    {
        playerCollision.ManageObjectCollisions(ref finalPlayerVelocity, playerInputDirection);

        transform.Translate(finalPlayerVelocity);

        playerAnimation.SetFloat("moveSpeed", finalPlayerVelocity.x * playerMovementTargetSpeed);

        if (isOnPlatform)
        {
            playerCollision.collisionData.isCollidingBelow = true;
        }
    }

    private void ManagePlayerGrounding()
    {
        isPlayerGrounded = playerCollision.collisionData.isCollidingBelow;

        if (isPlayerGrounded)
        {
            if (hasAirDashed)
            {
                hasAirDashed = false;
            }
        }
    }

    public override void ResetController()
    {
        //playerAudio.RequestAudioEvent("DeathSFX");

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
            if (canPlayerWallJump)
            {
                Vector2 newWallClimbVec = ReturnWallJumpVelocity(playerInputDirection, wallDirectionX);

                playerVelocity = newWallClimbVec;

                playerAnimation.SetTrigger("isJump");
            }
        }
        else if (playerCollision.collisionData.isCollidingBelow)
        {
            if (canPlayerJump)
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
                }
            }          
        }
    }

    private Vector2 ReturnWallJumpVelocity(Vector2 inputDirection, float wallDirection)
    {
        Vector2 newWallJumpVelocity = Vector2.zero;

        if (wallDirection == inputDirection.x)
        {
            newWallJumpVelocity.x = -wallDirectionX * playerWallClimbAttributes.wallJumpClimb.x;
            newWallJumpVelocity.y = playerWallClimbAttributes.wallJumpClimb.y;
        }
        else if (inputDirection.x == 0)
        {
            newWallJumpVelocity.x = -wallDirectionX * playerWallClimbAttributes.wallJumpOff.x;
            newWallJumpVelocity.y = playerWallClimbAttributes.wallJumpOff.y;
        }
        else
        {
            newWallJumpVelocity.x = -wallDirectionX * playerWallClimbAttributes.wallJumpLeap.x;
            newWallJumpVelocity.y = playerWallClimbAttributes.wallJumpLeap.y;
        }

        return newWallJumpVelocity;
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
        CalculateGravity();

        playerMinJumpVelocity.y = Mathf.Sqrt(2 * Mathf.Abs(playerGravity) * playerJumpAttributes.playerMinJumpHeight);
        playerMaxJumpVelocity.y = Mathf.Abs(playerGravity) * playerJumpAttributes.playerJumpTime;
    }

    private void CalculateGravity()
    {
        playerGravity = -(2 * playerJumpAttributes.playerMaxJumpHeight / Mathf.Pow(playerJumpAttributes.playerJumpTime, 2));
    }

    private void HandlePlayerWallSliding()
    {
        wallDirectionX = (playerCollision.collisionData.isCollidingLeft) ? -1 : 1;

        isWallSliding = false;

        if (canPlayerWallSlide)
        {
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

        playerAnimation.SetBool("isAgainstWall", isWallSliding);
    }

    public void PlayerDash()
    {
        if (canPlayerDash)
        {
            if (!isDashing)
            {
                if (playerInputDirection.x == 0)
                {
                    playerDashDirection = (playerCollision.collisionData.faceDirection == -1) ? DIRECTION.LEFT : DIRECTION.RIGHT;
                }
                else
                {
                    playerDashDirection = (playerInputDirection.x == -1) ? DIRECTION.LEFT : DIRECTION.RIGHT;
                }

                Vector2 dashAccelerationVector = Vector2.zero;

                float dashDistance = 0;
                float dashTime = 0;

                if (playerCollision.collisionData.isCollidingBelow)
                {
                    dashAccelerationVector = playerDashingAttributes.playerGroundDashDistance;

                    dashDistance = playerDashingAttributes.playerGroundDashDistance.magnitude;
                    dashTime = playerDashingAttributes.playerGroundDashTime;

                    hasAirDashed = false;
                }
                else if (!hasAirDashed)
                {
                    dashAccelerationVector = playerDashingAttributes.playerAirDashDistance;

                    dashDistance = playerDashingAttributes.playerAirDashDistance.magnitude;
                    dashTime = playerDashingAttributes.playerAirDashTime;

                    hasAirDashed = true;
                }
                else
                {
                    return;
                }

                playerDashVelocity = (playerDashDirection == DIRECTION.LEFT) ? CalculateRequiredVelocity(dashAccelerationVector, -dashDistance, dashTime) : CalculateRequiredVelocity(dashAccelerationVector, dashDistance, dashTime);

                playerDashTime = 0;
                playerDashEndTime = dashTime;

                playerVelocity = Vector2.zero;

                playerMovementSmoothVelocity = 0;

                playerAnimation.SetTrigger("isDash");

                isDashing = true;
            }
        }
    }

    private void HandlePlayerDash()
    {
        if (isDashing)
        {
            if (playerDashTime < playerDashEndTime)
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

                playerVelocity += playerDashVelocity * Time.deltaTime;

                playerDashTime += Time.deltaTime;
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

        playerDashTime = 0;
        playerDashEndTime = 0;

        playerVelocity = Vector2.zero;
        playerMovementSmoothVelocity = 0;

        isDashing = false;
    }

    public override void SetNewStaticVelocity(Vector2 velocity, float timeValue)
    {
        if (canPlayerMove)
        {
            staticVelocity = playerCollision.collisionData.faceDirection == -1 ? CalculateRequiredVelocity(velocity, -velocity.magnitude, timeValue) : CalculateRequiredVelocity(velocity, velocity.magnitude, timeValue);

            staticVelocityTime = 0;
            staticVelocityEndTime = timeValue;

            playerVelocity = Vector2.zero;
            playerMovementSmoothVelocity = 0;

            isPlayerBeingMovedByStaticVelocity = true;
        }
    }

    private void HandleControllerStaticVelocity()
    {
        if (isPlayerBeingMovedByStaticVelocity)
        {
            if (staticVelocityTime < staticVelocityEndTime)
            {
                playerVelocity += staticVelocity * Time.deltaTime;

                if (!playerCollision.collisionData.isCollidingBelow)
                {
                    playerVelocity.y += playerGravity * Time.deltaTime;
                }

                staticVelocityTime += Time.deltaTime;
            }
            else
            {
                EndAcceleration();
            }
        }
    }

    private void EndAcceleration()
    {
        staticVelocity = Vector2.zero;

        staticVelocityTime = 0;
        staticVelocityEndTime = 0;

        playerMovementSmoothVelocity = 0;

        playerVelocity = Vector2.zero;

        isPlayerBeingMovedByStaticVelocity = false;
    }

    public void SwitchWeapons(int newWeaponID)
    {
        playerAttackController.SetNewWeapon(newWeaponID);
    }

    public void PlayerAttack()
    {
        if (canPlayerAttack)
        {
            playerAttackController.RequestAttack(playerCollision.collisionData.faceDirection);
        }
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

    [Space(10)]
    public int playerJumpCount;
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
    public Vector2 playerGroundDashDistance;
    public float playerGroundDashTime;

    [Space(10)]
    public Vector2 playerAirDashDistance;
    public float playerAirDashTime;

    [Space(20)]
    public LayerMask dashCollisionLayer;
}
