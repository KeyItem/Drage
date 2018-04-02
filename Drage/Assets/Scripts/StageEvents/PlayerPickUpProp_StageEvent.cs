using System.Collections;
using UnityEngine;

public class PlayerPickUpProp_StageEvent : StageEvent
{
    private Interactable interactableObject;

    private PlayerPickUpController playerPickUpController;

    private void Start()
    {
        DisablePickUpUntilEvent();
    }

    private void DisablePickUpUntilEvent()
    {
        interactableObject = GetComponent<Interactable>();

        interactableObject.canBeInteractedWith = false;
    }

    public override void StageEventStart()
    {
        base.StageEventStart();

        interactableObject.canBeInteractedWith = true;

        playerPickUpController = ThirdPersonPlayerToolbox.Instance.ThirdPersonPickUpController;

        PlayerPickUpController.OnPlayerPickup += StageEventCompleted;
    }

    public override void StageEventAction()
    {
        if (isStageEventActive)
        {
            if (playerPickUpController.currentHeldProp == gameObject)
            {
                StageEventCompleted();
            }
        }
    }

    public override void StageEventCompleted()
    {
        PlayerPickUpController.OnPlayerPickup -= StageEventCompleted;

        base.StageEventCompleted();
    }
}
