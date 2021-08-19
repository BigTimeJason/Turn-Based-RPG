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
        GameManager.Instance.missionLevel = level;
        GameManager.Instance.enemies.Clear();
        foreach (Character enemy in enemies)
        {
            enemy.InitCharacterStats();
            GameManager.Instance.enemies.Add(enemy);
        }

        LevelLoader.Instance.LoadScene(sceneName);
    }
}
