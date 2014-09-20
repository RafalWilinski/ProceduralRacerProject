using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MeshGenerator : MonoBehaviour {

	public bool IsDebug;
	public GameObject debugCube;
	public int columns;
	public int rows;
	public float x_spacing;
	public float y_spacing;

	public List<Stack> stacksOfVertexes;


	private int counter;

	void Start () {
		Generate();
	}

	[Serializable]
	internal class Vertex {
		public Vector3 position;
		public int index;

		public Vertex(int i, Vector3 p) {
			index = i;
			position = p;
		}
	}
		
	void Generate() {
		for(int j = 0; j<rows; j++) {
			for(int i = 0; i<columns; i++) {
				int howManyVertexes = GetSplitCount(i, j);
				Stack vertexStack = new Stack();
				Log("Putting "+howManyVertexes+" verts at pos x = "+i+", y = "+j);
				for(int p = 0; p < howManyVertexes; p++) {
					if(IsDebug) Instantiate(debugCube, new Vector3(i * x_spacing, j * y_spacing, 0f), Quaternion.identity);

					vertexStack.Push(new Vertex(counter, new Vector3(i * x_spacing, j * y_spacing, 0f)));
					counter++;
				}
			}
		}

		GenerateTriangles();
	}

	void GenerateTriangles() {
		
	}


	#region Helpers

	private void Log(string msg) {
		Debug.Log("MeshGen: "+msg);
	}

	private int GetSplitCount(int col, int row) {
		if(row == 0) {
			if(col == 0) return 2;
			else if(col == columns - 1) return 1;
			else return 3;

		}
		else if(row == rows-1) {
				if(col == 0) return 1;
				else if(col == columns - 1) return 2;
				else return 3;
		}
		else {
			if(col == 0) return 3;
			else if(col == columns - 1) return 3;
			else return 6;
		}
	}

	#endregion


}
