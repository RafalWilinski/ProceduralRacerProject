using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextAlphaPingPong : MonoBehaviour {

	public Text text;
	public float stepWait = 0.03f;
	public float addition = 0.03f;
	public bool isEnabled;
	private int mode;

	void Start () {
		StartCoroutine(Coroutine());
	}
	
	IEnumerator Coroutine() {
		while(true) {
			if(isEnabled) {
				if(mode == 0) {
					text.color = new Color(1,0,0,text.color.a + addition);
					if(text.color.a >= 1.2f) mode = 1;
				}
				else {
					text.color = new Color(1,0,0,text.color.a - addition);
					if(text.color.a <= 0.0f) mode = 0;
				}
			}
			yield return new WaitForSeconds(stepWait);
		}
	}
}
