using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Accumulate texture class stores additional copy of each frame and pass them into next frame for composite.
/// </summary>
[RequireComponent (typeof (Camera))]
public class AccumulateTexture : MonoBehaviour {
	public Shader lightMaskAccumulateShader;
	public RenderTexture lastFrame;
	private Material lightMaskMaterial = null;
	private RenderTexture target;
	public int usePass = 0;

	// Use this for initialization
	void Start () {
		target = GetComponent<Camera> ().targetTexture;
		lastFrame = new RenderTexture(target.width, target.height, 0, RenderTextureFormat.R8);
		lastFrame.antiAliasing = 2;
		lastFrame.Create ();

		lightMaskMaterial = new Material (lightMaskAccumulateShader);
		lightMaskMaterial.SetTexture ("_LastFrame", lastFrame);
	}

	[ImageEffectOpaque]
	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		// Add together the colors from multiple frames.
		// Turns out blitting twice is way faster than copying it back from GPU
		Graphics.Blit (source, lastFrame, lightMaskMaterial, usePass); 
		Graphics.Blit (lastFrame, destination); 
	}
}
