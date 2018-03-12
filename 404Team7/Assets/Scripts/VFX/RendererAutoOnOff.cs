using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RendererAutoOnOff : MonoBehaviour {
	[Header ("Override VFXManager")]
	public int checkEveryNSecond = -1;
	public float fadeStartDistance = -1f;
	private List<MeshRenderer> childrenMeshes = new List<MeshRenderer>();
	private Transform playerT;

	// Use this for initialization
	void Start () {
		playerT = GameObject.Find ("Player").transform;
		StartCoroutine (DistCheck());
		if (checkEveryNSecond < 0)
			checkEveryNSecond = VFXManager.checkEveryNSec;
		if (fadeStartDistance < 0)
			fadeStartDistance =  VFXManager.fadeDistance;
		
		//Grab all the mesh renderers and store it.
		MeshRenderer[] meshes = gameObject.GetComponentsInChildren<MeshRenderer> ();
		foreach (MeshRenderer mesh in meshes) {
			childrenMeshes.Add (mesh);
		}
	}

	// Update is called once per frame
	void Update () {

	}


	IEnumerator DistCheck() {
		while (true) {
			float dist = Vector3.Magnitude (transform.position - playerT.position);
			if (dist > fadeStartDistance) {
				setRender(false);
			} else {
				setRender(true);
			}
			yield return new WaitForSeconds (checkEveryNSecond);
		}
	}

	private void setRender(bool b){
		for(int i = 0; i < childrenMeshes.Count; i++){
			childrenMeshes[i].enabled = b;
		}
	}
}
