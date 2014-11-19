using UnityEngine;
using System.Collections;

public class RectTweener : MonoBehaviour {

	public Vector2 from;
	public Vector2 to;
	public float time;
	public AnimationCurve curve;
	public bool playOnStart;

	private RectTransform myRect;

	void Start() {
		myRect = this.GetComponent<RectTransform>();
		if(playOnStart) Anim();
	}
	void Anim () {
		StopCoroutine("Tween");
		StartCoroutine("Tween");
	}

	IEnumerator Tween() {
		for(float i = 0.0f; i < 1; i += 0.01f) {
			myRect.anchoredPosition = from * curve.Evaluate(1-i) + to * curve.Evaluate(i);
			yield return new WaitForSeconds(time/100f);
		}
	}

	public static void ToAlpha(float to) {

	}
}
