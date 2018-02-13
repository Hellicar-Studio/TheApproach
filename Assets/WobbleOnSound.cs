using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WobbleOnSound : MonoBehaviour {

	public Material wobbleMat;
	public MicrophoneManager micManager;
	public float maxNoiseScale;
	private float soundLevel;
	private float noiseScale;

	float scale(float unscaledNum, float minAllowed, float maxAllowed, float min, float max)
	{
		float v = (maxAllowed - minAllowed) * (unscaledNum - min) / (max - min) + minAllowed;
		if (v < minAllowed)
			return minAllowed;
		if (v > maxAllowed)
			return maxAllowed;
		return v;
	}

	// Use this for initialization
	void Start () {
		if (micManager == null)
			micManager = GetComponent<MicrophoneManager>();
	}
	
	// Update is called once per frame
	void Update () {
		soundLevel = micManager.GetMicrophoneInputLevel();
		noiseScale = Mathf.Lerp(noiseScale, scale(soundLevel, 0, maxNoiseScale, 0, 1000), 0.1f);
		wobbleMat.SetFloat("_NoiseAmount", noiseScale);
	}
}
