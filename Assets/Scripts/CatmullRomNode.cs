﻿using UnityEngine;
using System.Collections;

public class CatmullRomNode : MonoBehaviour {

	private CatmullRomSpline root;
	private GameObject rootGo;
	private Transform myTransform;
	private Vector3 cachedPos;
	private GameObject thisGameObject;

	public bool shouldAddOnStart;
	public bool isPush;
	public float timeGiven;

	void Start() {
		thisGameObject = gameObject;
		myTransform = transform;
		rootGo = GameObject.Find("Root");
		myTransform.parent = rootGo.transform;
		root = (CatmullRomSpline) rootGo.GetComponent<CatmullRomSpline>();

		if(shouldAddOnStart) {
			AddNodeToSpline();
		}
	}

	void AddNodeToSpline () {
		if(root != null) {
			if(isPush) root.PushNode(thisGameObject);
			
			else root.AddNode(thisGameObject);
		}
		//StartCoroutine("positionCheck");
		//StartCoroutine("Destroy");
	}

	IEnumerator positionCheck() {
		while(true) {
			cachedPos = myTransform.position;
			yield return new WaitForSeconds(0.1f); //Update interval set to 0.1 sec.
			if(cachedPos != myTransform.position) {
				root = rootGo.GetComponent<CatmullRomSpline>();
				root.AddNode(thisGameObject);
			}
		}
	}

	IEnumerator Destroy() {
		yield return new WaitForSeconds(1f);
		Destroy(thisGameObject);
	}
}
