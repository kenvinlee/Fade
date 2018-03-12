using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawner : MonoBehaviour {
	public GameObject respawn_object;
	public int respawn_limit = 1;

	private List<GameObject> spawned_objects = new List<GameObject>();
	private GameObject _lastSpawnedObject;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < spawned_objects.Count; i++) {
			if (spawned_objects [i] == null) {
				spawned_objects.Remove (spawned_objects [i]);
				break;
			}
		}
		if (_lastSpawnedObject == null) {
			spawnObject ();
		} else {
			//Check if the last object has left the spawn area, so we can spawn a new object.
			if (Vector3.Distance (_lastSpawnedObject.transform.position, transform.position) > 1) {
				_lastSpawnedObject = null;
			}
		}
	}

	private void spawnObject(){
		if (spawned_objects.Count < respawn_limit) {
			_lastSpawnedObject = (GameObject)GameObject.Instantiate (respawn_object);
			_lastSpawnedObject.name = respawn_object.name;
			spawned_objects.Add(_lastSpawnedObject);

			_lastSpawnedObject.transform.position = new Vector3 (transform.position.x,
				transform.position.y,
				transform.position.z);
		}
	}
}
