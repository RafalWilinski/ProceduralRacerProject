using UnityEngine;
using System.Collections;

public class RegionCamDistance : MonoBehaviour {

	public Transform cam;
	public CanvasManager manager;
	public CanvasGroup mainMenuCanvas;
	public float refreshRate;
	public float minDistance;
	public float alpha; 
	public float divisionModifier;
	private Transform myTransform;

	void Start () {
		myTransform = transform;
		StartCoroutine("Refresh");
	}

	IEnumerator Refresh() {
		while(true) {
			if(manager.currentState == 1) {
				float dist = Vector3.Distance(myTransform.position, cam.position);
				if(dist < minDistance) alpha = ((minDistance - dist) / (minDistance * divisionModifier)) * (1-mainMenuCanvas.alpha);
				else alpha = 0f;

				GetComponent<CanvasGroup>().alpha = alpha;
			}
			else GetComponent<CanvasGroup>().alpha = 0f;

			yield return new WaitForSeconds(refreshRate);
		}
	}
}
