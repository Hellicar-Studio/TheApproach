using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkNoiseController : MonoBehaviour {

	public AudioSource audioSource;
	// Use this for initialization
	void Start () {
		if (audioSource == null)
			audioSource = GetComponent<AudioSource>();
	}

	public void PlayAudio()
	{
		audioSource.Play();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
