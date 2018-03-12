using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowingWater : MonoBehaviour {
	public float flowSpeed = 0.5f;
	public float flowSpeedBump = 0.5f;
	public Material waterMat;
	private float offset = 0f;
	private float offsetBump = 0f;

	// Update is called once per frame
	void Update () {
		offset -= flowSpeed * Time.deltaTime;
		offsetBump -= flowSpeedBump * Time.deltaTime;
		waterMat.SetTextureOffset ("_MainTex", new Vector2(offset, 0));
		waterMat.SetTextureOffset ("_BumpMap", new Vector2(offsetBump, 0));
	}

	void OnApplicationQuit() {
		waterMat.SetTextureOffset ("_MainTex", Vector2.zero);
		waterMat.SetTextureOffset ("_BumpMap", Vector2.zero);
	}
}
