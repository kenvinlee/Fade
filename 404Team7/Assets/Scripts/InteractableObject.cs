using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Object that a player can use an item on.
 * Is placed on objects like locked door.
 * If Lock script(s) is attached to the same
 * object it will use those locks as _locks.
 * 
 * Recommended to dragged onto an object
 * that cannot be seen (MeshRenderer off)
 * so that the interact area is larger.
*/

public class InteractableObject : MonoBehaviour {
	public string interactTriggerName = "";
	public string interactPrompt;
	public AudioClip audioOnInteraction;

	private Animator _animator;
	private bool _interactionComplete = false;
	private List<Lock> _locks = new List<Lock> ();
	private GameObject floatingTextObj;
	private int currPriority = 0;
	// Use this for initialization
	void Start () {
		gameObject.layer = LayerMask.NameToLayer (Utilities.INTERACTABLE_LAYER_NAME);

		//The interactableobject may be a child object a gameobject with the animator.
		Transform curr_parent = transform;
		while (curr_parent != null) {
			if (curr_parent.GetComponent<Animator> () != null) {
				_animator = curr_parent.GetComponent<Animator> ();
				break;
			}
			curr_parent = curr_parent.parent;
		}

		//Get all the lock components.
		for(int i = 0; i < transform.GetComponents<Lock>().Length; i++){
			Lock l = transform.GetComponents<Lock> ()[i];
			currPriority = Mathf.Max (currPriority, l.priority);
			_locks.Add (l);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public bool isInteractable(){
		return !_interactionComplete;
	}
		
	public void interact(GrabbableObject g=null){
		//Check if we've been given an item.
		if (g == null || (g != null && !useItemOnLock (g))) {
			//If there are no locks left, activate the animation (Ex: open door, tree is chopped down, etc)
			if (_locks.Count == 0) {
				_interactionComplete = true;
				if (audioOnInteraction != null) {
					AudioSource.PlayClipAtPoint (audioOnInteraction, this.transform.position, 0.7f);
				}
				if (interactTriggerName != "") {
					_animator.SetTrigger (Animator.StringToHash (interactTriggerName));
					if (transform.parent.GetComponent <BoxCollider> () != null)
						transform.parent.GetComponent <BoxCollider> ().isTrigger = true;
				}
			}
		}
	}

	/*
	 * Goes through all the locks and tries to use item g on the ones
	 * with highest priority.
	 */ 
	private bool useItemOnLock(GrabbableObject g){
		for (int i = 0; i < _locks.Count; i++) {
			if (_locks [i].unlock (g) && _locks[i].priority >= currPriority) {
				_animator.SetTrigger (Animator.StringToHash (_locks[i].animationTrigger));
				AudioSource.PlayClipAtPoint (_locks[i].audioOnUnlock, this.transform.position, 0.4f);
				_locks.RemoveAt (i);
				currPriority--;
				Destroy (g.gameObject);
				return true;
			}
		}
		return false;
	}

	/*
	 * Returns the prompt for the player based on which lock
	 * has most priority.
	 */
	public string getPrompt(){
		string current_prompt = interactPrompt + "\n[" + "LB" + "]";
		for (int i = 0; i < _locks.Count; i++) {
			if (_locks [i].priority >= currPriority) {
				current_prompt = _locks [i].prompt;
				if (_locks [i].requiredItem != null) {
					current_prompt += "\n[" + "LB" + "]";
				}
				break;
			}
		}
		return current_prompt;
	}

	/*
	 * Displays the prompt on our GUI camera
	 */
	public void showPrompt(GameObject floatingTextPrefab, GameObject GUICamera){
		if (floatingTextObj == null) {
			floatingTextObj = (GameObject)GameObject.Instantiate (floatingTextPrefab);
		}
		Vector3 GUICameraPos = GUICamera.transform.position;
		floatingTextObj.transform.position = new Vector3 (GUICameraPos.x,
			GUICameraPos.y,
			GUICameraPos.z + Utilities.getCurrentLightRadius () / 2);
		floatingTextObj.GetComponent<TextMesh> ().text = getPrompt(); 
		floatingTextObj = null;
	}

	public void removeLock(Lock l){
		_locks.Remove(l);
		currPriority--;
	}

	public void addLock(Lock l){
		_locks.Add(l);
		currPriority++;
	}
}
