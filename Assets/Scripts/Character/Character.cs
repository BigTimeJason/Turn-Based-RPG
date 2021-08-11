using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Character
{
    public string name;
    public int slot;
    public int xp;

    public Element offenseElement;
    public float basePower;
    public float currPower;

    public float baseHP;
    public float currHP;

    public Element shieldElement;
    public float shieldBaseHP;
    public float shieldCurrHP;

    public float baseSpeed;
    public float currSpeed;

    public BaseWeapon weapon;
    public List<CharacterAction> availableActions = new List<CharacterAction>();

    public void AddAction(CharacterAction action, string name)
    {
        availableActions.Add(action);
        availableActions[availableActions.Count - 1].name = name;
    }

    public Character(string name, int slot, float power, float hp, float speed, float shield = 0, Element shieldElement = Element.KINETIC, Element offensiveElement = Element.ARC)
    {
        this.name = name;
        this.slot = slot;
        this.basePower = power;
        this.currPower = power;
        this.baseHP = hp;
        this.currHP = hp;
        this.shieldBaseHP = shield;
        this.shieldCurrHP = shield;
        this.baseSpeed = speed;
        this.currSpeed = speed;
        this.shieldElement = shieldElement;
        this.offenseElement = offensiveElement;
    }
}
