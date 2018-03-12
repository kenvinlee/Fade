using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Used for player to grab/use items.
 * If the object the player is staring at (Raycast)
 * is a GrabbableObject or InteractableObject, we 
 * display a message in the GUI camera.
 * 
 * Also keeps track of what we are currently
 * holding on to so that we can use GrabbableObjects
 * on InteractableObjects.
 */ 
public class GrabberScript : MonoBehaviour {
	private Transform _playerCamera;
	//	private Transform _playerHolding;
	private GrabbableObject _lastObjGrabbed;
	private GrabbableObject _currentObjGrabbed;
	private GameObject floatingTextObj;
	private GameObject imagePromptObj;
	private Collider _lightCollider;
	public LayerMask ignoreLayer;
	private bool isTextVisible = false;

	public GameObject floatingTextPrefab;
	public GameObject GUICamera;
	public GameObject itemGrabImagePromptPrefab;
	public GameObject lightExpandLightPromptPrefab;
	public float grabRange = 6;

	private bool m_UISMExist = true;
	private bool wasTextVisible = false;
	private string lastText = "";

	// Use this for initialization
	void Start () {
		//		ignoreLayer = LayerMask.NameToLayer ("Water");
		//		Debug.Log (LayerMask.LayerToName(ignoreLayer.value));
		_playerCamera = GameObject.Find (Utilities.PLAYER_CAMERA_NAME).transform;
		_lightCollider = GameObject.Find (Utilities.LIGHT_NAME).GetComponent<Collider>();
		//		_playerHolding = GameObject.Find (Utilities.PLAYER_Holding).transform;
		m_UISMExist = UIStateManager.controller != null;
		GUICamera = GameObject.Find (Utilities.GUI_CAMERA_NAME);
	}

	// Update is called once per frame
	void Update () {
		wasTextVisible = isTextVisible;
		isTextVisible = false;

		// Raycast to see if something is in front
		RaycastHit hitInfo;
		bool interactableInsight = false;
		bool grabbableInsight = false;

		/// Raycast merged into update() on 3/14. Jack
		/// Original goal was to fix UISM prompt being blocked.
		/// Turns out I used wasTextVisible + compared with lastText instead.
		if (Physics.Raycast (_playerCamera.position, _playerCamera.forward, out hitInfo, grabRange, ignoreLayer)) {
			interactableInsight = detectInteractables(hitInfo);
			grabbableInsight = detectGrabbableObjects (hitInfo);
		}

		//Grab/Use an item.
		if (Input.GetKeyUp (Utilities.INTERACT_JOYSTICK_BUTTON) ||Input.GetKeyUp(Utilities.INTERACT_KEYBOARD_BUTTON)) {
			if (!useItem (hitInfo)) // If item cannot be used, release it.
				releaseItem ();
			if (grabbableInsight) 
				grabObject (hitInfo);
		}

		if (Input.GetKeyUp (Utilities.EXPAND_LIGHT_JOYSTICK_BUTTON)) {
			if (_lastObjGrabbed != null && _currentObjGrabbed == null) {
				if (!_lastObjGrabbed.name.Contains ("Exploding Bomb")) {
					_currentObjGrabbed = _lastObjGrabbed;
					attachGrabbedObjectToPlayer ();
					_currentObjGrabbed.grabObject ();
				}
			}
		}

		// Legacy prompt system clear
		if (floatingTextObj != null) {
			floatingTextObj.GetComponent<Renderer> ().enabled = isTextVisible;
			lastText = "";
		}

		// UISM clear only once
		if (m_UISMExist && wasTextVisible && !isTextVisible) {
			UIStateManager.controller.ClearPrompt ();
			lastText = "";
		}
	}

	private void attachGrabbedObjectToPlayer(){
		//Will change to adding the object to a player's hand or somewhere else once defined.
		Vector3 normDirection = Vector3.Normalize (_playerCamera.forward);
		Vector3 orthoDirection = new Vector3 (-1 * normDirection.z, 0, normDirection.x);
		_currentObjGrabbed.transform.position = new Vector3 (_playerCamera.position.x + normDirection.x + orthoDirection.x/2, 
			_playerCamera.position.y + normDirection.y, 
			_playerCamera.position.z + normDirection.z + orthoDirection.z/2);
		_currentObjGrabbed.transform.parent = _playerCamera;
	}

