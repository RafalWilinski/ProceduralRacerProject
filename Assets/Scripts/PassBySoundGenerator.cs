using UnityEngine;
using System.Collections;

public class PassBySoundGenerator : MonoBehaviour {

	public ContinousMovement continousMovement;
	public float cooldownTime;
	private bool cooldown_l;
	private bool cooldown_r;

	void Update () {
		if(!continousMovement.isGameOver) {
			Vector3 left = transform.TransformDirection(Vector3.left);
			Vector3 right = transform.TransformDirection(Vector3.right);
			RaycastHit hit;
	        if (Physics.Raycast(transform.position, left, out hit, 120)) {
	        	if(!cooldown_l) {
	        		SoundEngine.Instance.MakeSwoosh(hit.distance, hit.point - transform.position);
	        		// Debug.Log("Distance: "+hit.distance);
	        		StartCoroutine("L_CooldownCoroutine");
	        	}
	        }

	        if (Physics.Raycast(transform.position, right, out hit, 120)) {
	        	if(!cooldown_r) {
	        		SoundEngine.Instance.MakeSwoosh(hit.distance, hit.point - transform.position);
	        		// Debug.Log("Distance: "+hit.distance);
	        		StartCoroutine("R_CooldownCoroutine");
	        	}
	        }
	    }
	}

	private IEnumerator L_CooldownCoroutine() {
		cooldown_l = true;
		yield return new WaitForSeconds(cooldownTime * Random.Range(0.7f, 1.2f));
		cooldown_l = false;
	}

	private IEnumerator R_CooldownCoroutine() {
		cooldown_r = true;
		yield return new WaitForSeconds(cooldownTime * Random.Range(0.7f, 1.2f));
		cooldown_r = false;
	}
}
