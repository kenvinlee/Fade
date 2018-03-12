using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RechargeableCore : MonoBehaviour {
	private LightInitializer _li;

	// Use this for initialization
	void Start () {
		_li = transform.parent.GetComponent<LightInitializer> ();
	}

	void OnTriggerEnter(Collider other) {
		if (other.transform.parent != null && other.CompareTag ("Orb") &&  other.transform.parent.CompareTag ("Player")) {
			_li.Charge ();
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.transform.parent != null && other.CompareTag ("Orb") && other.transform.parent.CompareTag ("Player")) {
			_li.Discharge ();
		}
	}
}
