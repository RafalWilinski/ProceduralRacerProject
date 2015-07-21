using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CheckpointContainer : MonoBehaviour {

	public Text regionName;
	public Text regionIndex;
	public RegionSelector selector;
	public Text lockedLabel;

	public CanvasGroup unlockWithAdCanvas;

	public string checkPointName;
	public int index;
	public float offset;
	public float delay;

	private bool isAvailable;

	public void Create (string name, int index, float o, float d) {
		this.checkPointName = name;
		this.index = index;
		this.offset = o;
		this.delay = d;

		GetComponent<RegionByAdUnlocker>().regionIndex = index;

		CatmullRomMovement m = GetComponent<CatmullRomMovement>();
		m.startDelay = d;
		m.startOffset = o;
		m.DelayedStart();
		regionName.text = checkPointName;
		regionIndex.text = index+"/15";

		if(ThemeManager.Instance.GetThemeByIndex(index-1).isAvailable) {
			if(PlayerPrefs.GetFloat("total_distance") <= ThemeManager.Instance.GetThemeByIndex(index-1).unlockDistance) {
				StartCoroutine("ShowAndHide");
				lockedLabel.GetComponent<Button>().enabled = false;
				lockedLabel.GetComponent<RegionByAdUnlocker>().enabled = true;
				lockedLabel.text = PlayerPrefs.GetFloat("total_distance").ToString("f2") + " / " + ThemeManager.Instance.GetThemeByIndex(index-1).unlockDistance + "m";
			}
			else {
				lockedLabel.text = "u n l o c k e d";
				lockedLabel.GetComponent<RegionByAdUnlocker>().enabled = false;
			}
		}
		else {
			lockedLabel.text = "Coming soon";
			lockedLabel.GetComponent<Button>().enabled = false;
			lockedLabel.GetComponent<RegionByAdUnlocker>().enabled = false;
		}

		selector.regions.Add(m);
	}

	IEnumerator ShowAndHide() {
		while(true) {
			for(float f = 0f; f < 1f; f += 0.025f) {
				lockedLabel.color = new Color(1,1,1,1 - f);
				unlockWithAdCanvas.alpha = f;
				yield return new WaitForEndOfFrame();
			}

			yield return new WaitForSeconds(1.5f);

			for(float f = 0f; f < 1f; f += 0.025f) {
				lockedLabel.color = new Color(1,1,1,f);
				unlockWithAdCanvas.alpha = 1 - f;
				yield return new WaitForEndOfFrame();
			}

			yield return new WaitForSeconds(1.5f);
		}
	}
}
