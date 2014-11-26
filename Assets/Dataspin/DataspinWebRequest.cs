using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DataspinWebRequest {

	private WWW www;
	private DataspinRequestMethod Dataspin_Method;
	private HttpRequestMethod HTTP_Method;
	private Error error = null;
	private string url;
	private Dictionary<string,object> jsonDict;
	private Dictionary<string,string> extraData;
	private string responseText = null;

	public Error Error {
		get { return error; }
		set { error = value; }
	}

	public bool isError() {
		if(error != null) return true;
		return false;
	}

	public Dictionary<string,string> ExtraData {
		get { return extraData; }
	}

	public DataspinRequestMethod DSMethod {
		get { return Dataspin_Method; }
	}

	public HttpRequestMethod HTTPMethod {
		get { return HTTP_Method; }
	}

	public string Response {
		get { return responseText; }
	}

	public string URL {
		get { return url; }
	}

	public Dictionary<string,object> Dict {
		get { return jsonDict; }
	}

	public DataspinWebRequest(string url, HttpRequestMethod httpMethod, DataspinRequestMethod dataspinMethod, Dictionary<string, string> extraData = null) {
		BeginRequest(url, httpMethod, dataspinMethod, extraData);
	}

	//Better than previous one. 
	public DataspinWebRequest(Dictionary<string,object> dictionary, HttpRequestMethod httpMethod, DataspinRequestMethod dataspinMethod, Dictionary<string, string> extraData = null) {
		jsonDict = dictionary;
		if(extraData != null) this.extraData = extraData;
		if((string)jsonDict["uid"] == null || (string)jsonDict["uid"] == "" || ((string)jsonDict["uid"]).Length < 1) {
			Debug.Log("DataspinWebRequest without UID detected, probably from tape. Adding UID from this session.");
			jsonDict["uid"] = Dataspin.Instance.Uid;
		}
		string encryptedData = Dataspin.CreateEncryptedData(jsonDict); 
		string url = Dataspin.Instance.CurrentConfiguration.BaseURL + encryptedData;

		BeginRequest(url, httpMethod, dataspinMethod, extraData);
	}

	private void BeginRequest(string url, HttpRequestMethod httpMethod, DataspinRequestMethod dataspinMethod, Dictionary<string, string> extraData = null) {
		Debug.Log("Starting DataspinWebRequest, Type: "+dataspinMethod.ToString() + ", URL: "+url);

		HTTP_Method = httpMethod;
		Dataspin_Method = dataspinMethod;
		this.url = url;
		www = null;

		switch (httpMethod)
		{
			case HttpRequestMethod.HttpMethod_Post:
				WWWForm form = new WWWForm();
				if(extraData != null) {
					foreach(KeyValuePair<string, string> item in extraData) {
					    string itemKey = item.Key;
					    string itemValue = item.Value;

					    form.AddField(itemKey, itemValue);
					    www = new WWW(URL, form);
					}
				}
				break;

			default :
				www = new WWW(URL);
				break;
		}
		Dataspin.Instance.StartChildCoroutine(Connector());
	}

	IEnumerator Connector() {
		yield return www;

		if (www.error == null) {
			responseText = www.text;
			Debug.Log("DataspinWebRequest response: "+www.text);
		}

		else {
			//Server unavailable or lack of proper internet connection. 
			//TODO: Request should be stored on "Tape"
			DataspinTape.Instance.AddToTape(this);
			Error = new Error(www.error);
		}

		Dataspin.Instance.RequestDidComplete(this);
	}
}
