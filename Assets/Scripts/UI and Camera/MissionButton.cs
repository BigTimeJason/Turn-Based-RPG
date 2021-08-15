using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
public class MissionButton : MonoBehaviour
{
    public string sceneName;

    public Character[] enemies;
    public int level;
    public int prerequisite;
    public bool canClick;

    public void Awake()
    {
        if (GameManager.Instance.completedLevels.Contains(prerequisite))
        {
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
            canClick = true;
        } else
        {
            GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.4f);
            canClick = false;
        }
    }

    private void OnMouseDown()
    {
        if (canClick && !(EventSystem.current.IsPointerOverGameObject()))
        {
            SoundManager.Instance.PlayMusic(2);
            LoadScene();
        }
    }

    public void LoadScene()
    {
        //Character enemy1 = new Character("Dreg", 1, 550, 135, 9);
        //Character enemy2 = new Character("Vandal", 2, 550, 150, 10);
        //Character enemy3 = new Character("Captain", 3, 550, 200, 11, 100);
        GameManager.Instance.missionLevel = level;
        GameManager.Instance.enemies.Clear();
        foreach (Character enemy in enemies)
        {
            enemy.InitCharacter();
            GameManager.Instance.enemies.Add(enemy);
        }

        LevelLoader.Instance.LoadScene(sceneName);
        //FindObjectOfType<LevelLoader>().LoadScene(sceneName);
    }
}
