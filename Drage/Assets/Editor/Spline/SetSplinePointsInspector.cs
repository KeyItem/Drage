using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SetSplinePoints))]
public class SetSplinePointsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SetSplinePoints splineSetScript = (SetSplinePoints)target;

        if (GUILayout.Button("Set Spline Points"))
        {
            splineSetScript.SetPoints();
        }
    }
}
