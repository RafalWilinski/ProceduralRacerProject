using UnityEngine;
using System.Collections;

public class CheckpointsCreator : MonoBehaviour {

	public GameObject refferenceCheckpoint;
	public ThemeManager themeManager;
	//public string[] names;

	private int i;

	void Start () {
		i=1;
		refferenceCheckpoint.GetComponent<CheckpointContainer>().Create("Imperial Hallway", i, (float) ((i*1.0f)/2),  (float) ((i*1.0f)/5) );
		i++;
		foreach(ThemeManager.Theme t in themeManager.themes) {
			if(t != themeManager.themes[0]) {
				GameObject copy = (GameObject) Instantiate(refferenceCheckpoint, Vector3.zero, Quaternion.identity);
				copy.transform.parent = transform;
				copy.GetComponent<CheckpointContainer>().Create(t.fullName, i, (float) ((i*1.0f)/2),  (float) ((i*1.0f)/5) );
			}
			i++;
		}
	}
}
