using UnityEngine;
using System.Collections;

public class NodesRequester : MonoBehaviour {

	public float refreshRate;
	public float renderDistance;
	public NodeSpawner spawner;
	private Transform myTransform;

	void Start () {
		myTransform = transform;
		StartCoroutine("Refresh");
	}

	IEnumerator Refresh() {
		while(true) {
			if(myTransform.position.z + renderDistance > spawner.furthestZPos) {
				spawner.CreateNode();
			}
			yield return new WaitForSeconds(refreshRate);
		}
	}
}
