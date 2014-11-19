using UnityEngine;
using System.Collections;

public class NodeSpawner : MonoBehaviour {
	public GameObject node;
	public bool shouldCreateNodesAutomatically;
	public float zStep;
	public float zOffset;
	public float furthestZPos;
	private int zIterator;
	public GameObject movingNode;
	private Vector3 pos;
	public float xVariety;

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
		//LCG l = new LCG(22695477, 7, Random.Range(13457, 59267));
		while(true) {
			CreateNode();
			yield return new WaitForSeconds(sleep);
		}
	}

	public void CreateNode() {
		zIterator++;
		pos = new Vector3(Random.Range(-xVariety, xVariety), Random.Range(-0.5f, 0.5f), zStep * zIterator + zOffset);
		if(movingNode == null) {
			movingNode = (GameObject) Instantiate(node, pos, Quaternion.identity);
		}
		movingNode.transform.position = pos;
		movingNode.SendMessage("AddNodeToSpline");
		furthestZPos = pos.z;
	}
}
