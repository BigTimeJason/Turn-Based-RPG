using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Action", menuName = "Attacks/New Action")]
public class Action : ScriptableObject
{
    public TargetType targetType = TargetType.ENEMY;
    public Element element = Element.KINETIC;
    public AudioClip clip;
    public string actionName = "Base Attack";
    public string description = "This is a base attack, dealing damage.";
    public string characterAnimation = "Firing";
    public string specialEffect;
    public float[] damage = new float[] { 1 };
    public int minRange = 1;
    public int maxRange = 4;
    // 1 = push, 2 = pushAmount, 3 = pull, 4 = pullAmount, 5 = swap, 6 = pos1, 7 = pos2
    public int[] statusEffects = new int[] { 0, 0, 0, 0, 0 };

    public void Init(TargetType targetType, Element element, string actionName, string description, float[] damage, int minRange, int maxRange, string animation = "Idle")
    {
        this.targetType = targetType;
        this.element = element;
        this.actionName = actionName;
        this.description = description;
        this.damage = damage;
        this.minRange = minRange;
        this.maxRange = maxRange;
        this.characterAnimation = animation;
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
