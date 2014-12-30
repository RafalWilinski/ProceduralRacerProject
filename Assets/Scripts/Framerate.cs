using UnityEngine;
using System.Collections;
using System;

public class Framerate : MonoBehaviour {

	public int framerate;
	void Awake () {
		
		#if UNITY_EDITOR
			Application.targetFrameRate = framerate;
		#elif UNITY_ANDROID || UNITY_IOS
			Application.targetFrameRate = 60;
		#endif

		InvokeRepeating("RemoveGC", 1f, 1f);
	}

	void RemoveGC() {
		//Debug.Log("removing!");
		//Debug.Log("Before: "+System.GC.GetTotalMemory(false));
		GC.Collect();
		//Debug.Log("After: "+System.GC.GetTotalMemory(false));
	}
}
