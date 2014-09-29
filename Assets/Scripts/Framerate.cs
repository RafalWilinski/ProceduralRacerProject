using UnityEngine;
using System.Collections;

public class Framerate : MonoBehaviour {

	public int framerate;
	void Awake () {
		Application.targetFrameRate = framerate;
	}
}
