using System.Collections;
using UnityEngine;

[RequireComponent(typeof (CharacterController))]
[RequireComponent(typeof (FirstPersonInputManager))]

//@Chronoblast
public class FirstPersonPlayerController : PlayerController
{
    private FirstPersonCameraController firstPersonCameraController;

    [Header("Player Attributes")]
    private CharacterController playerCharacterController;

    private InputManager playerInput;

    [Header("Player Physics Attributes")]
    public float physicsCollisionMultiplier = 0f;

    [Space(10)]
    public bool canInteractWithRigidbodies = true;

    [Header("Player Movement Attributes")]
    public float playerBaseForwardSpeed = 1;
    public float playerBaseStrafeSpeed = 1;
    public float playerBaseBackwardSpeed = 1;

    [Space(10)]
    public float playerSprintSpeed;

    [Space(10)]
    public float playerMoveSmoothTime = 0.25f;

    [Space(10)]
    [Range(0, 1)]
    public float airControlValue;

    [Space(10)]
    [Range(0, 1)]
    public float wallRunControlValue;

    [Space(10)]
    public float playerCurrentMovementVelocity;

    private float playerTargetSpeed = 1;
    private float playerCurrentSpeed;

    [Space(10)]
    public bool canMovePlayer = true;

    [Space(10)]
    public bool isSprinting = false;

    [Space(10)]
    public bool isWallRunning = false;

    private Vector3 inputVector;
    private Vector3 desiredMove;

    private Vector2 lookVector;

    [Header("Player Jump Attributes")]
    public float playerNormalJumpForce = 1f;
    [Space(10)]
    public float playerWallJumpForce = 1f;

    private Vector3 playerGravity = Physics.gravity;

    [Space(10)]
    public float playerTerminalVelocity = -100;

    [Space(10)]
    public bool canJump = true;

    private Vector3 jumpVelocity;

    [Header("Player Wall Run Attributes")]
    public float playerWallRunSpeedThreshold;

    [Space(10)]
    public float playerWallRunGravityReductionMuliplier;

    [Header("Player Wall Climb Attributes")]
    public GameObject splinePrefab;

    private GameObject instantiatedSpline;

    [Space(10)]
    public float playerWallClimbGrabDistance = 1f;

    [Space(10)]
    public float playerWallClimbHorizontalDistance = 1f;
    public float playerWallClimbVerticalDistance = 1f;

    [Space(10)]
    [Range(0.05f, 1)]
    public float playerWallClimbSpeed = 0.05f;

    [Space(10)]
    public bool isWallClimbing = false;

    private Vector3[] wallClimbPoints = new Vector3[4];

    private RaycastHit wallClimbRayHit;

    [Header("Player Wall Adjustment Values")]
    public float playerWallRayLength;

    [Space(10)]
    public Vector3 attachedWallVector;
    public Vector3 attachedRelativeWallVector;

    private GameObject attachedWallObject;

    private Vector3 previousWallJumpVector;

    [Space(10)]
    public LayerMask wallMask;

    [Space(10)]
    public bool canCheckForWalls = true;

    [Space(10)]
    public bool isNextToWall = false;
    public bool canGrabWall = false;

    private RaycastHit wallRayHit;

    [Header("Player Ground Adjustment Values")]
    public float playerHeight = 0.5f;
    public float heightPadding = 0.05f;

    [Space(10)]
    public LayerMask groundMask;

    [Space(10)]
    public bool isGrounded = true;

    private bool canCheckForGround = true;

    private Vector3[] groundCheckVectors = new Vector3[4];

    [Header("Slope Adjustment Values")]
    public float slopeDetectionRayLength = 1;

    [Space(10)]
    public float currentSlopeAngle = 0;
    public float maxSlopeAngle = 120f;
    public float slopeAdjustTime = 5;

    private Vector3 currentGroundNormal;

    [Space(10)]
    public bool isOnSlope = false;

    private RaycastHit slopeHit;

    private void Start()
    {
        InitializePlayer();
    }

    private void Update()
    {
        ManageGround();

        ManageMovement();
    }

