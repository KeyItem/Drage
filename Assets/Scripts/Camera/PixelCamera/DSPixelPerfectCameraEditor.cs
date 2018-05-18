using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DSPixelPerfectCamera))]
public class DSPixelPerfectCameraEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DSPixelPerfectCamera pixelCamera = (DSPixelPerfectCamera)target;

        if (GUILayout.Button("Set New Pixel Aspect"))
        {
            pixelCamera.SetNewPixelAspect();
        }
    }
}
