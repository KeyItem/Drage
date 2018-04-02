using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Platform))]
public class MovingPlatformController : CollisionManager2D
{
    [Header("Platform Attributes")]
    public Vector3[] localWaypoints;
    private Vector3[] globalWaypoints;

    private int fromWaypointIndex;

    private float percentBetweenWaypoints;

    private List<PassengerMovementData> passengerMovementDataList;
    private Dictionary<Transform, Controller2D> passengerDictionary = new Dictionary<Transform, Controller2D>();

    private Vector2 holdVector = Vector2.zero;

    [Header("Platform Movement Attributes")]
    public float platformSpeed;

    [Range(0, 2)]
    public float platformEasingAmount;

    [Space(10)]
    public float platformWaitTime;

    private float nextWaitTime;

    [Space(10)]
    public bool isPlatformMovementCyclic = false;

    [Space(10)]
    public LayerMask passengerMask;

    [Header("DEBUG")]
    public bool canShowDebug = false;

    public override void Start()
    {
        base.Start();

        PlatformSetup();
    }

    private void Update()
    {
        ManageObjectCollisions(ref holdVector);
    }

    private void PlatformSetup()
    {
        globalWaypoints = new Vector3[localWaypoints.Length];

        for (int i = 0; i < globalWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }
    }

    public override void ManageObjectCollisions(ref Vector2 targetMovement)
    {
        UpdateObjectBounds();

        Vector2 velocity = ReturnPlatformMovement();

        CalculatePassengerMovement(velocity);

        MovePassengers(true);

        transform.Translate(velocity);

        MovePassengers(false);
    }

    private float Ease(float x)
    {
        float a = platformEasingAmount + 1;

        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }

    private Vector3 ReturnPlatformMovement()
    {
        if (Time.time < nextWaitTime)
        {
            return Vector3.zero;
        }

        fromWaypointIndex %= globalWaypoints.Length;

        int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;

        float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
        percentBetweenWaypoints += Time.deltaTime * platformSpeed / distanceBetweenWaypoints;
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);

        float easedPercentBetweenPoints = Ease(percentBetweenWaypoints);

