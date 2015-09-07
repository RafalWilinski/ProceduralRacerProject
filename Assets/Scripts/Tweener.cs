using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Tweener : MonoBehaviour {

	public enum Style {
		ImageSize,
		TextSize,
		ImageAlpha,
		TextAlpha,
		FillAmount
	}

	public bool shouldTweenOfStart;
	public Style style;
	public Vector2 toVector2;
	public float toFloat;
	public float time;
	public int steps;
	public AnimationCurve curve;

	private Text text;
	private Image image;

	void Awake() {
		if(shouldTweenOfStart) StartCoroutine("Tween");
	}

	public void ImageSize (Vector2 to, float time) {
		this.toVector2 = to;
		this.time = time;
		StartCoroutine("Tween");
	}

	public void FillAmount (float to, float time) {
		this.toFloat = to;
		this.time = time;
		StartCoroutine("Tween");
	}

	public void StartTween() {
		StopCoroutine("Tween");
		StartCoroutine("Tween");
	}

	IEnumerator Tween() {
		if(steps < 1) steps = 25;

		steps = time / Time.deltaTime;
		float step = 1f / steps;

		switch(style) {
			case (Style.FillAmount):
				if(image == null) image = this.GetComponent<Image>();
				for(int i = 0; i < steps; i++) {
					image.fillAmount = curve.Evaluate(step * i);
					yield return new WaitForEndOfFrame();
				}
			break;

			case(Style.TextAlpha):
				if(text == null) text = this.GetComponent<Text>();
				step = 1f / steps;
				for(int i = 0; i < steps; i++) {
					text.color = new Color(text.color.r, text.color.g, text.color.b, curve.Evaluate(step * i));
					yield return new WaitForEndOfFrame();
				}
			break;
		}
	}
}
