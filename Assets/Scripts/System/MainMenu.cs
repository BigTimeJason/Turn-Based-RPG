using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public AudioClip playSound, exitSound;
    public bool pressedButton = false;
    public void PlayGame()
    {
        if (!pressedButton)
        {
            pressedButton = true;
            SoundManager.Instance.Play(playSound);
            //SoundManager.Instance.StopMusic();
            SoundManager.Instance.PlayMusic(1);
            LevelLoader.Instance.LoadScene("LobbyScene");
        }
    }

    public void ExitGame()
    {
        if (!pressedButton)
        {
            pressedButton = true;
            StartCoroutine(ExitGameCoroutine());
        }
    }

    void Update()
    {
        if (Input.GetKey("escape"))
        {
            pressedButton = true;
            StartCoroutine(ExitGameCoroutine());
        }
    }

    IEnumerator ExitGameCoroutine()
    {
        SoundManager.Instance.StopMusic(1f);
        SoundManager.Instance.Play(exitSound);
        yield return new WaitForSeconds(2f);
        Application.Quit();
    }
}
