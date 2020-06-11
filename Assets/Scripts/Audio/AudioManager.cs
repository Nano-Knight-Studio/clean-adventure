using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public Sound[] sounds;
	public static AudioManager instance;

	void Awake ()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
			return;
		}
		foreach (Sound sound in sounds)
		{
			sound.source = gameObject.AddComponent<AudioSource>();
			sound.source.clip = sound.Clip;
			sound.source.volume = sound.volume;
			sound.source.pitch = sound.pitch;
			sound.source.loop = sound.loop;
			if (sound.playOnAwake)
			PlaySound (sound.name);
		}
		DontDestroyOnLoad(gameObject);
	}

	public void SetPitch (string name, float pitch)
	{
		Sound soundToUse = Array.Find (sounds, sound => sound.name == name);
		soundToUse.source.pitch = pitch;
	}

	public void SetVolume (string name, float volume)
	{
		Sound soundToUse = Array.Find (sounds, sound => sound.name == name);
		soundToUse.source.volume = volume;
	}

	public void PlaySound (string name)
	{
		Sound soundToUse = Array.Find (sounds, sound => sound.name == name);
		soundToUse.source.Play();
	}

	public void PlaySound (string name, Vector3 position)
	{
		Sound soundToUse = Array.Find (sounds, sound => sound.name == name);
		GameObject obj = new GameObject ("Spatial Sound (One Shot)");
		obj.transform.position = position;
		obj.AddComponent<AudioSource>();
		AudioSource src = obj.GetComponent<AudioSource>();
		src.volume = soundToUse.volume;
		src.spatialBlend = 1.0f;
		src.minDistance = 2.0f;
		src.maxDistance = 15.0f * soundToUse.volume;
		src.clip = soundToUse.Clip;
		src.playOnAwake = false;
		src.loop = false;
		src.spread = 0;
		src.Play();
		Destroy(obj, soundToUse.Clip.length + 1.0f);
	}
}