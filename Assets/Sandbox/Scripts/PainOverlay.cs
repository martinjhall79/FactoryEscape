// Shows a colour overlay when the player is shot, by controlling the opacity of an image overlaying the canvas
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PainOverlay : MonoBehaviour {

	public float fadeTime = 1.0f;

	Coroutine fadeRoutine;

	CanvasGroup canvasGroup;

	void Start () {
		canvasGroup = GetComponent<CanvasGroup>();
		// Start with the overlay hidden
		canvasGroup.alpha = 0;
	}
	
	public void StartFade() {
		// Stop fading it fading already happening
		if (fadeRoutine != null) {
			StopCoroutine(fadeRoutine);
		}

		fadeRoutine = StartCoroutine(DoFade());
	}

	IEnumerator DoFade() {
		float fadeElapsed = 0;

		while (fadeElapsed < fadeTime) {
			var alpha = 1.0f - (fadeElapsed / fadeTime);

			canvasGroup.alpha = alpha;

			fadeElapsed += Time.deltaTime;

			yield return null;
		}
	}
}
