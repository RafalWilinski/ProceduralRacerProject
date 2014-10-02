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
	public List<Vector3> vertices;
	private List<int> triangles;
	private Vector2[] uvs;
	private int counter;
	private float startTime;
	public List<float> args;
	public List<Vector3> lastRow;
	private MeshGenerator previousPart;

	private MeshFilter meshFilter;
	private Mesh mesh;
	private MeshCollider col;

	public float from;
	public float to;
	public bool isUsed;
	public Vector3 assignedPosition;

	private Transform myTransform;
	private GameObject cam;
	private Transform camTrans;

	private bool trianglesGenerated;

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
		//StartCoroutine(calculateCurveLength());
	}

	void Awake() {
		CalculateTargetArraySize();
		myTransform = transform;
		triangles = new List<int>();
		col = GetComponent<MeshCollider>();
		meshFilter = GetComponent<MeshFilter>();
		spline = (CatmullRomSpline) GameObject.Find("Root").GetComponent<CatmullRomSpline>();
		cam = GameObject.Find("Main Camera");
		camTrans = cam.transform;

	}

	public void Generate(float f, float t, AnimationCurve pc) {
		if(!isUsed) {
			stacksOfVertexes = new List<Stack>();
			vertices = new List<Vector3>();
			previousPart = null;
			StopAllCoroutines();
			StartCoroutine("Check");
			lastRow = null;
			isUsed = true;
			GC.Collect();
			from = f;
			to = t;
			profileCurve = pc;
			StartCoroutine("CreateVertices");
		}
		else {
			Debug.Log("This object is busy right now!");
		}
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
		Vector3 position = Vector3.zero;
		Vector3 splinePos;
		int howManyVertexes = 0;
		uvs = null;
		lastRow = null;
		counter = 0;
		float _step = (to - from) / (rows-1);
		stacksOfVertexes.Clear();
		vertices.Clear();

		lastRow = new List<Vector3>();
		for(int j = 0; j<rows; j++) {
			splinePos = spline.GetPositionAtTime(_step * j + from);
			//float tangent = spline.GetTanAtTime(_step * j + from);
			for(int i = 0; i<columns; i++) {
				howManyVertexes = GetSplitCount(i, j);
				Stack vertexStack = new Stack();
				//Log("Putting "+howManyVertexes+" at vert "+ (j*columns+i).ToString() +", pos x = "+i+", y = "+j);
				//Vector3 position = new Vector3(i * x_spacing + UnityEngine.Random.Range(-randomness, randomness), profileCurve.Evaluate(i * evaluationStep + UnityEngine.Random.Range(-evaluationDisturbance, evaluationDisturbance)) * y_spacing + UnityEngine.Random.Range(-randomness, randomness));
				position = new Vector3(x_spacing * args[i] + UnityEngine.Random.Range(-randomness, randomness), profileCurve.Evaluate(args[i]) * y_spacing + UnityEngine.Random.Range(-evaluationDisturbance, evaluationDisturbance) * y_spacing, 0);
				position += (splinePos + offset);
				if(j == rows-1) {
					lastRow.Add(position);
				}
				else if(j == 0) {
					if(gameObject.name != "0") {
						if(previousPart == null) previousPart = (MeshGenerator) GameObject.Find( (string)(int.Parse(gameObject.name) - 1).ToString() ).GetComponent<MeshGenerator>() as MeshGenerator;
						while(previousPart.lastRow.Count < columns) {
							yield return new WaitForSeconds(0.25f);
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
		//Log("#"+gameObject.name+" vertexes generated, t: "+Time.realtimeSinceStartup.ToString());
		if(!trianglesGenerated) StartCoroutine("GenerateTriangles");
		else ChangeVertices();
	}

	IEnumerator GenerateTriangles() {
		int a = 0;
		Log("#"+gameObject.name+", Generate triangles, t: "+Time.realtimeSinceStartup.ToString());
		//yield return new WaitForSeconds(0.5f);
		uvs = new Vector2[vertices.Count];
		Log("#"+gameObject.name+" - Creating uvs array, size: "+vertices.Count+" Stacks: "+stacksOfVertexes.Count);
		for(int j = 0; j<rows-1; j++) {
			for(int i = 0; i<columns-1; i++) {
				a = GetSplitVertexNumber(columns * j + i + 1 + columns);
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
		trianglesGenerated = true;
		SetMesh();
	}

	private int CalculateTargetArraySize() {
		int size = 1 + 1 + 2 + 2 + 6 * (columns - 2) + 6 * (rows - 2) + 6 * (columns-2) * (rows-2);
		return size;
	}

	void ChangeVertices() {
		mesh.vertices = vertices.ToArray();
		meshFilter.mesh = mesh;

		mesh.RecalculateBounds();
		mesh.RecalculateNormals();

		col.mesh = mesh;
		isUsed = false;
	}

	void SetMesh() {
		mesh = new Mesh();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs;
		meshFilter.mesh = mesh;

		mesh.RecalculateBounds();
		mesh.RecalculateNormals();

		col.mesh = mesh;
		isUsed = false;
	}

	IEnumerator Check() {
		while(true) {
			//Debug.Log("Checking! "+camTrans.position.z+ " : " + assignedPosition.z);
			if(assignedPosition.z + 500 < camTrans.position.z && !isUsed) {
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
		uvs = null;
		meshFilter.mesh = null;
		vertices.Clear();
		vertices.TrimExcess();
		stacksOfVertexes.Clear();
		stacksOfVertexes.TrimExcess();
		GameObject.Find("Root").GetComponent<ObjectPool>().Return(gameObject);
	}

	private int GetSplitVertexNumber(int sharedVertexNumber) {
		if(sharedVertexNumber > 199) Log("Possible error! Requesting for vert #"+sharedVertexNumber.ToString());
		Stack s = null;
		//Log("Requesring split for shared #"+sharedVertexNumber.ToString());
		s = stacksOfVertexes[sharedVertexNumber];
		Vertex splitVertex = (Vertex) s.Pop();
		//if(splitVertex.index > 1026) Log("Possible error! Size exceeded 1026. Index: "+splitVertex.index);
		//Log("Shared vertex: "+sharedVertexNumber+ " = " + splitVertex.index + " splitted vertex.");
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
