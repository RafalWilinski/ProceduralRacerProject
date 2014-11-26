using System;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OpponentsHud : MonoBehaviour
{

    public List<OpponentRepresentation> Opponents;
    public Transform HudParent;
    public GameObject HUD;
    public Transform OpponentsParent;
    public Transform Vehicle;
    public Camera Cam;
    public AnimationCurve alphaCurve;

    public float ComputeInterval;
    public float AlphaFunctionDivider;
    public float AlphaFunctionAdder;

    private static OpponentsHud _instance;

    public static OpponentsHud Instance {
        get { return _instance; }
    }

    [Serializable]
    public class OpponentRepresentation {
        public Opponent op;
        public Transform opTransform;
        public GameObject go;
        public CanvasGroup canvas;
        public RectTransform rect;

        public OpponentRepresentation(Opponent op, GameObject go) {
            this.op = op;
            this.go = go;
            this.canvas = go.GetComponent<CanvasGroup>();
            this.rect = go.GetComponent<RectTransform>();
            this.go.transform.parent = OpponentsHud.Instance.HudParent;
            this.opTransform = op.transform;
        }
    }

	void Start () {
        _instance = this;
        foreach (Transform t in OpponentsParent) {
            Opponents.Add(new OpponentRepresentation(t.GetComponent<Opponent>(),
                (GameObject) Instantiate(HUD, Vector3.zero, Quaternion.identity)));
        }

	    StartCoroutine("ComputeOpponentsDistance");
	}

    IEnumerator ComputeOpponentsDistance() {
        while (true) {
            foreach (OpponentRepresentation o in Opponents) {
                if (o.op.transform.position.z > Vehicle.position.z) {
                    float deltaDistance = o.op.transform.position.z - Vehicle.position.z;
                    o.canvas.alpha = alphaCurve.Evaluate(deltaDistance);
                    o.rect.localScale = new Vector3(3, 3, 3) * alphaCurve.Evaluate(deltaDistance);
                    //o.canvas.alpha = (AlphaFunctionDivider - deltaDistance + AlphaFunctionAdder) / deltaDistance;

                    Vector3 screenPoint = Cam.WorldToScreenPoint(o.opTransform.position);
                    //o.rect.position = screenPoint;
                    o.rect.position = new Vector3(((screenPoint.x/Screen.width) - 0.5f)*800,
                        ((screenPoint.y/Screen.height) - 0.5f)*600,
                        350f);
                }
                else {
                    o.canvas.alpha = 0f;
                }
            }
            yield return new WaitForSeconds(ComputeInterval);
        }
    }
}
