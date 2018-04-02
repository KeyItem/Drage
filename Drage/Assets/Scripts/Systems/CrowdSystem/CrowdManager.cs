using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdManager : MonoBehaviour
{
    private static CrowdManager _instance;
    public static CrowdManager Instance { get { return _instance; } }

    [Header("Crowd Manager Attributes")]
    public List<CrowdController> crowdMembers;

    [Space(10)]
    public STRENGTH currentCrowdStrength;

    [Space(10)]
    public float crowdSpeedLowMin;
    public float crowdSpeedLowMax;

    [Space(10)]
    public float crowdSpeedMedMin;
    public float crowdSpeedMedMax;

    [Space(10)]
    public float crowdSpeedHighMin;
    public float crowdSpeedHighMax;

    private void Awake()
    {
        InitializeCrowdManager();
    }

    private void InitializeCrowdManager()
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

    public void StartCrowd()
    {
        currentCrowdStrength = STRENGTH.MEDIUM;

        for (int i = 0; i < crowdMembers.Count; i++)
        {
            crowdMembers[i].StartCrowdBob(ReturnCrowdSpeed(currentCrowdStrength));
        }
    }

    public void SetNewCrowdSpeed(STRENGTH newCrowdStrength)
    {
        currentCrowdStrength = newCrowdStrength;
    }

    private float ReturnCrowdSpeed(STRENGTH newCrowdStrength)
    {
        float newCrowdSpeed = 0f;

        switch(newCrowdStrength)
        {
            case STRENGTH.LOW:
                newCrowdSpeed = Random.Range(crowdSpeedLowMin, crowdSpeedLowMax);
                break;

            case STRENGTH.MEDIUM:
                newCrowdSpeed = Random.Range(crowdSpeedMedMin, crowdSpeedMedMax);
                break;

            case STRENGTH.HIGH:
                newCrowdSpeed = Random.Range(crowdSpeedHighMin, crowdSpeedHighMax);
                break;
        }

        return newCrowdSpeed;
    }
}
