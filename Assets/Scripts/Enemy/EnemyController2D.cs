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

    [Header("Enemy Pathing Attributes")]
    public Vector2[] enemyLocalWaypoints;

    private Vector2[] enemyGlobalWaypoints;

    private Vector2 targetWaypoint;

    private int currentWaypointIndex = 0;

    private bool enemyHasWaypoints = false;

    [Space(10)]
    public float minDistanceToWaypoint = 0.2f;

    [Header("Player Detection Attributes")]
    public EnemyChaseAttributes2D enemyChaseAttributes;

    [Space(10)]
    public Transform currentTarget;

    [Space(10)]
    public float enemyDistanceToTarget = 0;

    [Space(10)]
    public bool isChasingPlayer = false;

    [Header("Enemy Collision Attributes")]
    public EnemyCollisionManager2D enemyCollisions;

    [Space(10)]
    public float enemyGravity;

    [Header("Enemy Attack Attributes")]
    public EnemyAttackController2D enemyAttackController;

    [Space(10)]
    public EnemyAttackAttributes2D enemyAttackAttributes;

    private void Start()
    {
        ObjectSetup();
        WaypointSetup();
    }

    private void Update()
    {
        ManageObjectVelocity();
    }

    public override void ObjectSetup()
    {
        base.ObjectSetup();

        enemyCollisions = GetComponent<EnemyCollisionManager2D>();
        enemyAttackController = GetComponent<EnemyAttackController2D>();
    }

    private void ManageObjectVelocity()
    {
        CheckForTarget();

        CalculateEnemyVelocity();

        HandleSpriteFaceDirection(enemyCollisions.collisionData.faceDirection);

        if (canEnemyMove)
        {
            MoveController(enemyVelocity * Time.deltaTime, false);
        }

        if (enemyCollisions.collisionData.isCollidingAbove || enemyCollisions.collisionData.isCollidingBelow)
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

        enemyVelocity.y += enemyGravity * Time.deltaTime;
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

    private void WaypointSetup()
    {
        if (enemyLocalWaypoints.Length > 0)
        {
            enemyHasWaypoints = true;

            ConvertLocalToGlobalWaypoints();

            currentWaypointIndex = 0;

            SetTargetWaypoint(enemyGlobalWaypoints[0]);
        }    
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
    }

    private Vector2 ReturnTargetDirection()
    {
        Vector2 directionVector = Vector2.zero;

        if (isChasingPlayer)
        {
            directionVector = (Vector2)currentTarget.position - (Vector2)transform.position;
        }
        else if (enemyHasWaypoints)
        {
            directionVector = targetWaypoint - (Vector2)transform.position;
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
