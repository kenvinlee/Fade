using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (ReAnimatedObject))]
public class ChargeableObject : MonoBehaviour {
	public float chargeSpeedScale = 1.0f;

	private bool _isCharging = false;
	private float _currentCharge = 0;
	private float _timeStamp = 0;
	private ReAnimatedObject _reAnimationHandler;
	// Use this for initialization
	void Start () {
		_reAnimationHandler = GetComponent<ReAnimatedObject> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (_reAnimationHandler.isAnimating ()) {
			if (_currentCharge < 1) {
				chargeObject ();	
			}
		} else {
			if (_currentCharge > 0) {
				loseCharge ();
			}
		}
//		if (_reAnimationHandler.getAnimator ().speed == 1) {
//			Debug.Log ("object stopped");
//		}
	}

	void chargeObject(){
		if(_reAnimationHandler.hasAnimator()){
			_reAnimationHandler.getAnimator ().SetFloat ("Speed", 1);
			_reAnimationHandler.getAnimator ().speed = 1;
		}
	}

	//At max charge we stop the animation.
	void maxCharge(){
		if(_reAnimationHandler.hasAnimator()){
			_reAnimationHandler.getAnimator ().SetFloat ("Speed", 0);
			_reAnimationHandler.getAnimator ().speed = 0;
		}
		_currentCharge = 1;
	}

	//Reverse the animation.
	void loseCharge(){
		if(_reAnimationHandler.hasAnimator()){
			_reAnimationHandler.setAnimateInDark (true);
			_reAnimationHandler.getAnimator ().SetFloat ("Speed", -1);
			_reAnimationHandler.getAnimator ().speed = 1;
		}
		_currentCharge = 0.5f;
	}

	//At min charge we stop the animation, so it doesn't loop in reverse.
	void minCharge(){
		if(_reAnimationHandler.hasAnimator()){
			_reAnimationHandler.getAnimator ().SetFloat ("Speed", 0);
			_reAnimationHandler.getAnimator ().speed = 0;
		}
		_currentCharge = 0;
	}


}