	private bool useItem(RaycastHit hitInfo){
		if (hitInfo.transform != null && Utilities.isObjectInSight (hitInfo.transform.gameObject)) {
			InteractableObject obj = hitInfo.transform.GetComponent<InteractableObject> ();
			if (obj != null && obj.isInteractable ()) {
				obj.interact (_currentObjGrabbed);
				return true;
			}
		}
		return false;
	}

	private bool grabObject(RaycastHit hitInfo){
		if (hitInfo.transform.GetComponent<GrabbableObject> () != null) {
			if (!hitInfo.collider.CompareTag ("Orb")) {
				GrabbableObject obj = hitInfo.transform.GetComponent<GrabbableObject> ();
				if (Utilities.isObjectInSight (obj.gameObject) || obj.grabbableInLight) {
					if (_currentObjGrabbed != null)
						releaseItem ();
					_currentObjGrabbed = obj;
					_lastObjGrabbed = _currentObjGrabbed;
					attachGrabbedObjectToPlayer ();
					obj.grabObject ();
					if (floatingTextObj != null) {
						floatingTextObj.GetComponent<Renderer> ().enabled = false;
					}
					return true;
				}
			}
		}
		return false;
	}

	//If we are starring at a grabbable object, prompt player to grab it.
	private bool detectGrabbableObjects(RaycastHit hitInfo){
		if (!hitInfo.collider.CompareTag("Orb")) {
			GrabbableObject obj = hitInfo.transform.GetComponent<GrabbableObject> ();
			if (obj != null && (Utilities.isObjectInSight (obj.gameObject) || obj.grabbableInLight)) {
				Prompt (hitInfo.transform.name + "\n[" + "LB" + "]");
				return true;
			}
		}
		return false;
	}

	private bool detectInteractables(RaycastHit hitInfo){
		if (hitInfo.transform.GetComponent<InteractableObject> () != null && hitInfo.transform.GetComponent<InteractableObject> ().isInteractable()) {
			if (Utilities.isObjectInSight (hitInfo.transform.gameObject)) {
				Prompt (hitInfo.transform.GetComponent<InteractableObject> ().getPrompt ());
				return true;
			}
		}
		return false;
	}

	private void Prompt(string text) {
		if (lastText != text) {
			if (m_UISMExist)
				UIStateManager.controller.Prompt (text, -1f);
			else
				loadFloatingText (text);
			lastText = text;
		}
		isTextVisible = true;
	}

	/// Prompt has been integrated into UISM. Sorry for the inconvenience. My cursor broke GUICamera. 
	/// Also Why not make a parent function for the above 4 functions and Raycast once?
	/// Jack. 3/13/2017

	private void loadFloatingText(string message){
		if (floatingTextObj == null) {
			floatingTextObj = (GameObject)GameObject.Instantiate (floatingTextPrefab);
		}
		Vector3 GUICameraPos = GUICamera.transform.position;
		floatingTextObj.transform.position = new Vector3 (GUICameraPos.x,
			GUICameraPos.y,
			GUICameraPos.z + Utilities.getCurrentLightRadius () / 2);
		//Utilities.INTERACT_BUTTON is LB, I just put LB for clarity at the moment
		floatingTextObj.GetComponent<TextMesh> ().text = message;
		if (itemGrabImagePromptPrefab != null) {
			imagePromptObj = (GameObject)GameObject.Instantiate (itemGrabImagePromptPrefab);
			imagePromptObj.transform.position = floatingTextObj.transform.position;
			imagePromptObj.tag = "UI";
		}
	}

	private void updateGrabbedObjectPosition(){
		Vector3 normDirection = Vector3.Normalize (_playerCamera.forward);
		Vector3 orthoDirection = new Vector3 (-1 * normDirection.z, 0, normDirection.x);
		_currentObjGrabbed.transform.position = new Vector3 (_playerCamera.position.x + normDirection.x + orthoDirection.x/2, 
			_playerCamera.position.y + normDirection.y, 
			_playerCamera.position.z + normDirection.z + orthoDirection.z/2);
	}

	public void releaseItem(){
		if (_currentObjGrabbed == null) {return;}
		_currentObjGrabbed.transform.parent = null;
		_currentObjGrabbed.activateItem ();
		_currentObjGrabbed = null;
	}

	public GameObject getCurrentObjectGrabbed(){
		if (_currentObjGrabbed == null) {return null;}
		return _currentObjGrabbed.gameObject;
	}
}
