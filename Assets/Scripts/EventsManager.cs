using System;
using System.Collections.Generic;
using System.Linq;
using MiniJSON;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class EventsManager : MonoBehaviour {

    public bool isDebug;
	public Transform vehicle;
	public Vector3 randomness;
	public OpponentsPool opponentPool;
    public ContinousMovement mov;
	public float slowerSpeed;
	public float fasterSpeed;
    public CatmullRomSpline spline;

    public Stack<NameAndPic> gPlusIds;
    public List<Props> propsList; 

    [SerializeField]
    private List<CoroutineObject> ongoingEvents;

    private void Log(string msg) {
        if (isDebug) Debug.Log("Events: " + msg);
    }

    [Serializable]
    public class Props {
        public string name;
        public float minSpawnInterval;
        public float maxSpawnInterval;
        public GameObject stackParent;
        public Stack<GameObject> stack;
        public bool isEventInProgress;
        public int availableObjects;
        public bool worldPos = false;
        public Vector3 propRezOffset;
        public Vector3 randomness;
        public string extraData = null;
    }

    [Serializable]
    public struct NameAndPic {
        public Texture2D pic;
        public string name;
    }

    [Serializable]
    internal class CoroutineObject {
        public Props p;
        public CoroutineObject(Props e) { 
            this.p = e;
            EventsManager.Instance().StartCoroutine("EventCoroutine",this.p); 
        }

        ~CoroutineObject() {
            EventsManager.Instance().StopCoroutine("EventCoroutine");
        }
    }

    public static EventsManager Instance() {
        return (EventsManager) GameObject.Find("EventsManager").GetComponent<EventsManager>() as EventsManager;
    }

	void Start() {
        gPlusIds = new Stack<NameAndPic>();
		StartCoroutine("CreateOpponents");

	    for(int i = 0; i < propsList.Count; i++) {
            propsList[i].stack = new Stack<GameObject>();
	        foreach (Transform t in propsList[i].stackParent.transform) {
                propsList[i].stack.Push(t.gameObject);
	        }
	        propsList[i].availableObjects = propsList[i].stack.Count;
            StartEvent(propsList[i].name);
	    }
	}

    public void EnableEvent(String name) {
         for(int i = 0; i < propsList.Count; i++) {
            if(name == propsList[i].name) {
                Log("Event " + name + " enabled!");
                propsList[i].isEventInProgress = true;
            }
         }
    }

    public void DisableEvent(String name) {
         for(int i = 0; i < propsList.Count; i++) {
            if(name == propsList[i].name) {
                Log("Event " + name + " disabled!");
                propsList[i].isEventInProgress = false;
            }
         }
    }

    public void DisableAllEvents() {
        for(int i = 0; i < propsList.Count; i++) {
            propsList[i].isEventInProgress = false;
         }

         
    }
	
	public void StopAllEvents () {
	   foreach(CoroutineObject o in ongoingEvents) {
            StopCoroutine("EventCoroutine");
       }
	}

    void OnEnable() {
        Dataspin.DataspinGooglePlusIds += OnGooglePlusIdsReceived;
    }

    void OnDisable() {
        Dataspin.DataspinGooglePlusIds -= OnGooglePlusIdsReceived;
    }

    private void OnGooglePlusIdsReceived(List<object> obj) {
        foreach (object o in obj) {
            Dictionary<string, object> dict = (Dictionary<string, object>)o;
            StartCoroutine(GetInformation((string) dict["gplus_id"]));
        }
    }

    IEnumerator GetInformation(string gplus_id) {
        string url = "https://www.googleapis.com/plus/v1/people/" + gplus_id + "?key=AIzaSyCVqa4G6A7UkkKHmSsZVaWlgJYVnmcduf8"; //100601054994187376077
        WWW www = new WWW(url);
        yield return www;
        if (www.error == null) {
            WWW www2 = null;
            NameAndPic container = new NameAndPic();
            try {
                Dictionary<string, object> dict = Json.Deserialize(www.text) as Dictionary<string, object>;
                Dictionary<string, object> nameDict = (Dictionary<string, object>) dict["name"];
                container.name = (string) nameDict["givenName"];
                nameDict = (Dictionary<string, object>) dict["image"];
                www2 = new WWW((string) nameDict["url"]);
            }
            catch (Exception e) {
                Log("Exception: " + e.Message);
            }

            yield return www2;
            if (www.error == null && www.texture != null) {
                container.pic = www2.texture;
                Log("Success! Name: "+container.name);
                gPlusIds.Push(container);
            }
            else {
                Log("Error while downloading picture! " + www2.error);
            }
        }
        else {
            Log("Error while getting profile from base URL. "+www.error);
        }
    }

	private IEnumerator CreateOpponents() {
		while(true) {
			yield return new WaitForSeconds(3f);
            Opponent op = opponentPool.GetFirstAvailable();
		    NameAndPic cont = new NameAndPic();
		    if(gPlusIds.Count > 0) cont = gPlusIds.Pop();
            if (op != null) {
                if (Random.Range(0, 1000) % 2 == 1) {
                    Log("Creating slower opponent!");
                    op.Create(vehicle.position + new Vector3(0, 0, 800), slowerSpeed, cont.name, cont.pic);
                }
                else {
                    Log("Creating faster opponent!");
                    op.Create(vehicle.position + new Vector3(0, 0, -150), fasterSpeed, cont.name, cont.pic);
                }
            }
            else {
                Log("Wanted to create Opponent but there was no ops available.");
            }
		}
	}

    public void ReturnEventObject(GameObject o) {
        bool isReturned = false;
        foreach (Props p in propsList) {
            if (o.name == p.name) {
                isReturned = true;
                p.stack.Push(o);
                p.availableObjects = p.stack.Count;
            }
        }

        if(!isReturned) Log("Warning! "+o.name+ " has been not returned to the stack correctly!");
    }

    private Props FindPropByName(string name) {
        foreach (Props p in propsList) {
            if (p.name == name) return p;
        }
        return new Props();
    }

    private IEnumerator StartEvent(string eventName) {
        Props p = FindPropByName(eventName);
        ongoingEvents.Add(new CoroutineObject(p));
        return null;
    }

    IEnumerator EventCoroutine(Props p) {
        Vector3 position = new Vector3(0,0,0);
        int i = 0;
        while (true) {
            if (p.stack.Count > 0 && p.isEventInProgress && mov.isPlaying) {

                if(!p.worldPos) {
                    position = vehicle.position + p.propRezOffset + 
                               new Vector3(Random.Range(-p.randomness.x, p.randomness.x), Random.Range(-p.randomness.y, p.randomness.y),
                                   Random.Range(-p.randomness.z, p.randomness.z));

                    position += new Vector3(0, spline.GetPositionAtTime(spline.GetClosestPointAtSpline(position, 10)).y, 0); //Affect only Y-Axis
                }
                else {
                    position = p.propRezOffset + new Vector3(Random.Range(-p.randomness.x, p.randomness.x), Random.Range(-p.randomness.y, p.randomness.y),
                                   Random.Range(-p.randomness.z, p.randomness.z));
                }

                p.stack.Pop().GetComponent<PropPoolObject>().Create(position, p.extraData);
                p.availableObjects = p.stack.Count;
                i++;
            }
            yield return new WaitForSeconds(Random.Range(p.minSpawnInterval, p.maxSpawnInterval));
        }
    }
}
