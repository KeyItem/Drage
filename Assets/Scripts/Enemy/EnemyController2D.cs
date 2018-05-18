using System.Collections;
using UnityEngine;

public class EnemyController2D : Controller2D
{
    [Header("Enemy Movement Attributes")]
    public EnemyMovementAttributes2D enemyMovementAttributes;

    [Space(10)]
    public Vector2 enemyVelocity;

    private float enemySmoothVelocity;
    private float enemyTargetSpeed;

    [Space(10)]
    public bool canEnemyMove = true;

    [Space(10)]
    public bool isEnemyActive = true;

    [Header("Player Static Velocity Attributes")]
    private Vector2 staticVelocity;

    private float staticVelocityTime;
    private float staticVelocityEndTime;

    private bool isEnemyBeingMovedByStaticVelocity = false;

    [Header("Enemy Pathing Attributes")]
    public Vector2[] enemyLocalWaypoints;

    private Vector2[] enemyGlobalWaypoints;

    private Vector2 targetWaypoint;

    private Vector2 enemyStartingPosition;

    private int currentWaypointIndex = 0;

    private bool enemyHasWaypoints = false;

    [Space(10)]
    public bool isChasingPlayer = false;
    public bool isMovingToWaypoint = false;
    public bool isReturningToStartingPosition = false;

    [Space(10)]
    public float minDistanceToWaypoint = 0.2f;
    public float minDistanceToStartingPoint = 0.1f;

    [Header("Player Detection Attributes")]
    public EnemyChaseAttributes2D enemyChaseAttributes;

    [Space(10)]
    public Transform currentTarget;

    [Space(10)]
    public float enemyDistanceToTarget = 0;

    [Header("Enemy Collision Attributes")]
    public EnemyCollisionManager2D enemyCollisions;

    [Space(10)]
    public float enemyGravity;

    [Header("Enemy Health Attributes")]
    public EnemyHealthController2D enemyHealth;

    [Header("Enemy Attack Attributes")]
    public EnemyAttackController2D enemyAttackController;

    [Space(10)]
    public EnemyAttackAttributes2D enemyAttackAttributes;

    private void Start()
    {
        ObjectSetup();
        WaypointSetup();
        PositioningSetup();
    }

    private void Update()
    {
        CheckIfEnemyIsStillActive();

        ManageEnemyVelocity();
    }

    public override void ObjectSetup()
    {
        base.ObjectSetup();

        enemyCollisions = GetComponent<EnemyCollisionManager2D>();
        enemyHealth = GetComponent<EnemyHealthController2D>();
        enemyAttackController = GetComponent<EnemyAttackController2D>();
    }

    private void PositioningSetup()
    {
        enemyStartingPosition = ReturnStartingPosition(transform.position);
    }

    private void WaypointSetup()
    {
        if (enemyLocalWaypoints.Length > 0)
        {
            enemyHasWaypoints = true;

            isMovingToWaypoint = true;

            ConvertLocalToGlobalWaypoints();

            currentWaypointIndex = 0;

            SetTargetWaypoint(enemyGlobalWaypoints[0]);
        }
    }

    private void ManageEnemyVelocity()
    {
        if (isEnemyActive)
        {
            if (isEnemyBeingMovedByStaticVelocity)
            {
                HandleControllerStaticVelocity();

                MoveControllerStaticVelocity(enemyVelocity * Time.deltaTime, false);
            }
            else
            {
                CheckForTarget();

                CalculateEnemyVelocity();

                MoveController(enemyVelocity * Time.deltaTime, false);

                if (enemyCollisions.collisionData.isCollidingAbove || enemyCollisions.collisionData.isCollidingBelow)
                {
                    if (!isEnemyBeingMovedByStaticVelocity)
                    {
                        if (enemyCollisions.collisionData.isSlidingDownSlope)
                        {
                            enemyVelocity.y += enemyCollisions.collisionData.slopeNormal.y * -enemyGravity * Time.deltaTime;
                        }
                        else
                        {
                            enemyVelocity.y = 0;
                        }
                    }
                }
            }

            HandleSpriteFaceDirection(enemyCollisions.collisionData.faceDirection);
        }
    }

