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
    public List<CharacterAction> availableActions = new List<CharacterAction>();

    public void AddAction(CharacterAction action, string name)
    {
        availableActions.Add(action);
        availableActions[availableActions.Count - 1].name = name;
    }
}
