using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (InteractableObject))]
public class Lock : MonoBehaviour {
	public GameObject requiredItem;
	public string prompt;
	public string animationTrigger;
	public int priority = 0;
	public AudioClip audioOnUnlock;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public bool unlock(GrabbableObject g){
		return g.gameObject.name.Contains(requiredItem.name);
	}
}
