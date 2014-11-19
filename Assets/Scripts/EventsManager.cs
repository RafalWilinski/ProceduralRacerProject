using UnityEngine;
using System.Collections;

public class EventsManager : MonoBehaviour {

	public Transform vehicle;
	public Vector3 baseRezOffset;
	public Vector3 randomness;
	public GameObject risingPillarPrefab;
	public GameObject opponent;
	public float slowerSpeed;
	public float fasterSpeed;
	private Opponent op;


	void Start() {
		//RisingPillars(100);
		StartCoroutine("CreateOpponents");
	}
	
	public void StopAllEvents () {
	
	}

	private IEnumerator CreateOpponents() {
		while(true) {
			yield return new WaitForSeconds(2f);
			GameObject go = (GameObject) Instantiate(opponent, Vector3.zero, Quaternion.identity);
			op = go.GetComponent<Opponent>();
			if(Random.Range(0,1000) % 2 == 1) op.Create(vehicle.position + new Vector3(0, 1000, 0), slowerSpeed);
			else op.Create(vehicle.position + new Vector3(0, -250, 0), fasterSpeed);
			//yield return new WaitForSeconds(2f);
		}
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
