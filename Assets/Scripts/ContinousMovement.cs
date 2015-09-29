using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Text;
using Thinksquirrel.Utilities;

public class ContinousMovement : MonoBehaviour {

	public InputManager inputManager;
	public Transform cameraTransform;
	public Camera cam;
	public Transform[] uiPanelElements;
	public ThemeManager themesManager;
	public Text pointsLabel;
	public Text speedLabel;
	public Text distanceLabel;
	public Text regionLabel;
	public Text cubesCollectedLabel;
	public RectTransform marker;
	public RectTransform bottomUI;
	public RectTransform topUI;
	public Text newRegionName;
	public Slider playerHealthBar;
	public Text playerHealthText;
	public Image rewindCircle;
	public Text rewindTimerLabel;
	public Text gameOverScoreLabel;
	public Text gameOverScoreTypeLabel;
	public Text gameOverRegionNameLabel;
	public Image gameOverCubeImage;
	public CanvasGroup streakPanel;
	public Text streakCountLabel;
	public Image streakGauge;
	public Text chargePercentageLabel;
	public Image chargeLineRight;
	public CanvasGroup leftGradient;
	public CanvasGroup rightGradient;
	public CatmullRomSpline spline;
	public CanvasManager canvasManager;
	public PanelsManager panelsManager;
	public EventsManager eventsManager;
	public Slider textureQualitySlider;
	public ParticleSystem particleFlakes;
	public Tweener cinematicDown;
	public Tweener cinematicTop;
	public Tweener cinematicRegionNameText;
	public CameraFilterPack_TV_VHS_Rewind rewindCameraEffect;
	public CameraFilterPack_Color_Chromatic_Aberration aberrationCameraEffect;
	public CameraFilterPack_Blur_Radial_Fast radialHighSpeedBlur;
	public CameraShake UICameraShake;
	public GameOverCloudAnimation gameOverCloud;
	public GameOverScenario gameOverScenario;
	public ParticleSystem particlePoints;


	public int CubesCollected {
		set {
			cubesCollected = value;
			cubesCollectedLabel.text = " x "+cubesCollected;
		}
		get {
			return cubesCollected;
		}
	}

	public int score;
	public int currentRegionIndex;

	public float playerHealth = 100f;
	public float fwdSpeed;
	public float targetFwdSpeed;
	public float menuFwdSpeed;
	public float directionSensitivity;
	public float dir;
	public float accel;
	public float rotationSpeed;
	public float cameraRotSensitivityX;
	public float cameraRotSensitivityY;
	public float cameraRotSensitivityZ;
	public float uiRotSensitivityX;
	public float uiRotSensitivityY;
	public float uiRotSensitivityZ;
	public float uiThreadSleep;
	public float glitchTime;
	public float _t;
	public float forceAffectorMultiplier;
	public float rewindTotalTime;
	public float controlMultiplier = 1;
	public float minChargeDistance;
	public float nearMissChargeCap;

	public LoopMode loopMode;
	private bool nearMissCooldown;
	public bool shouldRotateUI;
	public bool shouldTweenFOV;
	public bool controlsEnabled;
	public bool isPlaying;
	public bool isPreparing;
	public bool isPaused;
	public bool isGameOver;

	public AudioClip pointsCubeHit;


	private Transform myTransform;

	public Transform MyTransform {
		get { return myTransform; }
	}

	private Quaternion cameraRotationTarget;
	private Quaternion uiRotationTarget;
	private Vector3 vect;
	private Vector3 forceAffector;
	private int startingTheme;
	private int cubesCollected;
	private int lastRunDistance;
	private int cubesCollectedStreak;

	private bool isChangingRegion;
	private bool rewindPanelShown = false;
	private bool isSpeedingUp;

	private float distance;
	private float totalDistance;
	private float markerRegionWidth;
	private float hp;
	private float playerOldHealth;
	private float splineTimeLimit;
	private float multiplierGauge;
	private float nearMissCharge;
	private float highSpeedCameraAffector;
	private float highSpeedChargeStartAmount;


