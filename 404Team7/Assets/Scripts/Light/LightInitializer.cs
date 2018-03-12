using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent (typeof (Light))]
//[RequireComponent (typeof (Rigidbody))]
//[RequireComponent (typeof (SphereCollider))]
public class LightInitializer : MonoBehaviour {

	[Header("Rechargeable Light")]
	public bool rechargeable = false;
	public GameObject rechargeCore;
	public float maxChargeRadius = 4f;
	public float maxChargeIntensity = 1.3f;
	public float chargeSpeed = 0.7f;
	public float dischargeSpeed = 0.2f;
	public float lightSmoothing = 0.05f;

	private Light _l;
	private float _chargingSpeed = 0f;
	private float _origRadius;
	private float _origIntensity;
	private float _intensityScale;

	// Use this for initialization
	void Awake () {
		gameObject.tag = "Orb";

		Rigidbody _rb = GetComponent<Rigidbody> ();
		if (_rb != null) {
			_rb.isKinematic = true;
			_rb.useGravity = false;
		}

		if (rechargeable) {
			if (transform.Find ("RechargeCore") == null) {
				GameObject o = Instantiate (rechargeCore, transform.position, Quaternion.identity);
				o.transform.parent = transform;
			}
		} else {
			this.enabled = false;
		}

		_l = GetComponent<Light> ();
		_origRadius = _l.range;
		_origIntensity = _l.intensity;
		dischargeSpeed *= -1;
		_intensityScale = (maxChargeIntensity - _origIntensity) / (maxChargeRadius - _origRadius);

		transform.localScale = new Vector3 (_origRadius, _origRadius, _origRadius) * 2;
	}
	
	// Update is called once per frame
	void Update () {
		if (_chargingSpeed != 0) {
			float _goalRadius = Mathf.Clamp (_l.range + _chargingSpeed * Time.deltaTime, _origRadius, maxChargeRadius);
			float _goalIntensity = Mathf.Clamp (_l.intensity + _chargingSpeed * _intensityScale * Time.deltaTime, _origIntensity, maxChargeIntensity);
			_l.range = Mathf.Lerp (_l.range, _goalRadius, lightSmoothing);
			_l.intensity = Mathf.Lerp (_l.intensity, _goalIntensity, lightSmoothing);
			if (_l.range == _goalRadius) {
				_chargingSpeed = 0f;
			}
			transform.localScale = new Vector3 (_l.range, _l.range, _l.range) * 2;
		}
	}

	public void Charge() {
		_chargingSpeed = chargeSpeed;
	}

	public void Discharge() {
		_chargingSpeed = dischargeSpeed;
	}
}
