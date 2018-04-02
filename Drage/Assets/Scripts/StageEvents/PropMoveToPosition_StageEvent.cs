using System.Collections;
using UnityEngine;

public class PropMoveToPosition_StageEvent : StageEvent
{
    private PlayerPickUpController playerPickUpController;

    [Header("Custom Stage Event Attributes")]
    public Transform targetProp;

    [Space(10)]
    public float minDistanceToPosition = 2f;

    [Header("DEBUG")]
    public float currentDistanceToPosition = 0f;

    private void Update()
    {
        StageEventAction();
    }

    public override void StageEventStart()
    {
        base.StageEventStart();

        playerPickUpController = ThirdPersonPlayerToolbox.Instance.ThirdPersonPickUpController; 
    }

    public override void StageEventAction()
    {
        if (isStageEventActive)
        {
            if (targetProp != null)
            {
                if (playerPickUpController.currentHeldProp == targetProp.gameObject)
                {
                    float distanceToPosition = Vector3.Distance(targetProp.position, stageEventTransform.position);

                    currentDistanceToPosition = distanceToPosition;

                    if (distanceToPosition <= minDistanceToPosition)
                    {
                        StageEventCompleted();
                    }

                    if (canShowDebug)
                    {
                        Debug.DrawLine(targetProp.position, transform.position, Color.yellow);
                    }
                }            
            }
        }
    }

    public override void StageEventCompleted()
    {
        base.StageEventCompleted();
    }
}
