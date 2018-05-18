using System.Collections;
using UnityEngine;

public class PlayerAttackController2D : AttackController2D
{
    [Header("Player Weapon Attributes")]
    public WeaponData2D[] playerWeapons;

    [Space(10)]
    public WeaponData2D currentPlayerWeapon;

    private int currentWeaponID = -1;

    public override void SetupAttackController()
    {
        base.SetupAttackController();

        WeaponSetup();
    }

    private void WeaponSetup()
    {
        SetNewWeapon(0);
    }

    public void SetNewWeapon(int newWeaponID)
    {
        if (newWeaponID != currentWeaponID)
        {
            WeaponData2D newWeaponData = playerWeapons[newWeaponID];

            if (newWeaponData != null)
            {
                currentPlayerWeapon = playerWeapons[newWeaponID];
                currentWeaponID = newWeaponID;

                UpdateWeaponAnimator(currentPlayerWeapon.weaponAnimatorController);
            }
        }   
    }

    private void UpdateWeaponAnimator(RuntimeAnimatorController newAnimatorController)
    {
        objectAnimator.runtimeAnimatorController = newAnimatorController;
    }

    public override void RequestAttack(int faceDirection)
    {
        if (!isActivelyAttacking && !isWaitingOnAttackCooldown)
        {
            StartAttack(currentPlayerWeapon.weaponAttacks[0], faceDirection);
        }
    }
}