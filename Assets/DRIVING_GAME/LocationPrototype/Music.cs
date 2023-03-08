using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
	private AudioSource music;
	private void Awake()
	{
		DontDestroyOnLoad(transform.gameObject);
		music = GetComponent<AudioSource>();
	}
	
	public void PlayMusic() {
		if (music.isPlaying == true) {
			return;
		}
		music.Play();
	}
	
	public void StopMusic() {
		music.Stop();
	}
}
