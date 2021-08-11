using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Weapon", menuName = "Guns/New Gun")]
public class BaseWeapon : ScriptableObject
{
    public enum WeaponType
    {
        HANDCANNON,
        AUTORIFLE,
        PULSERIFLE,
        SCOUTRIFLE,
    }

    public WeaponType weaponType = WeaponType.HANDCANNON;
    public Element element = Element.KINETIC;
    public string gunName = "Base Gun";
    public string description = "Gun Description";
    public string flavourText = "This is a cool gun that I use for testing.";
    public Action weaponAttack;
}
