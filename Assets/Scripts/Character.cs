using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Character
{
    public string name;
    public int slot;

    public float baseHP;
    public float currHP;

    public float baseSpeed;
    public float currSpeed;

    public BaseWeapon weapon;
    public List<BaseAttack> availableAttacks = new List<BaseAttack>();
}
