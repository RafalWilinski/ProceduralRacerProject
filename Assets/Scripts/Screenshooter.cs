using UnityEngine;
using System.Collections;
using System;

public class Screenshooter : MonoBehaviour
{

	public string path;
	public int superSize;

	public int resWidth;
	public int resHeight;

	private bool takeScreenshotRender;
	private bool takeScreenshotLate;

	void OnGUI () {
		if (takeScreenshotLate)
		{
			string s = String.Format(path, (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
			Debug.Log("Screenshot took: " + s);
			Application.CaptureScreenshot(s, superSize);
			takeScreenshotLate = false;
		}
	}

	void OnPostRender() {
		if (takeScreenshotRender)
		{
			string s = String.Format(path, (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
			Debug.Log("Screenshot took: " + s);
			Application.CaptureScreenshot(s, superSize);
			takeScreenshotRender = false;
		}
	}

	void Update() {
		if (Input.GetKeyUp(KeyCode.L)) {
			takeScreenshotLate = true;
		}

		if (Input.GetKeyUp(KeyCode.K)) {
			takeScreenshotRender = true;
		}
	}
}
