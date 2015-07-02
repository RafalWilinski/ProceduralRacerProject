using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CaveGenerator : MonoBehaviour {

	[SerializeField]
	private List<Vector3> leftVerticesColumn;
	[SerializeField]
	private List<Vector3> rightVerticesColumn;

	public List<Vertex> stacksOfVertexes;
	private Vector3[] vertices;
	private List<int> triangles;
	private Vector2[] uvs;
	private Transform myTransform;
	private Vector3 initialCavePos;
	private Vector3 canionMeshOffset;

	public CatmullRomSpline spline;
	public ContinousMovement vehicle;
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
	public float initialDelay = 3.0f;
	public List<float> args;

	private bool stacksGenerated;

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

		//StartCoroutine(GenerateCoroutine());
	}

	private IEnumerator GenerateCoroutine() {
		yield return new WaitForSeconds(initialDelay);
		StartCoroutine(GetBoundaryVertices());
	}

	private MeshGenerator GetCorrespondingPathPart() {
		//float splineClosestPosition = spline.GetClosestPointAtSpline(myTransform.position);

		float splineClosestPosition = spline.GetClosestPointAtSpline(vehicle.MyTransform.position) + 5f;
		caveLength = 100;

		Debug.Log("Creating cave at spline pos: "+splineClosestPosition+" which is Vector3 world pos = "+ spline.GetPositionAtTime(splineClosestPosition));

		for(int i = 0; i < meshPool.allObjects.Count; i++) {
			if(meshPool.allObjects[i].from <= splineClosestPosition && 
				meshPool.allObjects[i].to >= splineClosestPosition) {
//				Debug.Log("Selected mesh is: "+meshPool.allObjects[i]);
				return meshPool.allObjects[i];
			}
		}

		Debug.LogWarning("Corresponding path of MeshGenerator was not found, probably cave exceeds rendered tunnel length. Closest point: "+splineClosestPosition.ToString("f2"));
		return null;
	}

	public IEnumerator GetBoundaryVertices() {
		MeshGenerator startingBaseMesh = GetCorrespondingPathPart();
		offset = startingBaseMesh.offset;

		initialCavePos = startingBaseMesh.transform.position;

		if(startingBaseMesh == null) {
			Debug.LogWarning("Abort!");
			return null;
		}

		leftVerticesColumn = new List<Vector3>();
		rightVerticesColumn = new List<Vector3>();

		int borderVerticesCount = caveLength;
		while(borderVerticesCount > 0) {

//			Debug.Log("Getting border vertices for part "+startingBaseMesh.gameObject);

			

			for(int i = 0; i < startingBaseMesh.rows * startingBaseMesh.columns; i += startingBaseMesh.columns) {
				leftVerticesColumn.Add(startingBaseMesh.Vertices[i].position);
			}

			for(int i = startingBaseMesh.columns-1; i < startingBaseMesh.rows * startingBaseMesh.columns; i += startingBaseMesh.columns) {
				rightVerticesColumn.Add(startingBaseMesh.Vertices[i].position);
				borderVerticesCount--;
			}

			string nextPartName = (int.Parse(startingBaseMesh.name) + 1).ToString();
			GameObject nextPart = GameObject.Find(nextPartName);

			if(nextPart) {
				startingBaseMesh = nextPart.GetComponent<MeshGenerator>();
			}
			else startingBaseMesh = null;

			if(!startingBaseMesh) {
				caveLength = caveLength - borderVerticesCount;
				borderVerticesCount = 0;
			}
		}

		Debug.Log("Final cave length: "+caveLength);

		StartCoroutine(GenerateVertices());

		return null;
	}

	private IEnumerator GenerateVertices() {

		if(vertices == null || vertices.Length < CalculateTargetArraySize()) vertices = new Vector3[CalculateTargetArraySize()];

		Vector3 position = Vector3.zero;
		Vector3 splinePos;
		int howManyVertexes;
		int counter = 0;
		double startTime = Time.realtimeSinceStartup;

		x_spacing = (rightVerticesColumn[0].x - leftVerticesColumn[0].x);

		for(int j = 0; j < caveLength; j++) {

			splinePos = (rightVerticesColumn[j] + leftVerticesColumn[j]) / 2;
			// splinePos = new Vector3(splinePos.x, splinePos.y, splinePos.z);
			
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

				if(j % 10 == 1 && j > 1) {
					position = stacksOfVertexes[j * columns + i - columns].position;
				}
				else {
					position = new Vector3(x_spacing * args[i] + UnityEngine.Random.Range(-randomness, randomness), profileCurve.Evaluate(args[i]) * y_spacing + UnityEngine.Random.Range(-evaluationDisturbance, evaluationDisturbance) * y_spacing, 0);
					position += (splinePos + offset); //preliczac offset na podstawie x_spacing
				}
				
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

			if(Time.realtimeSinceStartup - startTime > 0.015) {
				startTime = Time.realtimeSinceStartup;
				yield return new WaitForEndOfFrame();
			}
		}
		stacksGenerated = true;
		StartCoroutine(GenerateTriangles());
		//else ChangeVertices();
	}

	void ChangeVertices() {
		mesh.vertices = vertices;
		meshFilter.mesh = mesh;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
	}

	IEnumerator GenerateTriangles() {
		int a = 0;
		double startTime = Time.realtimeSinceStartup;
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

			if(Time.realtimeSinceStartup - startTime > 0.015) {
				startTime = Time.realtimeSinceStartup;
				yield return new WaitForEndOfFrame();
			}
		}
		
		mesh = new Mesh();
		mesh.MarkDynamic();
		mesh.vertices = vertices;
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs;
		meshFilter.mesh = mesh;

		mesh.RecalculateBounds();
		mesh.RecalculateNormals();

		//myTransform.position = Vector3.zero;
	}

	private int GetSplitVertexNumber(int sharedVertexNumber) {
		Vertex s = null;
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


	void OnDrawGizmos() {
		Gizmos.color = Color.white;

		if(stacksOfVertexes != null) {
			for(int i = 0; i < stacksOfVertexes.Count; i++) {
				Gizmos.DrawSphere (stacksOfVertexes[i].position, 10f);
			}

			Gizmos.color = Color.red;
			Gizmos.DrawSphere (initialCavePos, 10f);
		}
	}
}
