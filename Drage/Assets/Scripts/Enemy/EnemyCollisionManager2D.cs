using System.Collections;
using UnityEngine;

public class EnemyCollisionManager2D : CollisionManager2D
{
    [Header("Enemy Search Attributes")]
    public EnemySearchAttributes2D enemySearchAttributes;

    [Space(10)]
    public Transform foundTarget;

    [Space(10)]
    public bool canSearchForTarget = true;

    [Space(10)]
    public bool hasFoundTarget = false;

    public override void ManageObjectCollisions(ref Vector2 objectVelocity)
    {
        SearchForPlayer();

        base.ManageObjectCollisions(ref objectVelocity);
    }

    private void SearchForPlayer()
    {
        if (canSearchForTarget)
        {
            if (enemySearchAttributes.canOnlySearchForward)
            {
                float searchDirection = collisionData.faceDirection;
                float searchRayLength = enemySearchAttributes.enemySearchRange;

                for (int i = 0; i < collisionHorizontalRayCount; i++)
                {
                    Vector2 rayOrigin = (searchDirection == -1) ? rayOrigin = objectBounds.bottomLeft : objectBounds.bottomRight;
                    rayOrigin += Vector2.up * (collisionHorizontalRaySpacing * i);

                    Debug.DrawRay(rayOrigin, Vector2.right * searchDirection, Color.yellow);

                    RaycastHit2D rayHit = Physics2D.Raycast(rayOrigin, Vector2.right * searchDirection, searchRayLength, enemySearchAttributes.targetMask);

                    if (rayHit)
                    {
                        hasFoundTarget = true;

                        foundTarget = rayHit.transform;

                        return;
                    }
                }

                hasFoundTarget = false;

                foundTarget = null;
            }
            else
            {
                for (int o = -1; o <= 1; o += 2)
                {
                    float searchDirection = o;
                    float searchRayLength = enemySearchAttributes.enemySearchRange;

                    for (int i = 0; i < collisionHorizontalRayCount; i++)
                    {
                        Vector2 rayOrigin = (searchDirection == -1) ? rayOrigin = objectBounds.bottomLeft : objectBounds.bottomRight;
                        rayOrigin += Vector2.up * (collisionHorizontalRaySpacing * i);

                        Debug.DrawRay(rayOrigin, Vector2.right * searchDirection, Color.yellow);

                        RaycastHit2D rayHit = Physics2D.Raycast(rayOrigin, Vector2.right * searchDirection, searchRayLength, enemySearchAttributes.targetMask);

                        if (rayHit)
                        {
                            hasFoundTarget = true;

                            foundTarget = rayHit.transform;

                            return;
                        }
                    }
                }

                hasFoundTarget = false;
                foundTarget = null;
            }
        }     
    }
}

[System.Serializable]
public struct EnemySearchAttributes2D
{
    [Header("Enemy Searching Attributes")]
    public bool canOnlySearchForward;

    [Space(10)]
    public float enemySearchRange;

    [Space(10)]
    public LayerMask targetMask;
}
