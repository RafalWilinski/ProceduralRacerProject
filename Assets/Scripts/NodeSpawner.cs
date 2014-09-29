using UnityEngine;
using System.Collections;

public class NodeSpawner : MonoBehaviour {
	public GameObject node;
	public bool shouldCreateNodesAutomatically;
	public float zStep;
	public float zOffset;
	public float furthestZPos;
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
		zIterator = 1;
		if(shouldCreateNodesAutomatically) StartCoroutine("CreateNodes");
	}

	IEnumerator CreateNodes() {
		LCG l = new LCG(22695477, 7, Random.Range(13457, 59267));
		while(true) {
			CreateNode();
			yield return new WaitForSeconds(sleep);
		}
	}

	public void CreateNode() {
		zIterator++;
		Vector3 pos = new Vector3(Random.Range(-80.2f, 80.21f), Random.Range(-0.5f, 0.5f), zStep * zIterator + zOffset);
		Instantiate(node, pos, Quaternion.identity);
		furthestZPos = pos.z;
	}
}
