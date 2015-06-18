using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GraphicsSettingsManager : MonoBehaviour {

	public GameObject gameCamera;

	public Toggle anisoToggle;
	public Toggle bloomToggle;
	public Toggle blurToggle;
	public Toggle colorToggle;
	public Toggle aberrationToggle;

	private void Start() {
		StartCoroutine(SetAfterTime());
	}

	private IEnumerator SetAfterTime() {
		yield return new WaitForSeconds(0.5f);

		if(PlayerPrefs.GetInt("graphics_setup") != 123456) {
			PlayerPrefs.SetInt("graphics_setup",123456);

			PlayerPrefs.SetInt("anisotropic_filtering",1);
			PlayerPrefs.SetInt("bloom",0);
			PlayerPrefs.SetInt("chromatic_aberration",0);
			PlayerPrefs.SetInt("color_correction",1);
		}

		bool isOn;

		isOn = (PlayerPrefs.GetInt("anisotropic_filtering") == 1 ? true : false);
		anisoToggle.isOn = isOn;

		isOn = (PlayerPrefs.GetInt("bloom") == 1 ? true : false);
		bloomToggle.isOn = isOn;
		gameCamera.GetComponent<Bloom>().enabled = isOn;

		isOn = (PlayerPrefs.GetInt("motion_blur") == 1 ? true : false);
		blurToggle.isOn = isOn;
		gameCamera.GetComponent<AmplifyMotionEffect>().enabled = isOn;

		isOn = (PlayerPrefs.GetInt("color_correction") == 1 ? true : false);
		colorToggle.isOn = isOn;
		gameCamera.GetComponent<ColorCorrectionCurves>().enabled = isOn;

		isOn = (PlayerPrefs.GetInt("chromatic_aberration") == 1 ? true : false);
		aberrationToggle.isOn = isOn;
		gameCamera.GetComponent<Vignetting>().enabled = isOn;
	}

	public void OnAnisoChange() {
		Debug.Log("OnAnisoChange "+anisoToggle.isOn);

		if(anisoToggle.isOn) QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
		else QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;

		int preferencesNum = (anisoToggle.isOn ? 1 : 0);
		PlayerPrefs.SetInt("anisotropic_filtering", preferencesNum);
	}

	public void OnBloomChange() {
		Debug.Log("OnBloomChange "+bloomToggle.isOn);
		gameCamera.GetComponent<Bloom>().enabled = bloomToggle.isOn;

		int preferencesNum = (bloomToggle.isOn ? 1 : 0);
		PlayerPrefs.SetInt("bloom", preferencesNum);
	}

	public void OnMotionBlurChange() {
		Debug.Log("OnBlurChange " + blurToggle.isOn);
		gameCamera.GetComponent<AmplifyMotionEffect>().enabled = blurToggle.isOn;

		int preferencesNum = (blurToggle.isOn ? 1 : 0);
		PlayerPrefs.SetInt("motion_blur", preferencesNum);
	}

	public void OnCorrectionCurvesChange() {
		Debug.Log("OnCurvesChange "+colorToggle.isOn);
		gameCamera.GetComponent<ColorCorrectionCurves>().enabled = colorToggle.isOn;

		int preferencesNum = (colorToggle.isOn ? 1 : 0);
		PlayerPrefs.SetInt("color_correction", preferencesNum);
	}

	public void OnChromaticAberrationChange() {
		Debug.Log("OnAberrrationChange "+aberrationToggle.isOn);
		gameCamera.GetComponent<Vignetting>().enabled = aberrationToggle.isOn;

		int preferencesNum = (aberrationToggle.isOn ? 1 : 0);
		PlayerPrefs.SetInt("chromatic_aberration", preferencesNum);
	}
}
