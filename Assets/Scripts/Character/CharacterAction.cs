using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterAction
{
    public string name;
    public List<Action> actions = new List<Action>();

    public CharacterAction(List<Action> actions, string name = "This is an attack")
    {
        this.actions = actions;
        this.name = name;
    }

    public string GetName()
    {
        if(actions.Count > 1)
        {
            return name;
        } else if (actions.Count == 1)
        {
            return actions[0].actionName;
        } else
        {
            return "This button has no actions associated with it!";
        }
    }

    public List<Action> GetActions()
    {
        return actions;
    }

    public Action GetAction(int n)
    {
        if (n < actions.Count)
        {
            //Debug.Log("Returned an action: " + n + " count: " + actions.Count);
            return actions[n];
        }
        Debug.LogError("Tried to return an action which was out of index!");
        return actions[0];  
    }

    public bool CreatesNewMenu()
    {
        return actions.Count > 1;
    }
}
