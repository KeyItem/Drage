using System.Collections;
using UnityEngine;

public class PlayerClimbController : MonoBehaviour
{
    private ThirdPersonPlayerController playerController;

    [Header("Climbing System Attributes")]
    public LayerMask climbObjectMask;
    public LayerMask climbCollisionMask;

    private PlayerClimbingAttributes playerClimbingAttributes;

    private float[] heightCheckPadding = new float[3] {-0.4f, 0, 0.4f};

    private GameObject targetClimbObject;
    private Vector3 rayHitPoint;
    private RaycastHit rayHit;

    private bool isClimbing = false;

    [Header("Spline Attributes")]
    public GameObject newSplineGameObject;

    [Space(10)]
    private BezierSpline splineObject;

    [Header("DEBUG")]
    public bool canShowDebug = false;

    private void Start()
    {
        InitializeClimbSystem();
    }

    private void FixedUpdate()
    {
        WallCheck();
    }

    private void InitializeClimbSystem()
    {
        playerController = GetComponent<ThirdPersonPlayerController>();

        playerClimbingAttributes = playerController.playerClimbingAttributes;
    }

    private void WallCheck()
    {
        for (int i = 0; i < heightCheckPadding.Length; i++)
        {
            if (Physics.Raycast(transform.position - (Vector3.up * heightCheckPadding[i]), transform.forward, out rayHit, playerClimbingAttributes.playerClimbRayLength, climbObjectMask))
            {
                rayHitPoint = rayHit.point;

                targetClimbObject = rayHit.collider.gameObject;

                if (canShowDebug)
                {
                    Debug.DrawRay(transform.position, (transform.forward * playerClimbingAttributes.playerClimbRayLength), Color.green);
                }

                break;
            }
            else
            {
                rayHitPoint = Vector3.zero;

                targetClimbObject = null;

                if (canShowDebug)
                {
                    Debug.DrawRay(transform.position - (Vector3.up * heightCheckPadding[i]), (transform.forward * playerClimbingAttributes.playerClimbRayLength), Color.red);
                }
            }
        }    
    }

    public bool TryClimb()
    {
        if (rayHitPoint != Vector3.zero)
        {
            if (isClimbing)
            {
                return false;
            }

            //Get Collider Bounds, Determine if it will be either a scale or a climb

            Vector3 objectDirection = (rayHitPoint - transform.position).normalized;
            Vector3 playerDirection = Vector3.zero;

            if (Mathf.Abs(objectDirection.x) > Mathf.Abs(objectDirection.z)) //Left or Right
            {
                if (objectDirection.x > 0)
                {
                    playerDirection = Vector3.right;
                }
                else if (objectDirection.x < 0)
                {
                    playerDirection = Vector3.left;
                }
            }
            else //Forward/Backward
            {
                if (objectDirection.z > 0)
                {
                    playerDirection = Vector3.forward;
                }
                else if (objectDirection.z < 0)
                {
                    playerDirection = Vector3.back;
                }
            }

            Vector3 hitColliderBounds = targetClimbObject.GetComponent<Collider>().bounds.size;

            if (CanScale(hitColliderBounds, playerDirection) || CanVault(hitColliderBounds, playerDirection))
            {
                return true;
            }

        }
        return false;
    }

