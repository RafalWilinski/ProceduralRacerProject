using UnityEngine;
using System.Collections;

public class CatmullRomNode : MonoBehaviour {

	private CatmullRomSpline root;
	private GameObject rootGo;
	private Transform myTransform;
	private Vector3 cachedPos;

	public bool isPush;
	public float timeGiven;

	void Start () {
		myTransform = transform;
		rootGo = GameObject.Find("Root");
		myTransform.parent = rootGo.transform;
		root = (CatmullRomSpline) rootGo.GetComponent<CatmullRomSpline>();
		if(isPush) root.PushNode(this.gameObject);
		else root.AddNode(this.gameObject);
		//StartCoroutine("positionCheck");
		StartCoroutine("Destroy");
	}

	IEnumerator positionCheck() {
		while(true) {
			cachedPos = myTransform.position;
			yield return new WaitForSeconds(0.1f); //Update interval set to 0.1 sec.
			if(cachedPos != myTransform.position) {
				root = rootGo.GetComponent<CatmullRomSpline>();
				root.AddNode(this.gameObject);
			}
		}
	}

	IEnumerator Destroy() {
		yield return new WaitForSeconds(1f);
		Destroy(this.gameObject);
	}
}
