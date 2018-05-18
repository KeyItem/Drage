using System.Collections;
using UnityEngine;

public class Controller2D : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public virtual void ObjectSetup()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void HandleSpriteFaceDirection(int faceDirection)
    {
        Vector3 newLocalScale = Vector3.one;
        newLocalScale.x = (faceDirection == -1) ? -1 : 1;

        transform.localScale = newLocalScale;
    }

    public virtual void MoveController (Vector2 finalControllerVelocity, bool isOnPlatform = false)
    {
        transform.Translate(finalControllerVelocity);
    }

    public virtual void MoveControllerOnPlatform (Vector2 finalControllerVelocity, bool isOnPlatform)
    {
        MoveController(finalControllerVelocity, isOnPlatform);
    }

    public virtual void MoveControllerStaticVelocity(Vector2 finalControllerVelocity, bool isOnPlatform)
    {

    }

    public virtual void SetNewStaticVelocity(Vector2 newStaticVelocityDirection, float newStaticVelocityLifetime)
    {

    }

    public virtual void ResetController()
    {

    }

    public virtual void DisableController()
    {

    }

    public Vector2 CalculateRequiredVelocity(Vector2 accelerationVector, float accelerationDistance, float accelerationTime)
    {
        Vector2 newVelocity = Vector2.zero;
        float calculatedAcceleration = (2 * accelerationDistance / Mathf.Pow(accelerationTime, 2));
        float calculatedVelocity = calculatedAcceleration * accelerationTime;

        newVelocity = accelerationVector * calculatedVelocity;
        newVelocity.y = Mathf.Abs(newVelocity.y);

        return newVelocity;
    }

    public Vector2 CalculateLinearVelocity(Vector2 velocityVector, float velocityDistance, float velocityTime)
    {
        Vector2 newLinearVelocityVector = Vector2.zero;
        float newLinearVelocity = velocityDistance / velocityTime;

        newLinearVelocityVector = velocityVector.normalized * newLinearVelocity;
        Debug.Log(newLinearVelocity);

        return newLinearVelocityVector;
    }
}
