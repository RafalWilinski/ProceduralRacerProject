using UnityEngine;
using System.Collections;

public class SideColliderMeshGenModule : MonoBehaviour {

	public GameObject childGameObject;
	public Vector3 offset;
	public int partsNumber;
	public float colliderHeight;
	public CatmullRomSpline spline;

	private bool trianglesGenerated;
	private Transform myTransform;
	private MeshCollider childCollider;
	private Mesh mesh;

	public Vector3[] vertices;
	public int[] triangles;

	void Awake() {
		myTransform = transform;

		vertices = new Vector3[partsNumber * 2 + 2];
		triangles = new int[3 * (partsNumber * 2 - 2)];
		mesh = new Mesh();
	}

	public void Generate(float splineStart, float splineEnd) {
		Debug.Log("Generating side collider from "+splineStart.ToString("f2") + " to " + splineEnd.ToString("f2"));
		if(childGameObject == null) {
			childGameObject = new GameObject();
			childGameObject.name = "SideCollider";
			childGameObject.transform.parent = myTransform;

			if(childCollider == null) {
				childCollider = childGameObject.AddComponent<MeshCollider>();
				childGameObject.AddComponent<MeshRenderer>();
				childGameObject.AddComponent<MeshFilter>();
			}
		}

		int verticesCounter = 0;
		float f_iter_increment = (splineEnd - splineStart) / partsNumber;
		float f_iter = splineStart;
		Vector3 pos;

		// Generate vertices
		while(f_iter < splineEnd) {

			pos = spline.GetPositionAtTime(f_iter) + offset;
			vertices[verticesCounter++] = pos;
			vertices[verticesCounter++] = pos + new Vector3(0, colliderHeight, 0);

			f_iter += f_iter_increment;
		}

		mesh.vertices = vertices;

		if(!trianglesGenerated) {
			int trianglesCounter = 0;

			for(int i = 0; i < (partsNumber * 2 - 2); i+=2) {
				triangles[trianglesCounter++] = i;
				triangles[trianglesCounter++] = i+2;
				triangles[trianglesCounter++] = i+1;

				triangles[trianglesCounter++] = i+5;
				triangles[trianglesCounter++] = i+3;
				triangles[trianglesCounter++] = i+4;
			}
			mesh.triangles = triangles;
			trianglesGenerated = true;
		}	

		childGameObject.GetComponent<MeshFilter>().sharedMesh = null;
		childGameObject.GetComponent<MeshFilter>().sharedMesh = mesh;

		childCollider.sharedMesh = null;
		childCollider.sharedMesh = mesh;
	}
}
