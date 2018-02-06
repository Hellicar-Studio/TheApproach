using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WobbleOnSound : MonoBehaviour {

	public Material wobbleMat;
	public MicrophoneManager micManager;
	private float soundLevel;

	float scale(float unscaledNum, float minAllowed, float maxAllowed, float min, float max)
	{
		return (maxAllowed - minAllowed) * (unscaledNum - min) / (max - min) + minAllowed;
	}

	// Use this for initialization
	void Start () {
		if (micManager == null)
			micManager = GetComponent<MicrophoneManager>();
	}
	
	// Update is called once per frame
	void Update () {
		soundLevel = micManager.GetMicrophoneInputLevel();
		wobbleMat.SetFloat("_NoiseAmount", scale(soundLevel, 0, 1000, 0, 0.12f));
	}
}
