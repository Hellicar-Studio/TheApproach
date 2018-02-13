using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneManager : MonoBehaviour
{
	public int frequency = 41000;
	public int sampleWindow = 128;
	public int maxCaptureSeconds = 99;
	public bool loop = false;

	private AudioClip microphoneClip;
	private string deviceName;
	private bool isInitialized;

	void Start()
	{
		SetupMicrophone();
	}

	private void SetupMicrophone()
	{
		deviceName = Microphone.devices[0];
		microphoneClip = Microphone.Start(deviceName, loop, maxCaptureSeconds, frequency);
	}

	public float GetMicrophoneInputLevel()
	{
		float levelMax = 0;
		float[] waveData = new float[sampleWindow];
		int micPosition = Microphone.GetPosition(deviceName) - sampleWindow + 1;
		if (micPosition < 0) return 0;
		microphoneClip.GetData(waveData, micPosition);
		for (int i = 0; i < sampleWindow; i++)
		{
			float wavePeak = waveData[i] * waveData[i];
			if (levelMax < wavePeak)
				levelMax = wavePeak;
		}
		return levelMax * 1000;
	}

	private void CloseMicrophone()
	{
		Microphone.End(deviceName);
	}

	void OnApplicationQuit()
	{
		CloseMicrophone();
	}

	void OnApplicationFocus(bool appHasFocus)
	{
		if (appHasFocus)
			SetupMicrophone();
	}
}
