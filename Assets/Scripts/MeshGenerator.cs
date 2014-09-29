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
	private Vector2[] uvs;
	private int counter;
	private float startTime;
	public List<float> args;
	public List<Vector3> lastRow;
	private MeshGenerator previousPart;
	private Mesh mesh;
	private MeshCollider col;

	public float from;
	public float to;

	private Transform myTransform;
	private GameObject cam;
	private Transform camTrans;

	[Serializable]
	internal class Vertex {
		public Vector3 position;
		public int index;

		public Vertex(int i, Vector3 p) {
			index = i;
			position = p;
		}
	}

	void Start() {
		myTransform = transform;
		cam = GameObject.Find("Main Camera");
		camTrans = cam.transform;
	}
		
	public void Generate() {
		StartCoroutine("CreateVertices");
		//StartCoroutine(calculateCurveLength());
	}

	public void Generate(float f, float t, AnimationCurve pc) {
		from = f;
		to = t;
		profileCurve = pc;
		StartCoroutine("CreateVertices");
		//StartCoroutine(calculateCurveLength());
	}

	IEnumerator calculateCurveLength() {
		args = new List<float>();
		float totalLength = 0f;
		for(float f = 0.01f; f < 1.0f; f += 0.01f) {
			float delta_y = profileCurve.Evaluate(f) - profileCurve.Evaluate(f-0.01f);
			totalLength += Mathf.Sqrt( (Mathf.Pow(delta_y, 2) + Mathf.Pow(0.01f, 2f)) );
		}
		yield return new WaitForEndOfFrame();
		float desiredLength = totalLength / 11;
		Debug.Log("Curve Length: "+totalLength+", desired part length: "+desiredLength.ToString("f3"));
		float lastX = 0f;
		float partLength = 0f;
		for(float f = 0.01f; f <= 1.0f; f += 0.01f) {
			float delta_y = profileCurve.Evaluate(f) - profileCurve.Evaluate(f-0.01f);
			partLength += Mathf.Sqrt( (Mathf.Pow(delta_y, 2) + Mathf.Pow(0.01f, 2f)) );
			//Debug.Log("delta_Y: "+delta_y+", partLen: "+partLength);
			if(partLength >= desiredLength * 0.9f) {
				args.Add(f);
				Debug.Log("Found x: "+f);
				lastX = f;
				partLength = 0f;
			}
		}
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
			//float tangent = spline.GetTanAtTime(_step * j + from);
			///Debug.Log("Tan: "+tangent);
			for(int i = 0; i<columns; i++) {
				int howManyVertexes = GetSplitCount(i, j);
				Stack vertexStack = new Stack();
				Log("Putting "+howManyVertexes+" at vert "+ (j*columns+i).ToString() +", pos x = "+i+", y = "+j);
				//Vector3 position = new Vector3(i * x_spacing + UnityEngine.Random.Range(-randomness, randomness), profileCurve.Evaluate(i * evaluationStep + UnityEngine.Random.Range(-evaluationDisturbance, evaluationDisturbance)) * y_spacing + UnityEngine.Random.Range(-randomness, randomness));
				Vector3 position = new Vector3(x_spacing * args[i] + UnityEngine.Random.Range(-randomness, randomness), profileCurve.Evaluate(args[i]) * y_spacing + UnityEngine.Random.Range(-evaluationDisturbance, evaluationDisturbance) * y_spacing, 0);
				position += (splinePos + offset);
				if(j == rows-1) {
					lastRow.Add(position);
				}
				else if(j == 0) {
					if(gameObject.name != "0") {
						if(previousPart == null) previousPart = (MeshGenerator) GameObject.Find( (string)(int.Parse(gameObject.name) - 1).ToString() ).GetComponent<MeshGenerator>() as MeshGenerator;
						while(previousPart.lastRow.Count <= i) {
							yield return new WaitForSeconds(0.1f);
						}
						position = previousPart.lastRow[i];
					}
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
		uvs = new Vector2[vertices.Count];
		triangles = new List<int>();
		for(int j = 0; j<rows-1; j++) {
			for(int i = 0; i<columns-1; i++) {
				int a = GetSplitVertexNumber(columns * j + i + 1 + columns);
				triangles.Add(a);
				uvs[a] = new Vector2(0,0);

				a = GetSplitVertexNumber(columns * j + i + 1);
				triangles.Add(a);
				uvs[a] = new Vector2(1,0);

				a = GetSplitVertexNumber(columns * j + i);
				triangles.Add(a);	
				uvs[a] = new Vector2(0,1);

				//Log("Adding triangle: "+(GetSplitVertexNumber(rows * j + i)).ToString() + ", "+(GetSplitVertexNumber(rows * j + i + 1)).ToString() + ", "+(GetSplitVertexNumber(rows * j + i + rows)).ToString());

				a = GetSplitVertexNumber(columns * j + i);
				triangles.Add(a);
				uvs[a] = new Vector2(0,0);

				a = GetSplitVertexNumber(columns * j + i + columns);
				triangles.Add(a);
				uvs[a] = new Vector2(1,0);

				a = GetSplitVertexNumber(columns * j + i + 1 + columns);
				triangles.Add(a);
				uvs[a] = new Vector2(1,1);


				//Log("Adding triangle: "+(GetSplitVertexNumber(rows * j + i)).ToString() + ", "+(GetSplitVertexNumber(rows * j + i + rows)).ToString() + ", "+(GetSplitVertexNumber(rows * j + i + rows + 1)).ToString());
			}
			yield return new WaitForEndOfFrame();
		}
		SetMesh();
	}

	void SetMesh() {
		mesh = new Mesh();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs;
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		meshFilter.mesh = mesh;

		mesh.RecalculateNormals();
		mesh.RecalculateBounds();

		col = (MeshCollider) gameObject.AddComponent<MeshCollider>();
		col.mesh = mesh;

		StartCoroutine("Check");

		//mesh.Optimize();
		//Debug.Log("Mesh generated in: "+(Time.realtimeSinceStartup - startTime).ToString("f3") + " seconds.");
		//mesh.Optimize();
	}

	IEnumerator Check() {
		while(true) {
			if(lastRow[0].z + 25 < camTrans.position.z) {
				Remove();
			}
			yield return new WaitForSeconds(0.25f);
		}
	}

	#region Helpers

	private void Log(string msg) {
		if(IsDebug) Debug.Log("MeshGen: "+msg);
	}

	void Remove() {
		stacksOfVertexes = null;
		vertices = null;
		triangles = null;
		mesh = null;
		col.mesh = null;
		Destroy(gameObject.GetComponent<MeshFilter>().mesh);
		Destroy(gameObject);
		GC.Collect();
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
