using System.Collections;
using UnityEngine;

public class PlayerCollisionManager2D : CollisionManager2D
{
    [Header("Player Collision Attributes")]
    public PlayerWallCollisionData playerWallCollisionData;

    public override void ManageObjectCollisions(ref Vector2 objectVelocity, Vector2 playerInputDirection)
    {
        playerWallCollisionData.Reset();

        base.ManageObjectCollisions(ref objectVelocity, playerInputDirection);
    }

    public override void DetectHorizontalCollisions(ref Vector2 objectVelocity, Vector2 playerInputDirection)
    {
        float directionX = collisionData.faceDirection;
        float rayLength = Mathf.Abs(objectVelocity.x) + collisionAttributes.objectSkinWidth;

        int wallHitCount = 0;

        if (Mathf.Abs(objectVelocity.x) < collisionAttributes.objectSkinWidth)
        {
            rayLength = 2 * collisionAttributes.objectSkinWidth;
        }

        for (int i = 0; i < collisionHorizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? rayOrigin = objectBounds.bottomLeft : objectBounds.bottomRight;
            rayOrigin += Vector2.up * (collisionHorizontalRaySpacing * i);

            RaycastHit2D rayHit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionAttributes.collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

            if (rayHit)
            {
                if (rayHit.distance == 0)
                {
                    continue;
                }

                DIRECTION collisionDirection = (directionX == -1) ? DIRECTION.LEFT : DIRECTION.RIGHT;
                GameObject collisionObject = rayHit.collider.gameObject;

                CollisionInfo2D newCollisionInfo = new CollisionInfo2D(collisionDirection, collisionObject);

                if (!collisionData.collidedObjects.ContainsValue(newCollisionInfo))
                {
                    collisionData.collidedObjects.Add((collisionData.collidedObjects.Count + 1), newCollisionInfo);
                }

                float currentSlopeAngle = Vector2.Angle(rayHit.normal, Vector2.up);

                if (i == 0 && currentSlopeAngle <= collisionAttributes.objectMaxSlope)
                {
                    if (collisionData.isDescendingSlope)
                    {
                        collisionData.isDescendingSlope = false;

                        objectVelocity = collisionData.previousVelocity;
                    }

                    float distanceToSlopeStart = 0;

                    if (currentSlopeAngle != collisionData.previousSlopeAngle)
                    {
                        distanceToSlopeStart = rayHit.distance - collisionAttributes.objectSkinWidth;

                        objectVelocity.x -= distanceToSlopeStart * directionX;
                    }

                    AscendSlope(ref objectVelocity, currentSlopeAngle, rayHit.normal);

                    objectVelocity.x += distanceToSlopeStart * directionX;
                }

                if (!collisionData.isAscendingSlope || currentSlopeAngle > collisionAttributes.objectMaxSlope)
                {
                    objectVelocity.x = (rayHit.distance - collisionAttributes.objectSkinWidth) * directionX;

                    rayLength = rayHit.distance;

                    if (collisionData.isAscendingSlope)
                    {
                        objectVelocity.y = Mathf.Tan(collisionData.currentSlopeAngle * Mathf.Deg2Rad * Mathf.Abs(objectVelocity.x));
                    }

                    collisionData.isCollidingLeft = directionX == -1;
                    collisionData.isCollidingRight = directionX == 1;

                    if (Helper.LayerMaskContainsLayer(collisionAttributes.climbableWallMask, rayHit.collider.gameObject.layer))
                    {
                        wallHitCount++;
                    }
                }
            }

            if (wallHitCount >= collisionHorizontalRayCount / 2)
            {
                playerWallCollisionData.isAdjacentToClimbableWall = true;
            }
            else
            {
                playerWallCollisionData.isAdjacentToClimbableWall = false;
            }
        }
    }

    public override void DetectVerticalCollisions(ref Vector2 objectVelocity, Vector2 playerInputDirection)
    {
        float directionY = Mathf.Sign(objectVelocity.y);
        float rayLength = Mathf.Abs(objectVelocity.y) + collisionAttributes.objectSkinWidth;

        for (int i = 0; i < collisionVerticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? rayOrigin = objectBounds.bottomLeft : objectBounds.topLeft;
            rayOrigin += Vector2.right * (collisionVerticalRaySpacing * i + objectVelocity.x);

            RaycastHit2D rayHit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionAttributes.collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

            if (rayHit)
            {
                if (rayHit.distance == 0)
                {
                    return;
                }

                Platform hitPlatform = rayHit.collider.GetComponent<Platform>();

                if (hitPlatform != null)
                {
                    if (hitPlatform.platformCollisionType == PLATFORM_COLLISION_TYPE.THROUGH)
                    {
                        if (directionY == 1 || rayHit.distance == 0)
                        {
                            continue;
                        }

                        if (collisionData.isFallingThroughPlatform)
                        {
                            continue;
                        }

                        if (playerInputDirection.y == -1)
                        {
                            collisionData.isFallingThroughPlatform = true;

                            Invoke("ResetFallingThroughPlatform", 0.5f);

                            continue;
                        }
                    }
                    else if (hitPlatform.platformCollisionType == PLATFORM_COLLISION_TYPE.FAKE)
                    {
                        continue;
                    }
                }

                DIRECTION collisionDirection = (directionY == -1) ? DIRECTION.DOWN : DIRECTION.UP;
                GameObject collisionObject = rayHit.collider.gameObject;

                CollisionInfo2D newCollisionInfo = new CollisionInfo2D(collisionDirection, collisionObject);

                if (!collisionData.collidedObjects.ContainsValue(newCollisionInfo))
                {
                    collisionData.collidedObjects.Add((collisionData.collidedObjects.Count + 1), newCollisionInfo);
                }

                objectVelocity.y = (rayHit.distance - collisionAttributes.objectSkinWidth) * directionY;
                rayLength = rayHit.distance;

                if (collisionData.isAscendingSlope)
                {
                    objectVelocity.x = objectVelocity.y / Mathf.Tan(collisionData.currentSlopeAngle * Mathf.Deg2Rad) * Mathf.Sign(objectVelocity.x);
                }

                collisionData.isCollidingBelow = directionY == -1;
                collisionData.isCollidingAbove = directionY == 1;
            }

            if (collisionData.isAscendingSlope)
            {
                float directionX = Mathf.Sign(objectVelocity.x);

                rayLength = Mathf.Abs(objectVelocity.x) + collisionAttributes.objectSkinWidth;

                Vector2 newRayOrigin = ((directionX == -1) ? objectBounds.bottomLeft : objectBounds.bottomRight) + Vector2.up * objectVelocity.y;

                RaycastHit2D newRayHit = Physics2D.Raycast(newRayOrigin, Vector2.right * directionX, rayLength, collisionAttributes.collisionMask);

                if (newRayHit)
                {
                    float newSlopeAngle = Vector2.Angle(newRayHit.normal, Vector2.up);

                    if (newSlopeAngle != collisionData.currentSlopeAngle)
                    {
                        objectVelocity.x = (newRayHit.distance - collisionAttributes.objectSkinWidth) * directionX;

                        collisionData.currentSlopeAngle = newSlopeAngle;
                        collisionData.slopeNormal = newRayHit.normal;
                    }
                }
            }
        }
    }

    private void ResetFallingThroughPlatform()
    {
        collisionData.isFallingThroughPlatform = false;
    }
}

[System.Serializable]
public struct PlayerWallCollisionData
{
    public bool isAdjacentToClimbableWall;

    public void Reset()
    {
        isAdjacentToClimbableWall = false;
    }
}
