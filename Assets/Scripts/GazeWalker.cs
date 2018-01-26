using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRStandardAssets.Utils;

public class GazeWalker : MonoBehaviour {

	public Animator anim;
	public VRInteractiveItem interactiveItem;
	// Use this for initialization
	void Start () {
		if (anim == null)
			anim = GetComponent<Animator>();
		if (interactiveItem == null)
			interactiveItem = GetComponent<VRInteractiveItem>();

		interactiveItem.OnOver += HandleOver;
		interactiveItem.OnOut += HandleOut;
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