    public void StartClimb()
    {
        if (rayHitPoint != Vector3.zero)
        {
            //Get Collider Bounds, Determine if it will be either a scale or a climb

            Vector3 objectDirection = (rayHitPoint - transform.position).normalized;
            Vector3 playerDirection = Vector3.zero;

            float playerDistanceToObject = Vector3.Distance(transform.position, rayHitPoint);

            if (Mathf.Abs(objectDirection.x) > Mathf.Abs(objectDirection.z)) //Left or Right
            {
                if (objectDirection.x > 0)
                {
                    playerDirection = Vector3.right;
                }
                else if (objectDirection.x < 0)
                {
                    playerDirection = Vector3.left;
                }
            }
            else //Forward/Backward
            {
                if (objectDirection.z > 0)
                {
                    playerDirection = Vector3.forward;
                }
                else if (objectDirection.z < 0)
                {
                    playerDirection = Vector3.back;
                }
            }

            Vector3 hitColliderBounds = targetClimbObject.GetComponent<Collider>().bounds.size;

            if (CanScale(hitColliderBounds, playerDirection) && CanVault(hitColliderBounds, playerDirection))
            {
                VaultObject(targetClimbObject, hitColliderBounds, playerDirection, playerDistanceToObject); //If can do both, prefer Vaulting
            }
            else if (CanVault(hitColliderBounds, playerDirection))
            {
                VaultObject(targetClimbObject, hitColliderBounds, playerDirection, playerDistanceToObject); //If can, Vault
            }
            else if (CanScale(hitColliderBounds, playerDirection))
            {
                ScaleObject(targetClimbObject, hitColliderBounds, playerDirection, playerDistanceToObject); //If can, Scale 
            }
        }
    }

    private bool CanScale(Vector3 scaleObjectBounds, Vector3 playerDirection)
    {
        if (scaleObjectBounds.y < playerClimbingAttributes.playerScalingMinVertical || scaleObjectBounds.y > playerClimbingAttributes.playerScalingMaxVertical)
        {
            return false;  //If Higher than max scaling height or smaller than minimum scale height, return false;
        }

        return true;
    }

    private bool CanVault(Vector3 vaultObjectBounds, Vector3 playerDirection)
    {
        if (vaultObjectBounds.y < playerClimbingAttributes.playerVaultingVerticalMin || vaultObjectBounds.y > playerClimbingAttributes.playerVaultingVerticalMax)
        {
            return false; //If Higher than max Vaulting height or smaller than minimum scale height, return false;
        }

        if (playerDirection == Vector3.left || playerDirection == Vector3.right) //If Left or Right
        {
            if (vaultObjectBounds.x < playerClimbingAttributes.playerVaultingHorizontalMin || vaultObjectBounds.x > playerClimbingAttributes.playerVaultingHorizontalMax)
            {
                return false; //If not between both horizontal limits, return false;
            }
        }
        else if (playerDirection == Vector3.back || playerDirection == Vector3.forward) //If Forward of Backward
        {
            if (vaultObjectBounds.z < playerClimbingAttributes.playerVaultingHorizontalMin || vaultObjectBounds.z > playerClimbingAttributes.playerVaultingHorizontalMax)
            {
                return false; //If not between both horizontal limits, return false;
            }
        }

        return true;
    }

    private void VaultObject(GameObject targetVaultObject, Vector3 targetVaultObjectBounds, Vector3 playerDirection, float distanceFromPlayer)
    {
        Vector3[] vaultPoints = new Vector3[4];
        int directionInverter = 0;

        if (playerDirection == Vector3.left || playerDirection == Vector3.back)
        {
            directionInverter = -1;
        }
        else if (playerDirection == Vector3.right || playerDirection == Vector3.forward)
        {
            directionInverter = 1;
        }

        vaultPoints[0] = Vector3.zero;
        vaultPoints[1] = new Vector3(0, targetVaultObjectBounds.y, 0);

        if (playerDirection == Vector3.left || playerDirection == Vector3.right)
        {
            vaultPoints[2] = new Vector3(((targetVaultObjectBounds.x * 2) + distanceFromPlayer) * directionInverter, vaultPoints[1].y, 0f);
        }
        else if (playerDirection == Vector3.back || playerDirection == Vector3.forward)
        {
            vaultPoints[2] = new Vector3(0, vaultPoints[1].y, ((targetVaultObjectBounds.z * 2) + distanceFromPlayer) * directionInverter);
        }

        vaultPoints[3] = new Vector3(vaultPoints[2].x, 0f, vaultPoints[2].z);

        if (CheckForObstructionAlongClimbRoute(vaultPoints, targetVaultObject.transform.position))
        {
            GameObject newSplineObject = Instantiate(newSplineGameObject, transform.position, Quaternion.identity) as GameObject;
            splineObject = newSplineObject.GetComponent<BezierSpline>();

            splineObject.Reset(); //Reset Spline

            splineObject.transform.position = transform.position;

            splineObject.SetMultipleControlPointsClockwise(vaultPoints, BezierControlPointMode.Free); //WTF I don't understand how setting them up twice fixes the displacement bug.
            splineObject.SetMultipleControlPoints(vaultPoints, BezierControlPointMode.Free);

            playerController.PlayVaultAnimation();

            StartCoroutine(ClimbTargetSpline(vaultPoints, splineObject, playerClimbingAttributes.playerWallVaultingTime));
        }    
    }

