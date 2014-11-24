using UnityEngine;
using System.Collections;

public class Opponent : MonoBehaviour {

	public CatmullRomSpline spline;
	private CatmullRomMovement mov;

	public void Create(Vector3 pos, float initSpeed) { 
		mov = this.gameObject.GetComponent<CatmullRomMovement>();
		spline = (CatmullRomSpline) GameObject.Find("Root").GetComponent<CatmullRomSpline>();
		mov.startOffset = spline.GetClosestPointAtSpline(pos);	
		mov.speed = initSpeed;
        Debug.Log("Spawning at position: " + mov.startOffset + ", with init speed: " + mov.speed);
		mov.DelayedStart();
	}
}
