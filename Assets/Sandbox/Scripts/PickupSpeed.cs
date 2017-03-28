// Changes the player's walking speed for a certain time
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PickupSpeed : MonoBehaviour {

	// How long the effect lasts
	public float duration = 3.0f;
	// How fast the speed is multiplied by
	public float speedFactor = 3.0f;

	void PickupCollected(GameObject gameObject)
	{
		var controller = gameObject.GetComponent<FirstPersonController>();

		if (controller != null)
			StartCoroutine(ApplySpeedBoost(controller));
	}

	IEnumerator ApplySpeedBoost (FirstPersonController controller)
	{
		controller.SetSpeedFactor(speedFactor);

		// Reset walking and running speeds to default after time
		yield return new WaitForSeconds(duration);
		controller.SetSpeedFactor(1.0f / speedFactor);
	}
}
