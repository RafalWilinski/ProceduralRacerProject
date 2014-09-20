using UnityEngine;
using System.Collections;

public class CatmullRomMovement : MonoBehaviour {

	public float speed;
	public float waitInterval;
	public CatmullRomSpline spline;

	private float splineTimeLimit;
	private Transform myTransform;

	public enum LoopMode {
		ONCE, LOOP, PINGPONG
	}

	public LoopMode loopMode;

	void Start() {
		myTransform = transform;
		splineTimeLimit = spline.TimeLimit;
		StartCoroutine("Movement");
	}

	void OnEnable() {
		CatmullRomSpline.OnSplineUpdated += changeSplineLimit;
	}

	void changeSplineLimit(float limit) {
		splineTimeLimit = limit;
	}

	IEnumerator Movement() {
		float _t = 0f;

		while(true) {
			if(spline.IsReady) {
				if(loopMode == LoopMode.ONCE) {
					_t += speed;
				}
				else if(loopMode == LoopMode.LOOP) {
					if(_t >= splineTimeLimit) _t = 0f;
					else _t += speed;
				}
				else if(loopMode == LoopMode.PINGPONG) {
					if(_t >= splineTimeLimit || _t <= 0f) speed = -speed;
					_t += speed;
				}

				if(_t > splineTimeLimit) _t = splineTimeLimit;
				if(_t < 0) _t = 0f;

				myTransform.position = spline.GetPositionAtTime(_t);
				spline.GetRotAtTime(_t, this.gameObject);
			}
			yield return new WaitForSeconds(waitInterval);
		}
	}
}
