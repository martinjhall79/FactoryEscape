// Makes objects move up and down and rotate
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupBounce : MonoBehaviour {

	// Movement specifics
	public float upDownRange = 1.15f;
	public float upDownSpeed = 2.0f;
	public float rotationSpeed = 50f;

	// Update is called once per frame
	void Update () {
		transform.localPosition = new Vector3(
			transform.position.x,
			1 + (Mathf.Sin(Time.time * upDownSpeed) * upDownRange),
			transform.position.z
		);

		transform.Rotate(0, Time.deltaTime * rotationSpeed, 0);
	}
}
