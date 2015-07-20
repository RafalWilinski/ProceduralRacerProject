using UnityEngine;
using System.Collections;

public class OpenURL : MonoBehaviour {

	public string URL;
	void OnClick() {
		Application.OpenURL(URL);
	}
}
