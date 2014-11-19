using UnityEngine;
using System.Collections;

public class DestroyOnStart : MonoBehaviour {
	void Start () {
		Destroy(this.gameObject);
	}
}
