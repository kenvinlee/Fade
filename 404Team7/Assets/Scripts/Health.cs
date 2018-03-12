/// <summary>
/// Parent class of all that has health and can die. Inheritance/overriding is expected.
/// 
/// Each child class should override the dieHandler and DamageHandler so they handle there own
/// deaths and hurts differently. Currently PlayerCompact and MonsterAI inherit this class.
/// 
/// Other class can call TakeDamage() to cause damage, or start/stop-TakingDamage to 
/// start/stop a periodic damage. e.g. 20HP/s.
/// </summary>


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Health : MonoBehaviour {

	public int health = Utilities.MAX_PLAYER_HEALTH;
	private Dictionary<GameObject, IEnumerator> damageSource = new Dictionary<GameObject, IEnumerator> ();
	private bool isDead = false;

	// Record the source and fire the damage coroutine
	public void StartTakingDamage(GameObject source, int amount, float recurrentGap) {
		if (!damageSource.ContainsKey(source)) {
			IEnumerator couroutine = Damage (amount, recurrentGap);
			damageSource.Add (source, couroutine);
			StartCoroutine(couroutine);
//			Damage (amount);
		}
	}

	public void StopTakingDamage(GameObject source) {
		if (damageSource.ContainsKey (source)) {
			StopCoroutine (damageSource [source]);
			damageSource.Remove (source);
		}
	}

	public void StopAllDamage() {
		while (damageSource.Keys.Count > 0) {
			StopTakingDamage (damageSource.First().Key);
		}
	}

	// Controls how often health is taken off. If recurrentGap <0, fire once only.
	IEnumerator Damage(int amount, float recurrentGap) {
		while (true) {
			TakeDamage (amount);
			if (recurrentGap < 0)
				break;
			yield return new WaitForSeconds (recurrentGap);
		}
	}

	// Controls how often health is taken off. If recurrentGap <0, fire once only.
//	void Damage(int amount) {
//		TakeDamage (amount);
//	}

	// Actually takes off health. It is seperated from the coroutine for easy access.
	public void TakeDamage(int amount) {
		if (!isDead) {
			health -= amount;
			DamageHandler (amount);
			CheckHealth ();
		}
	}

	// Each derived classes may implement the following two differently.
	public virtual void DamageHandler(int amount) {
		Debug.Log (name + " is taking damage!! Health="+ health);
	}

	public virtual void DieHandler() {
		Debug.Log (name + " has died.");
	}

    public virtual void DieHandler(AudioSource deathSound)
    {
        if (!deathSound.isPlaying)
        {
            deathSound.PlayOneShot(deathSound.clip, 1);
        }
        Debug.Log(name + " has died.");
    }

    // Override this too if you want. e.g. screen turn gray when health is low
    public virtual void CheckHealth(){
		if (health <= 0) {
			isDead = true;
			DieHandler ();
		} else {
			isDead = false;
		}
	}

	public void ResetHealth() {
		StopAllDamage ();
		health = Utilities.MAX_PLAYER_HEALTH;
	}
}
