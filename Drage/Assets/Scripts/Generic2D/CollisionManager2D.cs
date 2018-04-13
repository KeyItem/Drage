using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager2D : MonoBehaviour
{
    [Header("Collision Attributes")]
    public ObjectCollisionAttributes collisionAttributes;

    [Space(10)]
    public CollisionData2D collisionData;

    [Space(10)]
    private Collider2D objectCollider;

    [Space(10)]
    public ObjectBounds2D objectBounds;

    [Header("Collision Ray Attributes")]
    [HideInInspector]
    public int collisionHorizontalRayCount;
    [HideInInspector]
    public int collisionVerticalRayCount;

    [Space(10)]
    [HideInInspector]
    public float collisionHorizontalRaySpacing;
    [HideInInspector]
    public float collisionVerticalRaySpacing;

    private const float distanceBetweenCollisionRays = 0.25f;

    public virtual void Start()
    {
        CollisionSetup();
    }

    private void CollisionSetup()
    {
        ColliderSetup();

        CollisionDataSetup();

        RaySpacingSetup();
    }

    private void ColliderSetup()
    {
        objectCollider = GetComponent<Collider2D>();

        collisionData.faceDirection = 1;
    }

    private void CollisionDataSetup()
    {
        collisionData.CollisionDataPrepare();
    }

    public virtual void ManageObjectCollisions(ref Vector2 objectVelocity)
    {
        UpdateObjectBounds();

        collisionData.CollisionDataReset();

        collisionData.previousVelocity = objectVelocity;

        if (objectVelocity.y < 0)
        {
            DescendSlope(ref objectVelocity);
        }

        if (objectVelocity.x != 0)
        {
            collisionData.faceDirection = (int)Mathf.Sign(objectVelocity.x);
        }

        DetectHorizontalCollisions(ref objectVelocity);

        if (objectVelocity.y != 0)
        {
            DetectVerticalCollisions(ref objectVelocity);
        }
    }

    public virtual void DetectHorizontalCollisions(ref Vector2 objectVelocity)
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
                }
            }
        }
    }

    public virtual void DetectVerticalCollisions(ref Vector2 objectVelocity)
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

    public virtual void AscendSlope(ref Vector2 objectVelocity, float slopeAngle, Vector2 slopeNormal)
    {
        float moveDistance = Mathf.Abs(objectVelocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (objectVelocity.y <= climbVelocityY)
        {
            objectVelocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(objectVelocity.x);
            objectVelocity.y = climbVelocityY;

            collisionData.isCollidingBelow = true;
            collisionData.isAscendingSlope = true;

            collisionData.currentSlopeAngle = slopeAngle;
            collisionData.slopeNormal = slopeNormal;
        }
    }

    public virtual void DescendSlope(ref Vector2 objectVelocity)
    {
        RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(objectBounds.bottomLeft, Vector2.down, Mathf.Abs(objectVelocity.y) + collisionAttributes.objectSkinWidth, collisionAttributes.collisionMask);
        RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(objectBounds.bottomRight, Vector2.down, Mathf.Abs(objectVelocity.y) + collisionAttributes.objectSkinWidth, collisionAttributes.collisionMask);

        if (maxSlopeHitLeft ^ maxSlopeHitRight)
        {
            SlideDownSlope(maxSlopeHitLeft, ref objectVelocity);
            SlideDownSlope(maxSlopeHitRight, ref objectVelocity);
        }

        if (!collisionData.isSlidingDownSlope)
        {
            float directionX = Mathf.Sign(objectVelocity.x);

            Vector2 rayOrigin = (directionX == -1) ? objectBounds.bottomRight : objectBounds.bottomLeft;

            RaycastHit2D rayHit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, collisionAttributes.collisionMask);

            if (rayHit)
            {
                float slopeAngle = Vector2.Angle(rayHit.normal, Vector2.up);

                if (slopeAngle != 0 && slopeAngle <= collisionAttributes.objectMaxSlope)
                {
                    if (Mathf.Sign(rayHit.normal.x) == directionX)
                    {
                        if (rayHit.distance - collisionAttributes.objectSkinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(objectVelocity.x))
                        {
                            float descendDistance = Mathf.Abs(objectVelocity.x);
                            float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * descendDistance;

                            objectVelocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * descendDistance * Mathf.Sign(objectVelocity.x);
                            objectVelocity.y -= descendVelocityY;

                            collisionData.isCollidingBelow = true;
                            collisionData.isDescendingSlope = true;

                            collisionData.slopeNormal = rayHit.normal;
                            collisionData.currentSlopeAngle = slopeAngle;
                        }
                    }
                }
            }
        }
    }

    public virtual void SlideDownSlope(RaycastHit2D rayHit, ref Vector2 objectVelocity)
    {
        if (rayHit)
        {
            float slopeAngle = Vector2.Angle(rayHit.normal, Vector2.up);

            if (slopeAngle > collisionAttributes.objectMaxSlope)
            {
                objectVelocity.x = Mathf.Sign(rayHit.normal.x) * (Mathf.Abs(objectVelocity.y) - rayHit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);

                collisionData.slopeNormal = rayHit.normal;
                collisionData.currentSlopeAngle = slopeAngle;

                collisionData.isCollidingBelow = true;
                collisionData.isSlidingDownSlope = true;
            }
        }
    }

    public bool CheckForCollision(Vector2 startPoint, Vector2 direction, float rayLength, LayerMask targetLayer)
    {
        RaycastHit2D hit = Physics2D.Raycast(startPoint, direction, rayLength, targetLayer);

        if (hit)
        {
            return true;
        }

        return false;
    }

    public void UpdateObjectBounds()
    {
        Bounds newBounds = objectCollider.bounds;
        newBounds.Expand(collisionAttributes.objectSkinWidth * -2f);

        objectBounds.bottomLeft = new Vector2(newBounds.min.x, newBounds.min.y);
        objectBounds.bottomRight = new Vector2(newBounds.max.x, newBounds.min.y);
        objectBounds.topLeft = new Vector2(newBounds.min.x, newBounds.max.y);
        objectBounds.topRight = new Vector2(newBounds.max.x, newBounds.max.y);

        objectBounds.leftSide = new Vector2[2] { objectBounds.bottomLeft, objectBounds.topLeft };
        objectBounds.rightSide = new Vector2[2] { objectBounds.bottomRight, objectBounds.topRight };
    }

    private void RaySpacingSetup()
    {
        Bounds newBounds = objectCollider.bounds;
        newBounds.Expand(collisionAttributes.objectSkinWidth * -2f);

        float boundsWidth = newBounds.size.x;
        float boundsHeight = newBounds.size.y;

        collisionHorizontalRayCount = Mathf.RoundToInt(boundsHeight / distanceBetweenCollisionRays);
        collisionVerticalRayCount = Mathf.RoundToInt(boundsWidth / distanceBetweenCollisionRays);

        collisionHorizontalRaySpacing = newBounds.size.y / (collisionHorizontalRayCount - 1);
        collisionVerticalRaySpacing = newBounds.size.x / (collisionVerticalRayCount - 1);
    }
}

