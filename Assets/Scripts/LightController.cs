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
	public Light light;

	void OnGUI () {
		cam.backgroundColor = backgroundColor;
		mat.color = materialColor;
		light.color = lightColor;
		RenderSettings.ambientLight = ambientColor;
	}
}
