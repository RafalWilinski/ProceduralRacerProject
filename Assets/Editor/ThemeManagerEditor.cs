using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ThemeManager))]
public class ThemeManagerEditor : Editor {

	public float sizeMultiplier;
	public string themeName;
	public Color lightsColor;
	public Color materialColor;
	public Color backgroundColor;
	public Color ambientColor = new Color(0.2f, 0.2f, 0.2f);
	public bool isRealtime;
	public bool showThemeCreator;

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Theme Toolbag");

		ThemeManager myScript = (ThemeManager)target;
		if(GUILayout.Button("Next Theme")) {
			myScript.NextTheme();
		}

		if(GUILayout.Button("Next Lerp to Theme")) {
			myScript.NextThemeLerp();
		}

		if(GUILayout.Button("Update Current Theme")) {
			myScript.UpdateCurrentTheme();
		}

		EditorGUILayout.Space();
		showThemeCreator = EditorGUILayout.Foldout(showThemeCreator, "Theme Creation");
		if(showThemeCreator) {
			themeName = EditorGUILayout.TextField("Name: ",themeName);
			lightsColor = EditorGUILayout.ColorField("Lights Color: ", lightsColor);
			materialColor = EditorGUILayout.ColorField("Material Color: ", materialColor);
			backgroundColor = EditorGUILayout.ColorField("Background Color: ", backgroundColor);
			ambientColor = EditorGUILayout.ColorField("Ambient Color: ", ambientColor);

			isRealtime = EditorGUILayout.Toggle("Realtime edit", isRealtime);

			if(GUILayout.Button("Save this Theme")) {
				myScript.currentThemeIndex = myScript.themes.Count;
				myScript.AddOrChangeTheme(new ThemeManager.Theme(themeName, lightsColor, materialColor, backgroundColor, ambientColor));
				isRealtime = false;
			}

			if(GUILayout.Button("Load theme from current settings")) {
				lightsColor = myScript.light.color;
				materialColor = myScript.mat.color;
				backgroundColor = RenderSettings.fogColor;
				ambientColor = RenderSettings.ambientLight;
			}

		}

		if(isRealtime) {
			myScript.SetTheme(new ThemeManager.Theme(themeName, lightsColor, materialColor, backgroundColor, ambientColor));
		}
	}
}
