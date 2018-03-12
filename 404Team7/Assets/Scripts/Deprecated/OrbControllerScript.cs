using System;
using UnityEngine;

public class OrbControllerScript : MonoBehaviour {

	public float minRadius = 1f;
	public float maxRadius = 6f;
	public float expandTime = 1.5f;
	public float contractTime = 1f;
	public AnimationCurve expandCurve = new AnimationCurve (new Keyframe (0f, 0f), new Keyframe (1f, 1f));
	public AnimationCurve contractCurve = new AnimationCurve (new Keyframe (0f, 1f), new Keyframe (1f, 0f));
	public KeyCode expandLightKey = KeyCode.Mouse0;
	public float radiusSmoothing = 0.1f;

	static float o_orbRadius = 0f;
	static bool o_orbExpanding = false;
	static bool o_orbContracting = false;
	float radiusDiff = 0f;
	float targetRadius = 1f;
	float keyDownTimmer = 0f;

	Light lightOrb;
	SphereCollider col;
	AnimationCurve o_ExpandCurve;
	AnimationCurve o_ContractCurve;


	public static float orbRadius
	{
		get { return o_orbRadius; }
	}

	public static bool orbExpanding
	{
		get { return o_orbExpanding; }
	}

	public static bool orbContracting
	{
		get { return o_orbContracting; }
	}

	public void SetOrbRadius(float r) {
		targetRadius = r;
	}


	// Wrap the class inputs into curves
	void ScaleCurves() {
		o_ExpandCurve = new AnimationCurve ();
		o_ContractCurve = new AnimationCurve ();
		float offset = minRadius;

		foreach (Keyframe k in expandCurve.keys) {
			Keyframe k2 = k;
			k2.time *= expandTime;
			k2.value = k.value * radiusDiff + offset;
			k2.inTangent *= radiusDiff;
			k2.outTangent *= radiusDiff;
			o_ExpandCurve.AddKey (k2);
		}
		foreach (Keyframe k in contractCurve.keys) {
			Keyframe k2 = k;
			k2.time *= contractTime;
			k2.value = k.value * radiusDiff + offset;
			k2.inTangent *= radiusDiff;
			k2.outTangent *= radiusDiff;
			o_ContractCurve.AddKey (k2);
		}
	}

	void ExpandOrb() {
		if (!o_orbExpanding) {
			o_orbContracting = false;
			o_orbExpanding = true;
			keyDownTimmer = 0; // A very rough transition
		}
		if (keyDownTimmer < expandTime && keyDownTimmer >= 0f) {
			keyDownTimmer += Time.deltaTime;
			SetOrbRadius (o_ExpandCurve.Evaluate (keyDownTimmer));
		} 
	}

	void ContractOrb() {
		if (!o_orbContracting) {
			o_orbExpanding = false;
			o_orbContracting = true;
			keyDownTimmer = 0;
		}
		if (keyDownTimmer < contractTime && keyDownTimmer >= 0f) {
			keyDownTimmer += Time.deltaTime;
			SetOrbRadius (o_ContractCurve.Evaluate (keyDownTimmer));
		}
	}
		

	void Start () {
		GameObject orb = transform.Find ("Orb").gameObject;
		lightOrb = orb.GetComponent<Light> ();
		lightOrb.range = minRadius;
		col = orb.GetComponent<SphereCollider> ();
		radiusDiff = maxRadius - minRadius;
		ScaleCurves ();
	}

	void FixedUpdate () {
		if (Input.GetKey(expandLightKey) || Input.GetKey(Utilities.EXPAND_LIGHT_JOYSTICK_AXIS)) {
			ExpandOrb ();
		} else {
			ContractOrb ();
		}
	}

	void Update() {
		float newR = Mathf.Lerp (lightOrb.range, targetRadius, radiusSmoothing);
		lightOrb.range = newR;
		col.radius = newR * 0.95f;
		o_orbRadius = newR;
	}
}
