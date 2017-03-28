using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawner : MonoBehaviour
{

	// The character to respawn
	public GameObject target;
	// Time to wait before respawning
	public float delay;

	// Use this for initialization
	IEnumerator Start ()
	{
		// Start by hiding the respawned character
		target.SetActive (false);

		yield return new WaitForSeconds (delay);

		// Respawn in a random location, from set of predefinined respawning locations
		var allRespawnPoints = GameObject.FindGameObjectsWithTag ("Respawn");
		var chosenPoint = allRespawnPoints [Random.Range (0, allRespawnPoints.Length)];
		// Respawn in selected position, using respawn locations rotation so character facing in predetermined direction
		target.transform.position = chosenPoint.transform.position;
		target.transform.rotation = chosenPoint.transform.rotation;
		target.SetActive (true);
		Destroy (gameObject);
	}
}
