using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

using MiniJSON;

public class DataspinTape : MonoBehaviour {


	//DataspinTape class is responsible for storaging unexecuted requests caused by lack of internet connection or server unavailability.

	public static bool isDebug = true;

	public static float secondsToCloseTape = 5.0f;

	public static string PREFERENCES_KEY = "dataspinTape"; //PlayerPrefs key, remember that is final and shouldn't be changed in future versions. 

	public static string ENCRYPTION_KEY = "39de5d3b2a503633"; //Key used in PlayerPrefs encryption, just like preferences key, shouldn't be changed after release 

	public Dictionary<string,object> tapeJson;
	public List<object> jsonList;

	//UPDATE: don't give developer option to select algorithm, give one working instead

	#region Singleton
	private static string GameObjectName = "Dataspin";
	private static DataspinTape _instance;
	public static DataspinTape Instance {
		get {
			if (_instance == null) {
				CreateGameObject();
			}
			return _instance;
		}
	}

	static void CreateGameObject() {
		if (_instance == null || _instance.gameObject == null) {
			GameObject ds = GameObject.Find(GameObjectName);
			ds.AddComponent<DataspinTape>();
		}
	}

	void Awake() {
		if (_instance == null) {
			_instance = this;
		}
		DontDestroyOnLoad(this.gameObject);
	}
	#endregion



	private static void Log(string msg) {
		if(isDebug) Debug.Log("DataspinTape: "+msg);
	}



	public void DeleteTape() {
		PlayerPrefs.DeleteKey(PREFERENCES_KEY);
		Log("Tape deleted!");
	}

	public void AddToTape(Dictionary<string,object> jsonDict) {
		Dictionary<string,object> record = new Dictionary<string,object>();
		record["json"] = Json.Serialize(jsonDict);

        if (jsonList == null) jsonList = new List<object>();
		jsonList.Add(record); //Add record to list

		StartSaveTimer();
	}

	public void AddToTape(DataspinWebRequest request) {
		Dictionary<string,object> record = new Dictionary<string,object>();

		//if(request.Dict != null) record["json"] = Json.Serialize(request.Dict);
		if(request.URL != null) record["url"] = request.URL;
		if(request.ExtraData != null) {
			record["extra_data"] = request.ExtraData;
		}

		record["http_method"] = (int)request.HTTPMethod;
		record["ds_method"] = (int)request.DSMethod;
        
        if(jsonList == null) jsonList = new List<object>();
		jsonList.Add(record); //Add record to list

		StartSaveTimer();
	}

	public void GetTape() {
		Log("Getting tape...");

		jsonList = new List<object>();
		string encryptedTape = PlayerPrefs.GetString(PREFERENCES_KEY);
		string tape = null;
		if(encryptedTape.Length > 0) tape = Decrypt(PlayerPrefs.GetString(PREFERENCES_KEY));
		else {
			Log("There is no tape!");
			return;
		}

		Log("Decrypted tape: "+tape);
		tapeJson = Json.Deserialize(tape) as Dictionary<string,object>;

		if(tapeJson.ContainsKey("array")) {
			jsonList = (List<object>)tapeJson["array"];
			if(jsonList.Count < 1) {
				Log("Tape is empty!");
			}
			else {
				Log("Tape length: "+jsonList.Count.ToString());
				foreach(object record in jsonList) {
					ParseRecord(record);
					jsonList.Remove(record);
				}
			}
			DeleteTape();
		}
		else {
			Log("There is no tape!");
		}
	}

	public void TestAES() {
		string data = "array:[\"key\":\"value\"]";
		for(int i = 0; i<15; i++) data += data; //String concencation is bad, I know, I know...
		string encrypted = Encrypt(data);
		Log("Encrypted: "+encrypted);
		string decrypted = Decrypt(encrypted);
		Log("Decrypted: "+decrypted);
	}

