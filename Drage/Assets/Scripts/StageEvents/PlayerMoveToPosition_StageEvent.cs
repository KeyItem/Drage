using System.Collections;
using UnityEngine;

public class PlayerMoveToPosition_StageEvent : StageEvent
{
    [Header("Custom Stage Event Attributes")]
    public Transform targetPlayer;

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

        if (targetPlayer == null)
        {
            targetPlayer = ThirdPersonPlayerToolbox.Instance.transform;
        }
    }

    public override void StageEventAction()
    {
        if (isStageEventActive)
        {
            if (targetPlayer != null && stageEventTransform != null)
            {
                float distanceToPosition = Vector3.Distance(targetPlayer.position, stageEventTransform.position);

                currentDistanceToPosition = distanceToPosition;

                if (distanceToPosition <= minDistanceToPosition)
                {
                    StageEventCompleted();
                }

                if (canShowDebug)
                {
                    Debug.DrawLine(targetPlayer.position, stageEventTransform.position, Color.yellow);
                }
            }
        }
    }

    public override void StageEventCompleted()
    {
        base.StageEventCompleted();
    }
}
