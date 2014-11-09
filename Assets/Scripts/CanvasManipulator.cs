using UnityEngine;
using System.Collections;

public class CanvasManipulator : MonoBehaviour {

	public CanvasGroup myCanvas;
	void Awake () {
		myCanvas = GetComponent<CanvasGroup>();
	}
	
	void Update () {
		if(myCanvas != null) {
			if(myCanvas.alpha <= 0) {
				myCanvas.interactable = false;
				myCanvas.blocksRaycasts = false;
			}
		}
	}
}
