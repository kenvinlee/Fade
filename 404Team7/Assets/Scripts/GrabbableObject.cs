using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Object that a player can grab/pick up.
 * Should be placed on any object with a
 * Collider.
 * Recommended to dragged onto an object
 * that cannot be seen (MeshRenderer off)
 * so that the grabbable area is larger.
*/
[RequireComponent(typeof (Collider))]
public class GrabbableObject : MonoBehaviour {
	public string interactTriggerName = "";
	public bool resetTriggerWhenDropped = false;
    public float scale = 0.5f;
	public bool grabbableInLight = false;
	public float objectRotateSpeed = 8f;
	public string objectName = "";
	private bool _heldByPlayer;
	private Color _originalColor;
	private GameObject floatingText;
	private Animator anim;

	private bool resetAnim = false;
	private Quaternion target;

	// Use this for initialization
	void Start () {
		gameObject.layer = LayerMask.NameToLayer (Utilities.INTERACTABLE_LAYER_NAME);
		_originalColor = transform.GetComponent<Renderer> ().material.color;
//		_rb = GetComponent<Rigidbody> ();
		if (interactTriggerName != "")
			anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
//		if (_heldByPlayer) {
//
//		}
		if (resetAnim && (transform.localRotation.eulerAngles - target.eulerAngles).magnitude > 0.5) {
			transform.localRotation = Quaternion.Slerp (transform.localRotation, target, Time.deltaTime * objectRotateSpeed);
		} else {
			resetAnim = false;
		}
	}

	//activate the item's use.
	public void activateItem(){
		releaseItem ();
		if (resetTriggerWhenDropped && anim != null)
			anim.SetTrigger("Reset");
	}

	//attach item to player.
	public void grabObject(){
		// Call OnTriggerExit on all colliding objects
		FlushTriggerCallBack ();
		//Don't want grabbed object to affect collisions.
		setCollideable (false);
		//Set alpha lower so we don't  lose all of screen realestate.
		setAlpha(0.4f);
        //Set the scale of the object on pickup
		setScale(scale);
		//Disable Physics
		Destroy (GetComponent<Rigidbody> ());
		// Start reset rotation animation.
		target = Quaternion.LookRotation (new Vector3(5, 0, -10), Vector3.up);
		resetAnim = true;
		_heldByPlayer = true;

		if (anim != null)
			anim.SetTrigger (interactTriggerName);
	}

    public void setScale(float s) {
        transform.localScale = new Vector3(transform.localScale.x * s, transform.localScale.y * s, transform.localScale.z * s);

    }
	//release item from player.
	public void releaseItem(){
		setAlpha(1f);
		setScale(1 / scale);

		// Enable physics.
		setCollideable (true);
		gameObject.AddComponent<Rigidbody>();
		resetAnim = false;

		_heldByPlayer = false;
	}

	private void setAlpha(float a){
		transform.GetComponent<Renderer> ().material.color = new Color (_originalColor.r, _originalColor.g, _originalColor.b, a);
		foreach(Transform child in transform){
			if (child.GetComponent<Renderer> () != null) {
				if (child.GetComponent<Renderer> ().material.HasProperty("_Color")) {
					Color c = child.GetComponent<Renderer> ().material.color;
					child.GetComponent<Renderer> ().material.color = new Color (c.r, c.g, c.b, a);
				}
			}
		}
	}

	private void setCollideable(bool isCollideable){
		transform.GetComponent<Collider> ().enabled = isCollideable;
//		foreach(Transform child in transform){
//			if (child.GetComponent<Collider> () != null) {
//				child.GetComponent<Collider> ().enabled = isCollideable;
//			}
//		}
	}
		
	public bool isHeld(){
		return _heldByPlayer;
	}

	public string getName(){
		if (objectName != "") {
			return objectName;
		} else {
			return name;
		}
	}

	// Below is fix for mushroom animating obj permenately bug
	// Keep all colliding objects' OnTriggerExists
	public delegate void TriggerCallBack(Collider obj);
	private List<TriggerCallBack> _cbs = new List<TriggerCallBack>();
	private Collider selfTriggerCollider = null;

	public void AddReleaseCallBack(Collider c, TriggerCallBack cb) {
		_cbs.Add (cb);
		selfTriggerCollider = c;
	}
	public void RemoveReleaseCallBack(TriggerCallBack cb) {
		_cbs.Remove (cb);
	}

	// When player grabs this mushroom, execute all trigger callbacks.
	// If obj is not mushroom, this fix does nothing.
	public void FlushTriggerCallBack() {
		foreach (TriggerCallBack cb in new List<TriggerCallBack> (_cbs)) {
			cb (selfTriggerCollider);
		}
		_cbs = new List<TriggerCallBack>();
	}
}
