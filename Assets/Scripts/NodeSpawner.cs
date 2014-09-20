using UnityEngine;
using System.Collections;

public class NodeSpawner : MonoBehaviour {

	public GameObject node;
	public float zStep;
	public float zOffset;
	private int zIterator;

	public float sleep;

	internal class LCG {
		internal int seed;

		internal float x;
		internal float a;
		internal float c;

		internal float GetNumber(int mod) {
			if(x == 0) {
				x = seed;
			}

			if(mod == 0 || mod == 1) { Debug.Log("Invalid modulo!"); return 0f; }

			x = ((a * x)+c) % mod;
			return x;
		}

		internal LCG(float aa, float cc, float s) { a = aa; c = cc; s = seed; }

	}

	void Start () {

		StartCoroutine("CreateNodes");
	}

	IEnumerator CreateNodes() {

		LCG l = new LCG(22695477, 7, Random.Range(13457, 59267));
		zIterator = 1;
		Vector3 pos;

		while(true) {
			pos = new Vector3(Random.Range(-3.2f, 3.21f), Random.Range(-3.2f, 3.21f), zStep * zIterator + zOffset);
			Debug.Log(pos);
			Instantiate(node, pos, Quaternion.identity);
			zIterator++;
			yield return new WaitForSeconds(sleep);
		}
	}
}
