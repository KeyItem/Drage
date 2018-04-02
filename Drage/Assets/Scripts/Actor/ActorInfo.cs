using System;
using UnityEngine;

public class ActorInfo : MonoBehaviour
{
    private static ActorInfo _instance;
    public static ActorInfo Instance { get { return _instance; } }

    private void Awake()
    {
        InitializeActorInfo();
    }

    private void InitializeActorInfo()
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

    public string ReturnActorName(ACTOR_NAME actorNameEnum)
    {
        string newActorName = "";

        switch (actorNameEnum)
        {
            case ACTOR_NAME.ANTUSAL:
                newActorName = "Antusal";
                break;

            case ACTOR_NAME.CHILD:
                newActorName = "Child";
                break;

            case ACTOR_NAME.TOLSTRUD:
                newActorName = "Tolstrud";
                break;

            case ACTOR_NAME.ULVAR:
                newActorName = "Ulvar";
                break;
        }

        return newActorName;
    }
}

[System.Serializable]
public class ActorMovementData
{
    public Transform[] actorMovePointTransform;

    [Space(10)]
    public float[] actorMovePointSpeeds;
}

#region ACTOR_NAMES

public enum ACTOR_NAME
    {
        NONE,
        TOLSTRUD,
        ULVAR,
        ANTUSAL,
        CHILD
    }

    #endregion
