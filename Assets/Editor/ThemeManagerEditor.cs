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
	public float lightIntensity = 1f;
	public Vector3 lightRotation = Vector3.zero;

	public bool isRealtime;
	public bool showThemeCreator;
	public bool showThemeDeletion;
	public bool showThemeSwapper;
	public string deletedThemeName;

	private string theme_1;
	private string theme_2;

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
			lightRotation = EditorGUILayout.Vector3Field("Light Rotation: ", lightRotation);
			lightIntensity = EditorGUILayout.Slider("Light Intensity: ", lightIntensity, 0f, 2f);

			isRealtime = EditorGUILayout.Toggle("Realtime edit", isRealtime);

			if(GUILayout.Button("Save this Theme")) {
				myScript.currentThemeIndex = myScript.themes.Count;
				myScript.AddOrChangeTheme(new ThemeManager.Theme(themeName, lightsColor, materialColor, backgroundColor, ambientColor, lightRotation, lightIntensity));
				isRealtime = false;
			}

			if(GUILayout.Button("Load theme from current settings")) {
				themeName = myScript.GetCurrentTheme().name;
				lightsColor = myScript.lights[0].color;
				lightIntensity = myScript.lights[0].intensity;
				lightRotation = myScript.lights[0].transform.eulerAngles;
				materialColor = myScript.mat.color;
				backgroundColor = RenderSettings.fogColor;
				ambientColor = RenderSettings.ambientLight;
			}

		}	

		showThemeDeletion = EditorGUILayout.Foldout(showThemeDeletion, "Theme Deletion");
		if(showThemeDeletion) {
			EditorGUILayout.LabelField("Theme deletion");
			// deletedThemeName = GUILayout.TextField(deletedThemeName,25);
			// if(GUILayout.Button("Delete Theme")) {
			// 	ThemeManager.Theme t = myScript.FindThemeByName(deletedThemeName);
			// 	myScript.themes.Remove(t);
			// }
		}


		showThemeSwapper = EditorGUILayout.Foldout(showThemeSwapper, "Theme Swapper");
		if(showThemeSwapper) {
			theme_1 = EditorGUILayout.TextField("Theme 1:",theme_1);
			theme_2 = EditorGUILayout.TextField("Theme 2:",theme_2);

			if(GUILayout.Button("Swap!")) {
				//ThemeManager.Theme t1 = myScript.FindThemeByName(theme_1);
				//ThemeManager.Theme t2 = myScript.FindThemeByName(theme_2);
				int i1 = myScript.GetThemeIndexByName(theme_1);
				int i2 = myScript.GetThemeIndexByName(theme_2);

				ThemeManager.Theme temp = myScript.GetThemeByIndex(theme_1);

				myScript.themes[i1] = myScript.themes[i2];
				myScript.themes[i2] = temp;
			}
		}


		if(isRealtime) {
			myScript.SetTheme(new ThemeManager.Theme(themeName, lightsColor, materialColor, backgroundColor, ambientColor, lightRotation, lightIntensity));
		}
	}
}
