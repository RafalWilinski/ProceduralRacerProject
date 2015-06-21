using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatmullRomSpline : MonoBehaviour {

	public GameObject meshPart;
	public bool isDebug;
	public bool invert;
	public float deriveDelta;
	public AnimationCurve profileCurve;
	public List<float> args;
	public float curveLength;
	public ObjectPool pool;
	public ThemeManager themesManager;

	public float xAxisDivider;

	private float startTimestep;
	private float nodeTimeLimit;
	private bool isReady;
	private float meshRenderedCap;
	private int meshCounter;

	public bool IsReady {
		get {
			return isReady;
		}
	}

	public float TimeLimit {
		get {
			return nodeTimeLimit;
		}
		set { Debug.Log("You are not allowed to change nodeTimeLimit!"); } //We don't want it to be changed!
	}

	internal class Node {
		internal GameObject go;
		internal Vector3 pos;
		internal Quaternion rot;
		internal float time;
		internal CatmullRomNode nodeScript;

		internal Node(GameObject g, Vector3 p) { go = g; pos = p; }
		internal Node(GameObject g, Vector3 p, float t) { go = g; pos = p; time = t; }
		internal Node(GameObject g, Vector3 p, Quaternion q, CatmullRomNode n) { go = g; pos = p; nodeScript = n; rot = q; }

		public override string ToString() {
			return "Pos :"+pos+",\nRot: "+rot+",\nTime: "+time;
		}

		internal string GetTime() { return time.ToString("f2"); }

		internal float Time {
			get {
				return time;
			}
			set {
				time = value;
				nodeScript.timeGiven = value;
			}
		}
	}

	public delegate void SplineUpdate(float limit);
    public static event SplineUpdate OnSplineUpdated;

	List<Node> nodes = new List<Node>();

	void log(string msg) {
		if(isDebug) Debug.Log("Spline: "+msg);
	}

	void Awake() {
		isReady = false;
	}

	void PrintNodeTimes() {
		//string report = "";
		for(int i = 0; i < (nodes.Count); i++) {
			//report += "Node #"+i+": "+nodes[i].GetTime()+"| ";
			nodeTimeLimit = i * startTimestep;
		}
		nodeTimeLimit -= (startTimestep*2);
		isReady = true;
		//report += "Limit: "+nodeTimeLimit;
		if(OnSplineUpdated != null) OnSplineUpdated(nodeTimeLimit);
		//log(report);
	}

	public void AddNode(GameObject gameObj) {
		isReady = false;
		if(CheckExistingNode(gameObj) == true) return;
		nodes.Add(new Node(gameObj, gameObj.transform.position, gameObj.transform.rotation, gameObj.GetComponent<CatmullRomNode>()));
		Destroy(gameObj);
		RecalculateNodeTimes();
	}

	public void PushNode(GameObject gameObj) {
		isReady = false;
		nodes.Add(new Node(gameObj, gameObj.transform.position, gameObj.transform.rotation, gameObj.GetComponent<CatmullRomNode>()));
		nodes[nodes.Count-1].Time = nodes[nodes.Count-2].Time + startTimestep;

		if(nodeTimeLimit > 1f) {
			GameObject g = (GameObject) pool.Create(Vector3.zero);
			//MeshGenerator meshGen = ((GameObject) Instantiate(meshPart, Vector3.zero, Quaternion.identity)).GetComponent<MeshGenerator>() as MeshGenerator;
			MeshGenerator meshGen = g.GetComponent<MeshGenerator>() as MeshGenerator;
			meshGen.assignedPosition = GetPositionAtTime(nodeTimeLimit);
			meshGen.profileCurve = themesManager.GetCurrentTheme().curve;
			meshGen.x_spacing = themesManager.GetCurrentTheme().x_spacing;
			meshGen.y_spacing = themesManager.GetCurrentTheme().y_spacing;
			g.name = meshCounter.ToString();
			//Debug.Log("Created: "+g.name);
			meshGen.Generate(meshRenderedCap, nodeTimeLimit, profileCurve);
			meshRenderedCap = nodeTimeLimit;
			meshCounter++;

		}
		else {
			Debug.Log("TimeLimit: "+nodeTimeLimit);
		}

		PrintNodeTimes();
	}

	bool CheckExistingNode(GameObject gameObj) {
		bool isFound = false;
		for(int i = 0; i < nodes.Count; i++) {
			if(nodes[i].go == gameObj) { //Selected node is already in list
				UpdateNode(gameObj, i);
				log("Node at index "+i+" updated!");
				isFound = true;
			}
		}
		isReady = true;
		return isFound;
	}

	public void UpdateNode(GameObject gameObj, int index) {
		nodes[index].pos = gameObj.transform.position;
	}

	public float GetClosestPointAtSpline(Vector3 pos) {
		float x1 = 0f;
		float x2 = nodeTimeLimit;
		float divisionPoint = nodeTimeLimit/2;
		int i = 0;
		while(i < 20) {
			//Debug.Log("GetPosAtTime: "+((divisionPoint + x1)/2).ToString("f4") + " & " + ((divisionPoint+x2)/2).ToString("f4") );
			if(CompareTwoSplinePoints(GetPositionAtTime( (divisionPoint + x1)/2 ), GetPositionAtTime( (divisionPoint+x2)/2 ), pos) == 1) {
				//Debug.Log("Case 1 - 1: "+x1+", x2: "+x2+", div: "+divisionPoint);
				x2 = divisionPoint;
				divisionPoint = (x2+x1)/2;
			}
			else {
				//Debug.Log("Case 2 - 1: "+x1+", x2: "+x2+", div: "+divisionPoint);
				x1 = divisionPoint;
				divisionPoint = (x2+x1)/2;
			}
			i++;
		}
		return divisionPoint;
	}

	public float GetClosestPointAtSpline(Vector3 pos, int accuracy) {
		float x1 = 0f;
		float x2 = nodeTimeLimit;
		float divisionPoint = nodeTimeLimit/2;
		int i = 0;
		while(i < accuracy) {
			//Debug.Log("GetPosAtTime: "+((divisionPoint + x1)/2).ToString("f4") + " & " + ((divisionPoint+x2)/2).ToString("f4") );
			if(CompareTwoSplinePoints(GetPositionAtTime( (divisionPoint + x1)/2 ), GetPositionAtTime( (divisionPoint+x2)/2 ), pos) == 1) {
				//Debug.Log("Case 1 - 1: "+x1+", x2: "+x2+", div: "+divisionPoint);
				x2 = divisionPoint;
				divisionPoint = (x2+x1)/2;
			}
			else {
				//Debug.Log("Case 2 - 1: "+x1+", x2: "+x2+", div: "+divisionPoint);
				x1 = divisionPoint;
				divisionPoint = (x2+x1)/2;
			}
			i++;
		}
		return divisionPoint;
	}

	private int CompareTwoSplinePoints(Vector3 p1, Vector3 p2, Vector3 point) {
		//Debug.Log("P1: "+p1 + ", P2: " + p2 + ", Org: " + point);
		float d1 = Vector3.Distance(p1, point);
		float d2 = Vector3.Distance(p2, point);
		//Debug.Log("D1: "+d1.ToString("f3") + ", D2: " + d2.ToString("f3"));
		if(d1 < d2) return 1;
		else return 2;
	}

	void RecalculateNodeTimes() {
		int nodesCount = nodes.Count;
		if(nodesCount < 4) {
			log("Spline too short! Unable to recalculate times and create spline.");
		}
		else {
			float timeStep = 0f;
			timeStep = 1.0f / (nodesCount - 3);
			startTimestep = timeStep;

			for(int i = 0; i < (nodesCount-2); i++) {
				nodes[i+1].Time = i * timeStep;
				Debug.Log(i*timeStep);
			}

			nodes[0].Time = -timeStep;
			nodes[nodes.Count - 1].Time = 1 + timeStep;

			log("Times recalculated.");
			PrintNodeTimes();
		}
	}

	int NearestNodeToTime(float _t) {
		for(int i = 0; i < (nodes.Count-2); i++) {
			if(nodes[i+1].time > _t) return i;
		}
		if(_t == nodeTimeLimit) return nodes.Count - 3; //Case when reaching almost least node in spline

		return -1;
	}

	public Quaternion GetRotAtTime(float t) {

		Quaternion rot = Quaternion.identity;
		int nearestNodeIndex = NearestNodeToTime(t);
		if(nearestNodeIndex < 0) {
			log("Nearest node not found for t = "+t+", aborting!");
			return Quaternion.identity;
		}
		try {

			t = (t - nodes[nearestNodeIndex].time) / (nodes[nearestNodeIndex+1].time - nodes[nearestNodeIndex].time); //T Conversion. Putting raw 0..1 input causes weird things

			Quaternion q0 = nodes[nearestNodeIndex-1].rot;
			Quaternion q1 = nodes[nearestNodeIndex].rot;
			Quaternion q2 = nodes[nearestNodeIndex+1].rot;
			Quaternion q3 = nodes[nearestNodeIndex+2].rot;

			Quaternion T1 = MathUtils.GetSquadIntermediate(q0, q1, q2);
			Quaternion T2 = MathUtils.GetSquadIntermediate(q1, q2, q3);

			rot = MathUtils.GetQuatSquad(t, q1, q2, T1, T2);
		}
		catch(System.Exception e) {
			Debug.Log("Unable to calculate quaternion. "+e);
		}

		return rot;
	}

	public void GetRotAtTime(float t, GameObject go) {
		Vector3 futurePos;
		if(t + deriveDelta < nodeTimeLimit) futurePos = GetPositionAtTime(t + deriveDelta);
		else futurePos = GetPositionAtTime(nodeTimeLimit);

		go.transform.LookAt(futurePos);
	}

	public float GetTanAtTime(float t) {
		Vector3 futurePos;
		Vector3 herePos;
		herePos = GetPositionAtTime(t);
		if(t + deriveDelta < nodeTimeLimit) futurePos = GetPositionAtTime(t + deriveDelta);
		else futurePos = GetPositionAtTime(nodeTimeLimit);

		float delta_x = futurePos.x - herePos.x;
		float delta_y = futurePos.z - herePos.z;
		//Debug.Log(futurePos.x.ToString("f2") + " - " + herePos.x.ToString("f2") + "=" + delta_x.ToString("f2")+", delta_y: "+delta_y+", Tan: "+((float)(delta_x / delta_y)).ToString()+", Atan: "+ Mathf.Atan(delta_x / delta_y).ToString("f2"));
		return (float) delta_x / delta_y;
	}

	public Vector3 GetPositionAtTime(float t) {

		//if(!invert) t = 1 - t; //Inversion. Works only when spline is in 0..1 range

		Vector3 pos = Vector3.zero;
		int nearestNodeIndex = NearestNodeToTime(t);
		if(nearestNodeIndex < 0) {
			log("Nearest node not found for t = "+t+", aborting!");
			return Vector3.zero;
		}
		//log("Nearest node found: "+nearestNodeIndex);
		t = (t - nodes[nearestNodeIndex].time) / (nodes[nearestNodeIndex+1].time - nodes[nearestNodeIndex].time); //T Conversion. Putting raw 0..1 input causes weird things

		Vector3 p0 = nodes[nearestNodeIndex-1].pos;
		Vector3 p1 = nodes[nearestNodeIndex].pos;
		Vector3 p2 = nodes[nearestNodeIndex+1].pos;
		Vector3 p3 = nodes[nearestNodeIndex+2].pos;

		Vector3 tension1 = 2 * p1;
		Vector3 tension2 = (-p0 + p2) * t;
		Vector3 tension3 = ((2 * p0) - (5 * p1) + (4 * p2) - p3) * t * t;
		Vector3 tension4 = (-p0 + (3 * p1) - (3 * p2) + p3) * t * t * t;

		pos = 0.5f * (tension1 + tension2 + tension3 + tension4);

		pos = new Vector3(pos.x/xAxisDivider, pos.y, pos.z);
		return pos;
	}

	void calculateCurveLength() {
		float totalLength = 0f;
		for(float f = 0.01f; f < 1.0f; f += 0.01f) {
			float delta_y = profileCurve.Evaluate(f) - profileCurve.Evaluate(f-0.01f);
			totalLength += Mathf.Sqrt( (Mathf.Pow(delta_y, 2) + Mathf.Pow(0.01f, 2f)) );
		}
		curveLength = totalLength;
		Debug.Log("Curve Length: "+totalLength);
	}
}
