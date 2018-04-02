using System.Collections;
using UnityEngine;

public class MoveToRandomPosition_StageEvent : StageEvent
{
    [Header("Custom Stage Event Attributes")]
    public Vector2 xAxisRange;
    public Vector2 yAxisRange;
    public Vector2 zAxisRange;

    private void Update()
    {
        StageEventAction();
    }

    public override void StageEventStart()
    {
        base.StageEventStart();
    }

    public override void StageEventAction()
    {
        if (isStageEventActive)
        {
            Vector3 newRandomPosition = new Vector3(Random.Range(xAxisRange.x, xAxisRange.y), Random.Range(yAxisRange.x, yAxisRange.y), Random.Range(zAxisRange.x, zAxisRange.y));

            transform.position = newRandomPosition;

            StageEventCompleted();
        }
    }

    public override void StageEventCompleted()
    {
        base.StageEventCompleted();
    }
}
