using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageScorer : MonoBehaviour
{
    private static StageScorer _instance;
    public static StageScorer Instance { get { return _instance; } }

    [Header("Stage Scoring Attributes")]
    public List<float> playerEventTimes = new List<float>();

    [Space(10)]
    public List<float> sceneEventDesiredTimes;

    [Header("Event Timer Attributes")]
    public float currentTrackedTime = 0;

    [Space(10)]
    public bool isTimerTracking = false;

    private void Awake()
    {
        InitializeStageScorer();
    }

    private void Update()
    {
        ManageEventTimer();
    }

    private void InitializeStageScorer()
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

    public void AddNewScoreEntry(float newPlayerEventTime)
    {
        playerEventTimes.Add(newPlayerEventTime);
    }

    private void ManageEventTimer()
    {
        if (isTimerTracking)
        {
            currentTrackedTime += Time.deltaTime;
        }
    }

    public void StartEventTimer()
    {
        if (!isTimerTracking)
        {
            currentTrackedTime = 0;

            isTimerTracking = true;
        }
    }

    public void StopEventTimer()
    {
        if (isTimerTracking)
        {
            isTimerTracking = false;

            AddNewScoreEntry(currentTrackedTime);

            currentTrackedTime = 0;
        }   
    }
}
