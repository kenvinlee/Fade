using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (SphereCollider))]
[RequireComponent(typeof (Light))]
public class Checkpoint : MonoBehaviour {
	public bool is_reusable = false;
	private int priority = 0;
	// Use this for initialization
	void Start () {
		if (GetComponent<SphereCollider> () != null) {
			GetComponent<SphereCollider> ().isTrigger = true;
			if (GetComponent<Light> () != null) {
				GetComponent<SphereCollider> ().radius = GetComponent<Light> ().range;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public int getPriority(){
		return priority;
	}

	public void setPriority(int p){
		priority = p;
	}
}