    private void CalculateEnemyVelocity()
    {
        CheckDistanceToTarget();

        if (CheckIfTargetIsInAttackRange())
        {
            EnemyAttack();
        }

        Vector2 waypointDirection = ReturnTargetDirection();

        enemyTargetSpeed = ReturnEnemySpeed() * waypointDirection.x;

        enemyVelocity.x = Mathf.SmoothDamp(enemyVelocity.x, enemyTargetSpeed, ref enemySmoothVelocity, ReturnSmoothedMovementVelocity());

        if (!isEnemyBeingMovedByStaticVelocity)
        {
            enemyVelocity.y += enemyGravity * Time.deltaTime;
        }
    }

    public override void MoveController(Vector2 finalControllerVelocity, bool isOnPlatform = false)
    {
        enemyCollisions.ManageObjectCollisions(ref finalControllerVelocity, Vector2.zero);

        transform.Translate(finalControllerVelocity);

        if (isOnPlatform)
        {
            enemyCollisions.collisionData.isCollidingBelow = true;
        }
    }

    public override void MoveControllerStaticVelocity(Vector2 finalControllerVelocity, bool isOnPlatform)
    {
        enemyCollisions.ManageObjectCollisions(ref finalControllerVelocity, Vector2.zero);

        transform.Translate(finalControllerVelocity);

        if (isOnPlatform)
        {
            enemyCollisions.collisionData.isCollidingBelow = true;
        }
    }

    public override void SetNewStaticVelocity(Vector2 velocity, float timeValue)
    {
        if (isEnemyActive)
        {
            Debug.Log("Setting New Static Velocity :: " + velocity + " for :: " + timeValue);

            staticVelocity = CalculateRequiredVelocity(velocity, velocity.magnitude, timeValue);

            staticVelocityTime = 0;
            staticVelocityEndTime = timeValue;

            enemyVelocity = Vector2.zero;
            enemySmoothVelocity = 0;

            isEnemyBeingMovedByStaticVelocity = true;
        }
    }

