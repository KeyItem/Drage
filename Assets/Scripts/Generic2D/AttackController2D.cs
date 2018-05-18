using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController2D : MonoBehaviour
{
    [Header("Controller Attributes")]
    public Controller2D controller;

    [Header("Attack Attributes")]
    public Attack2D[] attacks;

    [Space(10)]
    public BoxCollider2D attackCollider;

    [Header("Attack Animation Attributes")]
    [HideInInspector]
    public Animator objectAnimator;

    [Header("Current Attack Attributes")]
    private Attack2D currentAttack;

    private Attack2DAttributes currentAttackAttributes;

    private List<GameObject> attackHitObjects = new List<GameObject>();

    private int currentAttackIndex;

    private float attackTime;
    private float attackEndTime;

    private float attackCooldownTime;
    private float attackCooldownEndTime;

    private int attackDirection;

    private float attackDamage;

    private Vector2 attackKnockbackDirection;
    private float attackKnockbackTime;

    [Space(10)]
    public LayerMask attackableObjectMask;

    [Space(10)]
    public bool isActivelyAttacking = false;
    public bool isWaitingOnAttackCooldown = false;

    [Space(10)]
    public bool canMoveToNextAttack = false;

    [Header("Attack Movement Attributes")]
    private float attackMovementTime;
    private float attackMovementEndTime;

    [Space(10)]
    private bool isActivelyMoving = false;

    private void Start()
    {
        SetupAttackController();
    }

    private void Update()
    {
        ManageAttackMovement();
        ManageAttackCollisions();
    }

    public virtual void SetupAttackController()
    {
        controller = GetComponent<Controller2D>();

        objectAnimator = GetComponent<Animator>();

        if (attackCollider == null)
        {
            Debug.LogError("Attack Collider is not assigned :: please use the editor to assign it :: " + gameObject.name);
        }
    }

    private void ManageAttackCollisions()
    {
        if (isActivelyAttacking)
        {
            if (attackTime < attackEndTime)
            {
                attackTime += Time.deltaTime;
            }
            else
            {
                MoveToNextAttack();
            }
        }

        if (isWaitingOnAttackCooldown)
        {
            if (attackCooldownTime > attackCooldownEndTime)
            {
                isWaitingOnAttackCooldown = false;
            }
            else
            {
                attackCooldownTime += Time.deltaTime;
            }
        }
    }

    private void ManageAttackMovement()
    {
        if (isActivelyMoving)
        {
            if (attackMovementTime < attackMovementEndTime)
            {
                attackMovementTime += Time.deltaTime;
            }
        }
    }

    public virtual void RequestAttack(int faceDirection)
    {
        if (!isActivelyAttacking && !isWaitingOnAttackCooldown)
        {
            StartAttack(attacks[0], faceDirection);
        }
    }

    public virtual void StartAttack(Attack2D newAttack, int newAttackDirection)
    {
        currentAttack = newAttack;
        currentAttackIndex = 0;

        currentAttackAttributes = newAttack.attackAttributes[0];

        attackDirection = newAttackDirection;

        ImportAttackAttributes(currentAttackAttributes);

        objectAnimator.SetTrigger("isAttack");
        objectAnimator.SetTrigger(newAttack.attackName);

        isActivelyAttacking = true;
        isActivelyMoving = true;
    }

    public virtual void ClearAttackHitList()
    {
        attackHitObjects.Clear();
    }

    public virtual void EndAttack()
    {
        ResizeAttackCollider(Vector2.one, Vector2.zero);

        attackEndTime = 0;

        attackHitObjects.Clear();

        isActivelyAttacking = false;
    }

    private void MoveToNextAttack()
    {
        isActivelyAttacking = false;
        isActivelyMoving = false;

        if (CanMoveToNextAttackSection())
        {
            ClearAttackHitList();

            currentAttackAttributes = currentAttack.attackAttributes[currentAttackIndex];

            ImportAttackAttributes(currentAttackAttributes);

            isActivelyAttacking = true;
            isActivelyMoving = true;
        }
        else
        {
            EndAttack();
        }
    }

    private void ImportAttackAttributes(Attack2DAttributes newAttackAttributes)
    {
        ImportAttackTimingAttributes(newAttackAttributes);
        ImportAttackDamageAttributes(newAttackAttributes.attackDamageAttributes);
        ImportAttackMovementAttributes(newAttackAttributes.attackMovement);
        ImportAttackColliderAttributes(newAttackAttributes.attackColliderAttributes);
    }

    private void ImportAttackTimingAttributes(Attack2DAttributes newAttackAttributes)
    {
        attackTime = 0;
        attackEndTime = newAttackAttributes.attackLifetime;

        if (newAttackAttributes.attackCooldown > 0)
        {
            attackCooldownTime = 0;
            attackCooldownEndTime = newAttackAttributes.attackCooldown;

            isWaitingOnAttackCooldown = true;
        }
    }

    private void ImportAttackColliderAttributes(AttackColliderAttributes2D newAttackColliderAttributes)
    {
        ResizeAttackCollider(newAttackColliderAttributes.colliderSize, newAttackColliderAttributes.colliderOffset);
    }

    private void ImportAttackDamageAttributes(AttackDamageAttributes2D newAttackAttributes)
    {
        attackDamage = newAttackAttributes.attackDamage;

        attackKnockbackDirection = ConvertKnockbackToFaceDirection(newAttackAttributes.knockbackDirection, attackDirection);
        attackKnockbackTime = newAttackAttributes.knockbackTime;
    }

    private void ImportAttackMovementAttributes(AttackMovementAttributes2D newAttackMovementAttributes)
    {
        Vector2 attackVelocity = newAttackMovementAttributes.attackMovementVelocity;
        float attackLifeTime = newAttackMovementAttributes.attackMovementLifetime;
        float attackWaitTime = newAttackMovementAttributes.attackStartWaitTime;

        attackMovementTime = 0;
        attackMovementEndTime = attackLifeTime;

        controller.SetNewStaticVelocity(attackVelocity, attackLifeTime);
    }

    private Vector2 ConvertKnockbackToFaceDirection(Vector2 knockbackVector, int faceDirection)
    {
        Vector2 modifiedKnockback = new Vector2(knockbackVector.x * faceDirection, knockbackVector.y);

        return modifiedKnockback;
    }

    private void EndAttackMovement()
    {
        isActivelyMoving = false;
    }

    private bool CanMoveToNextAttackSection()
    {
        int nextAttackIndex = currentAttackIndex + 1;

        if (nextAttackIndex <= currentAttack.attackAttributes.Length - 1)
        {
            currentAttackIndex++;

            return true;
        }
        else
        {
            return false;
        }
    }

    private void ResizeAttackCollider(Vector2 newAttackColliderBounds, Vector2 newColliderOffset)
    {
        attackCollider.size = newAttackColliderBounds;
        attackCollider.offset = newColliderOffset;
    }

    public virtual void HitObject(GameObject hitObject)
    {
        Controller2D hitObjectController = hitObject.GetComponent<Controller2D>();
        HealthController2D hitObjectHealthController = hitObject.GetComponent<HealthController2D>();

        if (hitObjectController != null)
        {
            if (attackKnockbackDirection != Vector2.zero)
            {
                hitObjectController.SetNewStaticVelocity(attackKnockbackDirection, attackKnockbackTime);
            }
        }

        if (hitObjectHealthController != null)
        {
            hitObjectHealthController.TakeDamage(attackDamage);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActivelyAttacking)
        {
            if (Helper.LayerMaskContainsLayer(attackableObjectMask, collision.gameObject.layer))
            {
                if (!attackHitObjects.Contains(collision.gameObject))
                {
                    attackHitObjects.Add(collision.gameObject);

                    HitObject(collision.gameObject);
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isActivelyAttacking)
        {
            if (Helper.LayerMaskContainsLayer(attackableObjectMask, collision.gameObject.layer))
            {
                if (!attackHitObjects.Contains(collision.gameObject))
                {
                    attackHitObjects.Add(collision.gameObject);

                    HitObject(collision.gameObject);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (isActivelyAttacking)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawCube(attackCollider.transform.position + ((Vector3)currentAttackAttributes.attackColliderAttributes.colliderOffset * attackDirection), currentAttackAttributes.attackColliderAttributes.colliderSize);
        }
    }
}
