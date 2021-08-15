using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class LevelLoader : MonoBehaviour
{
    private static LevelLoader _instance;
    public static LevelLoader Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public Transform transitionShape;
    public float transitionTime = 1f;

    private void Start()
    {
        transitionShape.gameObject.SetActive(true);
        transitionShape.localScale = new Vector3(30, 30, transitionTime);
        transitionShape.DOScale(0, transitionTime).OnComplete(() =>
        {
            transitionShape.gameObject.SetActive(false);
        });
        transitionShape.DORotate(new Vector3(0, 0, 0), 1).SetEase(Ease.InExpo);
    }

    public void LoadScene(int i)
    {
        transitionShape.gameObject.SetActive(true);
        transitionShape.localScale = new Vector3(0, 0, 0);
        transitionShape.DOScale(30, transitionTime).OnComplete(() => {
            SceneManager.LoadScene(i);
        });
        transitionShape.DORotate(new Vector3(0, 0, 800), transitionTime).SetEase(Ease.InExpo);
    }
    public void LoadScene(string name)
    {
        transitionShape.gameObject.SetActive(true);
        transitionShape.localScale = new Vector3(0, 0, 1);
        transitionShape.DOScale(30, transitionTime).OnComplete(() =>
        {
            SceneManager.LoadScene(name);
        });
        transitionShape.DORotate(new Vector3(0, 0, 800), transitionTime).SetEase(Ease.InExpo);
    }
}