        Vector3 newPlatformPosition = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedPercentBetweenPoints);

        if (percentBetweenWaypoints >= 1)
        {
            percentBetweenWaypoints = 0;
            fromWaypointIndex++;

            if (!isPlatformMovementCyclic)
            {
                if (fromWaypointIndex >= globalWaypoints.Length - 1)
                {
                    fromWaypointIndex = 0;

                    System.Array.Reverse(globalWaypoints);
                }
            }

            nextWaitTime = Time.time + platformWaitTime;
        }

        return newPlatformPosition - transform.position;
    }

    private void MovePassengers(bool beforeMovePlatform)
    {
        foreach(PassengerMovementData passengerData in passengerMovementDataList)
        {
            if (!passengerDictionary.ContainsKey(passengerData.transform))
            {
                passengerDictionary.Add(passengerData.transform, passengerData.transform.GetComponent<Controller2D>());
            }

            if (passengerData.movePassengerBeforePlatform == beforeMovePlatform)
            {
                passengerDictionary[passengerData.transform].MoveControllerOnPlatform(passengerData.desiredVelocity, passengerData.isStandingOnPlatform);
            }
        }
    }

    private void CalculatePassengerMovement(Vector2 velocity)
    {
        HashSet<Transform> movedPassengers = new HashSet<Transform>();
        passengerMovementDataList = new List<PassengerMovementData>();

        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        //Vertical Platform
        if (velocity.y != 0)
        {
            float verticalRayLength = Mathf.Abs(velocity.y) + collisionAttributes.objectSkinWidth;

            for (int i = 0; i < collisionVerticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? objectBounds.bottomLeft : objectBounds.topLeft;
                rayOrigin += Vector2.right * (collisionVerticalRaySpacing * i);

                RaycastHit2D verticalHit2D = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, verticalRayLength, passengerMask);

                if (verticalHit2D && verticalHit2D.distance != 0)
                {
                    if (!movedPassengers.Contains(verticalHit2D.transform))
                    {
                        movedPassengers.Add(verticalHit2D.transform);

                        float pushX = (directionY == 1) ? velocity.x : 0;
                        float pushY = velocity.y - (verticalHit2D.distance - collisionAttributes.objectSkinWidth) * directionY;

                        passengerMovementDataList.Add(new PassengerMovementData(verticalHit2D.transform, new Vector2(pushX, pushY), directionY == 1, true));
                    }
                }

                if (canShowDebug)
                {
                    Debug.DrawRay(rayOrigin, (Vector2.up * directionY) * verticalRayLength, Color.red);
                }
            }

        }

        //Horizontal Platform
        if (velocity.x != 0)
        {
            float horizontalRayLength = Mathf.Abs(velocity.x) + collisionAttributes.objectSkinWidth;

            for (int i = 0; i < collisionHorizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? rayOrigin = objectBounds.bottomLeft : objectBounds.bottomRight;
                rayOrigin += Vector2.up * (collisionHorizontalRaySpacing * i);

                RaycastHit2D horizontalHit2D = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, horizontalRayLength, passengerMask);

                if (horizontalHit2D && horizontalHit2D.distance != 0)
                {
                    if (!movedPassengers.Contains(horizontalHit2D.transform))
                    {
                        movedPassengers.Add(horizontalHit2D.transform);

                        float pushX = velocity.x - (horizontalHit2D.distance - collisionAttributes.objectSkinWidth) * directionX;
                        float pushY = -collisionAttributes.objectSkinWidth;

                        passengerMovementDataList.Add(new PassengerMovementData(horizontalHit2D.transform, new Vector2(pushX, pushY), false, true));
                    }
                }

                if (canShowDebug)
                {
                    Debug.DrawRay(rayOrigin, (Vector2.right * directionX) * horizontalRayLength, Color.red);
                }
            }
        }

        //If Passengers on top of horizontal or downward moving platform
        if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
        {
            float verticalRayLength = collisionAttributes.objectSkinWidth * 2;

            for (int i = 0; i < collisionVerticalRayCount; i++)
            {
                Vector2 rayOrigin = objectBounds.topLeft + Vector2.right * (collisionVerticalRaySpacing * i);
                RaycastHit2D hit2D = Physics2D.Raycast(rayOrigin, Vector2.up, verticalRayLength, passengerMask);

                if (hit2D && hit2D.distance != 0)
                {
                    if (!movedPassengers.Contains(hit2D.transform))
                    {
                        movedPassengers.Add(hit2D.transform);

                        float pushX = velocity.x;
                        float pushY = velocity.y;

                        passengerMovementDataList.Add(new PassengerMovementData(hit2D.transform, new Vector2(pushX, pushY), true, false));
                    }
                }

                if (canShowDebug)
                {
                    Debug.DrawRay(rayOrigin, (Vector2.up * directionY) * verticalRayLength, Color.red);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (localWaypoints != null)
        {
            Gizmos.color = Color.red;

            float gizmosSize = 0.3f;

            for (int i = 0; i < localWaypoints.Length; i++)
            {
                Vector3 globalWaypointPosition = (Application.isPlaying) ? globalWaypoints[i] : localWaypoints[i] + transform.position;

                Gizmos.DrawLine(globalWaypointPosition - Vector3.up * gizmosSize, globalWaypointPosition + Vector3.up * gizmosSize);
                Gizmos.DrawLine(globalWaypointPosition - Vector3.left * gizmosSize, globalWaypointPosition + Vector3.left * gizmosSize);
            }
        }
    }
}

public struct PassengerMovementData
{
    public Transform transform;

    public Vector2 desiredVelocity;

    public bool isStandingOnPlatform;
    public bool movePassengerBeforePlatform;

    public PassengerMovementData(Transform _transform, Vector2 _desiredVelocity, bool _isStandingOnPlatform, bool _movePassengerBeforePlatform)
    {
        transform = _transform;
        desiredVelocity = _desiredVelocity;
        isStandingOnPlatform = _isStandingOnPlatform;
        movePassengerBeforePlatform = _movePassengerBeforePlatform;
    }
}
