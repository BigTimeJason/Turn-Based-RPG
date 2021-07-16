using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Attack", menuName = "Attacks/New Attack")]
public class BaseAttack : ScriptableObject
{
    public string attackName = "Base Attack";
    public string description = "This is a base attack, dealing damage.";
    public float[] damage = new float[] { 1 };
    public int minRange = 1;
    public int maxRange = 4;

    public BaseAttack(string attackName, string description, float[] damage, int minRange, int maxRange)
    {
        this.attackName = attackName;
        this.description = description;
        this.damage = damage;
        this.minRange = minRange;
        this.maxRange = maxRange;
    }
}
