using UnityEngine;
using System.Collections;
using System;

public class Screenshooter : MonoBehaviour
{

	public string path;
	public int superSize;

	public int resWidth;
	public int resHeight;

	void Update () {
		if (Input.GetKeyUp(KeyCode.K))
		{
			string s = String.Format(path, (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
			Debug.Log("Screenshot took: " + s);
			Application.CaptureScreenshot(s, superSize);
		}
	}

	private bool takeHiResShot = false;

	public static string ScreenShotName(int width, int height)
	{
		return string.Format("Screenshots/screen_{0}.png", (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
	}
}
