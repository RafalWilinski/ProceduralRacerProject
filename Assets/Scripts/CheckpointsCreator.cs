using UnityEngine;
using System.Collections;

public class CheckpointsCreator : MonoBehaviour {

	public GameObject refferenceCheckpoint;
	public string[] names;

	private int i;

	void Start () {
		i=1;
		refferenceCheckpoint.GetComponent<CheckpointContainer>().Create("Imperial Hallway", i, (float) ((i*1.0f)/2),  (float) ((i*1.0f)/5) );
		i++;
		foreach(string name in names) {
			GameObject copy = (GameObject) Instantiate(refferenceCheckpoint, Vector3.zero, Quaternion.identity);
			copy.transform.parent = transform;
			copy.GetComponent<CheckpointContainer>().Create(name, i, (float) ((i*1.0f)/2),  (float) ((i*1.0f)/5) );
			i++;
		}
	}
}