[System.Serializable]
public struct CollisionData2D
{
    [Header("Collision Data")]
    public bool isCollidingLeft;
    public bool isCollidingRight;

    [Space(10)]
    public bool isCollidingAbove;
    public bool isCollidingBelow;

    [Space(10)]
    public Dictionary<int, CollisionInfo2D> collidedObjects;

    [Space(10)]
    public int faceDirection;

    [Space(10)]
    public Vector2 previousVelocity;

    [Header("Slope Data")]
    public Vector2 slopeNormal;

    [Space(10)]
    public float currentSlopeAngle;
    public float previousSlopeAngle;

    [Space(10)]
    public bool isAscendingSlope;
    public bool isDescendingSlope;
    public bool isSlidingDownSlope;

    public void CollisionDataPrepare()
    {
        collidedObjects = new Dictionary<int, CollisionInfo2D>();
    }

    public void CollisionDataReset()
    {
        isCollidingLeft = false;
        isCollidingRight = false;

        isCollidingAbove = false;
        isCollidingBelow = false;

        slopeNormal = Vector2.zero;

        previousSlopeAngle = currentSlopeAngle;
        currentSlopeAngle = 0f;

        isAscendingSlope = false;
        isDescendingSlope = false;
        isSlidingDownSlope = false;

        collidedObjects.Clear();
    }
}

[System.Serializable]
public struct ObjectBounds2D
{
    public Vector2 topLeft;
    public Vector2 bottomLeft;
    public Vector2 topRight;
    public Vector2 bottomRight;

    [Space(10)]
    public Vector2[] leftSide;
    public Vector2[] rightSide;

    public ObjectBounds2D (Vector2 newTopLeft, Vector2 newBottomLeft, Vector2 newTopRight, Vector2 newBottomRight, Vector2[] newLeftSide, Vector2[] newRightSide)
    {
        this.topLeft = newTopLeft;
        this.bottomLeft = newBottomLeft;
        this.topRight = newTopRight;
        this.bottomRight = newBottomRight;
        this.leftSide = new Vector2[2] { newBottomLeft, newTopLeft };
        this.rightSide = new Vector2[2] { newBottomRight, newTopRight };
    }
}

[System.Serializable]
public struct ObjectCollisionAttributes
{
    [Header("Object Collision Attributes")]
    public float objectSkinWidth;

    [Space(10)]
    public float objectMaxSlope;

    [Space(10)]
    public LayerMask collisionMask;
    public LayerMask climbableWallMask;
}

[System.Serializable]
public struct CollisionInfo2D
{
    public DIRECTION collisionDirection;

    [Space(10)]
    public GameObject collidedObject;

    public CollisionInfo2D (DIRECTION newDirection, GameObject newCollidedObject)
    {
        this.collisionDirection = newDirection;
        this.collidedObject = newCollidedObject;
    }
}
