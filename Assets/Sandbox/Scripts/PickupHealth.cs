using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupHealth : MonoBehaviour {

	// Restore players health
	void PickupCollected(GameObject gameObject) {
		var damageTaking = gameObject.GetComponent<DamageTaking>();

		if (damageTaking != null) {
			damageTaking.RestoreHealth();
		}
	}
}