	private StringBuilder stringBuilder;

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
		stringBuilder = new StringBuilder();
		CatmullRomSpline.OnSplineUpdated += OnLimitChanged;
	}

	void OnLimitChanged(float f) {
		splineTimeLimit = f;
	} 

	void OnDisable() {
		CatmullRomSpline.OnSplineUpdated -= OnLimitChanged;
	}

	public void OnAnisoChange() {
		if(QualitySettings.anisotropicFiltering == AnisotropicFiltering.Disable) QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
		else QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
	}

	void FixedUpdate () {
		if(!isPaused) {
			if(controlsEnabled) {

				dir = inputManager.dir;

				forceAffector = new Vector3(forceAffector.x, 0, forceAffector.z/100);
				forceAffector = Vector3.ClampMagnitude(forceAffector, forceAffectorMultiplier * 10);
				vect = new Vector3(directionSensitivity*dir*-1f, 0, fwdSpeed * (1+(accel)) );
				vect += forceAffector * (1 - controlMultiplier);

				if(!isGameOver) transform.Translate(vect);

				myTransform.position = new Vector3(myTransform.position.x, spline.GetPositionAtTime(spline.GetClosestPointAtSpline(myTransform.position, 20)).y + 2, myTransform.position.z);

				Vector3 nearFuturePos = spline.GetPositionAtTime(spline.GetClosestPointAtSpline(myTransform.position, 20) + 0.1f);
				float cameraXAngle = myTransform.position.y - nearFuturePos.y;

				cameraRotationTarget = Quaternion.Euler (cameraXAngle * 4 + highSpeedCameraAffector, dir * 10, dir*cameraRotSensitivityZ);
				uiRotationTarget = Quaternion.Euler (accel * uiRotSensitivityX, dir*uiRotSensitivityY, dir*uiRotSensitivityZ);
				cameraTransform.localRotation = Quaternion.Lerp(cameraTransform.localRotation, cameraRotationTarget, Time.deltaTime*rotationSpeed);

				playerHealthBar.value = playerHealth / 100f;

				if(playerHealth != playerOldHealth)
					playerHealthText.text = "Health: " + playerHealth.ToString("f2") + "%";
				playerOldHealth = playerHealth;

				if(shouldRotateUI) {
					foreach(Transform t in uiPanelElements) {
						t.localRotation = Quaternion.Lerp(t.localRotation, uiRotationTarget, Time.deltaTime*rotationSpeed);
					}
				}

				if(shouldTweenFOV) cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 90 + (accel*75), Time.deltaTime*rotationSpeed);


				Vector3 left = transform.TransformDirection(Vector3.left);
				Vector3 right = transform.TransformDirection(Vector3.right);
				RaycastHit hit;

		        if (Physics.Raycast(transform.position, left, out hit, minChargeDistance)) {
		        	nearMissCharge += 0.1f;
		        	leftGradient.alpha += 0.1f;

		        	if(hit.collider.gameObject.tag == "Obstacle" && !nearMissCooldown) {
			        	score += 200;
						ScoreBonusManager.Instance.AddScore(200, "Near Miss");
						StartCoroutine("NearMissCooldown");
					}

		        }

		        if (Physics.Raycast(transform.position, right, out hit, minChargeDistance)) {
		        	nearMissCharge += 0.1f;
		        	rightGradient.alpha += 0.1f;

		        	if(hit.collider.gameObject.tag == "Obstacle" && !nearMissCooldown) {
			        	score += 200;
						ScoreBonusManager.Instance.AddScore(200, "Near Miss");
						StartCoroutine("NearMissCooldown");
					}
		        }

		        if(nearMissCharge >= nearMissChargeCap) nearMissCharge = nearMissChargeCap;
		        chargeLineRight.fillAmount = nearMissCharge / nearMissChargeCap;
		        chargePercentageLabel.text = ((nearMissCharge / nearMissChargeCap)*100).ToString("f1") + "%";

		        if(nearMissCharge < 1f) {
		        	TriggerSlowDown();
		        }


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

                	myTransform.position = Vector3.Lerp(myTransform.position, spline.GetPositionAtTime(_t), Time.deltaTime * 5);
                	//myTransform.position = spline.GetPositionAtTime(_t);
					spline.GetRotAtTime(_t + 0.5f, this.gameObject);
					
				}
			}
		}
	}

	private IEnumerator NearMissCooldown() {
		nearMissCooldown = true;
		yield return new WaitForSeconds(0.15f);
		nearMissCooldown = false;
	}

	public void TriggerSpeedUp() {
		if(nearMissCharge > 5.0f && !isGameOver) {
			StopCoroutine("SpeedUp");
			StartCoroutine("SpeedUp");
		}
	}

	public void TriggerSlowDown() {
		StartCoroutine("SlowDown");
	}

	private IEnumerator SpeedUp() {
		highSpeedChargeStartAmount = nearMissCharge;

		radialHighSpeedBlur.enabled = true;
		StopCoroutine("SlowDown");
		isSpeedingUp = true;
		this.GetComponent<AudioSource>().Play();


		while(accel < 0.35f) {
			// radialHighSpeedBlur.Intensity = accel / 10;
			highSpeedCameraAffector += 1.5f;
			accel += 0.025f;
			nearMissCharge -= 0.3f;
			this.GetComponent<AudioSource>().volume = accel;
			this.GetComponent<AudioSource>().pitch = 0.4f + accel;
			yield return new WaitForSeconds(0.02f);
		}
		while(true) {
			// radialHighSpeedBlur.Intensity = accel / 10;
			nearMissCharge -= 0.3f;
			yield return new WaitForSeconds(0.05f);
		}
	}

	private IEnumerator SlowDown() {
		if(isSpeedingUp) {
			score += (int) ((highSpeedChargeStartAmount - nearMissCharge) * 10);
			ScoreBonusManager.Instance.AddScore( (int)((highSpeedChargeStartAmount - nearMissCharge) * 10), "High Speed!");
		}
		isSpeedingUp = false;
		StopCoroutine("SpeedUp");
		while(accel > 0) {
			accel -= 0.025f;
			highSpeedCameraAffector -= 1.5f;
			this.GetComponent<AudioSource>().pitch = 0.4f + accel;
			this.GetComponent<AudioSource>().volume = accel;
			// radialHighSpeedBlur.Intensity = accel / 10;
			yield return new WaitForSeconds(0.02f);
		}
		radialHighSpeedBlur.enabled = false;
		this.GetComponent<AudioSource>().Stop();
	}

	void Start() {
		CubesCollected = 0;
		particleFlakes.Stop();
		particleFlakes.Clear();
		spline.xAxisDivider = 2.5f;
		myTransform = transform;
		splineTimeLimit = spline.TimeLimit;
		markerRegionWidth = 375f;
		StartCoroutine(OnUIRender());

		StartCoroutine("CheckController");
		StartCoroutine("FadeGradients");
	}

	private IEnumerator FadeGradients() {
		while(true) {
			yield return new WaitForSeconds(0.05f);
			leftGradient.alpha -= 0.05f;
			rightGradient.alpha -= 0.05f;
		}
	}

	public void StartGame() {
		cubesCollected = 0;
		if(!isPreparing) {
			score = 0;
			distance = 0;
			totalDistance = 0;
			Physics.IgnoreLayerCollision(0, 9, false);
			isPreparing = true;

			if(Input.GetJoystickNames().Length > 0) {
				fwdSpeed = 150;
			}

			fwdSpeed = targetFwdSpeed / 10f;
			StartCoroutine("straightenMovement"); 
			StartCoroutine("reCurveSpline");
			//canvasManager.StartGame();
			panelsManager.StartGame();
			Debug.Log("Starting game!");
			currentRegionIndex = themesManager.fakeCurrentThemeIndex;
			themesManager.currentThemeIndex = themesManager.fakeCurrentThemeIndex;
			regionLabel.text = themesManager.themes[themesManager.fakeCurrentThemeIndex].fullName;
			startingTheme = themesManager.currentThemeIndex;

			SoundEngine.Instance.StartSoundtrack(ThemeManager.Instance.currentThemeIndex);
		}
	}

	private IEnumerator CheckController() {
		yield return new WaitForSeconds(2f); 
		if(Input.GetJoystickNames().Length > 0) {
			Debug.Log("Controller found!");
			ControllerConnectedModal.Instance.Show();
		}
	}

	private IEnumerator reCurveSpline() {
		//spline.xAxisDivider = 1f;
		while(spline.xAxisDivider > 1) {
			spline.xAxisDivider -= 0.02f;
			yield return new WaitForEndOfFrame();
		}
	}

	private IEnumerator straightenMovement() {
		yield return new WaitForSeconds(0.1f);
		controlsEnabled = true;
		while(transform.rotation != Quaternion.identity) {
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime*5);
			yield return new WaitForSeconds(0.01f);
		}
		yield return new WaitForSeconds(0.1f);
		isPlaying = true;
		StartCoroutine("increaseFwdSpeed");
		particleFlakes.Play();
		isPreparing = false;
	}

	private IEnumerator increaseFwdSpeed() {
		fwdSpeed = targetFwdSpeed / 10f;
		while(fwdSpeed < targetFwdSpeed) {
			fwdSpeed += 0.05f;
			yield return new WaitForSeconds(0.01f);
		}
		StopCoroutine("straightenMovement");
		StopCoroutine("reCurveSpline");
	}

	void SetTheme(int index) {
		regionLabel.text = themesManager.themes[index].fullName;
		themesManager.LerpTo(index);
	}

	IEnumerator OnUIRender() {
		float speed = 0f;
		while(true) {
			if(isPlaying && !isGameOver) {
				speed = (fwdSpeed * (1 + accel)) * 10; 
				distance += speed * uiThreadSleep;

				pointsLabel.text = score.ToString() + "pts";
				distanceLabel.text = distance.ToString("f0") + " / " + themesManager.themes[currentRegionIndex].distance.ToString("f0") + "mi remaining";
				speedLabel.text = "Speed: "+speed.ToString("f2");
				marker.anchoredPosition = new Vector2(-markerRegionWidth + (2*markerRegionWidth) * (distance/themesManager.themes[currentRegionIndex].distance), marker.anchoredPosition.y);

				if(!isChangingRegion && distance > themesManager.themes[currentRegionIndex].distance) {
					StartCoroutine(ChangeToNextRegion());
				}
			}
			yield return new WaitForSeconds(uiThreadSleep);
		}
	}

	IEnumerator ChangeToNextRegion() {
		Debug.Log("ChangeToNextRegion");
		isChangingRegion = true;
		UICameraShake.CancelShake();
		UICameraShake.enabled = false;
		LeanTween.move( bottomUI, new Vector3(0, -60, 0), 2f ) .setEase( LeanTweenType.easeInQuad );
		LeanTween.move( topUI, new Vector3(0, 60, 0), 2f ) .setEase( LeanTweenType.easeInQuad );
		yield return new WaitForSeconds(1.25f);
		totalDistance += distance;
		gameOverScenario.AddEvent(themesManager.themes[currentRegionIndex+1].fullName, (int) totalDistance);
		distance = 0;
		currentRegionIndex++;
		SetTheme(currentRegionIndex);
		cinematicTop.StartTween();
		cinematicDown.StartTween();
		cinematicRegionNameText.StartTween();
		cinematicRegionNameText.gameObject.GetComponent<Text>().text = themesManager.themes[currentRegionIndex].fullName;
		yield return new WaitForSeconds(4f);
		LeanTween.move( topUI, new Vector3(0, -37, 0), 2f ) .setEase( LeanTweenType.easeInQuad );
		LeanTween.move( bottomUI, new Vector3(0, 29, 0), 2f ) .setEase( LeanTweenType.easeInOutQuad );
		isChangingRegion = false;
		UICameraShake.enabled = true;
	}

	public void OnCollision(Vector3 intersectingVector) {
		forceAffector = (spline.GetPositionAtTime(spline.GetClosestPointAtSpline(myTransform.position) + 0.2f) - myTransform.position) * forceAffectorMultiplier;

		Stun();
		CameraShake.ShakeAll();
		SoundEngine.Instance.MakeHit();
		StartCoroutine("GlitchEnumerator");

		playerHealth -= intersectingVector.magnitude * 2;
		

		if(playerHealth <= 0f) {
			RewindCountdown();
		}
		else if(playerHealth <= 10f) {
			SoundEngine.Instance.MakeHeartbeat();
		}
	}

	public void OnTriggerEnter(Collider col) {
		Debug.Log("OnTriggerEnter!");
		if(!isGameOver) {
			if(col.gameObject.tag == "CubePoints") {
				particlePoints.Emit(200);
				CameraShake.ShakeAll(CameraShake.ShakeType.CameraMatrix, 3, new Vector3(1,1,1), new Vector3(3,3,3), 0.2f, 70, 0.4f, 1f, true);
				CubesCollected = CubesCollected + 1;
				multiplierGauge += 0.2f;
				cubesCollectedStreak++;
				streakCountLabel.text = " x " + cubesCollectedStreak;
				streakGauge.fillAmount = multiplierGauge;
				StartCoroutine("GaugeCooldown");
				SoundEngine.Instance.CreateSound(pointsCubeHit);
				score += 100;
				ScoreBonusManager.Instance.AddScore(100, "Cube caught");

			}
			else {
				CameraShake.ShakeAll();
				SoundEngine.Instance.MakeHit();
				StartCoroutine("GlitchEnumerator");
			}
		}
	}

	private IEnumerator GaugeCooldown() {
		while(multiplierGauge > 0) {
			streakPanel.alpha = multiplierGauge * 10f;
			multiplierGauge -= 0.001f;
			streakGauge.fillAmount = multiplierGauge;
			yield return new WaitForSeconds(0.01f);
		}

		cubesCollectedStreak = 0;
	}

	private void RewindCountdown() {
		if(!rewindPanelShown) {
			StopCoroutine("CountdownCoroutine");
			StartCoroutine("CountdownCoroutine");
		}
		else {
			GameOver();
		}
	}

	private IEnumerator CountdownCoroutine() {
		SoundEngine.Instance.ChangeSoundtrackPitch(0.5f);

		SoundEngine.Instance.MakeHeartbeat();
		rewindPanelShown = true;
		panelsManager.ShowRewindPanel();
		_t = spline.GetClosestPointAtSpline(myTransform.position) + 0.05f;	
		// cam.GetComponent<NoiseEffect>().enabled = true;
		Time.timeScale = 0.05f;
		float startTime = Time.realtimeSinceStartup;
		while(startTime + rewindTotalTime > Time.realtimeSinceStartup) {
			rewindTimerLabel.text = (startTime + rewindTotalTime - Time.realtimeSinceStartup).ToString("f2").Replace('.', ':');
			rewindCircle.fillAmount = 1 - ((startTime + rewindTotalTime - Time.realtimeSinceStartup) / rewindTotalTime);
			yield return new WaitForEndOfFrame();
		}
		// cam.GetComponent<NoiseEffect>().enabled = false;
		SoundEngine.Instance.MusicFadeOut();
		GameOver();
	}

	IEnumerator GlitchEnumerator() {
		if(GraphicsSettingsManager.Instance.IsGlitchAllowed()) {
			aberrationCameraEffect.enabled = true;
			yield return new WaitForSeconds(glitchTime);
			aberrationCameraEffect.enabled = false;
		}
	}

	public void RewindEffectShort() {
		StopCoroutine("RewindEnumerator");
		StartCoroutine("RewindEnumerator");
	}

	public void AberrationEffectShort() {
		StopCoroutine("AberrationEnumerator");
		StartCoroutine("AberrationEnumerator");
	}

	IEnumerator RewindEnumerator() {
		if(GraphicsSettingsManager.Instance.IsGlitchAllowed()) {
			rewindCameraEffect.enabled = true;
			yield return new WaitForSeconds(1.0f);
			rewindCameraEffect.enabled = false;
		}
	}

	IEnumerator AberrationEnumerator() {
		if(GraphicsSettingsManager.Instance.IsGlitchAllowed()) {
			aberrationCameraEffect.Offset = 0.005f;
			aberrationCameraEffect.enabled = true;
			yield return new WaitForSeconds(1.0f);
			aberrationCameraEffect.enabled = false;
			aberrationCameraEffect.Offset = 0.0028f;
		}
	}

	private void Stun() {
		StopCoroutine("StunDiminishingReturns");
		StartCoroutine("StunDiminishingReturns");
	}

	IEnumerator StunDiminishingReturns() {
		controlMultiplier = 0;
		for(controlMultiplier = 0; controlMultiplier <= 1f; controlMultiplier += 0.1f) {
			yield return new WaitForSeconds(0.01f);
		}
	}

	public void GameOver() {
		if(!isGameOver) {
			TriggerSlowDown();
			particleFlakes.Stop();
			Physics.IgnoreLayerCollision(0, 9, true);
			gameOverScoreLabel.text = (totalDistance + distance).ToString("N");
			gameOverRegionNameLabel.text = "in "+themesManager.GetCurrentTheme().fullName;
			gameOverScoreTypeLabel.text = "Distance";
			StopCoroutine("ScoreSwitching");
			StartCoroutine("ScoreSwitching");

			Debug.Log("Closest point: "+spline.GetClosestPointAtSpline(myTransform.position));
			_t = spline.GetClosestPointAtSpline(myTransform.position) + 0.05f;	
			controlsEnabled = false;
			Time.timeScale = 1;
			gameOverScenario.DistanceTravelled = (int) (totalDistance + distance);
			panelsManager.ShowGameOverPanel();
			lastRunDistance = (int) (totalDistance + distance);
			gameOverCloud.Animate();
			PlayerPrefs.SetInt("power_cubes_collected", PlayerPrefs.GetInt("power_cubes_collected") + cubesCollected);

			PreferencesManager.Instance.Increment("total_distance", totalDistance + distance);
		}
		isGameOver = true;
	}

	IEnumerator ScoreSwitching() {

		while(true) {

			yield return new WaitForSeconds(1.5f);

			for(float i = 0f; i < 1f; i += 0.03f) {
				gameOverScoreLabel.color = new Color(1,1,1,1 - i);
				yield return new WaitForEndOfFrame();
			}

			gameOverScoreTypeLabel.text = "Cubes collected";
			gameOverScoreLabel.text = cubesCollected.ToString();

			for(float i = 0f; i < 1f; i += 0.03f) {
				gameOverScoreLabel.color = new Color(1,1,1,i);
				gameOverCubeImage.color = new Color(1,1,1,i);
				yield return new WaitForEndOfFrame();
			}

			yield return new WaitForSeconds(1.5f);

			for(float i = 0f; i < 1f; i += 0.03f) {
				gameOverScoreLabel.color = new Color(1,1,1,1 - i);
				gameOverCubeImage.color = new Color(1,1,1,1 - i);
				yield return new WaitForEndOfFrame();
			}

			gameOverScoreTypeLabel.text = "Distance";
			gameOverScoreLabel.text = (totalDistance + distance).ToString("N");

			for(float i = 0f; i < 1f; i += 0.03f) {
				gameOverScoreLabel.color = new Color(1,1,1,i);
				yield return new WaitForEndOfFrame();
			}
		}
	}

	public void RestartGame() {
		SoundEngine.Instance.ChangeSoundtrackPitch(1f);
		Debug.Log("Restaring game!");
		rewindPanelShown = false;
		playerHealth = 100f;
		isGameOver = false;
		themesManager.LerpTo(startingTheme);
		StartGame();
		panelsManager.HideGameOverPanel();
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
