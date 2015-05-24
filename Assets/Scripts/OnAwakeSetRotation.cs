using UnityEngine;
using System.Collections;

public class OnAwakeSetRotation : MonoBehaviour {

	public Vector3 rotation;
	void Awake () {
		transform.eulerAngles = rotation;
	}
}
