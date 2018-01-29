using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneReloader : MonoBehaviour {

	float currentTime;
	// Use this for initialization
	void Start () {
		currentTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time - currentTime > 5.0) {
			Application.LoadLevel(Application.loadedLevel);
		}
		currentTime = Time.time;
	}
}
