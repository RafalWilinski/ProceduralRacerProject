using UnityEngine;
using System.Collections;

public class Crashes : MonoBehaviour {
	void OnGUI() {
		var reports = CrashReport.reports;
		GUILayout.Label("Crash reports:");
		foreach (var r in reports) {
			GUILayout.BeginHorizontal();
			GUILayout.Label("Crash: " + r.time);
			if (GUILayout.Button("Log")) {
				Debug.Log(r.text);
			}
			if (GUILayout.Button("Remove")) {
				r.Remove();
			}
			GUILayout.EndHorizontal();
		}
	}
}