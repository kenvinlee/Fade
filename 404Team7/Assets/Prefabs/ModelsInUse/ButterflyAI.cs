using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButterflyAI : MonoBehaviour {
	private  Vector3 dest = Vector3.zero;
//	private Vector3 pos;
	public float flySpeed = 0.5f;
	public float rotateSpeed = 0.5f;
	public float idleRadius = 5f;
	public bool grounded = false;
	private Vector3 origin = Vector3.zero;
	public bool debug = true;
	private GameObject destination;

	void Start() {
		origin = transform.position;
		if (debug) {
			destination = new GameObject (this.name + " Destination (Read Only)");
			destination.transform.parent = transform.parent;
			SphereCollider sc = destination.AddComponent<SphereCollider> ();
			sc.radius = 0.5f;
			sc.enabled = false;
		}
		SetDest (origin);
	}
	
	// Update is called once per frame
	void Update () {
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dest - transform.position), Time.deltaTime * rotateSpeed);
		transform.position = transform.position + transform.forward * Time.deltaTime * flySpeed;
//		pos = transform.position;
		if (Vector3.Magnitude (transform.position - dest) < 1f) {
			SetDest (Random.insideUnitSphere * idleRadius + origin);
		}
	}

	public void SetDestination(Vector3 destin) {
		SetDest (destin);
		origin = destin;
	}

	private void SetDest(Vector3 destin) {
		if (grounded)
			destin = new Vector3 (destin.x, origin.y, destin.z);
		dest = destin;
		if (debug)
			destination.transform.position = destin;
	}
}
