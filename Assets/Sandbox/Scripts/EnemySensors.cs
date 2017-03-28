// Enables the enemy to detect when the player enters their line of sight, and attack
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySensors : MonoBehaviour {

	// If the player is within this range, and angle, the sensor will fire
	public float angle = 90;
	public float range = 30;

	private GameObject _player;

	public GameObject player {
		get {
			if (_player == null) {
				_player = GameObject.FindGameObjectWithTag("Player");
			}
			return _player;
		}
	}

	//public GameObject player;

	// Get distance from player's location to this enemy's location
	private float playerDistance {
		get {
			return (transform.position - player.transform.position).magnitude;
		}
	}

	// Get the direction of the player
	public Vector3 playerDirection {
		get { 
			return player.transform.position - transform.position;
		}
	}

	public bool checkForPlayer() {
		// Is the player in the field of view, calculate direction relative to our forward direction
		if ((Vector3.Angle(playerDirection, transform.forward)) < angle) {
			// If the player is hiding behind something, we can't see them
			RaycastHit hit;

			// The player is in the FOV, see if there's anything between us
			// If there's something between us, like the player is hiding behind object, we can't see the player
			if (Physics.Raycast(transform.position, playerDirection, out hit, range)) {
				if (hit.transform == player.transform) {
					// There's nothing between us and the player, and the player is in range
					return true;
				}
			}
		}
		// There's something blocking our view
		return false;
	}
}
