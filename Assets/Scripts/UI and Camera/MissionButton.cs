using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionButton : MonoBehaviour
{
    public string sceneName;

    public Character[] enemies;

    private void OnMouseDown()
    {
        LoadScene();
    }

    public void LoadScene()
    {
        //Character enemy1 = new Character("Dreg", 1, 550, 135, 9);
        //Character enemy2 = new Character("Vandal", 2, 550, 150, 10);
        //Character enemy3 = new Character("Captain", 3, 550, 200, 11, 100);
        GameManager.Instance.enemies.Clear();
        foreach(Character enemy in enemies)
        {
            GameManager.Instance.enemies.Add(enemy);
        }

        LevelLoader.Instance.LoadScene(sceneName);
        //FindObjectOfType<LevelLoader>().LoadScene(sceneName);
    }
}
