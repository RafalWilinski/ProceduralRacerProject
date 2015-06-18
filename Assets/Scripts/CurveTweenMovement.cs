using UnityEngine;
using System.Collections;

public class CurveTweenMovement : MonoBehaviour {

	public AnimationCurve animCurve;
	public float stepWait;
	public Vector3 movementVector;

	public Transform myTransform;
	public Renderer myRenderer;

	private float i = 0;
	
	void Start () {
		myTransform = transform;
		StartCoroutine("Tweening");
		i = Random.Range(0.01f,1f);
	}

	private IEnumerator Tweening() {
		while(true) {
			if(myRenderer.isVisible) {
				i += 0.01f;
				myTransform.position = new Vector3(myTransform.position.x, movementVector.y * animCurve.Evaluate(i % 1), myTransform.position.z);
			}

			yield return new WaitForSeconds(stepWait);
		}
	}

}
