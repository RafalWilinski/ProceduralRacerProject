using UnityEngine;
using System.Collections;

public class CanvasDisableInteraction : MonoBehaviour {

	public bool enableOrDisable;
	public float delay;

	public void EnableOrDisable () {
		StartCoroutine("WaitAndDoAction");
	}

	private IEnumerator WaitAndDoAction() {
		yield return new WaitForSeconds(delay);
		GetComponent<CanvasGroup>().interactable = enableOrDisable;
	}
}
