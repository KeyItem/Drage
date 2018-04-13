using System.Collections;
using UnityEngine;

public class HealthController2D : MonoBehaviour
{
    [Header("Health Controller Attributes")]
    public HealthControllerAttributes2D healthAttributes;

    [Space(10)]
    public float targetCurrentHealth;

    [Space(10)]
    public bool canTargetTakeDamage = true;

    private void Start()
    {
        HealthControllerSetup();
    }

    private void HealthControllerSetup()
    {
        targetCurrentHealth = healthAttributes.targetMaxHealth;
    }

    public virtual void TakeDamage(float damageValue)
    {
        if (canTargetTakeDamage)
        {
            targetCurrentHealth -= damageValue;

            if (!IsTargetAlive())
            {
                Die();
            }
        }        
    }

    public virtual void Die()
    {
        Debug.Log("Blargh.. I'm Dead :: " + gameObject.name);
    }

    private bool IsTargetAlive()
    {
        if (targetCurrentHealth <= 0)
        {
            return false;
        }

        return true;
    }

    public virtual void Heal(float healValue)
    {
        targetCurrentHealth += healValue;

        if (targetCurrentHealth > healthAttributes.targetMaxHealth)
        {
            targetCurrentHealth = healthAttributes.targetMaxHealth;
        }
    }
}

[System.Serializable]
public class HealthControllerAttributes2D
{
    public float targetMaxHealth;
}
