using System.Collections;
using UnityEngine;

public class HealthController2D : MonoBehaviour
{
    private EntityUIController2D entityUIController;

    [Header("Health Controller Attributes")]
    public HealthControllerAttributes2D healthAttributes;

    [Space(10)]
    public float targetCurrentHealth;

    [Space(10)]
    public bool canTargetTakeDamage = true;

    [Header("Hit Effect Attributes")]
    public HitEffectAttributes2D hitEffectAttributes;

    private SpriteRenderer spriteRenderer;
    private Color spriteOriginalColor;

    private void Start()
    {
        HealthControllerSetup();
    }

    public virtual void HealthControllerSetup()
    {
        targetCurrentHealth = healthAttributes.targetMaxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteOriginalColor = spriteRenderer.color;

        entityUIController = GetComponent<EntityUIController2D>();
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

            ShowDamageHitEffect();

            if (entityUIController != null)
            {
                entityUIController.UpdateHealthBarUI(targetCurrentHealth, healthAttributes.targetMaxHealth);
            }
        }
    }

    public virtual void ShowDamageHitEffect()
    {
        if (spriteRenderer != null)
        {
            StartCoroutine(ShowHitEffect(hitEffectAttributes.hitDamageColor, hitEffectAttributes.hitEffectLifetime));
        }
    }

    public virtual void Die()
    {
        if (entityUIController != null)
        {
            entityUIController.DisableHealthBarUI();
        }
    }

    public bool IsTargetAlive()
    {
        if (targetCurrentHealth <= 0)
        {
            return false;
        }

        return true;
    }

    public virtual void Heal(float healValue)
    {
        Debug.Log(gameObject.name + " was healed for  " + healValue + " of Health");

        targetCurrentHealth += healValue;

        if (targetCurrentHealth > healthAttributes.targetMaxHealth)
        {
            targetCurrentHealth = healthAttributes.targetMaxHealth;
        }

        ShowHealHitEffect();
    }

    public virtual void ShowHealHitEffect()
    {
        if (spriteRenderer != null)
        {
            StartCoroutine(ShowHitEffect(hitEffectAttributes.hitHealColor, hitEffectAttributes.hitEffectLifetime));
        }
    }

    private IEnumerator ShowHitEffect(Color newColor, float effectDuration)
    {
        spriteRenderer.color = newColor;

        if (effectDuration > 0)
        {
            yield return new WaitForSeconds(effectDuration);
        }

        spriteRenderer.color = spriteOriginalColor;

        yield return null;
    }
}

[System.Serializable]
public struct HealthControllerAttributes2D
{
    public float targetMaxHealth;
}

[System.Serializable]
public struct HitEffectAttributes2D
{
    public Color hitDamageColor;
    public Color hitHealColor;

    [Space(10)]
    public float hitEffectLifetime;
}
