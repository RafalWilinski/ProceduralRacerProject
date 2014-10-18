using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ThemeManager))]
public class ThemeManagerEditor : Editor {

public float sizeMultiplier;
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		ThemeManager myScript = (ThemeManager)target;
		if(GUILayout.Button("Next Theme")) {
			myScript.NextTheme();
		}
	}
}
