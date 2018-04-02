using System.Collections;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Light Command Data", menuName = "Light/Light Command Data", order = 1401)]
public class LightingCommandData : ScriptableObject
{
    [Header("Lighting Command Attributes")]
    public LIGHTCOMMAND_ACTION[] lightCommands = new LIGHTCOMMAND_ACTION[4];

    [Space(10)]
    public int[] targetLights = new int[4];

    [Space(10)]
    public Color[] targetLightColors = new Color[4];

    [Header("Light Rotate Command Attributes")]
    public float[] targetLightRotateTimes = new float[4];

    [Space(10)]
    public Vector3[] targetLightPositions = new Vector3[4];

    [Header("Light Pulse Command Attributes")]
    public LightPulseAttributes[] lightPulseAttributes;
}

[System.Serializable]
[CreateAssetMenu(fileName = "New Light Pulse Attributes", menuName = "Light/Light Pulse Attributes", order = 1402)]
public class LightPulseAttributes : ScriptableObject
{
    [Header("Light Pulse Attributes")]
    public Color[] newColors;

    [Space(10)]
    public float[] pulseTimes;

    [Space(10)]
    public float[] pulseInbetweenTimes;
}

public enum LIGHTCOMMAND_ACTION
{
    NONE,
    ROTATE,
    PULSE,
    TURN_ON,
    TURN_OFF
}
