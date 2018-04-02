using System.Collections;
using UnityEngine;

public class ThirdPersonCameraCollisionController : MonoBehaviour
{
    private ThirdPersonCameraController thirdPersonCameraController;
    private ThirdPersonCameraCollisionAttributes thirdPersonCameraCollisionAttributes;

    [Header("Collision Attributes")]
    public float[] cameraCollisionData = new float[2];

    [Space(10)]
    public float cameraCollisionDistance = 0f;
    public float cameraCollisionDirection = 0f;

    [Space(10)]
    public float cameraClipCollisionSize = 3.14f;

    [Space(10)]
    public LayerMask collisionMask;

    private Vector3[] adjustedCameraClipPoints;
    private Vector3[] desiredCameraClipPoints;

    [Space(10)]
    public bool isCameraColliding = false;

    [Header("DEBUG")]
    public bool canShowDebug = false;

    private void Start()
    {
        InitializeCameraCollisions();
    }

    private void Update()
    {
        UpdateCameraClipPoints(transform.position, transform.rotation, ref adjustedCameraClipPoints);
        UpdateCameraClipPoints(thirdPersonCameraController.cameraDesiredPosition, transform.rotation, ref desiredCameraClipPoints);
    }

    private void FixedUpdate()
    {
        CheckCameraColliding(thirdPersonCameraController.targetPlayer.position);
    }

    public void InitializeCameraCollisions()
    {
        thirdPersonCameraController = GetComponent<ThirdPersonCameraController>();
        thirdPersonCameraCollisionAttributes = thirdPersonCameraController.thirdPersonCameraCollisionAttributes;

        adjustedCameraClipPoints = new Vector3[5];
        desiredCameraClipPoints = new Vector3[5];

        UpdateCameraClipPoints(transform.position, transform.rotation, ref adjustedCameraClipPoints);
        UpdateCameraClipPoints(thirdPersonCameraController.cameraDesiredPosition, transform.rotation, ref desiredCameraClipPoints);
    }

    private void UpdateCameraClipPoints(Vector3 cameraPosition, Quaternion atRotation, ref Vector3[] intoArray)
    {
        if (!thirdPersonCameraController) //If no camera is attached, exit
        {
            return;
        }

        intoArray = new Vector3[5]; //Reset assigned Array

        float clipZ = thirdPersonCameraController.thirdPersonCamera.nearClipPlane;
        float clipX = Mathf.Tan(thirdPersonCameraController.thirdPersonCamera.fieldOfView / cameraClipCollisionSize) * clipZ;
        float clipY = clipX / thirdPersonCameraController.thirdPersonCamera.aspect;

        //Top Left Clip Plane Point
        intoArray[0] = (atRotation * new Vector3(-clipX, clipY, clipZ)) + cameraPosition; //Added and Rotated Point accoring to Camera

        //Top Right Clip Plane Point
        intoArray[1] = (atRotation * new Vector3(clipX, clipY, clipZ)) + cameraPosition; //Added and Rotated Point accoring to Camera

        //Bottom Left Clip Plane Point
        intoArray[2] = (atRotation * new Vector3(-clipX, -clipY, clipZ)) + cameraPosition; //Added and Rotated Point accoring to Camera

        //Bottom Right Clip Plane Point
        intoArray[3] = (atRotation * new Vector3(clipX, -clipY, clipZ)) + cameraPosition; //Added and Rotated Point accoring to Camera

        //Camera Position
        intoArray[4] = cameraPosition - thirdPersonCameraController.transform.forward; //Allows for padding room for camera to draw from
    }

    private void CheckCameraColliding(Vector3 targetPosition)
    {
        if (CollisionDetectAtClipPoints(desiredCameraClipPoints, targetPosition))
        {
            cameraCollisionData = ReturnCollisionDistanceAndDirection(targetPosition);

            cameraCollisionDistance = cameraCollisionData[0];
            cameraCollisionDirection = cameraCollisionData[1];

            SetNewCameraPosition(cameraCollisionDistance);

            isCameraColliding = true;
        }
        else
        {
            if (isCameraColliding)
            {
                if (CheckNewCameraPosition(thirdPersonCameraCollisionAttributes.cameraBaseDistanceFromTarget))
                {
                    cameraCollisionDistance = 0f;
                    cameraCollisionDirection = 0f;

                    ResetCameraPosition();

                    isCameraColliding = false;
                }
                
            }        
        }
    }