    private void InitializePlayer()
    {
        playerInput = GetComponent<InputManager>();

        playerCharacterController = GetComponent<CharacterController>();

        firstPersonCameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<FirstPersonCameraController>();

        groundCheckVectors[0] = new Vector3(-playerCharacterController.radius / 2, 0f, -playerCharacterController.radius / 2);
        groundCheckVectors[1] = new Vector3(playerCharacterController.radius / 2, 0f, -playerCharacterController.radius / 2);
        groundCheckVectors[2] = new Vector3(-playerCharacterController.radius / 2, 0f, playerCharacterController.radius / 2);
        groundCheckVectors[3] = new Vector3(playerCharacterController.radius / 2, 0f, playerCharacterController.radius / 2);
    }

    private void ManageMovement()
    {      
        if (inputVector.x > 0 || inputVector.x < 0) //Determine Input Direction
        {
            playerTargetSpeed = playerBaseStrafeSpeed * inputVector.magnitude; //Strafe
        }
        if (inputVector.z < 0)
        {
            playerTargetSpeed = playerBaseBackwardSpeed * inputVector.magnitude; //Backwards
        }
        if (inputVector.z > 0)
        {
            if (isSprinting && isGrounded)
            {
                playerTargetSpeed = playerSprintSpeed * inputVector.magnitude; //Forward
            }
            else
            {
                playerTargetSpeed = playerBaseForwardSpeed * inputVector.magnitude; //Forward
            }
        }

        playerCurrentSpeed = Mathf.SmoothDamp(playerCurrentSpeed, playerTargetSpeed, ref playerCurrentMovementVelocity, GetCurrentSmoothTime(playerMoveSmoothTime)); //Dampen Movement Speed to have speed build up

        if (canMovePlayer)
        {
            if (isGrounded) //If grounded, move at default speed
            {
                if (currentSlopeAngle <= maxSlopeAngle)
                {
                    Vector3 calculatedMove = transform.forward * inputVector.z + transform.right * inputVector.x;

                    desiredMove = Vector3.ProjectOnPlane(calculatedMove, currentGroundNormal).normalized;

                    desiredMove *= playerCurrentSpeed;

                    desiredMove += jumpVelocity;

                    playerCharacterController.Move(desiredMove * Time.deltaTime);

                    Debug.DrawRay(transform.position, desiredMove, Color.blue);
                }
            }
            else //If airborne, move at modified speed
            {
                Vector3 calculatedMove = transform.forward * inputVector.z + transform.right * inputVector.x;

                desiredMove = Vector3.ProjectOnPlane(calculatedMove, currentGroundNormal).normalized;

                desiredMove *= playerCurrentSpeed;

                if (jumpVelocity.y > playerTerminalVelocity)
                {
                    if (isNextToWall)
                    {
                        if (!isWallRunning)
                        {
                            if (playerCurrentSpeed > playerWallRunSpeedThreshold)
                            {
                                WallRunCameraUpdate();
                            }

                            isWallRunning = true;
                        }

                        jumpVelocity += (Time.deltaTime * playerGravity / playerWallRunGravityReductionMuliplier); //Apply Gravity
                    }
                    else
                    {
                        if (isWallRunning)
                        {
                            WallRunCameraReset();
                        }

                        isWallRunning = false;

                        jumpVelocity += (Time.deltaTime * playerGravity); //Apply Gravity
                    }
                }

                desiredMove += jumpVelocity;

                playerCharacterController.Move(desiredMove * Time.deltaTime);

                Debug.DrawRay(transform.position, desiredMove, Color.blue);
            }
        }
    }

    public override void StartSprint()
    {
        if (!isSprinting)
        {
            isSprinting = true;
        }
    }

    public override void StopSprint()
    {
        if (isSprinting)
        {
            isSprinting = false;
        }
    }

    public override void Interact()
    {
        Debug.Log("Interact :: ");
    }

    public override void Jump()
    {
        if (canJump)
        {
            if (isGrounded)
            {
                if (isNextToWall)
                {
                    if (CanClimb(attachedWallVector))
                    {
                        BezierSpline newSpline = GenerateNewClimbSpline();

                        StartCoroutine(PlayerClimb(newSpline, playerWallClimbSpeed));
                    }
                    else
                    {
                        GroundJump();
                    }
                }
                else
                {
                    GroundJump();
                }
            }
            else if (!isGrounded)
            {
                if (isNextToWall) //Try to Attach To Wall
                {
                    if (CanClimb(attachedWallVector))
                    {
                        BezierSpline newSpline = GenerateNewClimbSpline();

                        StartCoroutine(PlayerClimb(newSpline, playerWallClimbSpeed));
                    }
                    else
                    {
                        WallJump(attachedWallVector);
                    }
                }
            }
        }     
    }

