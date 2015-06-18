using UnityEngine;
using System.Collections;

// NewBehaviourScript created by Rafal Wilinski

public class NewBehaviourScript : MonoBehaviour {

	private static NewBehaviourScript _instance;

	public static NewBehaviourScript Instance {
		get {
			return _instance;
		}
	}

	private void Start () {
		_instance = this;
	}

	private IEnumerator Coroutine() {
		yield return new WaitForSeconds(0.1f);
	}
}
