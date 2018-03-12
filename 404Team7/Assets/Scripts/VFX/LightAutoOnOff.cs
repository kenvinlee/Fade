using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightAutoOnOff : MonoBehaviour {
	[Header ("Override VFXManager")]
	public int checkEveryNSecond = -1;
	public float fadeStartDistance = -1f;
	public float fadeLerpping = -1f;
	public float approximateEpsilon = -1f;

	private float initIntensity = 0f;
	private Light l;
	private Transform playerT;
	private float goalIntensity = 0f;

	// Use this for initialization
	void Start () {
		playerT = GameObject.Find ("Player").transform;
		l = GetComponent<Light> ();
		if (l == null) {
			enabled = false;
		}
		initIntensity = l.intensity;
		goalIntensity = initIntensity;
		StartCoroutine (DistCheck());

		if (checkEveryNSecond < 0)
			checkEveryNSecond = VFXManager.checkEveryNSec;
		if (fadeStartDistance < 0)
		fadeStartDistance =  VFXManager.fadeDistance;
		if (fadeLerpping < 0)
			fadeLerpping = VFXManager.fadeLerp;
		if (approximateEpsilon < 0)
			approximateEpsilon = VFXManager.epsilon;
	}
	
	// Update is called once per frame
	void Update () {
		if (l.enabled && Mathf.Abs(l.intensity - goalIntensity)>approximateEpsilon) {
			l.intensity = Mathf.Lerp (l.intensity, goalIntensity, fadeLerpping);
			if (l.intensity <= approximateEpsilon) {
				l.intensity = 0f;
				l.enabled = false;
			} 
		}
	}


	IEnumerator DistCheck() {
		while (true) {
			if (Vector3.Magnitude (transform.position - playerT.position) > fadeStartDistance) {
				goalIntensity = 0f;
			} else {
				goalIntensity = initIntensity;
				if (l.enabled == false) {
					l.enabled = true;
				}
			}
			yield return new WaitForSeconds (checkEveryNSecond);
		}
	}
}
