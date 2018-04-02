using System.Collections;
using UnityEngine;

[System.Serializable]
public class StageEvent : MonoBehaviour
{
    [Header("Base Stage Event Attributes")]
    public string stageEventName;

    [Space(10)]
    public Transform stageEventTransform;

    [Header("Stage Event Light Attributes")]
    public Color stageLightColor = Color.yellow;

    [Space(10)]
    public bool isStageEventActive = false;

    [Header("DEBUG")]
    public bool canShowDebug = false;

    private void OnDisable() //OnDisable Unsubscribe from all events
    {
        StageEventManager.OnStageEventActive -= StageEventActive;
    }

    public virtual void StageEventPrepare()
    {
        if (stageEventTransform == null)
        {
            stageEventTransform = transform;
        }
    }

    public virtual void StageEventStart() //Called to Start the Stage Event and for it to start listening for Events
    {
        StageEventManager.OnStageEventActive += StageEventActive;

        StageLightManager.Instance.RequestEventStageLight(stageEventTransform.position, Quaternion.identity, stageLightColor);

        isStageEventActive = true;
    }

    public virtual void StageEventActive() //On - Off for Event
    {
        if (isStageEventActive)
        {
            isStageEventActive = false;
        }
        else
        {
            isStageEventActive = true;
        }
    }

    public virtual void StageEventAction() //Logic for during Event
    {
        if (isStageEventActive)
        {
            StageEventCompleted();
        }
    }

    public virtual void StageEventCompleted() //Called Once Event is completed
    {
        StageEventManager.OnStageEventActive -= StageEventActive;

        StageEventManager.Instance.CompleteStageEvent();

        StageLightManager.Instance.ClearCurrentEventStageLights();

        isStageEventActive = false;
    }

    public virtual void InteractWithStageEventUsingProp(GameObject propInteraction)
    {

    }
}
