using UnityEngine;
using System.Collections;

public class CanvasManager : MonoBehaviour {

	public bool isDebug;
	public int currentState;
	public ContinousMovement mov;
	public GameObject cam;
	public CanvasGroup mainMenu;
	public RegionSelector regionSelector;

	private float fovValue;

	/*public class TweenParameters {
		public CanvasGroup canvas;
		public float time;
		public float delay;
		public TweenParameters(CanvasGroup c, float t, float d) {
			canvas = c;
			time = t;
			delay = d;
		}
	}
	*/ 


	public void GoToCheckpoint() {
		LeanTween.rotate (cam, Vector3.zero ,0.8f).setEase( LeanTweenType.easeInOutCubic);
		LeanTween.moveLocal (cam, Vector3.zero ,0.8f).setEase( LeanTweenType.easeInOutCubic);
		//StartCoroutine("HideCanvasGroup",new TweenParameters(mainMenu, 1f, 0f)) ;
		TweenCanvasAlpha.Show(new TweenParameters(mainMenu, 1f, 0f, 1f, 0f));
		currentState = 1;
		regionSelector.NextRegion();
	}

	public void GoBackToMainFromCheckpoint() {
		LeanTween.moveLocal (cam, Vector3.zero ,0.8f).setEase( LeanTweenType.easeInOutCubic);
		LeanTween.rotate (cam, new Vector3(0f,27f, -32.671f),1).setEase( LeanTweenType.easeInQuad );
		//StartCoroutine("ShowCanvasGroup",new TweenParameters(mainMenu, 1f, 0f)) ;
		TweenCanvasAlpha.Show(new TweenParameters(mainMenu, 0f, 1f, 1f, 0f));
		currentState = 0;
	}

	void Start() {
//		StartCoroutine(ShowCanvasGroup(new TweenParameters(mainMenu, 1.5f, 0f)));
		TweenCanvasAlpha.Show(new TweenParameters(mainMenu, 0f, 1f, 1.5f, 0f));
	}

	//Region - Helpers

	private void Log(string m) {
		if(isDebug) Debug.Log("CanvasManager: "+m);
	}
	//private IEnumerator TweenSpeed()
}
