using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public List<Character> heroes = new List<Character>();
    public List<Character> enemies = new List<Character>();

    public int level;
    public int missionLevel;
    public List<int> completedLevels = new List<int>();

    private void Start()
    {
        level = 0;
        foreach(Character hero in heroes)
        {
            hero.InitCharacter();
        }
        //Character character = new Character("Jelly-16", 1, 1100, 345, 15, 0);
        //Character character2 = new Character("Hydra-10", 2, 1100, 278, 20, 0);

        //heroes.Add(character);
        //heroes.Add(character2);
    }
    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void ResetCharacters()
    {
        foreach(Character character in heroes)
        {
            character.currHP = character.baseHP;
        }
    }

    public void FinishedMission()
    {
        level = missionLevel;
        if(!completedLevels.Contains(missionLevel)) completedLevels.Add(missionLevel);
    }
}
