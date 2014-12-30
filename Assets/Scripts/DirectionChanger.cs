using UnityEngine;
using System.Collections;
using VacuumShaders;

public class DirectionChanger : MonoBehaviour {

	public float changesSpeed;
	public float changesFrequency;
	public float limit;
	public float tolerance;

	public bool isWorking;

	public VacuumShaders.CurvedWorld.CurvedWorld_GlobalController controller;

	private float targetX;
	private float targetY;
	private float targetZ;

	void Start () {
		StartCoroutine("Coroutine");
	}
	
	IEnumerator Coroutine() {
		while(true) {
			if(isWorking) {
				targetX = Random.Range(-limit, limit);
				targetY = Random.Range(-limit, limit);
				targetZ = Random.Range(-limit, limit);
				StartCoroutine(Lerp());
			}
			yield return new WaitForSeconds(changesFrequency);
		}
	}

	IEnumerator Lerp() {
		while( Mathf.Abs(controller._V_CW_X_Bend_Size_GLOBAL - targetX) >= tolerance) {
			controller._V_CW_X_Bend_Size_GLOBAL = Mathf.Lerp(controller._V_CW_X_Bend_Size_GLOBAL, targetX, Time.timeScale * changesSpeed);
			controller._V_CW_Y_Bend_Size_GLOBAL = Mathf.Lerp(controller._V_CW_Y_Bend_Size_GLOBAL, targetY, Time.timeScale * changesSpeed);
			//controller._V_CW_Z_Bend_Size_GLOBAL = Mathf.Lerp(controller._V_CW_Z_Bend_Size_GLOBAL, targetZ, Time.timeScale * changesSpeed);
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForEndOfFrame();
	}
}
