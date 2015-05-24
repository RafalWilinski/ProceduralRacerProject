using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class CanvasManager : MonoBehaviour {

	public bool isDebug;
	public int currentState;
	public ContinousMovement mov;
	public GameObject cam;
	public CanvasGroup mainMenu;
	public CanvasGroup gameInterface;
	public CanvasGroup checkpointSelector;
	public GameObject checkpointSelectorParent;
	public CanvasGroup pauseMenu;
	public CanvasGroup tint;
	public RegionSelector regionSelector;

	public float checkpointsVisibility;
	private float fovValue;
	private bool isPaused;

	public void GoToCheckpoint() {
		checkpointsVisibility = 1;
		LeanTween.rotate (cam, Vector3.zero ,0.8f).setEase( LeanTweenType.easeInOutCubic);
		LeanTween.moveLocal (cam, Vector3.zero ,0.8f).setEase( LeanTweenType.easeInOutCubic);
		//StartCoroutine("HideCanvasGroup",new TweenParameters(mainMenu, 1f, 0f)) ;
		TweenCanvasAlpha.Show(new TweenParameters(mainMenu, 1f, 0f, 1f, 0f));
		currentState = 1;
		regionSelector.NextRegion();
	}

	public void GoBackToMainFromCheckpoint() {
		checkpointsVisibility = 0;
		LeanTween.moveLocal (cam, new Vector3(0, 27.4f, -32f) ,0.8f).setEase( LeanTweenType.easeInOutCubic);
		LeanTween.rotate (cam, new Vector3(40f,0f,0f),1).setEase( LeanTweenType.easeInQuad );
		//StartCoroutine("ShowCanvasGroup",new TweenParameters(mainMenu, 1f, 0f)) ;
		//TweenCanvasAlpha.Show(new TweenParameters(mainMenu, 0f, 1f, 0.5f, 0f));
		currentState = 0;
	}

	//Region - Helpers

	private void Log(string m) {
		if(isDebug) Debug.Log("CanvasManager: "+m);
	}

	private IEnumerator decreaseCheckpointsVisibility() {
		while(checkpointsVisibility > 0f) {
			checkpointsVisibility -= 0.02f;
			yield return new WaitForSeconds(0.01f);
		}
		Destroy(checkpointSelectorParent);
	}
}
