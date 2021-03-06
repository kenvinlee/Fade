using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A script that can be attached to an object.
 * When a player passes throught he object the
 * dialogue_lines are added to DialogueManager.
 * The dialgoue and timings can be set in Unity
 * Editor.
 */
[RequireComponent(typeof (Collider))]
public class DialogueEvent : MonoBehaviour {
	private bool _eventOccured = false;
	public List<string> dialogue_lines = new List<string>();
	public List<float> dialogue_line_times = new List<float>();
	public DialogueManager manager;
	public bool pushDialogueToStart = false;
	public bool activeIfInLight = true;

	[Header("Paused Instruction")]
	[Tooltip ("If UISM exist, dim screen and prompt")]
	public bool pauseGameAndPrompt = false;
	public string instruction = "";
	[Tooltip ("Shows up when instruction followed.")]
	public string instructionFollowedText = "";
	public KeyCode requireKey = KeyCode.I;
	public string requireAction = "down";


	// Use this for initialization
	void Start () {
		if (GetComponent<Collider> () != null) {
			GetComponent<Collider> ().isTrigger = true;
		}
		if (GameObject.Find ("Dialogue Manager") != null)
			manager = GameObject.Find ("Dialogue Manager").GetComponent<DialogueManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void activateDialogue(){
		if (_eventOccured) {
			Destroy (this.gameObject);
			return;
		}
		if (activeIfInLight) {
			if (!Utilities.isObjectInSight (this.gameObject)) {
				return;
			}
		}
		if (pauseGameAndPrompt && UIStateManager.controller != null) {
			UIStateManager.controller.PauseForInstruction (instruction, instructionFollowedText, requireKey, requireAction);
		} else {
			if (pushDialogueToStart) {
				addDialogueToStartOfQueue ();
			} else {
				addDialogueToQueue ();
			}
		}
		_eventOccured = true;
		GetComponent<Collider> ().enabled = false;
	}

	public void addDialogueToQueue(){
		for (int i = 0; i < dialogue_lines.Count; i++) {
			manager.addDialogue (dialogue_lines [i], dialogue_line_times[i]);
		}
	}

	public void addDialogueToStartOfQueue(){
		for (int i = 0; i < dialogue_lines.Count; i++) {
			manager.addDialogueToStart (dialogue_lines [i], dialogue_line_times[i]);
		}
	}
}
