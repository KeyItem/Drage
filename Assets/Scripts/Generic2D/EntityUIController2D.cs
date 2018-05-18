using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EntityUIController2D : MonoBehaviour
{
    [Header("Entity Health UI Attributes")]
    public Image entityHealthBarLife;
    public Image entityHealthBarDamage;

    private float healthBarFillAmount;

    public virtual void UpdateHealthBarUI(float newHealth, float maxHealth)
    {
        healthBarFillAmount = ReturnHealthBarFillAmount(newHealth, maxHealth);

        entityHealthBarLife.fillAmount = healthBarFillAmount;
    }

    public virtual void DisableHealthBarUI()
    {
        entityHealthBarLife.gameObject.SetActive(false);
        entityHealthBarDamage.gameObject.SetActive(false);
    }

    private float ReturnHealthBarFillAmount(float currentHealth, float maxHealth)
    {
        return currentHealth / maxHealth;
    }
}
