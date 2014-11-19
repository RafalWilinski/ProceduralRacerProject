using UnityEngine;
using System.Collections;

public class TiltPanel : MonoBehaviour {

	public float multiper;
	public float refreshRate;
	public float rotSpeed;
	public RectTransform tr;
	private Quaternion rotation;

	void Start () {
		StartCoroutine("Runtime");
	}

	IEnumerator Runtime() {
		while(true) {

			#if UNITY_EDITOR
				rotation = Quaternion.Euler( ((Input.mousePosition.y / Screen.height)-0.5f)*multiper,((Input.mousePosition.x / Screen.width)-0.5f)*multiper,0f);
			#endif

			tr.rotation = Quaternion.Lerp(tr.rotation,rotation,Time.deltaTime * rotSpeed);
			yield return new WaitForSeconds(refreshRate);
		}
	}
}
