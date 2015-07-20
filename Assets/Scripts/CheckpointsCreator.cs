using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckpointsCreator : MonoBehaviour {

	public GameObject refferenceCheckpoint;
	public ThemeManager themeManager;
	public List<GameObject> checkpoints;
	

	private int i;

	void Start () {
		i=1;
		refferenceCheckpoint.GetComponent<CheckpointContainer>().Create("Celestial Path", i, (float) ((i*1.0f)/2),  (float) ((i*1.0f)/5) );
		foreach(ThemeManager.Theme t in themeManager.themes) {
			if(t != themeManager.themes[0]) { 
				GameObject copy = (GameObject) Instantiate(refferenceCheckpoint, Vector3.zero, Quaternion.identity);
				copy.transform.parent = transform;
				copy.GetComponent<CheckpointContainer>().Create(t.fullName, i, (float) ((i*1.0f)/2.2f),  (float) ((i*1.0f)/4.2f) );
				copy.GetComponent<CanvasGroup>().interactable = false;
				copy.GetComponent<CanvasGroup>().blocksRaycasts = false;
				checkpoints.Add(copy);
			}
			i++;
		}
	}

	public void MakeInactive() {
		foreach(GameObject go in checkpoints) {
			go.GetComponent<CanvasGroup>().interactable = false;
			go.GetComponent<CanvasGroup>().blocksRaycasts = false;
		}
	}

	public void MakeActive() {
		foreach(GameObject go in checkpoints) {
			go.GetComponent<CanvasGroup>().interactable = true;
			go.GetComponent<CanvasGroup>().blocksRaycasts = true;
		}
	}
}
