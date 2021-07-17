using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Action", menuName = "Attacks/New Action")]
public class Action : ScriptableObject
{
    public string attackName = "Base Attack";
    public string description = "This is a base attack, dealing damage.";
    public float[] damage = new float[] { 1 };
    public int minRange = 1;
    public int maxRange = 4;

    public void Init(string attackName, string description, float[] damage, int minRange, int maxRange)
    {
        this.attackName = attackName;
        this.description = description;
        this.damage = damage;
        this.minRange = minRange;
        this.maxRange = maxRange;
    }

    public void Print()
    {
        Debug.Log("Attack:\t" + this.attackName + "\nDesc:\t" + this.description + "\nDmg:\t" + this.damage + "\nMin\t" + this.minRange + "\nMax\t" + this.maxRange);
    }
}
