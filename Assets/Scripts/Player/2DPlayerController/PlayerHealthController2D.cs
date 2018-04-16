using System.Collections;
using UnityEngine;

public class PlayerHealthController2D : HealthController2D
{
    public delegate void PlayerTakeDamage();
    public delegate void PlayerDie();
    public delegate void PlayerGameOver();

    public static event PlayerTakeDamage OnPlayerTakeDamage;
    public static event PlayerDie OnPlayerDeath;
    public static event PlayerGameOver OnPlayerGameOver;

    [Header ("Player Lives Attributes")]
    public PlayerLivesAttributes2D playerLivesAttributes;

    [Space(10)]
    public int playerCurrentLives;

    public override void HealthControllerSetup()
    {
        PlayerLivesSetup();

        base.HealthControllerSetup();
    }

    private void PlayerLivesSetup()
    {
        playerCurrentLives = playerLivesAttributes.playerMaxLives;
    }

    public override void TakeDamage(float damageValue)
    {
        if (OnPlayerTakeDamage != null)
        {
            OnPlayerTakeDamage();
        }

        LoseALife();
    }

    public void LoseALife()
    {
        if (OnPlayerDeath != null)
        {
            OnPlayerDeath();
        }

        playerCurrentLives--;

        if (playerCurrentLives < 0)
        {
            Die();
        }
    }

    public override void Die()
    {
        if (OnPlayerGameOver != null)
        {
            OnPlayerGameOver();
        }

        Debug.Log("Blargh.. I'm Dead :: " + gameObject.name);
    }
}

[System.Serializable]
public struct PlayerLivesAttributes2D
{
    [Header("Player Lives Attributes")]
    public int playerMaxLives;
}