    private void HandleControllerStaticVelocity()
    {
        if (isEnemyBeingMovedByStaticVelocity)
        {
            if (staticVelocityTime < staticVelocityEndTime)
            {
                enemyVelocity += staticVelocity * Time.deltaTime;

                if (!enemyCollisions.collisionData.isCollidingBelow)
                {
                    enemyVelocity.y += enemyGravity * Time.deltaTime;
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

        enemySmoothVelocity = 0;

        enemyVelocity = Vector2.zero;

        isEnemyBeingMovedByStaticVelocity = false;
    }

    private void CheckIfEnemyIsStillActive()
    {
        if (!enemyHealth.IsTargetAlive() && isEnemyActive)
        {
            DisableController();
        }
    }

    public override void DisableController()
    {
        isEnemyActive = false;

        enemyCollisions.DisableCollisions();
    }

    private float ReturnEnemySpeed()
    {
        if (enemyAttackController.isActivelyAttacking)
        {
            return 0;
        }
        else if (isChasingPlayer)
        {
            return enemyMovementAttributes.enemyChaseMovementSpeed;
        }
        else
        {
            return enemyMovementAttributes.enemyBaseMovementSpeed;
        }
    }

    private float ReturnSmoothedMovementVelocity()
    {
        if (enemyCollisions.collisionData.isCollidingBelow)
        {
            return enemyMovementAttributes.enemyGroundMovementSmoothing;
        }
        else
        {
            return enemyMovementAttributes.enemyAirMovementSmoothing;
        }
    }

    private void CheckForTarget()
    {
        if (enemyChaseAttributes.canSearchForATarget)
        {
            if (!isChasingPlayer)
            {
                if (enemyCollisions.foundTarget != null)
                {
                    FoundTarget(enemyCollisions.foundTarget);
                }
            }
        }      
    }

    private void FoundTarget(Transform newTarget)
    {
        isChasingPlayer = true;

        isReturningToStartingPosition = false;

        currentTarget = newTarget;

        enemyCollisions.canSearchForTarget = false;
    }

    private void LostTarget()
    {
        isChasingPlayer = false;

        currentTarget = null;

        enemyCollisions.canSearchForTarget = true;
        enemyCollisions.foundTarget = null;

        if (enemyHasWaypoints)
        {
            targetWaypoint = ReturnClosestWaypoint(enemyGlobalWaypoints);
        }
        else
        {
            isReturningToStartingPosition = true;
        }
    }

    private bool CheckIfPlayerIsWithinLineOfSight()
    {
        Vector2 interceptVector = currentTarget.position - transform.position;
        float rayDistance = interceptVector.magnitude;

        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, interceptVector.normalized, rayDistance, enemyChaseAttributes.chaseMask);

        if (rayHit)
        {
            Debug.DrawRay(transform.position, interceptVector.normalized * rayDistance, Color.red);

            return false;
        }

        Debug.DrawRay(transform.position, interceptVector.normalized * rayDistance, Color.green);

        return true;
    }

    private void CheckDistanceToTarget()
    {
        float distanceToTarget = 0;

        if (isChasingPlayer)
        {
            distanceToTarget = Vector2.Distance(transform.position, currentTarget.position);

            enemyDistanceToTarget = distanceToTarget;

            if (distanceToTarget > enemyChaseAttributes.maxChaseDistance)
            {
                enemySmoothVelocity = 0;

                LostTarget();
            }
            else if (!CheckIfPlayerIsWithinLineOfSight())
            {
                enemySmoothVelocity = 0;

                LostTarget();
            }
        }
        else if (enemyHasWaypoints)
        {
            distanceToTarget = Vector2.Distance(transform.position, targetWaypoint);

            enemyDistanceToTarget = distanceToTarget;

            if (distanceToTarget < minDistanceToWaypoint)
            {
                enemySmoothVelocity = 0;

                Vector2 nextWaypoint = ReturnNextWaypoint();

                SetTargetWaypoint(nextWaypoint);
            }
        }
        else if (isReturningToStartingPosition)
        {
            distanceToTarget = Vector2.Distance(transform.position, enemyStartingPosition);

            enemyDistanceToTarget = distanceToTarget;

            if (distanceToTarget < minDistanceToStartingPoint)
            {
                targetWaypoint = Vector2.zero;

                isReturningToStartingPosition = false;
            }
        }
    }

    private Vector2 ReturnTargetDirection()
    {
        Vector2 directionVector = Vector2.zero;

        if (isChasingPlayer)
        {
            directionVector = ((Vector2)currentTarget.position - (Vector2)transform.position).normalized;
        }
        else if (enemyHasWaypoints)
        {
            directionVector = targetWaypoint - (Vector2)transform.position;
        }
        else if (isReturningToStartingPosition)
        {
            directionVector = enemyStartingPosition - (Vector2)transform.position;
        }

        directionVector.y = 0;

        directionVector.Normalize();

        return directionVector;
    }

    private void ConvertLocalToGlobalWaypoints()
    {
        enemyGlobalWaypoints = new Vector2[enemyLocalWaypoints.Length];

        for (int i = 0; i < enemyGlobalWaypoints.Length; i++)
        {
            enemyGlobalWaypoints[i] = enemyLocalWaypoints[i] + (Vector2)transform.position;
        }
    }

    private Vector2 ReturnClosestWaypoint(Vector2[] waypoints)
    {
        Vector2 closestWaypoint = Vector2.zero;
        float closestDistance = float.MaxValue;

        foreach (Vector2 waypoint in waypoints)
        {
            float distanceToPoint = Vector2.Distance(transform.position, waypoint);

            if (distanceToPoint < closestDistance)
            {
                closestWaypoint = waypoint;
            }
        }

        return closestWaypoint;
    }

    private Vector2 ReturnStartingPosition(Vector2 spawnPosition)
    {
        RaycastHit2D startingPositionHit = Physics2D.Raycast(spawnPosition, Vector2.down, Mathf.Infinity, enemyCollisions.collisionAttributes.collisionMask);

        if (startingPositionHit)
        {
            Vector2 newStartingPosition = startingPositionHit.point + Vector2.up;

            return newStartingPosition;
        }
        else
        {
            return spawnPosition;
        }
    }

    private Vector2 ReturnNextWaypoint()
    {
        if (++currentWaypointIndex > enemyGlobalWaypoints.Length - 1)
        {
            currentWaypointIndex = 0;

            System.Array.Reverse(enemyGlobalWaypoints);

            return enemyGlobalWaypoints[currentWaypointIndex];
        }
        
        return enemyGlobalWaypoints[currentWaypointIndex];
    }

    private void SetTargetWaypoint(Vector2 newTargetWaypoint)
    {
        targetWaypoint = newTargetWaypoint;
    }

    private void EnemyAttack()
    {
        enemyAttackController.RequestAttack(enemyCollisions.collisionData.faceDirection);
    }

    private bool CheckIfTargetIsInAttackRange()
    {
        if (isChasingPlayer)
        {
            if (enemyDistanceToTarget <= enemyAttackAttributes.enemyMaxDistanceToAttack && enemyDistanceToTarget >= enemyAttackAttributes.enemyMinDistanceToAttack)
            {
                return true;
            }
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (enemyStartingPosition != null)
        {
            Gizmos.color = Color.yellow;

            float gizmosSize = 0.3f;

            Gizmos.DrawLine(enemyStartingPosition - Vector2.up * gizmosSize, enemyStartingPosition + Vector2.up * gizmosSize);
            Gizmos.DrawLine(enemyStartingPosition - Vector2.left * gizmosSize, enemyStartingPosition + Vector2.left * gizmosSize);
        }

        if (enemyLocalWaypoints != null)
        {
            if (enemyLocalWaypoints.Length > 0)
            {
                Gizmos.color = Color.blue;

                float gizmosSize = 0.3f;

                for (int i = 0; i < enemyLocalWaypoints.Length; i++)
                {
                    Vector2 globalWaypointPosition = (Application.isPlaying) ? enemyGlobalWaypoints[i] : enemyLocalWaypoints[i] + (Vector2)transform.position;

                    Gizmos.DrawLine(globalWaypointPosition - Vector2.up * gizmosSize, globalWaypointPosition + Vector2.up * gizmosSize);
                    Gizmos.DrawLine(globalWaypointPosition - Vector2.left * gizmosSize, globalWaypointPosition + Vector2.left * gizmosSize);
                }
            }
        }
    }
}

[System.Serializable]
public struct EnemyMovementAttributes2D
{
    [Header("Enemy Movement Attributes")]
    public float enemyBaseMovementSpeed;
    public float enemyChaseMovementSpeed;

    [Space(10)]
    public float enemyGroundMovementSmoothing;
    public float enemyAirMovementSmoothing;
}

[System.Serializable]
public struct EnemyChaseAttributes2D
{
    [Header("Enemy Chase Attributes")]
    public float maxChaseDistance;

    [Space(10)]
    public LayerMask chaseMask;

    [Space(10)]
    public bool canSearchForATarget;
}

[System.Serializable]
public struct EnemyAttackAttributes2D
{
    [Header("Enemy Attack Attributes")]
    public float enemyMinDistanceToAttack;
    public float enemyMaxDistanceToAttack;

    [Space(10)]
    public bool canEnemyAttack;
}
