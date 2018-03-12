using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformAnimator : MonoBehaviour {
	public Vector3 destination;
	public AnimationCurve path = new AnimationCurve (new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));
	public float loopTime = 6f;

	private float timmer = 0f;
	private Rigidbody _rb;
	private Vector3 localPos;

	void Start() {
		_rb = GetComponent<Rigidbody> ();
		localPos = transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		_rb.MovePosition (localPos + path.Evaluate (timmer / loopTime) * destination);
		timmer += Time.deltaTime;
		if (timmer > loopTime) {
			timmer -= loopTime;
		}
	}
}
