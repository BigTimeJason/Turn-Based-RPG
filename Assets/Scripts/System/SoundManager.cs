using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	private static SoundManager _instance;
	public static SoundManager Instance { get { return _instance; } }

	public AudioClip[] ost;
	public AudioClip[] sfx;

	public AudioSource EffectsSource;
	public AudioSource MusicSource;

	// Initialize the singleton instance.
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
		DontDestroyOnLoad(this);
	}

	public void Play(AudioClip clip)
	{
		EffectsSource.clip = clip;
		EffectsSource.Play();
	}

	public void Play(int id)
    {
		EffectsSource.clip = sfx[id];
		EffectsSource.Play();
	}


	public void PlayMusic(AudioClip clip)
	{
		MusicSource.clip = clip;
		MusicSource.Play();
	}

	public void PlayMusic(int id)
    {
		MusicSource.clip = ost[id];
		MusicSource.Play();
    }

	public void StopMusic(float fadeoutDuration = 0f)
    {
		StartCoroutine(FadeOut(MusicSource, fadeoutDuration));
    }

	private IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
	{
		float startVolume = audioSource.volume;

		while (audioSource.volume > 0)
		{
			audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

			yield return null;
		}

		audioSource.Stop();
		audioSource.volume = startVolume;
	}
}