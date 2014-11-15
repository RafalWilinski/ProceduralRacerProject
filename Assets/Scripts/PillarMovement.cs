using UnityEngine;
using System.Collections;

public class PillarMovement : MonoBehaviour {

	public float to_y;
	public float ySpeed;
	private Transform myTransform;

	void Start () {
		myTransform = transform;
		myTransform.eulerAngles = new Vector3(Random.Range(-15,15), Random.Range(-15,15), Random.Range(-15,15));
		StartCoroutine("Rise");
	}
	
	private IEnumerator Rise() {
		while(to_y > myTransform.position.y) {
			myTransform.Translate(new Vector3(0, ySpeed, 0));
			yield return new WaitForSeconds(0.01f);
		}
	}
}
