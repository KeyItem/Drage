using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CollisionManager2D))]
public class ObjectController2D : Controller2D
{
    private Vector2 objectVelocity;

    [Header("Object Collision Attributes")]
    public CollisionManager2D objectCollisions;

    [Space(10)]
    public float objectGravity;

    private void Start()
    {
        ObjectSetup();
    }

    private void Update()
    {
        ManageObjectVelocity();
    }

    public override void ObjectSetup()
    {
        base.ObjectSetup();

        objectCollisions = GetComponent<CollisionManager2D>();
    }

    private void ManageObjectVelocity()
    {
        HandleSpriteFaceDirection(objectCollisions.collisionData.faceDirection);

        objectVelocity.y += objectGravity * Time.deltaTime;

        MoveController(objectVelocity * Time.deltaTime, false);

        if (objectCollisions.collisionData.isCollidingAbove || objectCollisions.collisionData.isCollidingBelow)
        {
            if (objectCollisions.collisionData.isSlidingDownSlope)
            {
                objectVelocity.y += objectCollisions.collisionData.slopeNormal.y * -objectGravity * Time.deltaTime;
            }
            else
            {
                objectVelocity.y = 0;
            }
        }
    }

    public override void MoveController(Vector2 finalControllerVelocity, bool isOnPlatform = false)
    {
        HandleSpriteFaceDirection(objectCollisions.collisionData.faceDirection);

        objectCollisions.ManageObjectCollisions(ref finalControllerVelocity);

        transform.Translate(finalControllerVelocity);

        if (isOnPlatform)
        {
            objectCollisions.collisionData.isCollidingBelow = true;
        }
    }
}
