using UnityEngine;
using System.Collections;

public class ContinousMovement : MonoBehaviour {

	public Transform camera;
	public float fwdSpeed;
	public float sensitivity;
	public float dir;
	public float rotationSpeed;
	private Quaternion target;
	private Vector3 vect;
	public float cameraRotSensitivityZ;

	void Update () {
			vect = new Vector3(sensitivity*dir*-1f, 0f ,fwdSpeed);
			transform.Translate(vect);
			//rigidbody.AddForce(sensitivity*dir*Vector3.left);

			target = Quaternion.Euler (0, dir, dir*cameraRotSensitivityZ);
			camera.localRotation = Quaternion.Lerp(camera.localRotation, target,Time.deltaTime*rotationSpeed);

			#if UNITY_EDITOR
				dir = (Input.mousePosition.x / Screen.width) - 0.5f;
			#endif

			/*
			if (Input.GetKey (KeyCode.LeftArrow)) dir = -1;
			else if (Input.GetKey (KeyCode.RightArrow)) dir = 1;
			else dir = 0;
			*/
	}
}