	private void ParseRecord(object record) {
		Dictionary<string,object> json = new Dictionary<string,object>();
		//json = Json.Deserialize((string)record) as Dictionary<string,object>;
		//Or direct conversion into dict
		json = (Dictionary<string,object>)record;

		Dictionary<string,object> extraData = null;
		Dictionary<string, string> trueExtraData = new Dictionary<string, string>(); 
		try {
			extraData = (Dictionary<string,object>) json["extra_data"];
			//to samo co wyzej...

			foreach (KeyValuePair<string, object> keyValuePair in extraData) {
		  		trueExtraData.Add(keyValuePair.Key, keyValuePair.Value.ToString());
			}
		}
		catch(System.Exception e) {
			//No extra data at all
		}

		new DataspinWebRequest((string)json["url"], (HttpRequestMethod)(int)(long)json["http_method"], (DataspinRequestMethod) (int)(long)json["ds_method"], trueExtraData); //Execute request
	}


	private string prepareDataspinWebRequestStoringJson(DataspinWebRequest request) {
		//Parse DataspinWebRequest into JSON, decode it
		return null;
	}	

	private void StartSaveTimer() {

		StopCoroutine("Timer");
		StartCoroutine("Timer");
	}

	private IEnumerator Timer() {
		Log(secondsToCloseTape.ToString("f1")+" seconds to tape save...");

		yield return new WaitForSeconds(secondsToCloseTape);

		tapeJson = new Dictionary<string,object>();
		tapeJson["array"] = jsonList; 

		string encrypted = Encrypt(Json.Serialize(tapeJson));
		PlayerPrefs.SetString(PREFERENCES_KEY, encrypted);

		Log("Tape saved!");

	}

    public static string Encrypt (string toEncrypt)
	{
		var startTime = Time.realtimeSinceStartup;
		 byte[] keyArray = System.Text.UTF8Encoding.UTF8.GetBytes (ENCRYPTION_KEY);
		 // 256-AES key
		 byte[] toEncryptArray = System.Text.UTF8Encoding.UTF8.GetBytes (toEncrypt);
		 RijndaelManaged rDel = new RijndaelManaged ();
		 rDel.Key = keyArray;
		 rDel.Mode = CipherMode.ECB;
		 // http://msdn.microsoft.com/en-us/library/system.security.cryptography.ciphermode.aspx
		 rDel.Padding = PaddingMode.PKCS7;
		 // better lang support
		 ICryptoTransform cTransform = rDel.CreateEncryptor ();
		 byte[] resultArray = cTransform.TransformFinalBlock (toEncryptArray, 0, toEncryptArray.Length);
		 Log("Encryption of string.Length = "+toEncrypt.Length+" in "+((Time.realtimeSinceStartup - startTime)*1000).ToString("f6")+"ms");
		 return Convert.ToBase64String (resultArray, 0, resultArray.Length);
	}
	 
	public static string Decrypt (string toDecrypt)
	{
		var startTime = Time.realtimeSinceStartup;
		 byte[] keyArray = System.Text.UTF8Encoding.UTF8.GetBytes (ENCRYPTION_KEY);
		 // AES-256 key
		 byte[] toEncryptArray = Convert.FromBase64String (toDecrypt);
		 RijndaelManaged rDel = new RijndaelManaged ();
		 rDel.Key = keyArray;
		 rDel.Mode = CipherMode.ECB;
		 // http://msdn.microsoft.com/en-us/library/system.security.cryptography.ciphermode.aspx
		 rDel.Padding = PaddingMode.PKCS7;
		 // better lang support
		 ICryptoTransform cTransform = rDel.CreateDecryptor ();
		 byte[] resultArray = cTransform.TransformFinalBlock (toEncryptArray, 0, toEncryptArray.Length);
		  Log("Decryption of string.Length = "+toDecrypt.Length+" in "+((Time.realtimeSinceStartup - startTime)*1000).ToString("f6")+"ms");
		 return System.Text.UTF8Encoding.UTF8.GetString (resultArray);
	}
}
