using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VRStandardAssets
{
	public class SceneReloader : MonoBehaviour
	{

		float currentTime;
		// Use this for initialization
		void Start()
		{
			currentTime = Time.time;
		}

		// Update is called once per frame
		void Update()
		{
			if (Input.GetKeyDown("a"))
				SceneManager.LoadScene("Hellicar-Gallery-VR_02");


		}

		private void OnApplicationPause(bool pause)
		{
			if (!pause)
				SceneManager.LoadScene("Hellicar-Gallery-VR_02");
		}

		private void OnApplicationFocus(bool focus)
		{
			if (!focus)
				SceneManager.LoadScene("Hellicar-Gallery-VR_02");
		}
	}
}

