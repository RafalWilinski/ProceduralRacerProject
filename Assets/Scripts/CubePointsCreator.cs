using UnityEngine;
using System.Collections;

public class CubePointsCreator : MonoBehaviour {

	public int length;
	public GameObject prefab;
	public Vector3 offsetIteration;

	private Transform myTransform;

	void Start () {
		myTransform = transform;
		for(int i = 0; i < length; i++) {
			GameObject go = Instantiate(prefab, offsetIteration * i, Quaternion.identity) as GameObject;
			go.transform.parent = myTransform;
		}
		myTransform.position = new Vector3(0, -500, 0);
	}

	public void Replace() {
		float start = CatmullRomSpline.Instance.GetClosestPointAtSpline(myTransform.position, 20);
		
		foreach(Transform t in myTransform) {
			t.position = CatmullRomSpline.Instance.GetPositionAtTime(start);
			start += 0.25f;
		}
	}
}
