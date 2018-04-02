using System.Collections;
using UnityEngine;

public class PlayerDropProp_StageEvent : StageEvent
{
    private PlayerPickUpController playerPickUpController;

    public override void StageEventStart()
    {
        base.StageEventStart();

        playerPickUpController = ThirdPersonPlayerToolbox.Instance.ThirdPersonPickUpController;

        PlayerPickUpController.OnPlayerDrop += StageEventCompleted;
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
        PlayerPickUpController.OnPlayerDrop -= StageEventCompleted;

        base.StageEventCompleted();
    }
}
