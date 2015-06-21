using UnityEngine;
using System.Collections;

public class ZAxisFollower : MonoBehaviour {

	public Transform vehicle;
	public float offset;
	private Transform myTransform;

	void Awake() {
		this.myTransform = this.transform;
	}

	void Update () {
		myTransform.position = new Vector3(myTransform.position.x, myTransform.position.y, vehicle.position.z + offset);
	}
}
