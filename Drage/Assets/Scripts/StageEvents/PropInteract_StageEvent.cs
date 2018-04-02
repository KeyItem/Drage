using System.Collections;
using UnityEngine;

public class PropInteract_StageEvent : StageEvent
{
    [Header("Custom Stage Event Attributes")]
    public GameObject requiredProp;

    public override void StageEventStart()
    {
        base.StageEventStart();
    }

    public override void StageEventAction()
    {
        if (isStageEventActive)
        {
            StageEventCompleted();
        }
    }

    public override void StageEventCompleted()
    {
        base.StageEventCompleted();
    }

    public override void InteractWithStageEventUsingProp(GameObject propInteraction)
    {
        if (propInteraction == requiredProp)
        {
            StageEventAction();
        }
    }
}
