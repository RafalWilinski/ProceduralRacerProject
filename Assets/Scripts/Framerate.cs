using UnityEngine;
using System.Collections;
using System;

public class Framerate : MonoBehaviour {

	public int framerate;
	void Awake () {
		Application.targetFrameRate = -1;
		InvokeRepeating("RemoveGC", 0.1f, 0.1f);
	}

	void RemoveGC() {
		//Debug.Log("removing!");
		//Debug.Log("Before: "+System.GC.GetTotalMemory(false));
		GC.Collect();
		//Debug.Log("After: "+System.GC.GetTotalMemory(false));
	}
}
