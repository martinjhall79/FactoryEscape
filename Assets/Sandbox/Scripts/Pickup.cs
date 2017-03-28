// Enables player to pickup the power up pickup
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour {

	// Time to wait before respawning the power up
	public float respawnTime = 5.0f;

	// Check if the colliding object is the player
	void OnTriggerEnter(Collider c) {
		if (c.gameObject.CompareTag("Player") == true) {
			PickedUp(c.gameObject);
		}
	}

	// Send a message to pickup functions on the pickup objects
	void PickedUp(GameObject gameObject) {
		Debug.Log("picked up power up");
		SendMessage("PickupCollected", gameObject);

		StartCoroutine(DissappearAndReappear());
	}

	IEnumerator DissappearAndReappear ()
	{
		foreach (Transform child in transform) {
			child.gameObject.SetActive(false);
		}

		GetComponent<Collider>().enabled = false;

		yield return new WaitForSeconds(respawnTime);

		GetComponent<Collider>().enabled = true;

		foreach (Transform child in transform) {
			child.gameObject.SetActive(true);
		}
	}
}
