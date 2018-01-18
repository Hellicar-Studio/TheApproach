using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeWalker : MonoBehaviour {

	public Animator anim;
	// Use this for initialization
	void Start () {
		if (anim == null)
			anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("1"))
		{
			anim.SetBool("Walk", true);
		} else if(Input.GetKeyDown("2"))
		{
			anim.SetBool("Walk", false);
		}
	}
}
