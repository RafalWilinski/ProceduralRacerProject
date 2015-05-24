using UnityEngine;
using System.Collections;

public class PrintUID : MonoBehaviour {

	void Start () {
		Debug.Log(SystemInfo.deviceUniqueIdentifier);
		//6f87efb299a801de9f06eda541483436
	}
}
