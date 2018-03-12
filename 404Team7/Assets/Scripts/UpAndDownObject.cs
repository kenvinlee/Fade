using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpAndDownObject : MonoBehaviour {
	public Transform startObject;
	public Transform endObject;
	public float speed = 1.0f;
	public bool stopAtEnd = true;
	private bool forward = true;
	private bool animated = false;
	private ReAnimatedObject reAnimationScript;
	private float objectHeight;
	private float objectWidth;
	// Use this for initialization
	void Start () {
		foreach (Transform child in transform) {
			if (child.name == "start" && startObject != null) {
				startObject = child;
			}
			if (child.name == "end" && endObject != null) {
				endObject = child;
			}
		}
		GetComponent<Collider> ().isTrigger = true;
		objectHeight = GetComponent<Renderer> ().bounds.size.y;
		objectWidth = GetComponent<Renderer> ().bounds.size.x;
		reAnimationScript = GetComponent<ReAnimatedObject> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		reAnimationScript.enabled = true;
		animated = reAnimationScript.isAnimating ();
		float heightChange = 0;
		if (stopAtEnd) {
			heightChange = objectHeight / 2;
		}
		if (animated) {
			Vector3 newPos = new Vector3 (endObject.transform.position.x, endObject.transform.position.y - heightChange, endObject.transform.position.z);
			transform.position = Vector3.MoveTowards (transform.position, newPos, speed * 0.05f);
		} else {
			Vector3 newPos = new Vector3 (startObject.transform.position.x, startObject.transform.position.y - heightChange, startObject.transform.position.z);
			transform.position = Vector3.MoveTowards (transform.position, newPos, speed*0.05f);
		}
	
	}
	/*
	void OnTriggerEnter (Collider other) {
		if (other.tag == "Orb") {
			animated = true;
		}
	}

	void OnTriggerExit (Collider other) {
		if (other.tag == "Orb") {
			animated = false;
		}
	}*/
}
