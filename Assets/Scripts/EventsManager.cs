using UnityEngine;
using System.Collections;

public class EventsManager : MonoBehaviour {

	public Transform vehicle;
	public Vector3 baseRezOffset;
	public Vector3 randomness;
	public GameObject risingPillarPrefab;



	void Start() {
		RisingPillars(100);
	}
	
	public void StopAllEvents () {
	
	}

	public void RisingPillars(int count) {
		StartCoroutine("RisingPillarsCoroutine", count);
	}

	private IEnumerator RisingPillarsCoroutine(int count) {
		int i = 0;
		Vector3 position;
		while(i<count) {
			position = vehicle.position + baseRezOffset + new Vector3(Random.Range(-randomness.x, randomness.x), Random.Range(-randomness.y, randomness.y),Random.Range(-randomness.z, randomness.z));
			Instantiate(risingPillarPrefab, position, Quaternion.identity);
			i++;
			yield return new WaitForSeconds(1f);
		}
	}
	
}
