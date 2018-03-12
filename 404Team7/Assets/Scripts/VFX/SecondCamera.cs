/// <summary>
/// Have the camera render the edges of objects and sends the result to MainCamera for blending.
/// 
/// Attached to the Second Camera that is a child of the main camera.
/// 
/// Renders the specified layers with edge detection shader. Sets its output size same as parent's.
/// </summary>


using System;
using UnityEngine;
using System.Collections;
namespace UnityStandardAssets.ImageEffects
{
//	[ExecuteInEditMode]
	[RequireComponent (typeof (Camera))]
	public class SecondCamera : PostEffectsBase
	{


		public Shader edgeDetectShader;
		public Shader lightMaskShader;

		private Material edgeDetectMaterial = null;
		private Camera cam;
//		private Camera parentCam;
		private RenderTexture target;

		new void Start ()
		{
//			edgeDetectShader = Shader.Find ("OnTheHorizon/EdgeDetect");
//			lightMaskShader = Shader.Find ("OnTheHorizon/LightMasks");

			if (lightMaskShader == null) {
				Debug.Log("Missing shader in " + ToString ());
				enabled = false;
			}
			if (CheckSupport (true)) {
				edgeDetectMaterial = CheckShaderAndCreateMaterial (edgeDetectShader, edgeDetectMaterial);

				cam = GetComponent<Camera> ();
//				parentCam = transform.parent.GetComponent<Camera> ();
//				cam.depth = parentCam.depth - 1; // Renders before main camera (-1)
				cam.depthTextureMode |= DepthTextureMode.DepthNormals;
				cam.SetReplacementShader (lightMaskShader, ""); // Render light mask
//				target = new RenderTexture(Mathf.RoundToInt(Screen.width/2), Mathf.RoundToInt(Screen.height/2), 16); // Half res
				StartCoroutine ("SizeCheck");
			} else {
				ReportAutoDisable ();
				Destroy (gameObject);
			}
		}

		IEnumerator SizeCheck() {
			if (target == null || target.width != Screen.width || target.height != Screen.height) {
				if (target != null) 
					target.Release ();
				target = new RenderTexture(Screen.width, Screen.height, 16);
				cam.targetTexture = target;
//				parentCam.GetComponent<MainCamera> ().child = target;
				Shader.SetGlobalTexture ("_ChildTex", target);
			}
			yield return new WaitForSeconds(5f);
			StartCoroutine ("SizeCheck");
		}

		[ImageEffectOpaque]
		void OnRenderImage (RenderTexture source, RenderTexture destination)
		{

			Graphics.Blit (source, destination, edgeDetectMaterial); // Mix Light mask with edge detection
		}
	}
}

