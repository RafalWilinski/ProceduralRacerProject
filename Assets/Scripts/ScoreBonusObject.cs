using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreBonusObject : MonoBehaviour {
	private Text label;
	private CanvasGroup cg;

	public void Assign(int score, string cause) {
		label = GetComponent<Text>();
		cg = GetComponent<CanvasGroup>();
		label.text = "+"+score+"pts ("+cause+")";
		StartCoroutine("DestroyEnumerator");
	}

	private IEnumerator DestroyEnumerator() {
		yield return new WaitForSeconds(ScoreBonusManager.Instance.dieTime);
		TweenCanvasAlpha.Show(new TweenParameters(cg, 1f, 0f, 0.2f, 0f));
		yield return new WaitForSeconds(0.5f);
		ScoreBonusManager.Instance.SiftDown();
		Destroy(this.gameObject);
	}
}
