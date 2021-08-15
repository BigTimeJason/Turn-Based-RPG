using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Action", menuName = "Attacks/New Action")]
public class Action : ScriptableObject
{
    //public enum 
    public TargetType targetType = TargetType.ENEMY;
    public Element element = Element.KINETIC;
    public string actionName = "Base Attack";
    public string description = "This is a base attack, dealing damage.";
    public string animation = "Firing";
    public float[] damage = new float[] { 1 };
    public int minRange = 1;
    public int maxRange = 4;

    public void Init(TargetType targetType, Element element, string actionName, string description, float[] damage, int minRange, int maxRange, string animation = "Idle")
    {
        this.targetType = targetType;
        this.element = element;
        this.actionName = actionName;
        this.description = description;
        this.damage = damage;
        this.minRange = minRange;
        this.maxRange = maxRange;
        this.animation = animation;
    }

    public void Print()
    {
        Debug.Log("Attack:\t" + this.actionName + "\nDesc:\t" + this.description + "\nDmg:\t" + this.damage + "\nMin\t" + this.minRange + "\nMax\t" + this.maxRange);
    }

}
public enum TargetType
{
    FRIENDLY,
    ENEMY,
    NONE
}
