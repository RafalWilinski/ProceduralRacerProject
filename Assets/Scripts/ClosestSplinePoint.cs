using UnityEngine;
using System.Collections;

public class ClosestSplinePoint : MonoBehaviour {

	public float x;
	public CatmullRomSpline spline;
	void Start () {
		//StartCoroutine("Compute");
	}

	IEnumerator Compute() {
		while(true) {
			x = spline.GetClosestPointAtSpline(transform.position);
			yield return new WaitForSeconds(1f);
		}
	}

	public void Simulate() {
		x = spline.GetClosestPointAtSpline(transform.position);
	}
}
