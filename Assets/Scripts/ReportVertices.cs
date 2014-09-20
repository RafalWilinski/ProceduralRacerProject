using UnityEngine;
using System.Collections;

public class ReportVertices : MonoBehaviour {

	public RoadGenerator roadGen;

	void Start () {
		roadGen = (RoadGenerator)GameObject.Find("Road").GetComponent<RoadGenerator>();

		foreach(Transform t in transform) {
			roadGen.ReportVertex(t.position);
		}
	}
}
