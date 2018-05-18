using System.Collections;
using UnityEngine;

[System.Serializable]
public struct Attack2D
{
    [Header("Attack Attributes")]
    public string attackName;

    [Space(10)]
    public Attack2DAttributes[] attackAttributes;
}

[System.Serializable]
public struct Attack2DAttributes
{
    [Header("Player Attack Attributes")]
    public string subAttackName;

    [Space(10)]
    public int subAttackID;

    [Space(10)]
    public AttackMovementAttributes2D attackMovement;

    [Space(10)]
    public AttackColliderAttributes2D attackColliderAttributes;

    [Space(10)]
    public float attackLifetime;

    [Space(10)]
    public float attackCooldown;

    [Header("Attack Damage Attributes")]
    public AttackDamageAttributes2D attackDamageAttributes;
}

[System.Serializable]
public struct AttackMovementAttributes2D
{
    public Vector2 attackMovementVelocity;

    [Space(10)]
    public float attackMovementLifetime;

    [Space(10)]
    public float attackStartWaitTime;
}

[System.Serializable]
public struct AttackColliderAttributes2D
{
    public Vector2 colliderSize;
    public Vector2 colliderOffset;
}

[System.Serializable]
public struct AttackDamageAttributes2D
{
    public float attackDamage;

    [Space(10)]
    public float knockbackTime;
    public Vector2 knockbackDirection;
}
