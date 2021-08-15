using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroEvents : MonoBehaviour
{
    public AudioClip iconSound;
    public void PlayIntroMusic()
    {
        SoundManager.Instance.PlayMusic(0);
    }

    public void PlayIconSound()
    {
        SoundManager.Instance.Play(iconSound);
    }

    public void LoadLobby()
    {
        LevelLoader.Instance.LoadScene("MainMenu");
    }
}
