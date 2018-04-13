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
        spriteRenderer.flipX = (faceDirection == -1) ? true : false;
    }

    public virtual void MoveController (Vector2 finalControllerVelocity, bool isOnPlatform = false)
    {
        transform.Translate(finalControllerVelocity);
    }

    public virtual void MoveControllerOnPlatform (Vector2 finalControllerVelocity, bool isOnPlatform)
    {
        MoveController(finalControllerVelocity, isOnPlatform);
    }
}