    private void GroundJump()
    {
        float newJumpVelocity = Mathf.Sqrt(-2 * playerGravity.y * playerNormalJumpForce); //Kinematic Equation for calculating Jump Force

        Vector3 newJumpVector = Vector3.up * newJumpVelocity;

        jumpVelocity = newJumpVector;

        isGrounded = false;

        StartCoroutine(ResetGroundCheck());
    }

    private void WallJump(Vector3 attachedWallVector)
    {
        if (previousWallJumpVector != attachedWallVector)
        {
            Vector3 reflectedWallVector = ReturnReflectedJumpVector(attachedWallVector);

            jumpVelocity = reflectedWallVector;

            previousWallJumpVector = attachedWallVector;

            isNextToWall = false;

            StartCoroutine(ResetWallCheck());
        }
        else
        {
            Debug.Log("Same Wall Vector :: " + previousWallJumpVector + " :: " + attachedWallVector);
        }
    }

    private bool CanClimb(Vector3 attachedWallVector)
    {
        if (Physics.Raycast(transform.position, transform.forward, out wallClimbRayHit, playerWallClimbGrabDistance, wallMask)) //If there is a climbable wall in front of me, continue
        {
            if (attachedWallVector == Vector3.zero) //If wall isn't close enough, exit out.
            {
                return false;
            }

            if (wallRayHit.collider.gameObject != attachedWallObject) //If the object in front of me is not the same wall I'm attached to, exit out.
            {
                return false;
            }

            Debug.Log(wallRayHit.collider.bounds);

            Vector3 firstPoint = Vector3.zero; //Calculating points for Spline.
            Vector3 secondPoint = Vector3.up * playerWallClimbVerticalDistance;
            Vector3 thirdPoint = secondPoint + (attachedWallVector * playerWallClimbHorizontalDistance / 2);
            Vector3 fourthPoint = secondPoint + (attachedWallVector * playerWallClimbHorizontalDistance);

            if (Physics.Raycast(thirdPoint + transform.position, Vector3.down, out wallClimbRayHit, wallMask))
            {
                thirdPoint = wallClimbRayHit.point;

                thirdPoint += Vector3.up * playerHeight;

                thirdPoint -= transform.position;
            }
            else
            {
                return false;
            }

            if (Physics.Raycast(fourthPoint + transform.position, Vector3.down, out wallClimbRayHit, wallMask))
            {
                fourthPoint = wallClimbRayHit.point;

                fourthPoint += Vector3.up * playerHeight;

                fourthPoint -= transform.position;
            }
            else
            {
                return false;
            }

            if (!Physics.CheckSphere(fourthPoint + transform.position, 0.5f, wallMask)) //Make sure climbing area is clear.
            {     
                if (Physics.Raycast(fourthPoint, Vector3.down, wallMask)) //Make sure there is ground to stand on.
                {
                    wallClimbPoints[0] = firstPoint; //Set points.
                    wallClimbPoints[1] = secondPoint;
                    wallClimbPoints[2] = thirdPoint;
                    wallClimbPoints[3] = fourthPoint;

                    return true;
                }
            }
        }
       
        return false;
    }

    private Vector3 ReturnReflectedJumpVector(Vector3 wallVector)
    {
        Vector3 newReflectVector = -wallVector + Vector3.up;

        newReflectVector *= playerWallJumpForce;

        return newReflectVector;
    }

    private BezierSpline GenerateNewClimbSpline()
    {
        if (instantiatedSpline == null)
        {
            GameObject newSplinePrefab = Instantiate(splinePrefab, transform.position, Quaternion.identity) as GameObject;

            instantiatedSpline = newSplinePrefab;
        }

        instantiatedSpline.transform.position = transform.position;
     
        BezierSpline newSpline = instantiatedSpline.GetComponent<BezierSpline>();

        for (int i = wallClimbPoints.Length - 1; i >= 0; i--) //Cycle through Bezier Spline Points to set their new positions.
        {           
            newSpline.SetControlPointMode(i, BezierControlPointMode.Free);

            newSpline.SetControlPoint(i, wallClimbPoints[i]);
        }

        return newSpline;
    }

