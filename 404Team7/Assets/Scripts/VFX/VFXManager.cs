/// <summary>
/// VFX manager of the scene. Helper of Reveal3 shader and rendering.
/// 
/// Finds all lights and assign each a white sphere for rendering. Sets up 
/// all camera communications and sets global vars. Paints the mask and also
/// maintain the debugging planes.
/// 
/// Call AttachLightBlob(GameObject) to give your instantiated object special effects.
/// </summary>


using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class VFXManager : MonoBehaviour {
	[Header("Silhouette Effect")]
	[Tooltip ("A large value will blur the mask, making the latter inaccurate")]
	public float fadeTime = 8f;
	[Tooltip ("A Small value ensures mask accuracy at a price of more shutterings")]
	[Range (1, 8)]
	public int pixelPerfectLevel = 3;
	[Range (0,1)]
	public float emissionInDark = 1f;
	public float sensitivityDepth = 1.0f;
	public float sensitivityNormals = 1.0f;
	public float sampleDistance = 1.0f;
	public float faceIntensity = 0.1f;
	public float borderIntensity = 1.0f;
	public Color backgroundColor = Color.black;
	[Tooltip ("Applied at edge of silhouette effect mask")]
	public Texture maskRamp;
	[Tooltip ("Custom light falloff image")]
	public Texture lightFallOff;
//	[Tooltip ("Expand raius of silhouette effect, set to negative to hide white lines at edge of light orb")]
//	public float effectBoundaryExpand = 0f;
//	[Tooltip ("Expand radius of full rendering, set a small value so objects at edge of light orb don't appear clipped")]
//	public float lightRenderBoundaryExpand = 1f;
//	[Tooltip ("Inflate objects before edge detection, set to a small negative value to hide white lines around objects when held in front of light")]
//	public float edgeDetectInflate = -1f;

	[Header ("Light Auto On/Off")]
	public  int checkEveryNSecond = 2;
	public  float fadeStartDistance = 40f;
	public  float fadeLerpping = 0.3f;
	public  float approximateEpsilon = 0.01f;
	public static int checkEveryNSec;
	public static float fadeDistance;
	public static float fadeLerp;
	public static float epsilon;
//	private List<GameObject> stripLightOnOff = new List<GameObject>();  // Doesn't work

	[Header("Misc")]
	public bool debug = true;
	public GameObject sphere;
	[Tooltip("This light will be turned off in game")]
	public GameObject editorAidLight;
	private Material lightMaterial;
//	public Shader plainColourShader;
	public Shader lightMaskAccumulateShader;
//	public Shader Reveal5;
//	public Shader Reveal5Full;
	private Transform Projector, player, projectorDebug;
	private LayerMask hintLight, lightOrb;

	private GameObject[] cameras = new GameObject[3];
	public RenderTexture[] Masks = new RenderTexture[3];
	private List<GameObject> lightBlobs = new List<GameObject>();
	private Dictionary<Material, Shader> matsToRestore = new Dictionary<Material, Shader> ();
	private int texSize = 256*3;

	void Awake () {
		if (editorAidLight != null)
			editorAidLight.SetActive (false);

		// Find references
		Projector = GameObject.Find ("Projector").transform;
		player = GameObject.Find ("Player").transform;
		hintLight = LayerMask.NameToLayer ("HintLight");
		lightOrb = LayerMask.NameToLayer ("LightOrb");

		// Find and initialize all cameras and their targets.
		cameras [0] = Projector.Find ("CameraTop").gameObject;
		cameras [1] = Projector.Find ("CameraLeft").gameObject;
		cameras [2] = Projector.Find ("CameraFront").gameObject;

		for (int i=0;i<cameras.Length;i++) {
			cameras [i].GetComponent<Camera> ().targetTexture = Masks[i];
//			cameras [i].GetComponent<Camera> ().SetReplacementShader (plainColourShader, "");
			AccumulateTexture at = cameras[i].AddComponent<AccumulateTexture>();
			at.usePass = i;
			at.lightMaskAccumulateShader = lightMaskAccumulateShader;
		}

		// Set all global shader variables
		Shader.SetGlobalTexture("_MaskTop", Masks[0]);
		Shader.SetGlobalTexture("_MaskLeft", Masks[1]);
		Shader.SetGlobalTexture("_MaskFront", Masks[2]);
//		player.Find ("PlayerCamera/Camera/SecondCamera").GetComponent<Camera>().backgroundColor = backgroundColor;
//		Shader.SetGlobalColor ("_BgColor", backgroundColor);

		Shader.SetGlobalFloat ("_EmissionInDark", emissionInDark);
		Vector2 sensitivity = new Vector2 (sensitivityDepth, sensitivityNormals);
		Shader.SetGlobalVector ("_Sensitivity", new Vector4 (sensitivity.x, sensitivity.y, 1.0f, sensitivity.y));
		Shader.SetGlobalFloat ("_BorderIntensity", borderIntensity);
		Shader.SetGlobalFloat ("_FaceIntensity", faceIntensity);
		Shader.SetGlobalFloat ("_SampleDistance", sampleDistance);

//		Shader.SetGlobalFloat ("_ExpandEffect", effectBoundaryExpand);
//		Shader.SetGlobalFloat ("_ExpandLight", lightRenderBoundaryExpand);
//		Shader.SetGlobalFloat ("_InflateEffect", edgeDetectInflate);

		Shader.SetGlobalTexture ("_MaskRamp", maskRamp);
		Shader.SetGlobalTexture ("_Falloff", lightFallOff);

		texSize = Masks [0].width*pixelPerfectLevel;

		// Use full functionality of Reveal5. i.e. Stencil testing. Unfortunately this will disable Unity Editor Scene rendering.
//		Transform[] allObj = Resources.FindObjectsOfTypeAll<Transform> ();
//		foreach (Transform obj in allObj) {
//			MeshRenderer renderer = obj.gameObject.GetComponent<MeshRenderer> ();
//			if (renderer != null) {
//				foreach (Material m in renderer.sharedMaterials) {
//					if (m != null && m.shader != null && (m.shader == Reveal5 || m.shader.name == "OnTheHorizon/Reveal3")) {
//						matsToRestore.Add (m, m.shader);
//						m.shader = Reveal5Full;
//					}
//				}
//			}
//			SkinnedMeshRenderer srenderer = obj.gameObject.GetComponent<SkinnedMeshRenderer> ();
//			if (srenderer != null) {
//				foreach (Material m in srenderer.sharedMaterials) {
//					if (m != null && m.shader != null && (m.shader == Reveal5 || m.shader.name == "OnTheHorizon/Reveal3")) {
//						matsToRestore.Add (m, m.shader);
//						m.shader = Reveal5Full;
//					}
//				}
//			}
//		}
		// Static variables.
		checkEveryNSec = checkEveryNSecond;
		fadeDistance = fadeStartDistance;
		fadeLerp = fadeLerpping;
		epsilon = approximateEpsilon;


		// Prepare debug planes if needed
		GameObject projectorDebugG = GameObject.Find ("ProjectorDebug");
		if (projectorDebugG == null)
			debug = false;
		else
			projectorDebug = projectorDebugG.transform;

		if (debug) {
			projectorDebug.Find ("Top").gameObject.GetComponent<MeshRenderer> ().material.mainTexture = Masks [0];
			projectorDebug.Find ("Left").gameObject.GetComponent<MeshRenderer> ().material.mainTexture = Masks [1];
			projectorDebug.Find ("Front").gameObject.GetComponent<MeshRenderer> ().material.mainTexture = Masks [2];
		} else if (projectorDebug != null) {
			projectorDebug.gameObject.SetActive (false);
		}

	}

	void Start() {
		// Attach each light a white sphere for rendering. Hide them from the main cameras
		Light[] lights = Resources.FindObjectsOfTypeAll (typeof(Light)) as Light[];
		foreach (Light l in lights) {
			if (l.type == LightType.Point) {
				GameObject lg = l.gameObject;
				AttachLightBlob(lg);
				if (lg.layer == hintLight) {
					l.enabled = false;
				} else {
					lg.layer = lightOrb;
					if (l.enabled && lg.GetComponent<LightAutoOnOff> () == null) {
						lg.AddComponent<LightAutoOnOff> ();
//						stripLightOnOff.Add (lg);
					}
				}
			}
		}

		//Attach each complex object with a script to only render when needed. 
		GameObject[] complex_objects = GameObject.FindGameObjectsWithTag("Complex Object");
		foreach (GameObject complex_obj in complex_objects) {
			complex_obj.AddComponent<RendererAutoOnOff> ();
		}

	}

	private float dimDue = 1f;
	private Vector3 playerMovedDue = Vector3.zero;
	void Update() {
		// Update mask shader variables.
		playerMovedDue += (player.position - Projector.position) / Projector.localScale.x;
		Vector3 playerMoved = playerMovedDue * texSize;
		playerMoved = new Vector3 (Mathf.Floor (playerMoved.x), Mathf.Floor (playerMoved.y), Mathf.Floor (playerMoved.z)) / texSize;
		playerMovedDue -= playerMoved;
		Shader.SetGlobalVector ("_PlayerMoved", playerMoved);

		Projector.position = player.position;
		Shader.SetGlobalVector ("_ProjectorPos", Projector.position);
		Shader.SetGlobalFloat ("_MaskScale", Projector.localScale.x);

		// Dim the mask. Due to possible GPU overflow, no value smaller then 0.005f gets processed.
		dimDue += Time.deltaTime/ fadeTime;
		if (dimDue >= 0.005f) {
			Shader.SetGlobalFloat ("_DimRate", dimDue);
			dimDue = 0f;
		} else {
			Shader.SetGlobalFloat ("_DimRate", 0f);
		}

		if (debug && projectorDebug != null) {
			projectorDebug.position = player.position;
		}
	}

	// Prefabs have this problem of not being able to be child of others prefabs.
	// They issue errors when being deleted by Unity. So we do it before Unity does.
	void OnApplicationQuit() {
		foreach (GameObject s in lightBlobs) {
			s.transform.parent = null; //The fix.
			Destroy (s);
		}
		foreach (Material m in matsToRestore.Keys) {
			m.shader = matsToRestore[m];
		}
		// Unfortunately, scripts attached to prefabs can't be destroyed.
//		foreach (GameObject gl in stripLightOnOff) {
//			DestroyImmediate (gl.GetComponent<LightAutoOnOff> ());
//		}
	}

	// Callable this function for special effect on g.
	public void AttachLightBlob(GameObject g) {
		Light l = g.GetComponent<Light> ();
		if (l != null) {
			GameObject s = Instantiate (sphere, g.transform.position, g.transform.localRotation, g.transform);
			// Resolve deformation due to parent scale.
			Vector3 gs = s.transform.lossyScale;
			Vector3 ls = Vector3.one * l.range * 1.9f;
			s.transform.localScale = new Vector3 (ls.x / gs.x, ls.y / gs.y, ls.z / gs.z);
			lightBlobs.Add (s);
		}
	}
}