using UnityEngine;
using System.Collections;

public class PillarMovement : MonoBehaviour {

	public float to_y;
	public float ySpeed;
	private Transform myTransform;

	void Start () {
		myTransform = transform;
		StartCoroutine("Rise");
	}
	
	private IEnumerator Rise() {
		while(to_y > myTransform.position.y) {
			myTransform.Translate(new Vector3(0, ySpeed, 0));
			yield return new WaitForSeconds(0.01f);
		}
	}
}