    private IEnumerator PlayerClimb (BezierSpline climbSpline, float wallScaleTime)
    {
        canMovePlayer = false;
        canJump = false;
        canGrabWall = false;
        canCheckForWalls = false;
        canCheckForGround = false;

        float splineClimbProgress = 0;

        playerCurrentSpeed = 0;

        while (splineClimbProgress < 1)
        {
            splineClimbProgress += Time.deltaTime / wallScaleTime;

            if (splineClimbProgress > 1)
            {
                splineClimbProgress = 1;
            }

            Vector3 newPlayerSplinePosition = climbSpline.GetPoint(splineClimbProgress);

            transform.localPosition = newPlayerSplinePosition;

            yield return new WaitForEndOfFrame();
        }
              
        canMovePlayer = true;
        canJump = true;
        canGrabWall = true;
        canCheckForWalls = true;
        canCheckForGround = true;

        yield return null;
    }

    private void ManageGround()
    {
        WallCheck();

        GroundCheck();

        SlopeCheck();
    }

    private void WallCheck() //Check if there are walls next to the player
    {
        if (canCheckForWalls)
        {           
            if (Physics.Raycast(transform.position, Vector3.right, out wallRayHit, playerWallRayLength, wallMask))
            {
                Debug.DrawRay(transform.position, Vector3.right * playerWallRayLength, Color.green);

                attachedWallVector = Vector3.right;

                attachedWallObject = wallRayHit.collider.gameObject;

                isNextToWall = true;
            }
            else if (Physics.Raycast(transform.position, Vector3.left, out wallRayHit, playerWallRayLength, wallMask))
            {
                Debug.DrawRay(transform.position, Vector3.left * playerWallRayLength, Color.green);

                attachedWallVector = Vector3.left;

                attachedWallObject = wallRayHit.collider.gameObject;

                isNextToWall = true;
            }
            else if (Physics.Raycast(transform.position, Vector3.forward, out wallRayHit, playerWallClimbGrabDistance, wallMask))
            {
                Debug.DrawRay(transform.position, Vector3.forward * playerWallRayLength, Color.green);

                attachedWallVector = Vector3.forward;

                attachedWallObject = wallRayHit.collider.gameObject;

                isNextToWall = true;
            }
            else if (Physics.Raycast(transform.position, Vector3.back, out wallRayHit, playerWallClimbGrabDistance, wallMask))
            {
                Debug.DrawRay(transform.position, Vector3.back * playerWallRayLength, Color.green);

                attachedWallVector = Vector3.back;

                attachedWallObject = wallRayHit.collider.gameObject;

                isNextToWall = true;
            }
            else
            {
                Debug.DrawRay(transform.position, Vector3.right * playerWallRayLength, Color.red);
                Debug.DrawRay(transform.position, Vector3.left * playerWallRayLength, Color.red);
                Debug.DrawRay(transform.position, Vector3.forward * playerWallRayLength, Color.red);
                Debug.DrawRay(transform.position, Vector3.back * playerWallRayLength, Color.red);

                attachedWallVector = Vector3.zero;

                attachedWallObject = null;

                isNextToWall = false;
            }

            if (Physics.Raycast(transform.position, -transform.right, out wallRayHit, playerWallClimbGrabDistance, wallMask))
            {
                attachedRelativeWallVector = Vector3.left;
            }
            else if (Physics.Raycast(transform.position, transform.right, out wallRayHit, playerWallClimbGrabDistance, wallMask))
            {
                attachedRelativeWallVector = Vector3.right;
            }
            else if (Physics.Raycast(transform.position, -transform.forward, out wallRayHit, playerWallClimbGrabDistance, wallMask))
            {
                attachedRelativeWallVector = Vector3.back;
            }
            else if (Physics.Raycast(transform.position, transform.forward, out wallRayHit, playerWallClimbGrabDistance, wallMask))
            {
                attachedRelativeWallVector = Vector3.forward;
            }
            else
            {
                attachedRelativeWallVector = Vector3.zero;
            }
        }       
    }

