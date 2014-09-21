using UnityEngine;
using System.Collections;

public class CatmullRomMovement : MonoBehaviour {

	public float speed;
	public float waitInterval;
	public CatmullRomSpline spline;
	public float _t;

	private float splineTimeLimit;
	private Transform myTransform;

	public enum LoopMode {
		ONCE, LOOP, PINGPONG
	}

	public LoopMode loopMode;

	void OnEnable() {
		CatmullRomSpline.OnSplineUpdated += OnLimitChanged;
	}

	void OnLimitChanged(float f) {
		splineTimeLimit = f;
	} 

	void Start() {
		myTransform = transform;
		splineTimeLimit = spline.TimeLimit;
		StartCoroutine("Movement");
	}

	IEnumerator Movement() {
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

//				Debug.Log("position at time: "+_t.ToString("f3") + " = "+spline.GetPositionAtTime(_t));
				myTransform.position = spline.GetPositionAtTime(_t);
				spline.GetRotAtTime(_t, this.gameObject);
			}
			yield return new WaitForSeconds(waitInterval);
		}
	}
}
