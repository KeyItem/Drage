using System.Collections;
using UnityEngine;

[System.Serializable]
public struct Attack2D
{
    [Header("Player Attack Attributes")]
    public string attackName;

    [Space(10)]
    public AttackColliderAttributes2D attackColliderAttributes;

    [Space(10)]
    public float attackLifetime;

    [Space(10)]
    public float attackCooldown;

    [Header("Attack Damage Attributes")]
    public float attackDamage;
}

[System.Serializable]
public struct AttackMovementAttributes2D
{
    public Vector2 attackVelocity;
}

[System.Serializable]
public struct AttackColliderAttributes2D
{
    public Vector2 colliderSize;
    public Vector2 colliderOffset;
}
