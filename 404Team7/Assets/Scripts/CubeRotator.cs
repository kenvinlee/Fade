using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeRotator : MonoBehaviour {

	bool animated = false;

	void Update () {
		if (animated) {
			transform.Rotate (new Vector3 (15, 30, 45) * Time.deltaTime);
		}
	}

	void OnTriggerEnter (Collider other) {
		if (other.tag == "Orb") {
			animated = true;
		}
	}

	void OnTriggerExit (Collider other) {
		if (other.tag == "Orb") {
			animated = false;
		}
	}
}
