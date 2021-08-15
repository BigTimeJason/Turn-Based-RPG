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

    private void Start()
    {
        level = 0;
        Character character = new Character("Jelly-16", 1, 1100, 345, 24, 0);
        Character character2 = new Character("Hydra-10", 2, 1100, 278, 27, 0);

        heroes.Add(character);
        heroes.Add(character2);
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
}
