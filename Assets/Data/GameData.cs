using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName = "GameData", menuName = "GameData")]
public class GameData : ScriptableObject
{
    public Character[] characters;
}