    private void GroundCheck() //Check for the ground
    {
        if (canCheckForGround)
        {
            for (int i = 0; i < groundCheckVectors.Length; i++)
            {
                if (Physics.Raycast(groundCheckVectors[i] + transform.position, Vector3.down, playerHeight +heightPadding, groundMask))
                {
                    Debug.DrawRay(groundCheckVectors[i] + transform.position, Vector3.down, Color.green); 
                }
                else
                {
                    Debug.DrawRay(groundCheckVectors[i] + transform.position, Vector3.down, Color.red);
                }
            }

            if (Physics.Raycast(transform.position, Vector3.down, playerHeight + heightPadding, groundMask)) //Basic Ground Check
            {
                Debug.DrawRay(transform.position, Vector3.down * (playerHeight + heightPadding), Color.green);

                isGrounded = true;

                jumpVelocity = Vector3.zero;

                previousWallJumpVector = Vector3.zero;
            }
            else
            {
                Debug.DrawRay(transform.position, Vector3.down * (playerHeight + heightPadding), Color.red);

                isGrounded = false;
            }
        }        
    }

    private void SlopeCheck()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, slopeDetectionRayLength + heightPadding, groundMask))
        {
            Debug.DrawRay(transform.position, Vector3.down * (slopeDetectionRayLength + heightPadding), Color.green);

            currentGroundNormal = slopeHit.normal; //Set Current Normal based on groundHit Normal     
        }
        else
        {
            Debug.DrawRay(transform.position, Vector3.down * (slopeDetectionRayLength + heightPadding), Color.red);

            currentGroundNormal = Vector3.up; //If airborne, set ground normal to default value (flat surface)
        }

        CalculateGroundAngle();
    }

    private void CalculateGroundAngle() //Find the ground angle of the surface you are on
    {
        if (!isGrounded)
        {
            currentSlopeAngle = 90;

            isOnSlope = false;

            return;
        }

        currentSlopeAngle = Vector3.Angle(currentGroundNormal, transform.forward);

        if (currentSlopeAngle != 90)
        {
            isOnSlope = true;
        }
        else
        {
            isOnSlope = false;
        }
    }
  
    public override void ReceiveInputVectors(Vector2 inputMovementVector, Vector2 inputHeadVector) //Input passthrough from Input Manager class
    {
        inputVector.Set (inputMovementVector.x, 0, inputMovementVector.y);
        
        if (inputVector.sqrMagnitude > 1)
        {
            inputVector.Normalize();
        }

        lookVector = inputHeadVector;
    }

    private float GetCurrentSmoothTime(float baseSmoothTime)
    {
        if (isGrounded)
        {
            return baseSmoothTime; //If is grounded, return unmodified smooth time
        }
        else if (isWallRunning)
        {
            return wallRunControlValue;
        }

        if (airControlValue == 0)
        {
            return float.MaxValue; //If not grounded and aircontrol is set to 0, return unmodified smooth time
        }

        return baseSmoothTime / airControlValue; // if is airborne, return modified smoothtime
    }

    private IEnumerator ResetGroundCheck()
    {
        canCheckForGround = false;

        yield return new WaitForSeconds(0.1f);

        canCheckForGround = true;

        yield return null;
    }

    private IEnumerator ResetWallCheck()
    {
        canCheckForWalls = false;

        yield return new WaitForSeconds(0.1f);

        canCheckForWalls = true;

        yield return null;
    }

    private void WallRunCameraUpdate()
    {
       if (attachedRelativeWallVector == Vector3.right)
       {
          firstPersonCameraController.SetCameraWallRunAngle(DIRECTION.RIGHT);
       }
       else if (attachedRelativeWallVector == Vector3.left)
       {
          firstPersonCameraController.SetCameraWallRunAngle(DIRECTION.LEFT);
       }
    }

    private void WallRunCameraReset()
    {
        firstPersonCameraController.ResetCameraWallRunAngle();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (canInteractWithRigidbodies)
        {
            if (hit.gameObject.GetComponent<Rigidbody>())
            {
                Vector3 calculatedForce = transform.forward * playerCurrentSpeed;

                calculatedForce *= physicsCollisionMultiplier;

                hit.gameObject.GetComponent<Rigidbody>().AddForce(calculatedForce, ForceMode.Impulse);
            }
        }      
    }
}
