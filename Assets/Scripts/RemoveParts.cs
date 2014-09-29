using UnityEngine;
using System.Collections;
using System;

public class RemoveParts : MonoBehaviour {

	private bool hasEverBeenVisible;
	private Transform myTransform;
	private GameObject cam;
	private Transform camTrans;


	/*
	void OnBecameVisible() {
		hasEverBeenVisible = true;
	}

	void OnBecameInvisible() {
		if(hasEverBeenVisible) {
			StartCoroutine("Remove");
		}
	}
	*/

	void Remove() {
		Debug.Log("usuwam");
		Destroy(gameObject.GetComponent<MeshFilter>().mesh);
		Destroy(gameObject.GetComponent<MeshFilter>());
		Destroy(gameObject);
		GC.Collect();
	}


	void Start() {
		myTransform = transform;
		cam = GameObject.Find("Main Camera");
		camTrans = cam.transform;
		StartCoroutine("Check");
	}

	IEnumerator Check() {
		while(true) {
			if(myTransform.position.z + 1500 < camTrans.position.z) {
				Remove();
			}
			yield return new WaitForSeconds(0.25f);
		}
	}
}
