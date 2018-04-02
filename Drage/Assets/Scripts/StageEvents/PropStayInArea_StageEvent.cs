using System.Collections;
using UnityEngine;

public class PropStayInArea_StageEvent : StageEvent
{
    private PlayerPickUpController playerPickUpController;

    [Header("Custom Stage Event Attributes")]
    public Transform targetPropTransform;

    [Space(10)]
    public float targetInAreaWaitTime = 1f;

    [Space(10)]
    public float targetMinDistance = 2f;

    [Space(10)]
    public bool propMustBeHeld = false;

    [Space(10)]
    public bool isTargetInArea = false;

    [Header("DEBUG")]
    public float currentDistanceToPosition = 0f;

    [Space(10)]
    public float targetWaitTime = 1f;

    private void Update()
    {
        StageEventAction();

        ManageTargetInArea();
    }

    public override void StageEventStart()
    {
        base.StageEventStart();

        targetWaitTime = targetInAreaWaitTime;

        if (targetPropTransform == null)
        {
            Debug.LogError("Prop is not assigned :: " + this);
        }

        playerPickUpController = ThirdPersonPlayerToolbox.Instance.ThirdPersonPickUpController; 
    }

    public override void StageEventAction()
    {
        if (isStageEventActive)
        {
            if (targetPropTransform != null && stageEventTransform != null)
            {
                float distanceToPosition = Vector3.Distance(targetPropTransform.position, stageEventTransform.position);

                currentDistanceToPosition = distanceToPosition;

                if (distanceToPosition <= targetMinDistance)
                {
                    if (propMustBeHeld)
                    {
                        if (playerPickUpController.currentHeldProp == targetPropTransform.gameObject)
                        {
                            isTargetInArea = true;
                        }
                    }
                    else
                    {
                        isTargetInArea = true;
                    }
                }
                else
                {
                    isTargetInArea = false;
                }

                if (canShowDebug)
                {
                    Debug.DrawLine(targetPropTransform.position, stageEventTransform.position, Color.yellow);
                }
            }
        }
    }

    private void ManageTargetInArea()
    {
        if (isStageEventActive)
        {
            if (isTargetInArea)
            {
                targetWaitTime -= Time.deltaTime;

                if (targetWaitTime <= 0)
                {
                    StageEventCompleted();
                }
            }
            else
            {
                targetWaitTime = targetInAreaWaitTime;
            }
        }
    }

    public override void StageEventCompleted()
    {
        base.StageEventCompleted();
    }

}
