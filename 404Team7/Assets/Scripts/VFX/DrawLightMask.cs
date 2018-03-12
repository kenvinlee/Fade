/// <summary>
/// Draws a mask via vertex paint. Used by second camera for edge rendering.
/// 
/// Attached to each object that shows its edges when lit. Ideally this is done by VFXManager.
/// 
/// Detects if any light has contact with the object. Immediately colors that part white. 
/// Keeps dimming the color when light leaves. Sleeps when object goes completely black.
/// </summary>


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 
[RequireComponent( typeof(MeshFilter))]
public class DrawLightMask : MonoBehaviour {

	public float updateGap = 0.08f;
	public float fadeTime = 5f;
	public float transTime = 0.3f;

	private float fadeStep;

	private Mesh mesh;
	private Vector3[] vertices;
	private Color[] colors;  // r:lightMask

	public bool fading = false;
	public bool inLight = false;
	public bool scriptLoaded = false;
	private List<Transform> lights = new List<Transform>();
	private List<Light> lightcs = new List<Light>();


	void Start () {
        mesh = GetComponent<MeshFilter>().mesh;
		vertices = mesh.vertices;
		fadeStep = 1f*updateGap / fadeTime;

		// Start out all black;
		colors = new Color[vertices.Length];
		mesh.colors = colors;

		scriptLoaded = true; // Fix situations where object is lit at start
    }

	IEnumerator RePaint() {
		if (scriptLoaded) {
			fading = false;

			for (int j = 0; j < vertices.Length; j++) {
				if (colors [j].r >= 0f) {
					colors [j].r -= fadeStep;
					fading = true;
				}
				for (int i = 0; i < lights.Count; i++) {
//					Debug.Log ("Dest: " + Vector3.Distance (transform.TransformPoint (vertices [j]), lights [i].position)
//					+ "  Vert:" + transform.TransformPoint (vertices [j]) + "  Light:" + lights [i].position + "  Range:" + lightcs [i].range);
					if (Vector3.Distance (transform.TransformPoint(vertices [j]), lights [i].position) < lightcs [i].range) {
						colors [j].r = Mathf.Lerp(colors [j].r, 1f, transTime);
						fading = true;
						break;
					}
				}
			}
			mesh.colors = colors;
		}

		yield return new WaitForSeconds(updateGap);
		if (fading || inLight) {
			StartCoroutine ("RePaint");
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.CompareTag ("Orb")) {
			lights.Add (other.transform);
			lightcs.Add (other.GetComponent<Light> ());
			inLight = true;
			if (!fading) {
				fading = true;
				StartCoroutine ("RePaint");
			}
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.CompareTag ("Orb")) {
			lights.Remove (other.transform);
			lightcs.Remove (other.GetComponent<Light> ());
			if (lights.Count <= 0)
				inLight = false;
		}
	}
}
