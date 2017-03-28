// Keeps track of total amount of health player and enemies have
// Reduces amount of health the character has and removes if health drops below zero
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTaking : MonoBehaviour
{
	// Starting health
	public int maxHealth = 3;
	// Time to wait before respawning, player respawns faster than enemies
	public float respawnDelay = 2.0f;
	// Impact force, visual effect that animates the effects of a shot hit impact pushing character back a little
	public float impactForce = 2.0f;
	// An override setter that detects if health drops below zero
	// Backing field
	public UnityEngine.UI.Text displayText;

	// Explosions effect
	public GameObject deathPrefab;

	public PainOverlay painOverlay;

	int _currentHealth;

	int currentHealth {
		get {
			return _currentHealth;
		}
		set {
			_currentHealth = value;

			if (displayText != null)
				displayText.text = _currentHealth.ToString();

			if (_currentHealth <= 0)
				Die ();
		}
	}
		
	void Start ()
	{
		// All characters begin with max health
		currentHealth = maxHealth;
	}

	public void TakeDamage (int amount = 1)
	{
		currentHealth -= amount;

		// Show colour overlay to signal that we've been shot
		if (painOverlay != null && amount > 0) {
			painOverlay.StartFade();
		}
	}

	public void Die ()
	{
		// Remove character temporarily, then re-enable it
		var respawner = new GameObject ("Respawner for " + gameObject.name)
			.AddComponent<Respawner>();
		respawner.target = gameObject;
		respawner.delay = respawnDelay;
		// Reset health
		currentHealth = maxHealth;
		// Explosion effect
		if (deathPrefab != null) {
			var death = Instantiate(deathPrefab);
			death.transform.position = this.transform.position;
			// Vanish the explosion effect from the scene after few seconds
		}
	}

	public void RestoreHealth ()
	{
		currentHealth = maxHealth;

		Debug.Log("Restoring health!");
	}
}
