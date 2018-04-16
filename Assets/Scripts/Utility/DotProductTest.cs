using System.Collections;
using UnityEngine;

public class DotProductTest : MonoBehaviour
{
    [Header("Target Attributes")]
    public Transform targetTransform;

    [Space(10)]
    public float dotProduct = 0f;

    [Space(10)]
    public DIRECTION direction;

    private void Update()
    {
        ManageDirection();
    }

    private void ManageDirection()
    {
        Vector3 interceptVector = (targetTransform.position - transform.position).normalized;

        Vector3 localPoint = transform.InverseTransformPoint(targetTransform.position);

        dotProduct = Vector3.Dot(interceptVector, transform.forward);

        if (dotProduct >= 1)
        {
            direction = DIRECTION.FORWARD;
        }
        else if (dotProduct < 0)
        {
            direction = DIRECTION.BACK;
        }
        else if (dotProduct > 0)
        {
            if (localPoint.x > 0)
            {
                direction = DIRECTION.RIGHT;
            }
            else if (localPoint.x < 0)
            {
                direction = DIRECTION.LEFT;
            }
        }
    }
}
