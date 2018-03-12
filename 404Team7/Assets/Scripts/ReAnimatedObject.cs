using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ReAnimatedObject : MonoBehaviour {
	
	public float light_radius_contact_scale = 1.0f;
	public float animating_speed = 1.0f;

	[Header ("Audio")]
    public GameObject objectType;

	public MonoBehaviour _customAnimator;
	private Animator _animatorComponent;
	private NavMeshAgent _agent;
	private MonsterAI _ai;
	private int hintLightLayer;

	[HideInInspector]
	public Collider objCollider;
	private Collider _lightCollider;
    public AudioSource soundEffect;

	private bool is_animating, playerInSight, _animateInDark = false;
	private int lightInSight = 0;

	public bool isAnimating(){
		return is_animating;
	}

    // Use this for initialization
    void Start () {
		_animatorComponent = GetComponent<Animator> ();
		if (_animatorComponent != null) {
			_animatorComponent.enabled = false;
		}
		_agent = GetComponent<NavMeshAgent> ();
		if (_agent != null) {
			_agent.enabled = false;
		}
		_ai = GetComponent<MonsterAI> ();
		if (_ai != null) {
			_ai.enabled = false;
		}
		if (_customAnimator != null) {
			_customAnimator.enabled = false;
		}
		if (_customAnimator == null && _animatorComponent == null) {
			this.enabled = false;
		}

        if (GetComponent<AudioSource>())
        {
            soundEffect = GetComponent<AudioSource>();
        }

        if (objCollider == null) 
			objCollider = transform.GetComponent<Collider> ();
		_lightCollider = GameObject.Find (Utilities.LIGHT_NAME).GetComponent<Collider>();
		hintLightLayer = LayerMask.NameToLayer("HintLight");
	}

    void Update()
    {
        if (objectType) {
			if (objectType.name.Contains("ExplodingBomb") || objectType.name.Contains("Exploding Bomb"))
            {
                if (is_animating && soundEffect && soundEffect.time == 0)
                {
                    soundEffect.Play();
                }
                else if (!is_animating && soundEffect.isPlaying)
                {
                    soundEffect.Pause();
                }
                else if (is_animating && soundEffect.time != 0)
                {
                    soundEffect.UnPause();
                }

            }
        }
    }

	// Update is called once per frame
	void FixedUpdate () {
		if (lightInSight > 0) {
			is_animating = true;
		} else if (playerInSight) {
			//Need to check for distance too, so that we don't animate if a slight bit of light touches. We should at least see the object first.
			float dist = Vector3.Distance (_lightCollider.transform.position, transform.position);
			float light_radius = _lightCollider.bounds.size.x / 2;
			is_animating = (_lightCollider.bounds.Intersects (objCollider.bounds) && dist < light_radius * light_radius_contact_scale);
		} else {
			is_animating = false; //False, unless for special case.
		}

		//Just in case objects dont have an animator component, we can still test functionality.
		if (_customAnimator != null) {
			_customAnimator.enabled = is_animating;
		}

		//This is if we have animations attached to the object.
		if (_animatorComponent != null && !_animateInDark) {
			_animatorComponent.speed = animating_speed;
			_animatorComponent.enabled = is_animating;
		}
		if (_agent != null && _ai != null) {
			_agent.enabled = is_animating;
			_ai.enabled = is_animating;
		}
	}

	// Do the math only when some light is in bound.
	void OnTriggerEnter(Collider other) {
		if (other.CompareTag ("Orb") && other.gameObject.layer != hintLightLayer ) {
			if ((other.transform.parent!= null && getGrabbableObjectFromChild(other.gameObject) == null) && other.transform.parent.CompareTag("Player")) {
				playerInSight = true;
			} else {
				lightInSight++;

				// Pass the OnTriggerExit to other if other is mushroom
				GrabbableObject cbTarget = other.GetComponentInParent<GrabbableObject> ();
				if (cbTarget != null)
					cbTarget.AddReleaseCallBack (other, OnTriggerExit);
			}
		} 
	}

	void OnTriggerExit(Collider other) {
		if (other.CompareTag ("Orb") && other.gameObject.layer != hintLightLayer ) {
			if ((other.transform.parent!= null && getGrabbableObjectFromChild(other.gameObject) == null) && other.transform.parent.CompareTag ("Player")) {
				playerInSight = false;
			} else {
				lightInSight = Mathf.Max(0, lightInSight-1); 
				GrabbableObject cbTarget = other.GetComponentInParent<GrabbableObject> ();
				if (cbTarget != null)
					cbTarget.RemoveReleaseCallBack (OnTriggerExit);
			}
			FixedUpdate ();
		}
	}

	//Look for a parent object with GrabbableObject script.
	private GameObject getGrabbableObjectFromChild(GameObject other)
	{
		Transform root_object = other.transform;
		while (root_object != null)
		{
			if (root_object.GetComponent<GrabbableObject>() != null)
			{
				return root_object.gameObject;
			}
			root_object = root_object.transform.parent;
		}
		return null;
	}

	public void setAnimateInDark(bool b){
		_animateInDark = b;
		if (hasAnimator ()) {
			_animatorComponent.enabled = true;
		}
	}

	public bool hasAnimator(){
		return _animatorComponent != null;
	}

	public Animator getAnimator(){
		return _animatorComponent;
	}

	public void deleteObjectEvent(){
		Destroy (this.gameObject);
	}
}
