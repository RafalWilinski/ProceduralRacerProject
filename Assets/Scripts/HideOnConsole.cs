using UnityEngine;
using System.Collections;

public class HideOnConsole : MonoBehaviour {

	public GameObject go;

	void Start () {
		if(go == null) go = this.gameObject;

		if(Input.GetJoystickNames().Length > 0)
			gameObject.SetActive(false);
	}
}
