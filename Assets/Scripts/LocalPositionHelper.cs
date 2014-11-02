using UnityEngine;
using System.Collections;

public class LocalPositionHelper : MonoBehaviour {

	public Vector3 pos;

	[ExecuteInEditMode]
	void Update () {
		pos = transform.position;
	}
}
