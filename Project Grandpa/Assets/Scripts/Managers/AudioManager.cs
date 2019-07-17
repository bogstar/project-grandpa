using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Manager<AudioManager>
{
	[Header("References")]
	[SerializeField] private AudioClip click;
	[SerializeField] private AudioClip death;
	[SerializeField] private AudioClip loss;
	[SerializeField] private AudioClip soundtrack;
	[SerializeField] private AudioClip intro;

	List<AudioSource> audioSources = new List<AudioSource>();


	private void Update()
	{
		for (int i = audioSources.Count - 1; i > -1; i--)
		{
			if (!audioSources[i].isPlaying)
			{
				audioSources.RemoveAt(i);
			}
		}
	}

	public enum Audio
	{
		Click, Death, Loss, Soundtrack, Intro
	}

	public static void PlayClipStackable(Audio audio)
	{
		AudioSource source = Instance.gameObject.AddComponent<AudioSource>();
		switch (audio)
		{
			case Audio.Click:
				source.clip = Instance.click;
				break;
			case Audio.Death:
				source.clip = Instance.death;
				break;
			case Audio.Loss:
				source.clip = Instance.loss;
				break;
			case Audio.Intro:
				source.clip = Instance.intro;
				break;
		}
		source.Play();
		Instance.audioSources.Add(source);
	}

	public static void PlayClipLoop()
	{
		AudioSource source = Instance.gameObject.AddComponent<AudioSource>();
		source.clip = Instance.soundtrack;
		source.loop = true;
		source.Play();
	}
}
