/// <summary>
/// Blends the output of this camera and the one sent by child camera and render to screen.
/// 
/// Attached to the Main camera in player.
/// 
/// Finds all hintLights in scene and turn them off when rendering. Turn them back on when done.
/// Uses composite shader to blend the results. The output is written to screen.
/// </summary>


using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
namespace UnityStandardAssets.ImageEffects
{
//	[ExecuteInEditMode]
	[RequireComponent (typeof (Camera))]
	public class MainCamera : PostEffectsBase
	{
//		[HideInInspector]
//		public RenderTexture child;

		public Shader compositeShader;
		private Material compositeMaterial;

		new void Start ()
		{
//			compositeShader = Shader.Find ("OnTheHorizon/Composite");
			if (CheckSupport (true)) {
//				cam = GetComponent<Camera> ();
				compositeMaterial = CheckShaderAndCreateMaterial (compositeShader, compositeMaterial);
			} else {
				ReportAutoDisable (); 
			}
		}


		[ImageEffectOpaque]
		void OnRenderImage (RenderTexture source, RenderTexture destination)
		{
//			compositeMaterial.SetTexture ("_ChildTex", child);
			Graphics.Blit (source, destination, compositeMaterial); // Mix Light mask with edge detection
		}
	}
}

