
#region PLAYER
    
public enum PLAYER_NUMBER
{
    NONE,
    PLAYER_1,
    PLAYER_2,
    PLAYER_3,
    PLAYER_4,
}

public enum PLAYER_COLOR
{
    NONE,
    RED,
    BLUE,
    GREEN,
    YELLOW,
}

public enum PLAYER_STATE
{
    NONE,
    STAND,
    IN_COVER,
    IS_VAULTING
}

#endregion

#region CAMERA

public enum CAMERA_STATE
{
    NONE,
    FOLLOWING_PLAYER,
    LOCKED_ON,
    IN_COVER,
    IN_CONVERSATION
}


public enum CAMERA_FADE_TYPE
{
    NONE,
    IN,
    OUT,
    FADE_OUT_THEN_IN,
    FADE_IN_THEN_OUT
}

public enum CONVERSATION_CAMERA_ACTION
{
    NONE,
    DIRECT,
    ROTATE,
    MOVETO,
    MOVE_AND_ROTATE
}

public enum CONVERSATION_CAMERA_MOVETYPE
{
    NONE,
    LERP,
    SLERP
}

#endregion

#region INTERACTION

public enum INTERACTABLE_OBJECT_TYPE
{
    NONE,
    PHYSICS,
    ACTOR,
    PICKUP,
    STAGE_EVENT
}

#endregion

#region ABSTRACT

public enum STRENGTH
{
    NONE,
    LOW,
    MEDIUM,
    HIGH
}

public enum DIRECTION
{
    NONE,
    LEFT,
    RIGHT,
    UP,
    DOWN,
    FORWARD,
    BACK
}

public enum AXIS
{
    NONE,
    X,
    Y,
    Z,
    X_AND_Y
}
#endregion

