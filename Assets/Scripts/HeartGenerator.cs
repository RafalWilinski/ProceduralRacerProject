using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class HeartGenerator : MonoBehaviour {

	public Vector3[] vertices;

	private List<Vertex> stacksOfVertexes;
	private List<int> triangles;
	private Vector2[] uvs;
	private Transform myTransform;
	private MeshFilter meshFilter;
	private Mesh mesh;
	private int vertexCounter;

	[Serializable]
	internal class Vertex {
		public Vector3 position;
		public Stack<int> indexes;

		public Vertex(Vector3 p) {
			position = p;
		}
	}

	void Start () {
		myTransform = transform;
		triangles = new List<int>();
		stacksOfVertexes = new List<Vertex>();
		meshFilter = GetComponent<MeshFilter>();

		vertices = new Vector3[12 * 5];
		uvs = new Vector2[12 * 5];

		CreateBaseVertices();
	}

	private void addVertex(Vector3 vert) {
		Vertex newVertex = new Vertex(vert);
		newVertex.indexes = new Stack<int>();
		for (int i = 0; i < 5; i++) {
			newVertex.indexes.Push(vertexCounter);
			vertices[vertexCounter] = vert;
			vertexCounter++;
		}

		stacksOfVertexes.Add(newVertex);
	}

	private void AddTriangleIndices(int a, int b, int c) {
		Vertex v = stacksOfVertexes[a];
		int poppedValue = v.indexes.Pop();
		triangles.Add(poppedValue);
		uvs[poppedValue] = new Vector2(0, 0);

		v = stacksOfVertexes[b];
		poppedValue = v.indexes.Pop();
		triangles.Add(poppedValue);
		uvs[poppedValue] = new Vector2(1, 0);

		v = stacksOfVertexes[c];
		poppedValue = v.indexes.Pop();
		triangles.Add(poppedValue);
		uvs[poppedValue] = new Vector2(0, 1);
	}

	private void CreateBaseVertices() {
		float t = (float)((1.0 + Math.Sqrt(5.0f)) / 2);

		addVertex(new Vector3(-1,  t,  0) * UnityEngine.Random.Range(0.5f, 1.3f));
		addVertex(new Vector3( 1,  t,  0) * UnityEngine.Random.Range(0.5f, 1.7f));
		addVertex(new Vector3(-1, -t,  0) * UnityEngine.Random.Range(0.8f, 1.3f));
		addVertex(new Vector3( 1, -t,  0) * UnityEngine.Random.Range(0.8f, 1.3f));
		addVertex(new Vector3( 0, -1,  t) * UnityEngine.Random.Range(0.8f, 1.3f));
		addVertex(new Vector3( 0,  1,  t) * UnityEngine.Random.Range(0.8f, 1.3f));
		addVertex(new Vector3( 0, -1, -t) * UnityEngine.Random.Range(0.8f, 1.3f));
		addVertex(new Vector3( 0,  1, -t) * UnityEngine.Random.Range(0.8f, 1.3f));
		addVertex(new Vector3( t,  0, -1) * UnityEngine.Random.Range(0.3f, 1.3f));
		addVertex(new Vector3( t,  0,  1) * UnityEngine.Random.Range(0.8f, 1.3f));
		addVertex(new Vector3(-t,  0, -1) * UnityEngine.Random.Range(0.8f, 1.3f));
		addVertex(new Vector3(-t,  0,  1) * UnityEngine.Random.Range(0.8f, 1.9f));

		CreateBaseTriangles();
	}

	private void CreateBaseTriangles() {

		// 5 triangles around point 0
		AddTriangleIndices(0, 11, 5);
		AddTriangleIndices(0, 5, 1);
		AddTriangleIndices(0, 1, 7);
		AddTriangleIndices(0, 7, 10);
		AddTriangleIndices(0, 10, 11);

		// 5 adjacent triangles
		AddTriangleIndices(1, 5, 9);
		AddTriangleIndices(5, 11, 4);
		AddTriangleIndices(11, 10, 2);
		AddTriangleIndices(10, 7, 6);
		AddTriangleIndices(7, 1, 8);

		// 5 triangles around point 3
		AddTriangleIndices(3, 9, 4);
		AddTriangleIndices(3, 4, 2);
		AddTriangleIndices(3, 2, 6);
		AddTriangleIndices(3, 6, 8);
		AddTriangleIndices(3, 8, 9);

		// 5 adjacent face
		AddTriangleIndices(4, 9, 5);
		AddTriangleIndices(2, 4, 11);
		AddTriangleIndices(6, 2, 10);
		AddTriangleIndices(8, 6, 7);
		AddTriangleIndices(9, 8, 1);

		SetMesh();
	}

	// private void SubdivideIsosphere(int recursionLevel) {
	// 	for (int i = 0; i < recursionLevel; i++) {
	// 		List<int> newTriangles = new List<int>();
	// 		foreach (var tri in faces) {
	// 			// replace triangle by 4 triangles
	// 			int a = getMiddlePoint(tri.v1, tri.v2);
	// 			int b = getMiddlePoint(tri.v2, tri.v3);
	// 			int c = getMiddlePoint(tri.v3, tri.v1);

	// 			faces2.Add(new TriangleIndices(tri.v1, a, c));
	// 			faces2.Add(new TriangleIndices(tri.v2, b, a));
	// 			faces2.Add(new TriangleIndices(tri.v3, c, b));
	// 			faces2.Add(new TriangleIndices(a, b, c));
	// 		}
	// 		faces = faces2;
	// 	}

	// }

	private void SetMesh() {
		mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs;
		meshFilter.mesh = mesh;

		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
	}
}
