﻿using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

	public ContinousMovement vehicle;
	public InputType inputType;
	public float accel;
	public float dir;
	public float sideMovementClamp;
	public float sideMovementMultiplier = 2;
	public float calibration;

	public enum InputType {
		MousePosition,
		LeftRightArrows,
		SideTapping,
		Accelerometer,
		Joysticks
	}

	private void Awake() {
		GetDefaultInputMethod();
	}

	private void GetDefaultInputMethod() {
		#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBPLAYER

		inputType = InputType.MousePosition;

		#elif UNITY_IPHONE || UNITY_ANDROID

		inputType = InputType.Accelerometer;

		#endif
	}

	public void SetInputMethodToTilt() {
		inputType = InputType.Accelerometer;
	}

	public void SetInputMethodToSideTapping() {
		inputType = InputType.SideTapping;
	}

	private void FixedUpdate() {
		switch(inputType) {
			case(InputType.MousePosition): 
				dir = (Input.mousePosition.x / Screen.width) - 0.5f;
				if(Input.GetMouseButtonDown(0)) {
					Debug.Log("MouseDown!");
					vehicle.TriggerSpeedUp();
				}
				else if(Input.GetMouseButtonUp(0)) {
					Debug.Log("MouseUp!");
					vehicle.TriggerSlowDown();
				}
				break;

			case(InputType.LeftRightArrows):
				if(Input.GetKey(KeyCode.LeftArrow)) dir = -sideMovementClamp;
				else if(Input.GetKey(KeyCode.RightArrow)) dir = sideMovementClamp;
				break;

			case(InputType.SideTapping):
				if(Input.GetMouseButton(0))
					dir = Mathf.Clamp(((Input.mousePosition.x / Screen.width) - 0.5f) * 10000, -0.3f, 0.3f);
				else dir = 0;
				break;

			case(InputType.Accelerometer):
				dir = Mathf.Clamp(Input.acceleration.x, -sideMovementClamp, sideMovementClamp) * vehicle.controlMultiplier;
				break;

			case(InputType.Joysticks):
				dir = Input.GetAxis("Horizontal") / 2;
				break;
		}
		dir *= sideMovementMultiplier;
	}
}
