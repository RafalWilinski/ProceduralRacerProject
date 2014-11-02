using UnityEngine;
using System.Collections;

public class EffectsController : MonoBehaviour {

	public void SwitchBloom () {
		if(this.GetComponent<FastBloom>().enabled == true) {
			this.GetComponent<FastBloom>().enabled = false;
		}
		else {
			this.GetComponent<FastBloom>().enabled = true;
		}
	}

	public void SwitchVig () {
		if(this.GetComponent<Vignetting>().enabled == true) {
			this.GetComponent<Vignetting>().enabled = false;
		}
		else {
			this.GetComponent<Vignetting>().enabled = true;
		}
	}

	public void SwitchCurves () {
		if(this.GetComponent<ColorCorrectionCurves>().enabled == true) {
			this.GetComponent<ColorCorrectionCurves>().enabled = false;
		}
		else {
			this.GetComponent<ColorCorrectionCurves>().enabled = true;
		}
	}
}
