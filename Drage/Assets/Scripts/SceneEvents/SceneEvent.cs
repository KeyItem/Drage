using System.Collections;
using UnityEngine;

public class SceneEvent : MonoBehaviour
{
    [Header("Scene Event Attributes")]
    public string sceneEventName;

    [Header("Scene Event Actor Attributes")]
    public ACTOR_NAME[] actorsInvolvedInScene;

    [Space(10)]
    public ActorMovementData[] actorMoveData;

    [Header("Scene Event Stage Event Attributes")]
    public StageEvent targetStageEvent;

    [Space(10)]
    public float stageEventActiveTime = 3f;

    [Space(10)]
    public bool stageEventHasActiveTime = false;
}
