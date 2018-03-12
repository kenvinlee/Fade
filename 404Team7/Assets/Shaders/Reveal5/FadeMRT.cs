using UnityEngine;

[ExecuteInEditMode]
public class FadeMRT : MonoBehaviour
{
	public Shader Composite;
	public RenderTexture rt1, rt2, rt3, rt4;
	public Camera cam;

    private Material _compMat;
	private RenderBuffer[] _mrt;

    void OnEnable()
    {
		if (SystemInfo.supportedRenderTargetCount > 1) {
			_compMat = new Material (Composite);
			_compMat.hideFlags = HideFlags.DontSave;

			// Setup MRT output RenderBuffers
			if (Screen.width > 0 && Screen.height > 0) {
				_mrt = new RenderBuffer[4];
				rt1 = new RenderTexture (Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
				rt2 = new RenderTexture (Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
				rt3 = new RenderTexture (Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
				rt4 = new RenderTexture (Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
				_mrt [0] = rt1.colorBuffer;
				_mrt [1] = rt2.colorBuffer;
				_mrt [2] = rt3.colorBuffer;
				_mrt [3] = rt4.colorBuffer;
				cam.SetTargetBuffers (_mrt, rt1.depthBuffer);
			} else {
				enabled = false;
			}

			// Setup MRT camera
//		cam.CopyFrom(gameObject.GetComponent<Camera>());
//		cam.cullingMask = 0;
//			cam.enabled = false;
		} else {
			Debug.LogWarning ("MRT not supported by this GPU. Disabling effects.");
		}
    }

    void OnDisable()
    {
        DestroyImmediate(_compMat);
        _compMat = null;
        _mrt = null;
		rt1.Release ();
		rt2.Release ();
		rt3.Release ();
		rt4.Release ();
    }

	void OnPreRender() {

		// Also clear the second render target
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = rt2;
		GL.Clear (true, true, Color.clear);
		RenderTexture.active = rt3;
		GL.Clear (true, true, Color.clear);
		RenderTexture.active = rt4;
		GL.Clear (true, true, Color.clear);
		RenderTexture.active = active;

		// Render to MRT
//		cam.RenderWithShader(Reveal5, "");
		cam.Render();
	}

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // Combine them and output to the destination.
        _compMat.SetTexture("_MaskTex", rt2);
		_compMat.SetTexture("_DepthNormals", rt3);
		_compMat.SetTexture("_Overlay", rt4);
		Graphics.Blit(rt1, destination, _compMat);
    }

//    void OnGUI()
//    {
//        var text = "Supported MRT count: ";
//        text += SystemInfo.supportedRenderTargetCount;
//        GUI.Label(new Rect(0, 0, 200, 200), text);
//    }
}
