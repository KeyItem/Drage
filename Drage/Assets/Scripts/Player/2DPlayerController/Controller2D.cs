using System.Collections;
using UnityEngine;

public class Controller2D : MonoBehaviour
{
    public virtual void MoveController (Vector2 finalControllerVelocity, bool isOnPlatform = false)
    {
        transform.Translate(finalControllerVelocity);
    }

    public virtual void MoveControllerOnPlatform (Vector2 finalControllerVelocity, bool isOnPlatform)
    {
        MoveController(finalControllerVelocity, isOnPlatform);
    }
}