    private void ScaleObject(GameObject targetScaleObject, Vector3 targetScaleObjectBounds, Vector3 playerDirection, float distanceFromPlayer)
    {
        Vector3[] scalePoints = new Vector3[4];
        int directionInverter = 0;

        if (playerDirection == Vector3.left || playerDirection == Vector3.back)
        {
            directionInverter = -1;
        }
        else if (playerDirection == Vector3.right || playerDirection == Vector3.forward)
        {
            directionInverter = 1;
        }

        scalePoints[0] = Vector3.zero;
        scalePoints[1] = new Vector3(0, targetScaleObjectBounds.y, 0);

        if (playerDirection == Vector3.left || playerDirection == Vector3.right)
        {         
            scalePoints[2] = new Vector3((((targetScaleObjectBounds.x / 4) + distanceFromPlayer) * directionInverter), scalePoints[1].y + 0.5f, 0f);
        }
        else if (playerDirection == Vector3.back || playerDirection == Vector3.forward)
        {
            scalePoints[2] = new Vector3(0, scalePoints[1].y + 0.5f, (((targetScaleObjectBounds.z / 4) + distanceFromPlayer) * directionInverter));
        }

        scalePoints[3] = new Vector3(scalePoints[2].x, scalePoints[1].y, scalePoints[2].z);

        if (CheckForObstructionAlongClimbRoute(scalePoints, targetScaleObject.transform.position))
        {
            GameObject newSplineObject = Instantiate(newSplineGameObject, transform.position, Quaternion.identity) as GameObject;
            splineObject = newSplineObject.GetComponent<BezierSpline>();

            splineObject.Reset(); //Reset Spline

            splineObject.transform.position = transform.position;

            splineObject.SetMultipleControlPointsClockwise(scalePoints, BezierControlPointMode.Free); //WTF I don't understand how setting them up twice fixes the displacement bug.
            splineObject.SetMultipleControlPoints(scalePoints, BezierControlPointMode.Free);

            playerController.PlayScaleAnimation();

            StartCoroutine(ClimbTargetSpline(scalePoints, splineObject, playerClimbingAttributes.playerWallScaleTime));
        }     
    }

    private bool CheckForObstructionAlongClimbRoute(Vector3[] climbPoints, Vector3 climbObjectPosition)
    {
        for (int i = 0; i < climbPoints.Length; i++)
        {
            if (Physics.CheckBox(climbPoints[i] + climbObjectPosition, Vector3.one * 0.25f, Quaternion.identity, climbCollisionMask))
            {
                Debug.Log("Obstruction Detected in Climb Route");

                return false;
            }
        }

        return true;
    }

    private IEnumerator ClimbTargetSpline(Vector3[] splinePoints, BezierSpline climbSpline, float splineFollowTime)
    {        
        playerController.DisableMovementBeforeClimb();
        isClimbing = true;

        float climbProgress = 0f;

        while(climbProgress < 1f)
        {
            climbProgress += Time.deltaTime / splineFollowTime;

            if (climbProgress > 1)
            {
                climbProgress = 1f;
            }

            Vector3 currentSplinePosition = climbSpline.GetPoint(climbProgress);

            transform.position = currentSplinePosition;

            yield return new WaitForEndOfFrame();
        }

        isClimbing = false;

        playerController.EnableMovementAfterClimb();

        Destroy(splineObject.gameObject);

        yield return null;
    }
}
