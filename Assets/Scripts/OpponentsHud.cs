using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;
using UnityEngine.UI;

public class OpponentsHud : MonoBehaviour
{

    public List<OpponentRepresentation> Opponents;
    public Transform HudParent;
    public GameObject HUD;
    public Transform OpponentsParent;
    public Transform Vehicle;
    public Camera Cam;
    public AnimationCurve alphaCurve;
    public Texture2D defaultProfilePic;

    public float ComputeInterval;
    public Vector3 hudOffset;

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
                (GameObject) Instantiate(HUD, new Vector3(1000,0,0), Quaternion.identity)));
        }

        //Dataspin.Instance.GetRandomGooglePlusIds(10);

	    StartCoroutine("ComputeOpponentsDistance");
	}

    public void SetHudNameAndPic(string name, Texture2D pic, Opponent sender) {
        if (name == null || name == "") name = "User_" + UnityEngine.Random.Range(100, 1000);
        if (pic == null) pic = defaultProfilePic;
        foreach (OpponentRepresentation o in Opponents) {
            if (o.op == sender) {
                foreach (Transform op in o.canvas.transform) {
                    if (op.gameObject.name == "Name") op.GetComponent<Text>().text = name;
                    else if (op.gameObject.name == "RawImage") op.GetComponent<RawImage>().texture = pic;
                }
            }
        }
    }

    IEnumerator ComputeOpponentsDistance() {
        while (true) {
            foreach (OpponentRepresentation o in Opponents) {
                if (o.op.transform.position.z > Vehicle.position.z) {
                    float deltaDistance = o.op.transform.position.z - Vehicle.position.z;
                    o.canvas.alpha = alphaCurve.Evaluate(deltaDistance);
                    o.rect.localScale = new Vector3(5, 5, 5) * alphaCurve.Evaluate(deltaDistance);
                    //o.canvas.alpha = (AlphaFunctionDivider - deltaDistance + AlphaFunctionAdder) / deltaDistance;

                    Vector3 screenPoint = Cam.WorldToScreenPoint(o.opTransform.position);
                    o.rect.position = new Vector3(((screenPoint.x/Screen.width) - 0.5f)*800,
                        ((screenPoint.y/Screen.height) - 0.5f)*600,
                        350f) + hudOffset;
                }
                else {
                    o.canvas.alpha = 0f;
                }
            }
            yield return new WaitForSeconds(ComputeInterval);
        }
    }
}
