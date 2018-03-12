using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectColor : MonoBehaviour {
	public Color silhouetteColor = Color.white;
	private Mesh mesh;
	private Color[] oldColors, colors;

	// Use this for initialization
	void Start () {
		if (GetComponent<MeshFilter> () != null) {
			mesh = GetComponent<MeshFilter> ().mesh;
		} else if (GetComponent<SkinnedMeshRenderer> () != null) {
			mesh = GetComponent<SkinnedMeshRenderer> ().sharedMesh;
		} else {
			enabled = false;
		}

		oldColors = mesh.colors;
		colors = new Color[mesh.vertices.Length];
		Paint (silhouetteColor);
	}

	public void Paint(Color c) {
		if (mesh != null) {
			for (int j = 0; j < colors.Length; j++) {
				colors [j] = silhouetteColor;
			}
			mesh.colors = colors;
		}
	}

	public void OnApplicationQuit() {
		mesh.colors = oldColors;
	}
}
