﻿/// <summary>
/// Controls the movement of player. Attached to playerCollider object inside playercompact.
/// Assumes a player sibling object exists which is the true player. 
/// 
/// Blends the physics and player input response. The player is seperated from it and lerps
/// to its location / orientation constantly for smoothness.
/// </summary>


// Adapted from Unity standard assets -> character -> first person
using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovement : Health
{
    public MovementSettings movementSettings = new MovementSettings();
    public AdvancedSettings advancedSettings = new AdvancedSettings();
    public float CameraSmoothing = 0.2f;

    private Transform m_player;
    private Rigidbody m_RigidBody;
    private CapsuleCollider m_Capsule;
    private float m_YRotation;
    private Vector3 m_GroundContactNormal;
    public bool m_Jump, m_Jumping, m_IsGrounded;
    private Transform m_playerParent;

    private Rigidbody m_playerStaying;
    private Vector3 m_StayLastPos;

    private Checkpoint curr_checkpoint;
    private int curr_checkpoint_priority = 0;

    private bool m_UISMExist = true;

    private GrabberScript grabber;

    private AudioSource footstepAudio;
    private AudioSource deathAudio;
	private DialogueManager dialogueManager;
	private Quaternion lastCPRot;

    [Serializable]
    public class MovementSettings
    {
        public float ForwardSpeed = 8.0f;   // Speed when walking forward
        public float BackwardSpeed = 4.0f;  // Speed when walking backwards
        public float StrafeSpeed = 4.0f;    // Speed when walking sideways
        public float RunMultiplier = 2.0f;   // Speed when sprinting
        public KeyCode RunKey = KeyCode.LeftShift;
        public float JumpForce = 30f;
        public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
        [HideInInspector]
        public float CurrentTargetSpeed = 8f;

        private bool m_Running;

        public void UpdateDesiredTargetSpeed(Vector2 input)
        {
            if (input == Vector2.zero) return;
            if (input.x > 0 || input.x < 0)
            {
                //strafe
                CurrentTargetSpeed = StrafeSpeed;
            }
            if (input.y < 0)
            {
                //backwards
                CurrentTargetSpeed = BackwardSpeed;
            }
            if (input.y > 0)
            {
                //forwards
                //handled last as if strafing and moving forward at the same time forwards speed should take precedence
                CurrentTargetSpeed = ForwardSpeed;
            }
            if (Input.GetKey(RunKey))
            {
                CurrentTargetSpeed *= RunMultiplier;
                m_Running = true;
            }
            else
            {
                m_Running = false;
            }
        }
        public bool Running
        {
            get { return m_Running; }
        }
    }


    [Serializable]
    public class AdvancedSettings
    {
        public float groundCheckDistance = 0.01f; // distance for checking if the controller is grounded ( 0.01f seems to work best for this )
                                                  //		public float stickToGroundHelperDistance = 0.5f; // stops the character
        public float slowDownRate = 20f; // rate at which the controller comes to a stop when there is no input
        public bool airControl; // can the user control the direction that is being moved in the air
        [Tooltip("set it to 0.1 or more if you get stuck in wall")]
        public float shellOffset; //reduce the radius by that ratio to avoid getting stuck in wall (a value of 0.1f is nice)
        public Checkpoint startCP;
    }

    public Vector3 Velocity
    {
        get { return m_RigidBody.velocity; }
    }

    public bool Grounded
    {
        get { return m_IsGrounded; }
    }

    public bool Jumping
    {
        get { return m_Jumping; }
    }

    public bool Running
    {
        get { return movementSettings.Running; }
    }


    private void Start()
    {
        m_RigidBody = GetComponent<Rigidbody>();
        m_Capsule = GetComponent<CapsuleCollider>();
        m_playerParent = transform.parent;
        m_player = m_playerParent.Find("Player");

        if (GetComponents<AudioSource>() != null)
        {
            footstepAudio = GetComponents<AudioSource>()[0];
            deathAudio = GetComponents<AudioSource>()[1];
        }

        if (advancedSettings.startCP && !curr_checkpoint)
            curr_checkpoint = advancedSettings.startCP;

        m_RigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;

        m_UISMExist = UIStateManager.controller != null;
//		if (m_UISMExist)
//			dialogueManager = GameObject.Find("Dialogue Manager").GetComponent<DialogueManager>();
//		else
			dialogueManager = GameObject.Find(Utilities.GUI_CAMERA_NAME).GetComponentInChildren<DialogueManager>();
        grabber = GetComponent<GrabberScript>();

		lastCPRot = m_player.transform.localRotation;
    }


    private void Update()
    {
        if (!m_UISMExist || UIStateManager.controller.inGame)
        {
            if (Input.GetButtonDown("Jump") || Input.GetButtonDown("Jump_Mac") && !m_Jump)
            {
                //m_Jump = true;
            }
			m_player.position = Vector3.Lerp(m_player.position, transform.position, CameraSmoothing + 6*Time.deltaTime);
		}

		if (m_IsGrounded && m_RigidBody.velocity.magnitude > 0 && Time.timeScale > 0)
        {
            if (footstepAudio)
            {
                if (!footstepAudio.isPlaying)
                {
                    footstepAudio.Play();
                }
            }
        }

    }


    private void FixedUpdate()
    {
        GroundCheck();
        RotateView();
        Vector2 input = GetInput();

        if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) && (advancedSettings.airControl || m_IsGrounded))
        {
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward * input.y + transform.right * input.x;
            desiredMove = Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal).normalized;

            desiredMove.x = desiredMove.x * movementSettings.CurrentTargetSpeed;
            desiredMove.z = desiredMove.z * movementSettings.CurrentTargetSpeed;
            desiredMove.y = desiredMove.y * movementSettings.CurrentTargetSpeed;

            m_RigidBody.AddForce(desiredMove * SlopeMultiplier(), ForceMode.Impulse);

            Vector3 groundSpeed;
            //			if (m_playerStaying != null) {
            //				groundSpeed = new Vector3 (m_RigidBody.velocity.x - m_playerStaying.velocity.x, 0, m_RigidBody.velocity.z - m_playerStaying.velocity.z);
            //			} else {
            groundSpeed = new Vector3(m_RigidBody.velocity.x, 0, m_RigidBody.velocity.z);
            //			}
            float speedingRatio = Mathf.Sqrt(groundSpeed.sqrMagnitude - Mathf.Pow(movementSettings.CurrentTargetSpeed, 2));
            if (speedingRatio > 1)
            {
                m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x / speedingRatio, m_RigidBody.velocity.y, m_RigidBody.velocity.z / speedingRatio);
            }
        }
        else if (m_IsGrounded && m_RigidBody.velocity.magnitude > 0)
        {
            m_RigidBody.velocity = Vector3.Lerp(m_RigidBody.velocity, new Vector3(0f, m_RigidBody.velocity.y, 0f), advancedSettings.slowDownRate);

        }

        if (m_IsGrounded)
        {
            //			if (!m_PreviouslyGrounded) {
            //				m_RigidBody.drag = 1f;
            //			}
            if (m_playerStaying != null)
            {
                m_RigidBody.MovePosition(m_RigidBody.position + m_playerStaying.position - m_StayLastPos);

            }

            if (m_Jump)
            {
                //				m_RigidBody.drag = 0f;
                m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
                m_RigidBody.AddForce(new Vector3(0f, movementSettings.JumpForce, 0f), ForceMode.Impulse);
                m_Jumping = true;
            }

            if (!m_Jumping && Mathf.Abs(input.x) < float.Epsilon && Mathf.Abs(input.y) < float.Epsilon && m_RigidBody.velocity.magnitude < 1f)
            {
                m_RigidBody.Sleep();
            }
        }
        else
        {
            //			m_RigidBody.drag = 1f;
            //			if (m_PreviouslyGrounded && !m_Jumping)
            //			{
            //				StickToGroundHelper();
            //			}
        }
        m_Jump = false;

        if (m_playerStaying != null)
        {
            m_StayLastPos = m_playerStaying.position;
        }
    }


    private float SlopeMultiplier()
    {
        float angle = Vector3.Angle(m_GroundContactNormal, Vector3.up);
        return movementSettings.SlopeCurveModifier.Evaluate(angle);
    }


    //	private void StickToGroundHelper()
    //	{
    //		RaycastHit hitInfo;
    //		if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
    //			((m_Capsule.height/2f) - m_Capsule.radius) +
    //			advancedSettings.stickToGroundHelperDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
    //		{
    //			if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f)
    //			{
    //				m_RigidBody.velocity = Vector3.ProjectOnPlane(m_RigidBody.velocity, hitInfo.normal);
    //			}
    //		}
    //	}


    private Vector2 GetInput()
    {

        Vector2 input = new Vector2
        {
            x = Input.GetAxis("Horizontal"),
            y = Input.GetAxis("Vertical")
        };
        movementSettings.UpdateDesiredTargetSpeed(input);
        return input;
    }


    private void RotateView()
    {
        //avoids the mouse looking if the game is effectively paused
        if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

        // get the rotation before it's changed
        float oldYRotation = transform.eulerAngles.y;

        transform.rotation = m_player.rotation;

        if (m_IsGrounded || advancedSettings.airControl)
        {
            // Rotate the rigidbody velocity to match the new direction that the character is looking
            Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
            m_RigidBody.velocity = velRotation * m_RigidBody.velocity;
        }
    }

    /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
    private void GroundCheck()
    {
        //		m_PreviouslyGrounded = m_IsGrounded;
        RaycastHit hitInfo;
        if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
            ((m_Capsule.height / 2f) - m_Capsule.radius) + advancedSettings.groundCheckDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            m_IsGrounded = true;
            m_GroundContactNormal = hitInfo.normal;
        }
        else
        {
            m_IsGrounded = false;
            m_GroundContactNormal = Vector3.up;
        }
        if (m_IsGrounded && m_Jumping)
        {
            m_Jumping = false;
        }
    }



	public override void DieHandler()
	{
		if (m_UISMExist) {
			UIStateManager.controller.ShowDead (RealDieHandler);
		} else {
			RealDieHandler ();
		}
	}

	private void RealDieHandler() {
		System.Random rng = new System.Random();
		String[] deathLines = new String[] 
		{
			"Ouch, that hurt. Let's not do that again.",
			"Oof, maybe I shouldn't touch that.",
			"Please stop hurting me.",
		};


		if (grabber != null)
		{
			grabber.releaseItem();
		}
		if (curr_checkpoint != null)
		{
			transform.position = curr_checkpoint.transform.position;
			m_player.GetComponentInChildren<PlayerLook>().SetPlayerRotation(lastCPRot);
            //			if (m_UISMExist && dialogueManager != null) 
            //				dialogueManager.addDialogue(deathLines[rng.Next(deathLines.Length)], 3);
            if (dialogueManager != null && dialogueManager.isDialogueQueueEmpty())
			{
				dialogueManager.addDialogue(deathLines[rng.Next(deathLines.Length)], 3);
			}
		}

        if (deathAudio && !deathAudio.isPlaying)
        {
            deathAudio.PlayOneShot(deathAudio.clip, 1);
        }

		ResetHealth ();
		CheckHealth ();
	}

	public override void DamageHandler(int amount) {
		if (m_UISMExist) {
			UIStateManager.controller.ShowHurt (0.4f);
		} 
	}


    private void OnTriggerEnter(Collider other)
    {
        System.Random rng = new System.Random();
        String[] cpLines = new String[]
        {
            "Phew, we've made some progress.",
            "Looks like we got past that.",
            "Finally!"
        };

        if (other.CompareTag("PlayerStay"))
        {
            m_playerStaying = other.GetComponent<Rigidbody>();
            m_StayLastPos = m_playerStaying.position;
        }

        if (other.CompareTag("Monster"))
        {
            other.GetComponent<MonsterAI>().Attack(this);
        }
        //		if (other.GetComponent<DangerousObject> () != null) {
        //			other.GetComponent<DangerousObject> ().DoDamage (TakeDamage);
        //			if (other.GetComponent<DangerousObject> ().destroyOnHit) {
        //				GameObject root_object = getGrabbableObjectFromChild(other.gameObject);
        //				//If the object is from something the player is grabbing then release it before destroying it.
        //				if (root_object != null) {
        //					//If the player has died, release it.
        //					if (grabber != null && grabber.getCurrentObjectGrabbed () == root_object.gameObject) {
        //						grabber.releaseItem ();
        //					}
        //					Destroy (root_object.gameObject);
        //				} else {
        //					Destroy (other.gameObject);
        //				}
        //			}
        //		}
        if (other.GetComponent<DialogueEvent>() != null)
        {
            other.GetComponent<DialogueEvent>().activateDialogue();
        }
        if (other.GetComponent<Checkpoint>() != null)
        {
            Checkpoint cp = other.GetComponent<Checkpoint>();

            if (cp != curr_checkpoint)
            {
				curr_checkpoint = cp;
				lastCPRot = cp.transform.localRotation;
                curr_checkpoint_priority++;
                cp.setPriority(curr_checkpoint_priority);
                //				if (m_UISMExist && dialogueManager != null) 
                //					dialogueManager.addDialogue(cpLines[rng.Next(cpLines.Length)], 2);
                if (dialogueManager != null && dialogueManager.isDialogueQueueEmpty())
                {
                    dialogueManager.addDialogue(cpLines[rng.Next(cpLines.Length)], 2);
                }
            }
        }
		if (other.GetComponent<LevelPortal> () != null) {
			other.GetComponent<LevelPortal> ().teleportToLevel ();
		}
    }

	//Look for a parent object with GrabbableObject script.
	private GameObject getGrabbableObjectFromChild(GameObject other)
	{
		Transform root_object = other.transform;
		while (root_object.transform != null)
		{
			if (root_object.GetComponent<GrabbableObject>() != null)
			{
				return root_object.gameObject;
			}
			root_object = root_object.transform.parent;
		}
		return null;
	}

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerStay"))
        {
            if (other.GetComponent<Rigidbody>() == m_playerStaying)
            {
                m_playerStaying = null;
            }
        }
        if (other.CompareTag("Monster"))
        {
            other.GetComponent<MonsterAI>().StopAttack();
        }
    }
}
