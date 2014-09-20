using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoadGenerator : MonoBehaviour {

	public bool isDebug;

	public CatmullRomSpline spline;
	public GameObject markerContainer;

	public float zStep;
	private int zIterator = 0;

	public List<Vector3> vertices = new List<Vector3>();
	public List<int> triangles = new List<int>();
	public List<Vector2> uuvsvs = new List<Vector2>();

	private MeshFilter meshFilter;
	private Mesh mesh;
	private int triangleIterator;

	//Pobiera na poczatku pewien profil drogi
	//Na jego podstawie co N tworzy drogę wzluz spline'a
	//co M dzieli mesh na batche, przejechany batch usuwa

	void OnEnable() {
		CatmullRomSpline.OnSplineUpdated += BuildRoad;
	}

	void log(string msg) {
		if(isDebug) Debug.Log("Road: "+msg);
	}

	public void ReportVertex(Vector3 vertexPos) {
		vertices.Add(vertexPos);
		log("Vertex add at pos "+vertexPos);
		UpdateTriangles();
	}

	void UpdateTriangles() {
		if(vertices.Count == 4) triangleIterator = 3; 
		else if(vertices.Count > 4 && vertices.Count % 2 == 0) triangleIterator += 2;
		
		if(triangleIterator > 0) {

			triangles.Add(triangleIterator-2);
			triangles.Add(triangleIterator);
			triangles.Add(triangleIterator-1);

			triangles.Add(triangleIterator-2);
			triangles.Add(triangleIterator-1);
			triangles.Add(triangleIterator-3);

			UpdateMesh();
		}
	}

	void UpdateMesh() {
		if(triangles.Count >= 4) {
			mesh.vertices = vertices.ToArray();
			mesh.triangles = triangles.ToArray();
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
		}
	}

	void AddSegment() {
		Vector3 centerPos = spline.GetPositionAtTime(zStep * zIterator);
		Quaternion rot = spline.GetRotAtTime(zStep * zIterator);
		log("Centerpos: "+centerPos);
		Vector3 vertexPos;
		vertexPos = centerPos;

		Instantiate(markerContainer,vertexPos,rot);

		zIterator++;
	}

	void Start () {
		meshFilter = (MeshFilter)this.gameObject.AddComponent<MeshFilter>();
		mesh = new Mesh();
		meshFilter.mesh = mesh;
		mesh.MarkDynamic();
		mesh.Clear();
	}

	void BuildRoad(float limit) {
		for(int i = 0; i < 10; i++) {
			AddSegment();
		}
	}
}
