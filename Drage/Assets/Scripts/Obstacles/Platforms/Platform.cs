using System.Collections;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public PLATFORM_COLLISION_TYPE platformCollisionType;
}

public enum PLATFORM_COLLISION_TYPE
{
    NONE,
    SOLID,
    THROUGH
}
