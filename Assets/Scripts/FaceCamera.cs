using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is used to face the head at the camera in an attempt to make everything much creepier.
public class FaceCamera : MonoBehaviour {

	public Transform target;
	public Transform head;
	public Transform anchor;
	private Vector3 dynamicTarget;
	private Vector3 anchorPos;
	private Vector3 forward;
	private Vector3 camPos;
	public float range;
	// Use this for initialization
	void Start () {
		forward = head.forward;

		anchorPos = anchor.position;

		camPos = target.position;

		dynamicTarget = camPos;

	}

	
	// Update is called once per frame
	void Update () {
		if (Vector3.Distance(head.position, anchorPos) < range)
		{
			dynamicTarget = camPos;
		}
		else
		{
			dynamicTarget = Vector3.Lerp(dynamicTarget, head.position + forward, 0.1f);
		}

		head.LookAt(dynamicTarget);
	}
}
