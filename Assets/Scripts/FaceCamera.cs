using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is used to face the head at the camera in an attempt to make everything much creepier.
public class FaceCamera : MonoBehaviour {

	public Transform target;
	public Transform head;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		head.LookAt(new Vector3(target.position.x, target.position.y, target.position.z));
	}
}
