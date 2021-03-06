﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
//using VacuumShaders;

public class ThemeManager : MonoBehaviour {

	public List<Theme> themes;

	public Light[] lights;
	public Camera[] cams;
	public Material mat;
//	public VacuumShaders.CurvedWorld.CurvedWorld_GlobalController controller;
	public float lerpSpeed;
	public float lerpMultiplication;
	public int currentThemeIndex;
	public int fakeCurrentThemeIndex;
	public bool shouldTweenCurvedFog;
	public EventsManager eventManager;
	public NodeSpawner nodeSpawner;

	[Serializable]
	public class Theme {
		[Header("Theme")]
		public string name;
		public string fullName;
		public float distance; 
		public Color lightsColor;
		public Color materialColor;
		public Color backgroundColor;
		public Color ambientColor = new Color(0.2f, 0.2f, 0.2f);
		public AnimationCurve curve;
		public float x_spacing;
		public float y_spacing;
		public Vector3 lightRotation;
		public float lightIntensity;
		public float x_SplineVariety = 150;
		public float ySplineVariety;
		public int unlockDistance;
		public bool isAvailable;
		public AudioClip soundtrack;
		public List<String> events;

		public Theme(string n, Color l, Color m, Color b, Color a, Vector3 r, float li) { name = n; lightsColor = l; materialColor = m; backgroundColor = b; ambientColor = a; lightRotation = r; lightIntensity = li; }
	}	

	public List<String> permaEvents;

	private static ThemeManager _instance;

	public static ThemeManager Instance {
		get { return _instance; }
	}

	private void Awake() { 
		_instance = this;
	}

	void Update() {
		if(Input.GetKeyUp(KeyCode.N)) {
			NextThemeLerp();
		}
	}

	public Theme GetThemeByIndex(string theme_name) {
		foreach(Theme t in themes) {
			if(t.name == theme_name) return t;
		}
		return null;
	}

	public Theme GetThemeByIndex(int i) {
		return themes[i];
	}

	public int GetThemeIndexByName(string name) {
		for(int i = 0; i<themes.Count; i++) {
			if(themes[i].name == name) return i;
		}
		return -1;
	}

	public Theme GetCurrentTheme() {
		return themes[currentThemeIndex];
	}

	public void SwitchTo (int i) {
		currentThemeIndex = i;
		SetTheme(themes[i]);
	}

	public void LerpTo (int i) {
		currentThemeIndex = i;
		StopCoroutine("LerpToTheme");
		StartCoroutine("LerpToTheme",themes[i]);
	}

	public void FakeLerpTo (int i) {
		fakeCurrentThemeIndex = i;
		StopCoroutine("LerpToTheme");
		StartCoroutine("LerpToTheme",themes[i]);
	}

	public void NextTheme() {
		if(currentThemeIndex + 1 >= themes.Count) currentThemeIndex = 0;
		else currentThemeIndex++;
		SwitchTo(currentThemeIndex);
	}

	public void NextThemeLerp() {
		if(currentThemeIndex + 1 >= themes.Count) currentThemeIndex = 0;
		else currentThemeIndex++;
		LerpTo(currentThemeIndex);
	}

	public void SetTheme(Theme t) {
		eventManager.DisableAllEvents();

		foreach(Light l in lights)  {
			l.color = t.lightsColor;
			l.intensity = t.lightIntensity;
			l.transform.eulerAngles = t.lightRotation;
		}	
		
		foreach(Camera cam in cams) cam.backgroundColor = t.backgroundColor;

		mat.color = t.materialColor;
		RenderSettings.fogColor = t.backgroundColor;
		RenderSettings.ambientLight = t.ambientColor;

		foreach(String s in t.events) {
			eventManager.EnableEvent(s);
		}

		foreach(String s in permaEvents) {
			eventManager.EnableEvent(s);
		}

		nodeSpawner.yVariety = t.ySplineVariety;
		nodeSpawner.xVariety = t.x_SplineVariety;
//		if(shouldTweenCurvedFog) controller._V_CW_Fog_Color_GLOBAL = t.backgroundColor;
	}

	public void UpdateCurrentTheme() {
		SetTheme(themes[currentThemeIndex]);
	}

	public void AddOrChangeTheme(Theme t) {
		for(int i = 0; i<themes.Count; i++) {
			if(themes[i].name == t.name) {
				t.fullName = themes[i].fullName;
				t.x_spacing = themes[i].x_spacing;
				t.curve = themes[i].curve;
				t.y_spacing = themes[i].y_spacing;
				t.distance = themes[i].distance;
				themes[i] = t;
				return;
			}
		}

		t.lightIntensity = lights[0].intensity;
		t.lightRotation = lights[0].transform.eulerAngles;
		themes.Add(t);
	}

	public Theme FindThemeByName(string n) {
		for(int i = 0; i<themes.Count; i++) {
			if(themes[i].name == n) return themes[i];
		}
		return null;
	}

	private IEnumerator LerpToTheme(Theme t) {
		Debug.Log("Lerping theme to: "+t.name);
		float elapsedTime = 0f;
		while(lerpSpeed >= elapsedTime) {
			foreach(Light l in lights) {
				l.color = Color.Lerp(l.color, t.lightsColor, Time.deltaTime);
				l.intensity = Mathf.Lerp(l.intensity, t.lightIntensity, Time.deltaTime);
				l.transform.rotation = Quaternion.Lerp (l.transform.rotation, Quaternion.Euler(t.lightRotation), Time.deltaTime);
			}

			foreach(Camera cam in cams) cam.backgroundColor = Color.Lerp(cam.backgroundColor, t.backgroundColor, Time.deltaTime);
			//mat.color = Color.Lerp(mat.color, t.materialColor, Time.deltaTime);
			RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, t.backgroundColor, Time.deltaTime);
			RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight, t.ambientColor, Time.deltaTime);
			//if(shouldTweenCurvedFog) controller._V_CW_Fog_Color_GLOBAL = Color.Lerp(controller._V_CW_Fog_Color_GLOBAL, t.backgroundColor, Time.deltaTime);
			elapsedTime += Time.deltaTime;
			yield return new WaitForSeconds(Time.deltaTime*lerpMultiplication);
		}
		SetTheme(t);
	}
}
