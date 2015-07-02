using UnityEngine;
using System.Collections;

public class SideColliderMeshGenModule : MonoBehaviour {

	public GameObject childGameObject;
	public Vector3 offset;
	public int partsNumber;
	public float colliderHeight;
	public CatmullRomSpline spline;

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
		if(childGameObject == null) {
			childGameObject = new GameObject();
			childGameObject.name = "SideCollider";
			childGameObject.transform.parent = myTransform;

			if(childCollider == null) {
				childCollider = childGameObject.AddComponent<MeshCollider>();
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

		int trianglesCounter = 0;

		for(int i = 0; i < (partsNumber * 2 - 2); i++) {
			triangles[trianglesCounter++] = i;
			triangles[trianglesCounter++] = i+1;
			triangles[trianglesCounter++] = i+2;
		}

		mesh.vertices = vertices;
		mesh.triangles = triangles;

		childCollider.sharedMesh = null;
		childCollider.sharedMesh = mesh;
	}
}
