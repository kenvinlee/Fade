using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockEvent : MonoBehaviour {
	public bool correctTime = false;
	public Lock timeLock;
	private Lock timeLockCopy;
	private GameObject lockOwner;
	// Use this for initialization
	void Start () {
		timeLockCopy = timeLock;
		lockOwner = timeLock.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setCorrectTime(){
		correctTime = true;
		lockOwner.GetComponent<InteractableObject> ().removeLock (timeLock);
		Destroy (timeLock);
	}

	public void setIncorrectTime(){
		correctTime = false;
		lockOwner.AddComponent<Lock> ();
		timeLock = lockOwner.GetComponent<Lock> ();
		timeLock.prompt = timeLockCopy.prompt;
		lockOwner.GetComponent<InteractableObject> ().addLock (timeLock);
	}
}
