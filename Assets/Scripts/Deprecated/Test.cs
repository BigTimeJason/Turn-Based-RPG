using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour
{
    public string sceneName;
    public void LoadScene()
    {
        Character character = new Character("Jelly-16", 1, 1100, 345, 24, 0);
        Character character2 = new Character("Hydra-10", 2, 1100, 278, 27, 0);
        GameManager.Instance.heroes.Add(character);
        GameManager.Instance.heroes.Add(character2);

        Character enemy = new Character("Dreg", 1, 550, 135, 9);
        Character enemy2 = new Character("Vandal", 2, 550, 150, 10);
        Character enemy3 = new Character("Captain", 3, 550, 200, 11, 100);
        GameManager.Instance.enemies.Add(enemy);
        GameManager.Instance.enemies.Add(enemy2);
        GameManager.Instance.enemies.Add(enemy3);

        SceneManager.LoadScene(sceneName);
        //FindObjectOfType<LevelLoader>().LoadScene(sceneName);
    }
}
