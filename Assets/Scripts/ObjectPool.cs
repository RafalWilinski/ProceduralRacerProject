using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour {

	public Stack<GameObject> availableObjects;
	public int availableCount;

	void Start() {
		availableObjects = new Stack<GameObject>();
		foreach(Transform t in transform) {
			if(t.gameObject.name == "Mesh") {
				availableCount++;
				availableObjects.Push(t.gameObject);
			}
		}
	}

	public GameObject Create (Vector3 pos) {
		availableCount--;
		GameObject gen = availableObjects.Pop();
		gen.transform.position = pos;
		//gen.SendMessage("StartGeneration");
		if(availableCount < 3) Debug.Log("Warning, fewer than 3 object available!");
		return gen;
	}

	public void Return(GameObject g) {
		availableCount++;
		availableObjects.Push(g);
	}
}
