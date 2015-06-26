using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CaveGenerator : MonoBehaviour {

	[SerializeField]
	private List<Vector3> leftVerticesColumn;
	[SerializeField]
	private List<Vector3> rightVerticesColumn;

	private List<Vertex> stacksOfVertexes;
	private Vector3[] vertices;
	private List<int> triangles;
	private Vector2[] uvs;
	private Transform myTransform;

	public CatmullRomSpline spline;
	public ObjectPool meshPool;
	public GameObject visualDebugCube;
	public int caveLength = 20;
	public int columns = 15;
	public float randomness;
	public float evaluationDisturbance;
	public float y_spacing;
	public float x_spacing;
	public Vector3 offset;
	public AnimationCurve profileCurve;
	public float initialDelay = 1.0f;
	public List<float> args;

	private bool trianglesGenerated;
	private bool stacksGenerated;
	private bool isUsed;

	private MeshFilter meshFilter;
	private Mesh mesh;

	[Serializable]
	public class Vertex {
		public Vector3 position;
		public Stack<int> indexes;

		public Vertex(Vector3 p) {
			position = p;
		}
	}

	private void Awake() {
		myTransform = transform;
		triangles = new List<int>();
		stacksOfVertexes = new List<Vertex>();
		meshFilter = GetComponent<MeshFilter>();

		StartCoroutine(GenerateCoroutine());
	}

	private IEnumerator GenerateCoroutine() {
		yield return new WaitForSeconds(initialDelay);
		GetBoundaryVertices();
	}

	private MeshGenerator GetCorrespondingPathPart() {
		float splineClosestPosition = spline.GetClosestPointAtSpline(myTransform.position);

		for(int i = 0; i < meshPool.allObjects.Count; i++) {
			if(meshPool.allObjects[i].from <= splineClosestPosition && 
				meshPool.allObjects[i].to >= splineClosestPosition) {
				return meshPool.allObjects[i];
			}
		}

		Debug.LogWarning("Corresponding path of MeshGenerator was not found, probably cave exceeds rendered tunnel length. Closest point: "+splineClosestPosition.ToString("f2"));
		return null;
	}

	private void GetBoundaryVertices() {
		MeshGenerator startingBaseMesh = GetCorrespondingPathPart();

		if(startingBaseMesh == null) {
			Debug.LogWarning("Abort!");
			return;
		}

		for(int i = 0; i < startingBaseMesh.rows * startingBaseMesh.columns; i += startingBaseMesh.columns) {
			leftVerticesColumn.Add(startingBaseMesh.Vertices[i].position);
		}

		for(int i = startingBaseMesh.columns-1; i < startingBaseMesh.rows * startingBaseMesh.columns; i += startingBaseMesh.columns) {
			rightVerticesColumn.Add(startingBaseMesh.Vertices[i].position);
		}

		StartCoroutine(GenerateVertices());
	}

	private IEnumerator GenerateVertices() {

		isUsed = true;
		if(vertices == null || vertices.Length < CalculateTargetArraySize()) vertices = new Vector3[CalculateTargetArraySize()];

		Vector3 position = Vector3.zero;
		Vector3 splinePos;
		int howManyVertexes;
		int counter = 0;

		for(int j = 0; j<caveLength; j++) {

			splinePos = (leftVerticesColumn[j] + rightVerticesColumn[j])/2;
			
			howManyVertexes = GetSplitCount(0, j);
			Vertex v = new Vertex(leftVerticesColumn[j]);
			Stack<int> vertexStack = new Stack<int>();

			for(int p = 0; p < howManyVertexes; p++) {
				vertices[counter] = leftVerticesColumn[j];
				vertexStack.Push(counter);
				counter++;
			}
			v.indexes = vertexStack;
			stacksOfVertexes.Add(v);

			// Instantiate(visualDebugCube, )

			for(int i = 1; i<columns-1; i++) {

				position = new Vector3(x_spacing * args[i] + UnityEngine.Random.Range(-randomness, randomness), profileCurve.Evaluate(args[i]) * y_spacing + UnityEngine.Random.Range(-evaluationDisturbance, evaluationDisturbance) * y_spacing, 0);
				position += (splinePos + offset); //preliczac offset na podstawie x_spacing
				
				howManyVertexes = GetSplitCount(i, j);

				if(!stacksGenerated) {
					v = new Vertex(position);
					vertexStack = new Stack<int>();

					for(int p = 0; p < howManyVertexes; p++) {
						vertices[counter] = position;
						vertexStack.Push(counter);
						counter++;
					}
					v.indexes = vertexStack;
					stacksOfVertexes.Add(v);
				}
				else {
					for(int p = 0; p < howManyVertexes; p++) {
						vertices[counter] = position;
						counter++;
					}
				}
			}

			howManyVertexes = GetSplitCount(columns-1, j);
			v = new Vertex(rightVerticesColumn[j]);
			vertexStack = new Stack<int>();

			for(int p = 0; p < howManyVertexes; p++) {
				vertices[counter] = rightVerticesColumn[j];
				vertexStack.Push(counter);
				counter++;
			}
			v.indexes = vertexStack;
			stacksOfVertexes.Add(v);

			yield return new WaitForEndOfFrame();
		}
		stacksGenerated = true;
		if(!trianglesGenerated) StartCoroutine(GenerateTriangles());
		else ChangeVertices();
	}

	void ChangeVertices() {
		mesh.vertices = vertices;
		meshFilter.mesh = mesh;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		isUsed = false;
	}

	IEnumerator GenerateTriangles() {
		int a = 0;
		uvs = new Vector2[CalculateTargetArraySize()];
		for(int j = 0; j<caveLength-1; j++) {
			for(int i = 0; i<columns-1; i++) {

				a = GetSplitVertexNumber(columns * j + i);
				triangles.Add(a);
				uvs[a] = new Vector2(0,0);

				a = GetSplitVertexNumber(columns * j + i + 1);
				triangles.Add(a);
				uvs[a] = new Vector2(1,0);

				a = GetSplitVertexNumber(columns * j + i + 1 + columns);
				triangles.Add(a);
				uvs[a] = new Vector2(0,1);

				a = GetSplitVertexNumber(columns * j + i + 1 + columns);
				triangles.Add(a);
				uvs[a] = new Vector2(0,0);

				a = GetSplitVertexNumber(columns * j + i + columns);
				triangles.Add(a);
				uvs[a] = new Vector2(1,0);

				a = GetSplitVertexNumber(columns * j + i);
				triangles.Add(a);
				uvs[a] = new Vector2(1,1);
			}
			yield return new WaitForEndOfFrame();
		}
		trianglesGenerated = true;
		
		mesh = new Mesh();
		mesh.MarkDynamic();
		mesh.vertices = vertices;
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs;
		meshFilter.mesh = mesh;

		mesh.RecalculateBounds();
		mesh.RecalculateNormals();

		isUsed = false;
	}

	private int GetSplitVertexNumber(int sharedVertexNumber) {
		Vertex s = null;
		Debug.Log("Getting shared vertex #"+sharedVertexNumber);
		s = stacksOfVertexes[sharedVertexNumber];
		return s.indexes.Pop();
	}

	private int CalculateTargetArraySize() {
		int size = 1 + 1 + 2 + 2 + 6 * (columns - 2) + 6 * (caveLength - 2) + 6 * (columns-2) * (caveLength-2);
		return size;
	}

	private int GetSplitCount(int col, int row) {
		if(row == 0) {
			if(col == 0) return 2;
			else if(col == columns - 1) return 1;
			else return 3;

		}
		else if(row == caveLength-1) {
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
}
