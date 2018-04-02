using System.Collections;
using UnityEngine;

public class ChangeToRandomColor_StageEvent : StageEvent
{
    [Header("Custom Stage Event Attributes")]
    public Vector2 randomColorRange;

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
            Color newRandomColor = new Color(Random.Range(randomColorRange.x, randomColorRange.y), Random.Range(randomColorRange.x, randomColorRange.y), Random.Range(randomColorRange.x, randomColorRange.y), 255);

            Renderer myRenderer = GetComponent<Renderer>();

            myRenderer.material.color = newRandomColor;

            StageEventCompleted();
        }
    }

    public override void StageEventCompleted()
    {
        base.StageEventCompleted();
    }
}
