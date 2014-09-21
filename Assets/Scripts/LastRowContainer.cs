using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LastRowContainer : MonoBehaviour {

	public List<Vector3> vertexPositions;

	private static LastRowContainer _instance;

	void Start() {
		_instance = this;
		Refresh();
	}

	private void Refresh() {
		vertexPositions = new List<Vector3>();
	}

	public Vector3 VertexPosition(int pos) {
		if(pos == vertexPositions.Count-1) {
			Vector3 temp = vertexPositions[pos];
			Refresh();
			return temp;
		}
		else return vertexPositions[pos];
	}

	public static LastRowContainer Instance {
		get {
			return _instance;
		}
	}
}