    private bool CheckNewCameraPosition(float newCameraDistance)
    {
        Vector3 newCameraPosition = thirdPersonCameraController.targetPlayer.position - transform.forward * newCameraDistance;

        UpdateCameraClipPoints(newCameraPosition, transform.rotation, ref desiredCameraClipPoints);

        if (CollisionDetectAtClipPoints(desiredCameraClipPoints, thirdPersonCameraController.targetPlayer.position))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void SetNewCameraPosition(float collisionDistance)
    {
        float newCameraDistance = collisionDistance;

        if (newCameraDistance < thirdPersonCameraCollisionAttributes.cameraMinDistanceFromTarget)
        {
            newCameraDistance = thirdPersonCameraCollisionAttributes.cameraMinDistanceFromTarget;
        }
        else if (newCameraDistance > thirdPersonCameraCollisionAttributes.cameraMaxDistanceFromTarget)
        {
            newCameraDistance = thirdPersonCameraCollisionAttributes.cameraBaseDistanceFromTarget;
        }

        thirdPersonCameraController.cameraCurrentDistanceFromPlayer = newCameraDistance;
    }

    private void ResetCameraPosition()
    {
        thirdPersonCameraController.cameraCurrentDistanceFromPlayer = thirdPersonCameraCollisionAttributes.cameraBaseDistanceFromTarget;
    }

    private bool CollisionDetectAtClipPoints(Vector3[] cameraClipPoints, Vector3 fromPosition)
    {
        for (int i = 0; i < cameraClipPoints.Length; i++)
        {
            Ray newRay = new Ray(fromPosition, cameraClipPoints[i] - fromPosition);
            float rayDistance = Vector3.Distance(cameraClipPoints[i], fromPosition);

            if (Physics.Raycast(newRay, rayDistance, collisionMask))
            {
                if (canShowDebug)
                {
                    Debug.DrawLine(thirdPersonCameraController.targetPlayer.position, cameraClipPoints[i], Color.red);
                }

                return true;
            }

            if (canShowDebug)
            {
                Debug.DrawLine(thirdPersonCameraController.targetPlayer.position, cameraClipPoints[i], Color.green);
            }
        }

        return false;
    }

    private float[] ReturnCollisionDistanceAndDirection(Vector3 fromPosition)
    {
        Vector3 closestHitPoint = Vector3.zero;
        float[] collisionData = new float[2]; //Setting default value for closest Collision

        for (int i = 0; i < desiredCameraClipPoints.Length; i++)
        {
            Ray newRay = new Ray(fromPosition, desiredCameraClipPoints[i] - fromPosition); //Create new Ray going from clip point, to forward)
            RaycastHit newRayHit;

            if (Physics.Raycast(newRay, out newRayHit))
            {
                if (collisionData[0] == 0) //If value isnt set, set it
                {
                    collisionData[0] = newRayHit.distance;

                    closestHitPoint = newRayHit.point;
                }
                else
                {
                    if (newRayHit.distance < collisionData[0]) //If newHit is closer, set distance to new value
                    {
                        collisionData[0] = newRayHit.distance;

                        closestHitPoint = newRayHit.point;
                    }
                }
            }
        }

        Vector3 forwardDirection = transform.TransformDirection(Vector3.forward);
        Vector3 hitPoint = closestHitPoint - transform.position;

        collisionData[1] = Vector3.Dot(forwardDirection, hitPoint);

        return collisionData;
    }

    private float ReturnCollisionDirection(Vector3 collisionPoint)
    {
        float collisionDirection = 0f;

        Vector3 playerForward = transform.TransformDirection(Vector3.forward);
        Vector3 toHitPoint = collisionPoint - transform.position;

        Debug.Log(Vector3.Dot(playerForward, toHitPoint));

        return collisionDirection;
    }
}

[System.Serializable]
public class ThirdPersonCameraCollisionAttributes
{
    [Header("Third Person Camera Collision Attributes")]
    public float cameraBaseDistanceFromTarget = 3.5f;

    [Space(10)]
    public float cameraMinDistanceFromTarget = 1f;
    public float cameraMaxDistanceFromTarget = 5f;

    [Space(10)]
    public float cameraCollisionSmoothTime = 0.5f;
}
