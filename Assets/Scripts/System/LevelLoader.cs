using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class LevelLoader : MonoBehaviour
{
    public Transform transitionShape;

    private void Start()
    {
        transitionShape.gameObject.SetActive(true);
        transitionShape.localScale = new Vector3(30, 30, 1);
        transitionShape.DOScale(0, 3).SetEase(Ease.InExpo).OnComplete(() =>
        {
            transitionShape.gameObject.SetActive(false);
        });
        transitionShape.DORotate(new Vector3(0, 0, 0), 3).SetEase(Ease.InExpo);
    }

    public void LoadScene(int i)
    {
        transitionShape.gameObject.SetActive(true);
        transitionShape.localScale = new Vector3(0, 0, 1);
        transitionShape.DOScale(30, 3).SetEase(Ease.InExpo).OnComplete(() => {
            SceneManager.LoadScene(i);
        });
        transitionShape.DORotate(new Vector3(0, 0, 800), 3).SetEase(Ease.InExpo);
    }
    public void LoadScene(string name)
    {
        transitionShape.gameObject.SetActive(true);
        transitionShape.localScale = new Vector3(0, 0, 1);
        transitionShape.DOScale(30, 3).SetEase(Ease.InExpo).OnComplete(() =>
        {
            SceneManager.LoadScene(name);
        });
        transitionShape.DORotate(new Vector3(0, 0, 800), 3).SetEase(Ease.InExpo);
    }
}
