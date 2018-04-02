using System.Collections;
using UnityEngine;

public class StageEventManager : MonoBehaviour
{
    private static StageEventManager _instance;
    public static StageEventManager Instance { get { return _instance; } }

    [Header("Stage Events Attributes")]
    public StageEvent[] stageEvents;

    [Space(10)]
    public int stageEventIndex = 0;
    public StageEvent currentStageEvent = null;

    [Space(10)]
    public bool isFinishedAllStageEvents = false;

    public delegate void StageEventStart();
    public delegate void StageEventActive();
    public delegate void StageEventComplete();

    public static event StageEventStart OnStageEventStarted;
    public static event StageEventActive OnStageEventActive;
    public static event StageEventComplete OnStageEventCompleted;

    [Header("DEBUG")]
    public bool canShowDebug = false;

    private string targetDebugString;

    private void Awake()
    {
        InstanceSetup();
    }

    private void InstanceSetup()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void StartStageEvent(StageEvent newStageStageEvent)
    {
        if (newStageStageEvent != null)
        {
            currentStageEvent = newStageStageEvent;

            newStageStageEvent.StageEventStart();

            if (canShowDebug)
            {
                targetDebugString = "SE :: " + currentStageEvent.stageEventName;
            }

            if (OnStageEventStarted != null)
            {
                Debug.Log("StageEvent :: Start :: {Event}");

                OnStageEventStarted();
            }
        }     
    }

    public void ActivateStageEvent()
    {
        if (canShowDebug)
        {

        }

        if (OnStageEventActive != null)
        {
            Debug.Log("StageEvent :: Active :: Event");

            OnStageEventActive();
        }
    }

    public void CompleteStageEvent()
    {
        if (canShowDebug)
        {
            targetDebugString = "";
        }

        if (OnStageEventCompleted != null)
        {
            OnStageEventCompleted();
        }

        currentStageEvent = null;
    }

    public void PrepareNewStageEvent(StageEvent newStageEvent)
    {
        if (newStageEvent != null)
        {
            stageEventIndex++;

            newStageEvent.StageEventPrepare();
        }

        if (!CanMoveToNextStageEvent(stageEventIndex, stageEvents))
        {
            isFinishedAllStageEvents = true;
        }
    }

    private bool CanMoveToNextStageEvent(int currentStageEventIndex, StageEvent[] stageEvents)
    {
        if (stageEvents.Length > 0)
        {
            currentStageEventIndex++;

            if (currentStageEventIndex <= stageEvents.Length - 1)
            {
                return true;
            }
        }

        return false;
    }
    
    private void OnGUI()
    {
        if (canShowDebug)
        {
            GUI.Box(new Rect(35, 0, 300, 25), "");
            GUI.Label(new Rect(35, 0, 300, 25), targetDebugString);
        }       
    }
}
