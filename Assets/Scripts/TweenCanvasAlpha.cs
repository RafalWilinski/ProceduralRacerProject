using UnityEngine;
using System.Collections;

public class TweenCanvasAlpha : MonoBehaviour {

	public CanvasGroup canvas;
	public float from;
	public float to;
	public float time;
	public float delay;

	public void Show() {
		TweenCanvasAlpha instance = canvas.GetComponent<TweenCanvasAlpha>();
		TweenParameters t = new TweenParameters(canvas, from, to, time, delay);
		instance.ObjectiveShow(t);
	}

	public static void Show (TweenParameters parameters) {
		TweenCanvasAlpha instance = parameters.canvas.GetComponent<TweenCanvasAlpha>();
		if(instance == null) {
			instance = parameters.canvas.gameObject.AddComponent<TweenCanvasAlpha>();
		}
		instance.ObjectiveShow(parameters);
	}

	private void ObjectiveShow(TweenParameters parameters) {
		StopAllLocalCoroutines();

		canvas = parameters.canvas;
		to = parameters.to;
		from = parameters.from;
		time = parameters.time;
		delay = parameters.delay;

		StartCoroutine("Tweening");
	}

	private IEnumerator Tweening() {
		float stepWait = time/60;
		yield return new WaitForSeconds(delay);
		if(from < to) {
			while(canvas.alpha < to) {
				canvas.alpha += (to-from)/60f;
				yield return new WaitForSeconds(stepWait);
			}
		}
		else {
			while(canvas.alpha > to) {
				canvas.alpha += (to-from)/60f;
				yield return new WaitForSeconds(stepWait);
			}
		}
		if(canvas.alpha <= 0.02) canvas.interactable = false;
	}

	private void StopAllLocalCoroutines() {
		StopCoroutine("Tweening");
	}
}

public class TweenParameters {
		public CanvasGroup canvas;
		public float from;
		public float to;
		public float time;
		public float delay;
		public TweenParameters(CanvasGroup c, float from, float to, float t, float d) {
			this.from = from;
			this.to = to;
			canvas = c;
			time = t;
			delay = d;
		}
	}
