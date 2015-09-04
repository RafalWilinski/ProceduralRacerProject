using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RevealEvent : MonoBehaviour {

	public Text text;

	public void Reveal() {
		TweenCanvasAlpha.Show(new TweenParameters(this.GetComponent<CanvasGroup>(), 0f, 1f, 1f, 0f));
	}

	public void SetDesc(string name, int distance) {
		text.text = name + "\n" + distance.ToString();
	}
}
