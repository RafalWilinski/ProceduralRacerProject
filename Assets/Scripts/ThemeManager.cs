using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ThemeManager : MonoBehaviour {

	public List<Theme> themes;

	public Light light;
	public Camera cam;
	public Material mat;

	private int currentThemeIndex;

	[Serializable]
	public class Theme {
		public string name;
		public Color lightsColor;
		public Color materialColor;
		public Color backgroundColor;
		public Color ambientColor = new Color(0.2f, 0.2f, 0.2f);
	}

	public void SwitchTo (int i) {
		currentThemeIndex = i;
		SetTheme(themes[i]);
	}

	public void NextTheme() {
		if(currentThemeIndex + 1 >= themes.Count) currentThemeIndex = 0;
		else currentThemeIndex++;
		SwitchTo(currentThemeIndex);
	}

	private void SetTheme(Theme t) {
		Debug.Log("Switching theme to: "+t.name);
		light.color = t.lightsColor;
		cam.backgroundColor = t.backgroundColor;
		mat.color = t.materialColor;
		RenderSettings.fogColor = t.backgroundColor;
		RenderSettings.ambientLight = t.ambientColor;
	}
}
