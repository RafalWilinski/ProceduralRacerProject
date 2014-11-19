using UnityEngine;
using System.Collections;

public class CatmullRomMovement : MonoBehaviour {

	public float speed;
	public float startOffset;
	public float waitInterval;
	public CatmullRomSpline spline;
	public float _t;
	public float startDelay;
	public bool shouldntStartAuto;
	public Vector3 targetOffset;
	public Vector3 offsetLimits;
	public Vector3 offset;
	public float changesFrequency;

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
		spline = (CatmullRomSpline) GameObject.Find("Root").GetComponent<CatmullRomSpline>();
		myTransform = transform;
		splineTimeLimit = spline.TimeLimit;
		if(!shouldntStartAuto) {
			StartCoroutine("ComputeOffset");
			StartCoroutine("LerpOffset");
			StartCoroutine("Movement");
		}
	}

	public void DelayedStart() {
		StartCoroutine("Movement");
		StartCoroutine("ComputeOffset");
		StartCoroutine("LerpOffset");
	}

	IEnumerator Movement() {
		yield return new WaitForSeconds(startDelay);
		_t += startOffset;
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
				myTransform.position = spline.GetPositionAtTime(_t) + offset;
				spline.GetRotAtTime(_t, this.gameObject);
			}
			yield return new WaitForSeconds(waitInterval);
		}
	}

	IEnumerator LerpOffset() {
		while(true) {
			offset = Vector3.Lerp(offset, targetOffset, Time.deltaTime * 0.5f);
			yield return new WaitForEndOfFrame();
		}
	}

	IEnumerator ComputeOffset() {
		while(true) {
			targetOffset = new Vector3(Random.Range(-offsetLimits.x,offsetLimits.x), Random.Range(-offsetLimits.y, offsetLimits.y), Random.Range(-offsetLimits.z, offsetLimits.z));
			yield return new WaitForSeconds(changesFrequency);
		}
	}
}





