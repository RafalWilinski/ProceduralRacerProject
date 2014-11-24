using UnityEngine;
using System.Collections;

public class EventsManager : MonoBehaviour {

    public bool isDebug;
	public Transform vehicle;
	public Vector3 baseRezOffset;
	public Vector3 randomness;
	public GameObject risingPillarPrefab;
	public OpponentsPool opponentPool;
	public float slowerSpeed;
	public float fasterSpeed;

    private void Log(string msg) {
        if (isDebug) Debug.Log("Events: " + msg);
    }
	void Start() {
		RisingPillars(100);
		StartCoroutine("CreateOpponents");
	    StartCoroutine(waiting());
	}
	
	public void StopAllEvents () {
	
	}

	private IEnumerator CreateOpponents() {
		while(true) {
			yield return new WaitForSeconds(3f);
            Opponent op = opponentPool.GetFirstAvailable();
            if (op != null) {
                if (Random.Range(0, 1000) % 2 == 1) {
                    Log("Creating slower opponent!");
                    op.Create(vehicle.position + new Vector3(0, 0, 600), slowerSpeed);
                }
                else {
                    Log("Creating faster opponent!");
                    op.Create(vehicle.position + new Vector3(0, 0, -150), fasterSpeed);
                }
            }
            else {
                Log("Wanted to create Opponent but there was no ops available.");
            }
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

    IEnumerator waiting() {
        float i = Mathf.Pow(Random.value, Random.value);
        yield return new WaitForEndOfFrame();
        StartCoroutine(waiting());
    }
	
}
