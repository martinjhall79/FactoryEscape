using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
	// Where the enemy roams to, walking randomly between these points
	GameObject[] roamPoints;
	// The nav agent
	NavMeshAgent navAgent;
	// How long the enemy waits after reaching a destination
	public float pauseTime = 1.5f;
	// Simple state machine to track which state we are currently in
	enum State {
		Roaming,
		AttackingPlayer
	}
	// Use a property so setting a State auto calls EnterState
	State _state;

	// Coroutine that represents the current state
	// Stopped and started when state is changed
	Coroutine currentStateRoutine;

	State state {
		get { return _state; }
		set {
			EnterState (_state, value);
			_state = value;
		}
	}

	// Look for player
	EnemySensors sensors;

	// Enemy weapons, multiple weapons to shoot with
	BasicWeapon[] weapons;
	// Turn head to follow player
	public Transform headTransform;
	public float delayBetweenShots = 1;

	// When we want to restart AI
	// Because all coroutines are stopped when an object is disabled
	void OnEnable ()
	{
		roamPoints = GameObject.FindGameObjectsWithTag ("Roaming Point");

		navAgent = GetComponent<NavMeshAgent> ();

		sensors = GetComponent<EnemySensors>();
		Debug.Log("Initialising sensors = " + sensors);

		state = State.Roaming;

		weapons = GetComponentsInChildren<BasicWeapon>();
		foreach (var weapon in weapons) {
			weapon.playerControlled = false;
		}
	}

	void EnterState (State oldState, State newState)
	{
		// Stop the current state routine, if it exists
		if (currentStateRoutine != null)
			StopCoroutine (currentStateRoutine);
		
		Debug.LogFormat ("Changing state from {0} to {1}", oldState.ToString (), newState.ToString ());

		// Start a new state
		switch (newState) {
		case State.Roaming:
			currentStateRoutine = StartCoroutine (RunRoaming ());
			break;
		case State.AttackingPlayer:
			currentStateRoutine = StartCoroutine (RunAttacking ());
			break;
		default:
			throw new System.ArgumentOutOfRangeException ();
		}
	}
		
	// Runs every frame. Picks new destination to roam to
	// Checks if destination reached
	// Pause and wait for certain amount of time, then repeat
	IEnumerator RunRoaming ()
	{
		do {
			// Resume walking if stopped
			navAgent.Resume ();
			// Pick a random location to roam to
			var newDestination = roamPoints [Random.Range (0, roamPoints.Length)];
			navAgent.SetDestination (newDestination.transform.position);

			while (true) {
				// If not on a nav mesh, stop
				if (navAgent.isOnNavMesh == false) {
					Debug.LogError ("EnemyAI is not a navmesh!!");
					yield break;
				}

				// Check if enemy has already reached the destination, break if it has
				if (!navAgent.pathPending &&
				    navAgent.remainingDistance <= navAgent.stoppingDistance &&
				    (!navAgent.hasPath || navAgent.velocity.sqrMagnitude == 0f)) {
					// Time to pick a new destination
					break;
				}
				try {
					// Is the player in range and visible, attack player
					if (sensors.checkForPlayer() == true) {
						state = State.AttackingPlayer;
						yield break;
					}
				}
				catch {
					Debug.Log("Sensors or state not set to instance!");
				}

				// Wait a frame and then loop again
				yield return null;
			}

			// Pause and wait, used to check if enemy can see player while it's waiting
			float timeSincePause = 0.0f;

			while (timeSincePause < pauseTime) {
				try {
					// Is the player in range and visible, attack player
					if (sensors.checkForPlayer() == true) {
						state = State.AttackingPlayer;
						yield break;
					}
				}
				catch {
					Debug.Log("Sensors or state not set to instance!");
				}
				timeSincePause += Time.deltaTime;
				yield return null;
			}
		} while (state == State.Roaming);
	}

	// Attack player
	IEnumerator RunAttacking ()
	{
		float timeSinceLastFire = 0.0f;

		// Stop walking
		navAgent.Stop();

		int weaponIndex = 0;

		// Do while the enemy can see the player
		do {
			// Turn head towards player
			headTransform.LookAt(sensors.player.transform);

			// Is it time to fire
			if (timeSinceLastFire > delayBetweenShots) {

				var currentWeapon = weapons[weaponIndex];

				// Tweak the aim of the current weapon, so it's easier for the enemy to hit the player
				// Needed as weapon is offset from enemy's centre
				var shootDirection = sensors.player.transform.position - currentWeapon.transform.position;
				currentWeapon.shootPoint.transform.forward = shootDirection;

				// Fire the weapon
				currentWeapon.Fire();

				// Move to next weapon, cycling back to start
				weaponIndex += 1;
				if (weaponIndex >= weapons.Length)
					weaponIndex = 0;
				timeSinceLastFire = 0.0f;
			}

			timeSinceLastFire += Time.deltaTime;

			// Wait until next frame
			yield return null;
		} while (sensors.checkForPlayer() == true);

		// When enemy can't see the player
		// Reset the head's direction
		headTransform.transform.forward = transform.forward;

		state = State.Roaming;
		yield break;
	} 
}
