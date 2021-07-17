using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandleTurn
{
    public string attacker;
    public string type;
    public GameObject attackerGameObject;
    public List<GameObject> targetGameObject = new List<GameObject>();
    public Action attack;

    public HandleTurn() { }

    public HandleTurn(string attacker, string type, GameObject attackerGameObject, List<GameObject> targetGameObjects, Action attack)
    {
        this.attacker = attacker;
        this.type = type;
        this.attackerGameObject = attackerGameObject;
        this.targetGameObject = targetGameObjects;
        this.attack = attack;
    }
}
