using UnityEngine;
using UnityEngine.UI;
using ScionEngine;
using System.Collections;
	using UnityStandardAssets.ImageEffects;

public class GraphicsSettingsManager : MonoBehaviour {

	public GameObject gameCamera;
	public GameObject uiCamera;
	public ParticleSystem smallFragments;
	public Material mat;
	public Slider qualitySlider;

	public int motionBlurMinRequirement = 3;
	public int bloomMinRequirement = 2;
	public int colorCorrectionCurvesMinRequirement = 1;
	public int smallFragmentsMinRequirement = 1;
	public int glitchMinRequirement = 2;
	public int hdrMinRequirement = 3;

	void Start() {
		qualitySlider.value = PlayerPrefs.GetInt("qualitySettings");
		OnQualityChange(PlayerPrefs.GetInt("qualitySettings"));
	}

	public void OnQualityChange(float value) {
		Debug.Log("Quality set to: "+value);
		if(value >= motionBlurMinRequirement) gameCamera.GetComponent<AmplifyMotionEffect>().enabled = true;
		else gameCamera.GetComponent<AmplifyMotionEffect>().enabled = false;

		if(value >= smallFragmentsMinRequirement) smallFragments.Play();
		else {
			smallFragments.Stop();
			smallFragments.Clear();
		}

		if(value >= bloomMinRequirement) gameCamera.GetComponent<ScionPostProcess>().enabled = true;
		else gameCamera.GetComponent<ScionPostProcess>().enabled = false;

		if(value >= colorCorrectionCurvesMinRequirement) gameCamera.GetComponent<ColorCorrectionCurves>().enabled = true;
		else gameCamera.GetComponent<ColorCorrectionCurves>().enabled = false;

		PlayerPrefs.SetInt("qualitySettings", (int) value);
	}

	public void OnMaterialChange(float value) {
		switch((int)value) {
			case 0:
				mat.shader = Shader.Find("Mobile/Diffuse");
			break;
			case 1:
				mat.shader = Shader.Find("Mobile/VertexLit");
			break;
			case 2:
				mat.shader = Shader.Find("Mobile/VertexLit (Only Directional Lights)");
			break;
			case 3:
				mat.shader = Shader.Find("Legacy Shaders/Diffuse");
			break;
			case 4:
				mat.shader = Shader.Find("Legacy Shaders/VertexLit");
			break;
			case 5:
				mat.shader = Shader.Find("Legacy Shaders/Diffuse Fast");
			break;
		}
	}
}
