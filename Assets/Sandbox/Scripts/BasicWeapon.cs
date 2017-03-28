using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;

public class BasicWeapon : MonoBehaviour
{
	// Clip holds a fixed maximum amount of ammo
	public int maxClipSize = 80;
	// If target outside this range, hits will not register
	public float maxRange = 100.0f;
	// If this is an enemy's weapon, don't respond to input
	public bool playerControlled = true;
	// Remaining ammo UI label
	public Text ammoRemainingLabel;
	// Point the shot emminates from, set in front of player slightly to avoid shooting ourself
	public Transform shootPoint;
	// Sparks effect on impact of projectile hitting surface
	public GameObject impactEffectPrefab;

	AudioSource audioSource;

	public AudioClip fireClip;

	private int ammoRemaining = 0;

	// Display amount of ammo remaining in the clip, in ammo field of UI
	void Start ()
	{
		ammoRemaining = maxClipSize; // Weapon begins fully loaded
		UpdateUI(); // Reset ammmo UI label
		audioSource = GetComponent<AudioSource>();
	}

	void Update ()
	{
		// Only respond to player input if this is the player's weapon
		if (playerControlled == false)
			return;

		// If fire button pressed and ammo left, shoot
		// Reload if out of ammo, no shot fired. Weapon won't fire when you run out of ammo but auto reloads
		if (CrossPlatformInputManager.GetButtonDown ("Fire1")) {
			if (ammoRemaining > 0) {
				Fire ();
			} else {
				Reload ();
			}
		}
	}

	// Update UI to show ammount of ammo remaining
	void UpdateUI ()
	{
		if (ammoRemainingLabel != null) {
			ammoRemainingLabel.text = ammoRemaining.ToString ();
		}
	}

	public void Fire ()
	{
		ammoRemaining -= 1;
		UpdateUI ();

		// Use a raycast to see if any hit occured when weapon fired, on hit play sparks effect
		RaycastHit hitInfo;
		var ray = new Ray(shootPoint.position, shootPoint.forward);
		bool hit = Physics.Raycast(ray, out hitInfo, maxRange);

		if (hit) {
			var impactEffect = Instantiate(impactEffectPrefab);
			impactEffect.transform.position = hitInfo.point;
			// Sparks are angled so they appear to bounce off the surface
			var direction = Vector3.Reflect(shootPoint.forward, hitInfo.normal);
			impactEffect.transform.forward = direction;
			Destroy(impactEffect, 4);

			// If hit a character, decrement characters health by 1
			var damage = hitInfo.collider.GetComponentInParent<DamageTaking>();
			if (damage != null)
				damage.TakeDamage();
		}

		audioSource.PlayOneShot(fireClip);
	}

	// Clip auto reloads when out of ammo
	void Reload ()
	{
		ammoRemaining = maxClipSize;
		UpdateUI ();
	}
}
