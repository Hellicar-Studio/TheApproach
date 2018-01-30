using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRStandardAssets.Utils;

public class GazeWalker : MonoBehaviour {

	public Animator anim;
	public VRInteractiveItem interactiveItem;
	public Transform anchor;
	private Vector3 anchorPos;
	public float controlRange;
	public float resetRange;

	// Use this for initialization
	void Start () {
		if (anim == null)
			anim = GetComponent<Animator>();
		if (interactiveItem == null)
			interactiveItem = GetComponent<VRInteractiveItem>();

		interactiveItem.OnOver += HandleOver;
		interactiveItem.OnOut += HandleOut;

		anchorPos = anchor.position;
		 
	}

	void Update()
	{
		if(Vector3.Distance(transform.position, anchorPos) > controlRange)
		{
			interactiveItem.OnOut -= HandleOut;
			interactiveItem.OnOver -= HandleOver;
			anim.SetBool("Walk", true);
		}

		if (Vector3.Distance(transform.position, anchorPos) > resetRange)
		{
			transform.position = anchorPos;
			interactiveItem.OnOut += HandleOut;
			interactiveItem.OnOver += HandleOver;
			anim.SetBool("Walk", false);
		}

	}

	private void HandleOver ()
	{
		anim.SetBool("Walk", true);

	}

	private void HandleOut()
	{
		anim.SetBool("Walk", false);
	}
}
