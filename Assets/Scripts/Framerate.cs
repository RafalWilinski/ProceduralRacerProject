using UnityEngine;
using System.Collections;
using System;

public class Framerate : MonoBehaviour {

	public int framerate;
	void Awake () {
<<<<<<< Updated upstream
		Application.targetFrameRate = -1;
		InvokeRepeating("RemoveGC", 0.1f, 0.1f);
=======
		Application.targetFrameRate = 60;
		//InvokeRepeating("RemoveGC", 0.1f, 0.1f);
>>>>>>> Stashed changes
	}

	void RemoveGC() {
		//Debug.Log("removing!");
		//Debug.Log("Before: "+System.GC.GetTotalMemory(false));
		GC.Collect();
		//Debug.Log("After: "+System.GC.GetTotalMemory(false));
	}
}
