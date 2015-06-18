using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class LightController : MonoBehaviour {

	public Color backgroundColor;
	public Color materialColor;
	public Color lightColor;
	public Color ambientColor;

	public Camera cam;
	public Material mat;
	public Light sceneLight;

	void OnGUI () {
		cam.backgroundColor = backgroundColor;
		mat.color = materialColor;
		sceneLight.color = lightColor;
		RenderSettings.ambientLight = ambientColor;
	}
}
