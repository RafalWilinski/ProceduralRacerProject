using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ControllerConnectedModal : MonoBehaviour {

	[SerializeField] private Image maskRectangle;
	[SerializeField] private float animationTime;
	[SerializeField] private float showTime;

	private static ControllerConnectedModal _instance; 

	void Awake () {
		_instance = this;
	}

	public static ControllerConnectedModal Instance {
		get {
			return _instance;
		}
	}

	public void Show() {
		StopCoroutine("MaskAnimation");
		StartCoroutine("MaskAnimation");
	}

	private IEnumerator MaskAnimation() {
		maskRectangle.fillOrigin = 0;

		float floatVariable = 0f;
		for(floatVariable = 0; floatVariable <= 1f; floatVariable += 0.02f) {
			maskRectangle.fillAmount = floatVariable;
			yield return new WaitForSeconds(animationTime/100);
		}

		yield return new WaitForSeconds(showTime);

		maskRectangle.fillOrigin = 1;

		for(floatVariable = 1; floatVariable >= 0f; floatVariable -= 0.02f) {
			maskRectangle.fillAmount = floatVariable;
			yield return new WaitForSeconds(animationTime/100);
		}

	}
}
