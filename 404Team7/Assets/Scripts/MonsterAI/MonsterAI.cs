/// <summary>
/// A very basic monster AI.
/// 
/// Attached to Monsters. Require NavMeshAgent and a baked navmesh to work.
/// 
/// When in light go for the player and when player enters attack zone, attack.
/// Destroys the monster if its health <= 0
/// </summary>


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class MonsterAI : Health {
//	public isSmart = false;  // avoids traps
//	public Collider physicsCollider, attackCollider;
	public int damage = 25;
	public float attackGap = 1.5f;
	public bool isAttacking = false;
	public bool isWalking = true;
	public float epsilon = 1f;

	NavMeshAgent agent;
	Animator animator;
	Transform player;
	Health playerVictim;
	Vector3 goal  = Vector3.zero;
	bool targetPlayer = true;
	IEnumerator attackEvent;


	void Start () {
		// we need to make this more dynamic
		agent = GetComponent<NavMeshAgent>();
		player = GameObject.Find ("PlayerController").transform;
		playerVictim = player.GetComponent<PlayerMovement> ();
		animator = GetComponent<Animator> ();
//		GetComponent<ReAnimatedObject> ().objCollider = physicsCollider;
	}

	void Update () {
		if (targetPlayer)
			goal = player.position;

		if (Vector3.Magnitude(goal - transform.position) > epsilon) {
			isWalking = true;
			agent.SetDestination (goal);
			if (isAttacking)
				StopAttack ();
			animator.SetBool ("isWalking", true);
		} else {
			isWalking = false;
			animator.SetBool ("isWalking", false);

			if (targetPlayer && !isAttacking)
				Attack (playerVictim);
		}

		// add a circumstance to kill/despawn the monster		
	}

	public void SetDestination(GameObject g) {
		if (g == null || g.transform == player) {
			targetPlayer = true;
		} else {
			targetPlayer = false;
			goal = g.transform.position;
		}
			

	}

	// Monster attack one victim at a time. Attack not paused in dark.
	public void Attack(Health victim) {
		isAttacking = true;
		if (attackEvent != null)
			StopCoroutine (attackEvent);
		attackEvent = DoDamage (victim, damage, attackGap);
		StartCoroutine (attackEvent);
	}

	IEnumerator DoDamage(Health victim, int amount, float recurrentGap) {
		while (isAttacking) {
			victim.TakeDamage (amount);
			animator.SetTrigger ("Attack");
			yield return new WaitForSeconds (recurrentGap);
		}
	}

	public void StopAttack() {
		isAttacking = false;
		StopCoroutine (attackEvent);
	}

	// Currently objs monsters touches tells monsters what to do.
	// e.g. when touched, player calls monster's Attack()
	// To have monsters decide what to do instead, we need to add additional trigger on player.

	public override void DieHandler() {
		Destroy (gameObject);
	}

}


