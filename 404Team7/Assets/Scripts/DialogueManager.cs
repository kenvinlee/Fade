using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A script to be placed once in every scene.
 * Is used to manaage the order at which the
 * dialogue plays.
 */
//[RequireComponent(typeof (TextMesh))]
public class DialogueManager : MonoBehaviour {
	private float _timeStamp = 0;
	private List<string> _dialogueQueue = new List<string>();
	private Dictionary<string, float> _dialogueToTime = new Dictionary<string, float>();
	private string _currDialogue = "";
	private bool m_UISMExist = true;
	private TextMesh _textBox;
	// Use this for initialization
	void Start () {
		m_UISMExist = UIStateManager.controller != null;
		_textBox = GetComponent<TextMesh> ();

		// Destroy GUI cam if UISM exists. Fixes dialogue not showing bug.
//		if (m_UISMExist && _textBox != null)
//			Destroy (transform.parent.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		updateDialogueText ();
	}

	/*
	 * Play the current dialgoue for a specific amount of time.
	 * Once finished, loads the next dialgoue in.
	 */
	private void updateDialogueText(){
		if (_dialogueQueue.Count > 0 && _currDialogue == "") {
			_currDialogue = _dialogueQueue [0];
			_timeStamp = Time.time + _dialogueToTime [_currDialogue];

			_dialogueQueue.Remove (_currDialogue);
			_dialogueToTime.Remove (_currDialogue);
		} else {
			if (Time.time < _timeStamp) {
//				if (!m_UISMExist)
					_textBox.text = _currDialogue;
//				else
//					UIStateManager.controller.Dialogue(_currDialogue, -1);
			} else {
				_currDialogue = "";
//				if (!m_UISMExist)
					_textBox.text = "";
//				else
//					UIStateManager.controller.ClearDialogue();
			}
		}
	}

	public void addDialogue(string text, float time_on_screen){
        if (!_dialogueToTime.ContainsKey(text)) {
            _dialogueQueue.Add(text);
            _dialogueToTime.Add(text, time_on_screen);
        }
        else
        {
            Debug.Log("Missed a Dialogue trigger due to pushing the same message too quickly");
        }
		
	}

	//Used to add more important dialogue to the start (Ex: checkpoint dialogue)
	public void addDialogueToStart(string text, float time_on_screen){

        if (!_dialogueToTime.ContainsKey(text))
        {
            _dialogueQueue.Insert(0, text);
            _dialogueToTime.Add(text, time_on_screen);
        }
        else
        {
            Debug.Log("Missed a Dialogue trigger due to pushing the same message too quickly");
        }
    }

    public bool isDialogueQueueEmpty()
    {
        return (_dialogueQueue.Count == 0);
    }
}