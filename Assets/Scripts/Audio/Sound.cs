using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
	public string name;
	public AudioClip Clip;
	[Range(0f, 1f)]
	public float volume = 1;
	[Range(0.1f, 2.1f)]
	public float pitch = 1;
	public bool loop = false;
	public bool playOnAwake = false;
	[HideInInspector]
	public AudioSource source;
}