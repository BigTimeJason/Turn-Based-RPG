using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandleTurn
{
    public string attacker;
    public string type;
    public GameObject attackerGameObject;
    public List<GameObject> targetGameObjects = new List<GameObject>();
    public Action attack;

    public HandleTurn() { }

    public HandleTurn(string attacker, string type, GameObject attackerGameObject, List<GameObject> targetGameObjects, Action attack)
    {
        this.attacker = attacker;
        this.type = type;
        this.attackerGameObject = attackerGameObject;
        this.targetGameObjects = targetGameObjects;
        this.attack = attack;
    }

    public string GetAttackerName()
    {
        return this.attackerGameObject.GetComponent<CharacterStateMachine>().character.charName;
    }

    public string GetTargetName(int i)
    {
        if (i < targetGameObjects.Count) return this.targetGameObjects[i].GetComponent<CharacterStateMachine>().character.charName;
        return "error";
    }

    public List<string> GetTargetNames()
    {
        List<string> names = new List<string>();
        foreach(GameObject target in targetGameObjects)
        {
            names.Add(target.GetComponent<CharacterStateMachine>().character.charName);
        }
        return names;
    }
}
