using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController2D : MonoBehaviour
{
    [Header("Attack Attributes")]
    public Attack2D[] attacks;

    [Space(10)]
    public BoxCollider2D attackCollider;

    [Header("Current Attack Attributes")]
    private Attack2D currentAttack;

    private List<GameObject> attackHitObjects = new List<GameObject>();

    private float attackEndTime;
    private float attackCooldownTime;

    private int attackDirection;

    [Space(10)]
    public LayerMask attackableObjectMask;

    [Space(10)]
    public bool isActivelyAttacking = false;

    private bool isWaitingOnAttackCooldown = false;

    private void Start()
    {
        SetupAttackController();
    }

    private void Update()
    {
        ManageAttackCollisions();
    }

    private void SetupAttackController()
    {
      
    }

    private void ManageAttackCollisions()
    {
        if (isActivelyAttacking)
        {
            if (Time.time < attackEndTime)
            {

            }
            else
            {
                EndAttack();
            }
        }

        if (isWaitingOnAttackCooldown)
        {
            if (Time.time > attackCooldownTime)
            {
                isWaitingOnAttackCooldown = false;
            }
        }
    }

    public void RequestAttack(int faceDirection)
    {
        if (!isActivelyAttacking && !isWaitingOnAttackCooldown)
        {
            StartAttack(attacks[0], faceDirection);
        }
    }

    private void StartAttack(Attack2D newAttack, int newAttackDirection)
    {
        currentAttack = newAttack;

        attackDirection = newAttackDirection;

        ResizeAttackCollider(newAttack.attackColliderAttributes.colliderSize, newAttack.attackColliderAttributes.colliderOffset * attackDirection);

        attackEndTime = Time.time + newAttack.attackLifetime;

        if (newAttack.attackCooldown > 0)
        {
            attackCooldownTime = Time.time + newAttack.attackCooldown;

            isWaitingOnAttackCooldown = true;
        }

        isActivelyAttacking = true;
    }

    private void EndAttack()
    {
        ResizeAttackCollider(Vector2.one, Vector2.zero);

        attackEndTime = 0;

        attackHitObjects.Clear();

        isActivelyAttacking = false;
    }

    private void ResizeAttackCollider(Vector2 newAttackColliderBounds, Vector2 newColliderOffset)
    {
        attackCollider.size = newAttackColliderBounds;
        attackCollider.offset = newColliderOffset;
    }

    private void HitObject(GameObject hitObject)
    {
        HealthController2D hitObjectHealthController = hitObject.GetComponent<HealthController2D>();

        if (hitObjectHealthController != null)
        {
            hitObjectHealthController.TakeDamage(currentAttack.attackDamage);
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

            Gizmos.DrawCube(transform.position + ((Vector3)currentAttack.attackColliderAttributes.colliderOffset * attackDirection), currentAttack.attackColliderAttributes.colliderSize);
        }
    }
}
