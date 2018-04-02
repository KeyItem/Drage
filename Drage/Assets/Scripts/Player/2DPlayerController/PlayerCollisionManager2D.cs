using System.Collections;
using UnityEngine;

public class PlayerCollisionManager2D : CollisionManager2D
{
    [Header("Player Collision Attributes")]
    public PlayerWallCollisionData playerWallCollisionData;

    public override void ManageObjectCollisions(ref Vector2 objectVelocity)
    {
        playerWallCollisionData.Reset();

        base.ManageObjectCollisions(ref objectVelocity);
    }

    public override void DetectHorizontalCollisions(ref Vector2 objectVelocity)
    {
        float directionX = collisionData.faceDirection;
        float rayLength = Mathf.Abs(objectVelocity.x) + collisionAttributes.objectSkinWidth;

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
                        playerWallCollisionData.isAdjacentToClimbableWall = true;
                    }
                }
            }
        }
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
