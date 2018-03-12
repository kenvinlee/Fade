using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*********************************************************************
purpose:  this class handles dangerous object collisions
when this occurs it affects the health of the gameplayer.

added isDangerous flag to allow for objects to change their "dangerous"
status.
***********************************************************************/



[RequireComponent(typeof (Collider))]
public class DangerousObject : MonoBehaviour {
	public int damage = Utilities.MAX_PLAYER_HEALTH;
	public bool destroyOnHit = false;
	public float timeBtweenDamage = 1f;

    public bool isDangerous = true;  //can have dangerous status of object change.

	// This sets all colliders to be triggers. Disabling physics. We do this manually
	void Start () {
		if (GetComponent<Collider> () != null) {
			GetComponent<Collider> ().isTrigger = true;
		}
	}


    void OnTriggerEnter(Collider other)
    {

        if (isDangerous)
        {
            foreach (MonoBehaviour mb in other.gameObject.GetComponents<MonoBehaviour>())
            {
                if (mb is Health)
                {
                    ((Health)mb).StartTakingDamage(gameObject, damage, timeBtweenDamage);
                    //Check if player is hit.
                    if (other.GetComponent<PlayerMovement>() != null)
                    {
                        if (destroyOnHit)
                        {
                            Destroy(getRootParentObjectFromChild(this.gameObject));
                        }
                    }
                }
            }
        }
    }

	//Look for a parent object with GrabbableObject script.
    //reset its position to previous spot?

	private GameObject getRootParentObjectFromChild(GameObject child)
	{
		GameObject root_object = child.gameObject;
		while (root_object.transform.parent != null)
		{
			//Check if its being held by player
			if (root_object.GetComponent<GrabbableObject>() != null)
			{
				return root_object;
			}
			root_object = root_object.transform.parent.gameObject;
		}
		return root_object;
	}

	void OnTriggerExit(Collider other) {
		foreach (MonoBehaviour mb in other.gameObject.GetComponents<MonoBehaviour>()) {
			if (mb is Health) {
				((Health)mb).StopTakingDamage(gameObject);
			}
		}
	}
}
