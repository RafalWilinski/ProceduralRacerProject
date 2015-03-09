using UnityEngine;
using System.Collections;

public class PrintUID : MonoBehaviour {

	void Start () {
		Debug.Log(SystemInfo.deviceUniqueIdentifier);
	}
}
