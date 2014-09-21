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
	public AnimationCurve profileCurve;
	public float evaluationStep;
	public float randomness;
	public float z_depth;

	public List<Stack> stacksOfVertexes;
	public List<Vector3> vertices;
	public List<int> triangles;

	private int counter;
	private float startTime;

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
		startTime = Time.realtimeSinceStartup;
		stacksOfVertexes = new List<Stack>();
		for(int j = 0; j<rows; j++) {
			for(int i = 0; i<columns; i++) {
				int howManyVertexes = GetSplitCount(i, j);
				Stack vertexStack = new Stack();
				Log("Putting "+howManyVertexes+" at vert "+ (j*columns+i).ToString() +", pos x = "+i+", y = "+j);
				Vector3 position = new Vector3(i * x_spacing, j * y_spacing, profileCurve.Evaluate(i * evaluationStep) * z_depth + UnityEngine.Random.Range(-randomness, randomness));
				for(int p = 0; p < howManyVertexes; p++) {
					if(IsDebug) {
						GameObject g = (GameObject)Instantiate(debugCube, position, Quaternion.identity);
						g.name = counter.ToString();
					}
					vertices.Add(position);
					vertexStack.Push(new Vertex(counter, position));
					counter++;
				}
				stacksOfVertexes.Add(vertexStack);
			}
		}

		GenerateTriangles();
	}

	void GenerateTriangles() {
		for(int j = 0; j<rows-1; j++) {
			for(int i = 0; i<columns-1; i++) {
				triangles.Add(GetSplitVertexNumber(columns * j + i));
				triangles.Add(GetSplitVertexNumber(columns * j + i + 1));
				triangles.Add(GetSplitVertexNumber(columns * j + i + 1 + columns));	

				//Log("Adding triangle: "+(GetSplitVertexNumber(rows * j + i)).ToString() + ", "+(GetSplitVertexNumber(rows * j + i + 1)).ToString() + ", "+(GetSplitVertexNumber(rows * j + i + rows)).ToString());

				triangles.Add(GetSplitVertexNumber(columns * j + i + 1 + columns));
				triangles.Add(GetSplitVertexNumber(columns * j + i + columns));
				triangles.Add(GetSplitVertexNumber(columns * j + i));	

				//Log("Adding triangle: "+(GetSplitVertexNumber(rows * j + i)).ToString() + ", "+(GetSplitVertexNumber(rows * j + i + rows)).ToString() + ", "+(GetSplitVertexNumber(rows * j + i + rows + 1)).ToString());
			}
		}

		SetMesh();
	}

	void SetMesh() {
		Mesh mesh = new Mesh();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		meshFilter.mesh = mesh;

		mesh.RecalculateNormals();
		mesh.RecalculateBounds();

		mesh.Optimize();
		//Debug.Log("Mesh generated in: "+(Time.realtimeSinceStartup - startTime).ToString("f3") + " seconds.");
		//mesh.Optimize();
	}

	#region Helpers

	private void Log(string msg) {
		if(IsDebug) Debug.Log("MeshGen: "+msg);
	}

	private int GetSplitVertexNumber(int sharedVertexNumber) {
		Stack s = stacksOfVertexes[sharedVertexNumber];
		Vertex splitVertex = (Vertex) s.Pop();
		Log("Shared vertex: "+sharedVertexNumber+ " = " + splitVertex.index + " splitted vertex.");
		return splitVertex.index;
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
