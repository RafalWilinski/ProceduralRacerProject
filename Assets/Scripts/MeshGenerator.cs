using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MeshGenerator : MonoBehaviour {

	public bool IsDebug;
	public bool CreateDebugCubes;
	public CatmullRomSpline spline;
	public GameObject debugCube;
	public int columns = 10;
	public int rows = 10;
	public float x_spacing = 0.5f;
	public float y_spacing = 0.1f;
	public float evaluationStep = 0.1f;
	public float evaluationDisturbance;
	public float randomness;
	public AnimationCurve profileCurve;
	public Vector3 offset;

	private List<Stack> stacksOfVertexes;
	private List<Vector3> vertices;
	private List<int> triangles;
	private int counter;
	private float startTime;

	public float from;
	public float to;

	[Serializable]
	internal class Vertex {
		public Vector3 position;
		public int index;

		public Vertex(int i, Vector3 p) {
			index = i;
			position = p;
		}
	}
		
	public void Generate() {
		StartCoroutine("CreateVertices");
	}

	public void Generate(float f, float t, AnimationCurve pc) {
		from = f;
		to = t;
		profileCurve = pc;
		StartCoroutine(CreateVertices());
	}

	IEnumerator CreateVertices() {
		float _step = (to - from) / (rows-1);
		spline = (CatmullRomSpline) GameObject.Find("Root").GetComponent<CatmullRomSpline>();
		startTime = Time.realtimeSinceStartup;
		stacksOfVertexes = new List<Stack>();
		vertices = new List<Vector3>();
		for(int j = 0; j<rows; j++) {
			Vector3 splinePos = spline.GetPositionAtTime(_step * j + from);
			for(int i = 0; i<columns; i++) {
				int howManyVertexes = GetSplitCount(i, j);
				Stack vertexStack = new Stack();
				Log("Putting "+howManyVertexes+" at vert "+ (j*columns+i).ToString() +", pos x = "+i+", y = "+j);
				Vector3 position = new Vector3(i * x_spacing + UnityEngine.Random.Range(-randomness, randomness), profileCurve.Evaluate(i * evaluationStep + UnityEngine.Random.Range(-evaluationDisturbance, evaluationDisturbance)) * y_spacing + UnityEngine.Random.Range(-randomness, randomness), 0);
				position += (splinePos + offset);
				if(j == rows-1) {
					LastRowContainer.Instance.vertexPositions.Add(position);
				}
				else if(j == 0) {
					if(LastRowContainer.Instance.vertexPositions.Count > 1) position = LastRowContainer.Instance.VertexPosition(i);
				}
				for(int p = 0; p < howManyVertexes; p++) {
					if(CreateDebugCubes) {
						GameObject g = (GameObject)Instantiate(debugCube, position, Quaternion.identity);
						g.name = counter.ToString();
					}
					vertices.Add(position);
					vertexStack.Push(new Vertex(counter, position));
					counter++;
				}
				stacksOfVertexes.Add(vertexStack);
			}
			yield return new WaitForEndOfFrame();
		}

		StartCoroutine(GenerateTriangles());
	}

	IEnumerator GenerateTriangles() {
		triangles = new List<int>();
		for(int j = 0; j<rows-1; j++) {
			for(int i = 0; i<columns-1; i++) {
				triangles.Add(GetSplitVertexNumber(columns * j + i + 1 + columns));
				triangles.Add(GetSplitVertexNumber(columns * j + i + 1));
				triangles.Add(GetSplitVertexNumber(columns * j + i));	

				//Log("Adding triangle: "+(GetSplitVertexNumber(rows * j + i)).ToString() + ", "+(GetSplitVertexNumber(rows * j + i + 1)).ToString() + ", "+(GetSplitVertexNumber(rows * j + i + rows)).ToString());

				triangles.Add(GetSplitVertexNumber(columns * j + i));
				triangles.Add(GetSplitVertexNumber(columns * j + i + columns));
				triangles.Add(GetSplitVertexNumber(columns * j + i + 1 + columns));	

				//Log("Adding triangle: "+(GetSplitVertexNumber(rows * j + i)).ToString() + ", "+(GetSplitVertexNumber(rows * j + i + rows)).ToString() + ", "+(GetSplitVertexNumber(rows * j + i + rows + 1)).ToString());
			}
			yield return new WaitForEndOfFrame();
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

		//mesh.Optimize();
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
