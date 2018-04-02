using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageLightManager : MonoBehaviour
{
    private static StageLightManager _instance;
    public static StageLightManager Instance { get { return _instance; } }

    private const string lightIdentifier = "StageLight";

    [Header("Stage Light Manager Attributes")]
    public Light[] staticStageLights;

    [Space(10)]
    public List<GameObject> activeEventStageLights;

    [Space(10)]
    public float lightRestingHeight = 10f;

    [Header("Light Command Data Attributes")]
    public LightingCommandData currentLightCommandData;

    [Space(10)]
    public List<LightPulseAttributes> lightPulseAttributes;

    private void Awake()
    {
        InitializeStageLightManager();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ParseStageLightingCommandData(currentLightCommandData);
        }
    }

    private void InitializeStageLightManager()
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

    public void ParseStageLightingCommandData(LightingCommandData newLightingCommandData)
    {
        if (newLightingCommandData != null)
        {
            currentLightCommandData = newLightingCommandData;

            List <LightPulseAttributes> newPulseAttributesList = new List<LightPulseAttributes>(newLightingCommandData.lightCommands.Length);

            for (int i = 0; i < newLightingCommandData.lightCommands.Length; i++)
            {
                Light targetLight = staticStageLights[newLightingCommandData.targetLights[i]];
                Color newColor = newLightingCommandData.targetLightColors[i];

                switch (newLightingCommandData.lightCommands[i])
                {
                    case LIGHTCOMMAND_ACTION.PULSE:
                        newPulseAttributesList.Add(newLightingCommandData.lightPulseAttributes[i]);
                        break;

                    case LIGHTCOMMAND_ACTION.ROTATE:
                        break;

                    case LIGHTCOMMAND_ACTION.TURN_OFF:
                        DisableStaticStageLight(targetLight);
                        break;

                    case LIGHTCOMMAND_ACTION.TURN_ON:
                        EnableStaticStageLight(targetLight, newColor);
                        break;
                }
            }

            if (newPulseAttributesList.Count > 0)
            {
                lightPulseAttributes = newPulseAttributesList;

                ManageStageLightPulse(newPulseAttributesList);
            }
        }
    }

    private void EnableStaticStageLight(Light newLight, Color newColor)
    {
        newLight.color = newColor;
        newLight.enabled = true;
    }

    private void DisableStaticStageLight(Light newLight)
    {
        newLight.enabled = false;
    }

    private void DisableAllStaticStageLights()
    {
        for (int i = 0; i < staticStageLights.Length; i++)
        {
            staticStageLights[i].enabled = false;
        }
    }

    private void EnableAllStaticStageLights()
    {
        for (int i = 0; i < staticStageLights.Length; i++)
        {
            staticStageLights[i].enabled = true;
        }
    }

    public void RequestEventStageLight(Vector3 lightPosition, Quaternion lightRotation, Color lightColor)
    {
        Vector3 modifiedLightPosition = lightPosition;
        modifiedLightPosition.y = lightRestingHeight;

        if (lightRotation == Quaternion.identity)
        {
            lightRotation = Quaternion.Euler(90, 0, 0);
        }

        GameObject newLightObject = ObjectPooler.Instance.CreateObjectFromPool_Reuseable(lightIdentifier, modifiedLightPosition, lightRotation);
        activeEventStageLights.Add(newLightObject);

        Light newLight = newLightObject.GetComponent<Light>();
        newLight.color = lightColor;
        newLight.enabled = true;
    }

    public void ClearCurrentEventStageLights()
    {
        for (int i = 0; i < activeEventStageLights.Count; i++)
        {
            ObjectPooler.Instance.ReturnObjectToQueue(lightIdentifier, activeEventStageLights[i]);
        }

        activeEventStageLights.Clear();
    }

    private void ManageStageLightPulse(List<LightPulseAttributes> lightPulseAttributes)
    {
        for (int i = 0; i < lightPulseAttributes.Count; i++)
        {
            StartCoroutine(StageLightPulser(lightPulseAttributes[i], i));
        }
    }

    private IEnumerator RotateStageLights(Light targetLight, Color targetColor)
    {
        yield return null;
    }

    private IEnumerator StageLightPulser(LightPulseAttributes lightPulseAttributes, int targetIndex)
    {
        int pulseCount = lightPulseAttributes.newColors.Length;

        Light targetLight = staticStageLights[targetIndex];

        Color initialLightColor = targetLight.color;

        for (int i = 0; i < pulseCount; i++)
        {
            float startTime = Time.time;

            Color newColor = lightPulseAttributes.newColors[i];
            float waitTime = lightPulseAttributes.pulseTimes[i];
            float inbetweenTime = lightPulseAttributes.pulseInbetweenTimes[i];

            targetLight.color = newColor;

            if (waitTime > 0)
            {
                while (Time.time < startTime + waitTime)
                {
                    yield return null;
                }
            }

            if (inbetweenTime > 0)
            {
                startTime = Time.time;

                targetLight.color = Color.black;

                while (Time.time < startTime + inbetweenTime)
                {
                    yield return null;
                }
            }
        }

        targetLight.color = initialLightColor;

        yield return null;
    }
}
