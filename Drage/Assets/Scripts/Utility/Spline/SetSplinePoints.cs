using System.Collections;
using UnityEngine;

public class SetSplinePoints : MonoBehaviour
{
    [Header("Spline Transform Points")]
    public Transform[] splinePointTargetsTranforms;

    [Space(10)]
    public BezierControlPointMode pointMode;

    private Vector3[] splinePointPositions;
    private BezierSpline targetSpline;

    public void SetPoints()
    {
        targetSpline = GetComponent<BezierSpline>();
        targetSpline.Reset();
        
        int requiredCurveCount = splinePointTargetsTranforms.Length / 4;

        if (splinePointTargetsTranforms.Length % 4 != 0)
        {
            requiredCurveCount++;
        }

        for (int i = 0; i < requiredCurveCount; i++)
        {
            targetSpline.AddCurve();
        }

        splinePointPositions = new Vector3[splinePointTargetsTranforms.Length];

        for (int i = 0; i < splinePointTargetsTranforms.Length; i++)
        {
            splinePointPositions[i] = splinePointTargetsTranforms[i].transform.localPosition;
        }

        targetSpline.SetMultipleControlPoints(splinePointPositions, pointMode);

    }
}
