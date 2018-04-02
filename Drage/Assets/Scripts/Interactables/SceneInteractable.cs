using System.Collections;
using UnityEngine;

public class SceneInteractable : Interactable
{
    private StageEvent attachedStageEvent;

    private void Start()
    {
        InitializeInteractable();
    }

    private void InitializeInteractable()
    {
        attachedStageEvent = GetComponent<StageEvent>();
    }

    public override void Interact(GameObject playerInteracting)
    {
        attachedStageEvent.StageEventAction();
    }
    public override void InteractWithProp(GameObject objectToInteractWith)
    {
        attachedStageEvent.InteractWithStageEventUsingProp(objectToInteractWith);
    }
}
