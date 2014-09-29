using UnityEngine;
using System.Collections;

public class ContinousMovement : MonoBehaviour {

	public Vector3 vect;
	void Update () {
			transform.Translate(vect);
	}
}
