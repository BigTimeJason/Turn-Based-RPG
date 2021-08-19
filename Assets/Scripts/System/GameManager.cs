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
        //Character character = new Character("Jelly-16", CharacterStatSheet.JELLY, 0);
        //Character character2 = new Character("Hydra", CharacterStatSheet.HYDRA, 0);
        foreach(Character hero in heroes)
        {
            hero.InitCharacterStats();
        }

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
            character.CurrHP = character.MaxHP;
        }
    }

    public void FinishedMission()
    {
        level = missionLevel;
        if(!completedLevels.Contains(missionLevel)) completedLevels.Add(missionLevel);
    }
}
