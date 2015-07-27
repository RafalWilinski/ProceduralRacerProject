using UnityEngine;
using System.Collections;

public class ContinousRotation : MonoBehaviour {

	[SerializeField] private float rotationsPerMinute = 1.0f;
	[SerializeField] private Vector3 axis = new Vector3(0, 6, 0);
	[SerializeField] private Space space = Space.World;

	private bool shouldUpdate;

	void OnBecameVisible() {
		shouldUpdate = true;
	}

	void OnBecameInvisible() {
		shouldUpdate = false;
	}

 	void Update()
 	{
     	if(shouldUpdate) transform.Rotate(rotationsPerMinute*axis*Time.deltaTime, space);
 	}
}
