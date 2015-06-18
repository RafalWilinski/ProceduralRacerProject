using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameOverCloudAnimation : MonoBehaviour {

	public CanvasGroup self;
	public Text textLabel;

	public Vector3[] positions;

	private void Start() {
		self = this.gameObject.GetComponent<CanvasGroup>();
	}

	public void Animate() {
		StartCoroutine("AnimCoroutine");
	}

	private IEnumerator AnimCoroutine() {
		TweenCanvasAlpha.Show(new TweenParameters(self, 0f, 1f, 0.5f, 0f));
		textLabel.text = "Share your score";
		this.GetComponent<RectTransform>().localPosition = positions[0];
		yield return new WaitForSeconds(1f);
		TweenCanvasAlpha.Show(new TweenParameters(self, 1f, 0f, 0.5f, 0f));
		yield return new WaitForSeconds(0.5f);
		TweenCanvasAlpha.Show(new TweenParameters(self, 0f, 1f, 0.5f, 0f));
		textLabel.text = "Check your achievements";
		this.GetComponent<RectTransform>().localPosition = positions[1];
		yield return new WaitForSeconds(1f);
		TweenCanvasAlpha.Show(new TweenParameters(self, 1f, 0f, 0.5f, 0f));
		yield return new WaitForSeconds(0.5f);
		TweenCanvasAlpha.Show(new TweenParameters(self, 0f, 1f, 0.5f, 0f));
		textLabel.text = "Leaderboards";
		this.GetComponent<RectTransform>().localPosition = positions[2];
		yield return new WaitForSeconds(1f);
		TweenCanvasAlpha.Show(new TweenParameters(self, 1f, 0f, 0.5f, 0f));
		yield return new WaitForSeconds(0.5f);
		TweenCanvasAlpha.Show(new TweenParameters(self, 0f, 1f, 0.5f, 0f));
		textLabel.text = "See Replay";
		this.GetComponent<RectTransform>().localPosition = positions[3];
		yield return new WaitForSeconds(1f);
		TweenCanvasAlpha.Show(new TweenParameters(self, 1f, 0f, 0.5f, 0f));
	}
}
