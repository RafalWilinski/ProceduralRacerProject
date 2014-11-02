using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ContinousMovement : MonoBehaviour {

	public Transform camera;
	public Camera cam;
	public Transform[] uiPanelElements;
	public ThemeManager themesManager;
	public Text speedLabel;
	public Text distanceLabel;
	public Text regionLabel;
	public RectTransform marker;
	public RectTransform bottomUI;
	public RectTransform topUI;
	public Text newRegionName;
	public CatmullRomSpline spline;

	public int currentRegionIndex;

	public float fwdSpeed;
	public float menuFwdSpeed;
	public float directionSensitivity;
	public float dir;
	public float accel;
	public float calibration;
	public float rotationSpeed;
	public float cameraRotSensitivityX;
	public float cameraRotSensitivityY;
	public float cameraRotSensitivityZ;
	public float uiRotSensitivityX;
	public float uiRotSensitivityY;
	public float uiRotSensitivityZ;
	public float uiThreadSleep;

	public LoopMode loopMode;
	public bool shouldRotateUI;
	public bool shouldTweenFOV;
	public bool controlsEnabled;

	private Transform myTransform;
	private Quaternion cameraRotationTarget;
	private Quaternion uiRotationTarget;
	private Vector3 vect;
	private float _t;
	private float distance;
	private bool isChangingRegion;
	private float markerRegionWidth;
	private float hp;
	private float splineTimeLimit;

	[Serializable]
	public class Region {
		public string name;
		public float distance;
		public int themeIndex;
	}

	public enum LoopMode {
		ONCE, LOOP, PINGPONG
	}

	void OnEnable() {
		CatmullRomSpline.OnSplineUpdated += OnLimitChanged;
	}

	void OnLimitChanged(float f) {
		splineTimeLimit = f;
	} 

	void OnDisable() {
		CatmullRomSpline.OnSplineUpdated -= OnLimitChanged;
	}

	void Update () {

		if(controlsEnabled) {
			#if UNITY_EDITOR 
				dir = (Input.mousePosition.x / Screen.width) - 0.5f;
				accel = (Input.mousePosition.y / Screen.height) - 0.5f;
			#elif UNITY_IPHONE || UNITY_ANDROID
				dir = Mathf.Clamp(Input.acceleration.x,-0.5f, 0.5f);
				accel = Mathf.Atan2(Input.acceleration.y, Input.acceleration.z)*-1* Mathf.Rad2Deg;
				accel = Mathf.Abs(accel) - calibration;
				accel = Mathf.Clamp(accel,-1,1);
			#endif

			vect = new Vector3(directionSensitivity*dir*-1f, 0f ,fwdSpeed * (1+(accel/5f)) );
			transform.Translate(vect);

			cameraRotationTarget = Quaternion.Euler (accel * 5, dir, dir*cameraRotSensitivityZ);
			uiRotationTarget = Quaternion.Euler (accel * uiRotSensitivityX, dir*uiRotSensitivityY, dir*uiRotSensitivityZ);
			camera.localRotation = Quaternion.Lerp(camera.localRotation, cameraRotationTarget,Time.deltaTime*rotationSpeed);

			if(shouldRotateUI) {
				foreach(Transform t in uiPanelElements) {
					t.localRotation = Quaternion.Lerp(t.localRotation, uiRotationTarget, Time.deltaTime*rotationSpeed);
				}
			}

			if(shouldTweenFOV) cam.fov = Mathf.Lerp(cam.fov, 90 + (accel*10), Time.deltaTime*rotationSpeed);
		}
		else {
			if(spline.IsReady) {
				if(loopMode == LoopMode.ONCE) {
					_t += menuFwdSpeed;
				}
				else if(loopMode == LoopMode.LOOP) {
					if(_t >= splineTimeLimit) _t = 0f;
					else _t += menuFwdSpeed;
				}
				else if(loopMode == LoopMode.PINGPONG) {
					if(_t >= splineTimeLimit || _t <= 0f) menuFwdSpeed = -menuFwdSpeed;
					_t += menuFwdSpeed;
				}

				if(_t > splineTimeLimit) _t = splineTimeLimit;
				if(_t < 0) _t = 0f;

				myTransform.position = spline.GetPositionAtTime(_t);
				spline.GetRotAtTime(_t, this.gameObject);
			}
		}
	}

	void Start() {
		myTransform = transform;
		splineTimeLimit = spline.TimeLimit;
		markerRegionWidth = 400;
		cam = camera.GetComponent<Camera>();
		StartCoroutine(OnUIRender());
	}

	public void StartGame() {

	}

	void SetTheme(int index) {
		regionLabel.text = themesManager.themes[index].fullName;
		themesManager.LerpTo(index);
	}

	IEnumerator OnUIRender() {
		float speed = 0f;
		while(true) {
			speed = (fwdSpeed * (1 + (accel/5f))) * 10;
			distance += speed * uiThreadSleep;

			distanceLabel.text = distance.ToString("f0") + " / " + themesManager.themes[currentRegionIndex].distance.ToString("f0") + "mi remaining";
			speedLabel.text = "Speed: "+speed.ToString("f2");
			marker.anchoredPosition = new Vector2(-markerRegionWidth + (2*markerRegionWidth) * (distance/themesManager.themes[currentRegionIndex].distance), marker.anchoredPosition.y);

			if(!isChangingRegion && distance > themesManager.themes[currentRegionIndex].distance) {
				StartCoroutine(ChangeToNextRegion());
			}

			yield return new WaitForSeconds(uiThreadSleep);
		}
	}

	IEnumerator ChangeToNextRegion() {
		Debug.Log("ChangeToNextRegion");
		isChangingRegion = true;
		LeanTween.move( bottomUI, new Vector3(0, -300, 0), 2f ) .setEase( LeanTweenType.easeInQuad );
		LeanTween.move( topUI, new Vector3(0, 300, 0), 2f ) .setEase( LeanTweenType.easeInQuad );
		yield return new WaitForSeconds(1.25f);
		distance = 0;
		currentRegionIndex++;
		SetTheme(currentRegionIndex);
		yield return new WaitForSeconds(1.25f);
		LeanTween.move( topUI, new Vector3(0, 150, 0), 2f ) .setEase( LeanTweenType.easeInQuad );
		LeanTween.move( bottomUI, new Vector3(0, -150, 0), 2f ) .setEase( LeanTweenType.easeInOutQuad );
		isChangingRegion = false;
	}

	void OnCollisionStay(Collision col) {
		Debug.Log("OnCollisionStay with " + col.collider.name);
	}

	void OnCollisionEnter(Collision col) {
		Debug.Log("OnCollisionEnter with " + col.collider.name);
	}

	public void OnInvertControls() {
		directionSensitivity = -directionSensitivity;
	}

	public void OnInvertUIX() {
		uiRotSensitivityY = -uiRotSensitivityY;
		uiRotSensitivityZ = -uiRotSensitivityZ;
	}

	public void OnInvertCameraX() {
		cameraRotSensitivityZ = -cameraRotSensitivityZ;
	}

}
