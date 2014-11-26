using System;
using System.Collections.Generic;
using MiniJSON;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class EventsManager : MonoBehaviour {

    public bool isDebug;
	public Transform vehicle;
	public Vector3 baseRezOffset;
	public Vector3 randomness;
	public GameObject risingPillarPrefab;
	public OpponentsPool opponentPool;
	public float slowerSpeed;
	public float fasterSpeed;
    public Stack<NameAndPic> gPlusIds;

    private void Log(string msg) {
        if (isDebug) Debug.Log("Events: " + msg);
    }
	void Start() {
        gPlusIds = new Stack<NameAndPic>();
		RisingPillars(100);
		StartCoroutine("CreateOpponents");
	    StartCoroutine(waiting());
	    Dataspin.Instance.GetRandomGooglePlusIds(1);
	}

    [Serializable]
    public struct NameAndPic {
        public Texture2D pic;
        public string name;
    }
	
	public void StopAllEvents () {
	
	}


    void OnEnable() {
        Dataspin.DataspinGooglePlusIds += OnGooglePlusIdsReceived;
    }

    void OnDisable() {
        Dataspin.DataspinGooglePlusIds -= OnGooglePlusIdsReceived;
    }

    private void OnGooglePlusIdsReceived(List<object> obj) {
        Log("Fetching GooglePlus ids...");
        foreach (object o in obj) {
            Dictionary<string, object> dict = (Dictionary<string, object>)o;
            Log("Getting information about ID " + (string)dict["gplus_id"]);
            StartCoroutine(GetInformation((string) dict["gplus_id"]));
        }
        Log("Fetching complete!");
    }

    IEnumerator GetInformation(string gplus_id) {
        string url = "https://www.googleapis.com/plus/v1/people/" + gplus_id + "?key=AIzaSyCVqa4G6A7UkkKHmSsZVaWlgJYVnmcduf8"; //100601054994187376077
        Log("New request: "+url);
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
            Dataspin.Instance.GetRandomGooglePlusIds(1);
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

	public void RisingPillars(int count) {
		StartCoroutine("RisingPillarsCoroutine", count);
	}

	private IEnumerator RisingPillarsCoroutine(int count) {
		int i = 0;
		Vector3 position;
		while(i<count) {
			position = vehicle.position + baseRezOffset + new Vector3(Random.Range(-randomness.x, randomness.x), Random.Range(-randomness.y, randomness.y),Random.Range(-randomness.z, randomness.z));
			Instantiate(risingPillarPrefab, position, Quaternion.identity);
			i++;
			yield return new WaitForSeconds(1f);
		}
	}

    IEnumerator waiting() {
        float i = Mathf.Pow(Random.value, Random.value);
        yield return new WaitForEndOfFrame();
        StartCoroutine(waiting());
    }
	
}
