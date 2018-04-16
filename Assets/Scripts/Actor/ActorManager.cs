using System.Collections;
using UnityEngine;

public class ActorManager : MonoBehaviour
{
    private static ActorManager _instance;
    public static ActorManager Instance { get { return _instance; } }

    [Header("Actor Manager Attributes")]
    public ActorController[] sceneActors;

    private void Awake()
    {
        InitializeActorManager();
    }

    private void InitializeActorManager()
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

   public void PassOffActorMovement(ACTOR_NAME[] actorsRequired, ActorMovementData[] actorMovementData)
    {
        if (actorsRequired.Length > 0)
        {
            for (int i = 0; i < actorsRequired.Length; i++)
            {
                ActorController newActorController = ReturnRequestedActorInScene(actorsRequired[i]);

                if (newActorController != null)
                {
                    newActorController.ReceiveNewActorMovementData(actorMovementData[i]);
                }
            }
        }  
    }

    private ActorController ReturnRequestedActorInScene(ACTOR_NAME targetActorName)
    {
        if (targetActorName != ACTOR_NAME.NONE)
        {
            for (int i = 0; i < sceneActors.Length; i++)
            {
                if (sceneActors[i].actorName == targetActorName)
                {
                    return sceneActors[i];
                }
            }
        }

        return null;
    }
}
