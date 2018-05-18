using System.Collections;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Weapon/Weapon Data", order = 1201)]
public class WeaponData2D : ScriptableObject
{
    [Header("Weapon Attributes")]
    public string weaponName;

    [Header("Weapon Attack Attributes")]
    public Attack2D[] weaponAttacks;

    [Header("Weapon Animation Attributes")]
    public RuntimeAnimatorController weaponAnimatorController;
}
