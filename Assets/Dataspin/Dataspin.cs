using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;
using DataspinEncryption;

/*
 * Dataspin Unity Component
 * Version 0.1
 * Made by Rafal Wilinski
 * 
 * Please do not make modifications to this code. 
 *
 * Copyright 2014 Dataspin.io
 * 
 */

public class Dataspin : MonoBehaviour {

	#region Setup Variables
	public bool UseSanboxEnvironment = true;
	public string versionNumber;
	public ServerConfiguration SandboxConfiguration, LiveConfiguration;
	public ServerConfiguration CurrentConfiguration;
	#endregion


	#region Internal Variables
	private string Action_GetUid = "getuid";
	private string Action_StartSession = "appdata";
	private string Action_EndSession = "appdata";
	private string Action_PostRace = "scores";
	private string Action_CustomEvent = "register_event";
	private string Action_AddBalance = "addcoins";
	private string Action_GetBalance = "getcoinsamount";
	private string Action_RegisterGPlusID = "register_gplus_user_id";
	private string Action_CoinpackPurchased = "coinpackpurchased";
	private string Action_GetAppItems = "getappitems";
	private string Action_PurchaseItem = "purchaseitem";
	private string Action_CheckCoupon = "check_coupon";
	private string Action_RegisterMail = "register_email";
	private string Action_RegisterNickname = "nicknames";
	private string Action_ReportNotification = "notification";
	private string Action_GetGooglePlusIds = "get_random_gplus_ids";
	private string Action_UpdateAdID = "update_ad_id";
	#endregion


	#region Additional Variables
	private string uid;
	private string sessionId;
	private bool isSessionInProgress;

	public string Uid {
		get { return uid; }
	}
	#endregion


	#region Singleton
	private static string GameObjectName = "_DataspinManager";
	private static Dataspin _instance;
	public static Dataspin Instance {
		get {
			if (_instance == null) {
				CreateGameObject();
			}
			return _instance;
		}
	}

	static void CreateGameObject() {
		if (_instance == null || _instance.gameObject == null) {
			GameObject ds = new GameObject(GameObjectName);
			ds.AddComponent<Dataspin>();
		}
	}

	void Awake() {
		if (_instance == null) {
			_instance = this;
		}
		DontDestroyOnLoad(this.gameObject);
		CurrentConfiguration = UseSanboxEnvironment ? SandboxConfiguration : LiveConfiguration;
		
		#if UNITY_ANDROID
		System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => { 
			return true; 
		};
		#endif
	}
	#endregion


	#region Events

	public static event Action<string> DataspinUidRetrieved;
	public static event Action DataspinSessionStarted;
	public static event Action DataspinSessionEnded;
	public static event Action DataspinBankChanged;
	public static event Action<string> DataspinPurchaseFailed; //DataspinPurchaseFailed(string reason);
	public static event Action DataspinPurchaseSuccess;
	public static event Action<string> DataspinCouponDeclined; //DataspinCouponDeclined(string reason);
	public static event Action<string> DataspinCouponAccepted; //DataspinCouponAccepted(string params);
	public static event Action DataspinMailRegistered;
	public static event Action<string> DataspinNicknameChanged; //DataspinNicknameChanged(string nickname);
	public static event Action DataspinGooglePlusIdRegistered;
	public static event Action DataspinNicknameChangeFailed;
	public static event Action <List<object>> DataspinGooglePlusIds;
	public static event Action<Error> DataspinError;
	public static event Action<string> DataspinTapeExecutionProgress; //Report tape execution progress

	#endregion


	#region Calls

	public void GetUid(string ip = null, string geo = null, string GoogleCloudMessagingID = null, string playerId = null) {
		if(Application.internetReachability != NetworkReachability.NotReachable) {
			Log("Getting uid...");
			Dictionary<string,object> jsonDict = new Dictionary<string,object>();

			//Required parameters
			jsonDict.Add("action", Action_GetUid);
			jsonDict.Add("device_id", SystemInfo.deviceUniqueIdentifier);
			jsonDict.Add("device_type", SystemInfo.deviceModel);
			jsonDict.Add("av", versionNumber);
			#if UNITY_ANDROID 
				jsonDict.Add("platform", "a");
			#elif UNITY_IPHONE
				jsonDict.Add("platform", "i");
			#elif UNITY_EDITOR
				jsonDict.Add("platform", CurrentConfiguration.defaultPlatform);     
			#endif

			//Optional Parameters
			if(GoogleCloudMessagingID != null) jsonDict.Add("GCM_device_id", GoogleCloudMessagingID); //Google Cloud Messaging ID or Apple Push Token
			if(ip == null && Application.internetReachability != NetworkReachability.NotReachable) jsonDict.Add("ip", "auto"); //Auto - let server map IP on his own.
			else jsonDict.Add("ip", ip); //Or add ip manually
			if(playerId != null) jsonDict.Add("GCM_device_id", GoogleCloudMessagingID); //Google+ ID or Game Center Player ID

			string encryptedData = CreateEncryptedData(jsonDict); //Encrypt JsonDictionary into encrypted bytes

			string url = CurrentConfiguration.BaseURL + encryptedData;
			new DataspinWebRequest(url, HttpRequestMethod.HttpMethod_Get, DataspinRequestMethod.Dataspin_GetUid);
		}
		else {
			DataspinError(new Error("Check internet connection!", DataspinRequestMethod.Dataspin_GetUid));
		}
	}

	public void StartSession() {
		if(Application.internetReachability != NetworkReachability.NotReachable) {
			if(isSessionInProgress) {
				DataspinError(new Error("Session is already in progress! No need to create new one."));
			}
			else {
				if(uid == null) {
					DataspinError(new Error("Uid is null! Call Dataspin.GetUid first!", DataspinRequestMethod.Dataspin_StartSession));
				}
				Dictionary<string,object> jsonDict = new Dictionary<string,object>();

				//Required parameters
				jsonDict.Add("action", Action_StartSession);
				jsonDict.Add("uid", uid);
				jsonDict.Add("av", versionNumber);

				string encryptedData = CreateEncryptedData(jsonDict); //Encrypt JsonDictionary into encrypted bytes

				string url = CurrentConfiguration.BaseURL + encryptedData;
				new DataspinWebRequest(url, HttpRequestMethod.HttpMethod_Get, DataspinRequestMethod.Dataspin_StartSession);
			}
		}
		else {
			DataspinError(new Error("Check internet connection!", DataspinRequestMethod.Dataspin_StartSession));
		}
	}

	public void EndSession(int duration = 0) {
		if(!isSessionInProgress) {
			DataspinError(new Error("There is no session so you can't EndSession!"));
		}
		else {
			if(uid == null) {
				DataspinError(new Error("Uid is null! Call Dataspin.GetUid first!", DataspinRequestMethod.Dataspin_EndSession));
			}
			else {
				Dictionary<string,object> jsonDict = new Dictionary<string,object>();

				//Required parameters
				jsonDict.Add("action", Action_EndSession);
				jsonDict.Add("uid", uid);
				jsonDict.Add("sessid", sessionId);
				jsonDict.Add("av", versionNumber);
				if(duration != 0) jsonDict.Add("duration", duration);

				string encryptedData = CreateEncryptedData(jsonDict); //Encrypt JsonDictionary into encrypted bytes

				string url = CurrentConfiguration.BaseURL + encryptedData;
				new DataspinWebRequest(url, HttpRequestMethod.HttpMethod_Get, DataspinRequestMethod.Dataspin_EndSession);
			}
		}
	}

	public void CustomEvent(int id, string param = null, string param2 = null, string param3 = null, string param4 = null, string duration = null) {
		Dictionary<string,object> jsonDict = new Dictionary<string,object>();
		//Required parameters
		jsonDict.Add("action", Action_CustomEvent);
		jsonDict.Add("uid", uid);
		jsonDict.Add("id", id);

		//Optional parameters
		if(param != null) jsonDict.Add("param", param);
		if(param2 != null) jsonDict.Add("param2", param2);
		if(param3 != null) jsonDict.Add("param3", param3);
		if(param4 != null) jsonDict.Add("param4", param4);
		if(duration != null) jsonDict.Add("duration", duration);

		if(IsDataspinReady(DataspinRequestMethod.Dataspin_CustomEvent)) {
			new DataspinWebRequest(jsonDict, HttpRequestMethod.HttpMethod_Get, DataspinRequestMethod.Dataspin_CustomEvent);
		}
		else {
			DataspinTape.Instance.AddToTape(jsonDict);
		}
	}

	public void PostRace(int score, int distance = 0, int endTime = 0) {
		Dictionary<string,object> jsonDict = new Dictionary<string,object>();

		//Required
		jsonDict.Add("action", Action_PostRace);
		jsonDict.Add("uid", uid);
		jsonDict.Add("s", score);
		if(distance != 0) jsonDict.Add("d", distance);
		if(endTime != 0) jsonDict.Add("et", endTime);

		if(IsDataspinReady(DataspinRequestMethod.Dataspin_PostRace)) {
			new DataspinWebRequest(jsonDict, HttpRequestMethod.HttpMethod_Get, DataspinRequestMethod.Dataspin_PostRace);
		}
		else {
			//Save request to tape
			DataspinTape.Instance.AddToTape(jsonDict);
		}
	}

	public void GetBalance() {
		if(IsDataspinReady(DataspinRequestMethod.Dataspin_GetBalance)) {
			Dictionary<string,object> jsonDict = new Dictionary<string,object>();

			jsonDict.Add("action", Action_GetBalance);
			jsonDict.Add("uid", uid);

			string encryptedData = CreateEncryptedData(jsonDict); 

			string url = CurrentConfiguration.BaseURL + encryptedData;
			new DataspinWebRequest(url, HttpRequestMethod.HttpMethod_Get, DataspinRequestMethod.Dataspin_GetBalance);
		}
	}

	public void AddBalance(int count, string source, string order_id = null, bool as_total = false) {
		Dictionary<string,object> jsonDict = new Dictionary<string,object>();

		jsonDict.Add("action", Action_AddBalance);
		jsonDict.Add("uid", uid);
		jsonDict.Add("coins", count);
		jsonDict.Add("source", source);
		jsonDict.Add("as_total", as_total.ToString());
		if(order_id != null) jsonDict.Add("order_id", order_id);

		if(IsDataspinReady(DataspinRequestMethod.Dataspin_AddBalance)) {
			new DataspinWebRequest(jsonDict, HttpRequestMethod.HttpMethod_Get, DataspinRequestMethod.Dataspin_AddBalance);
		}
		else {
			//Save request to tape
			DataspinTape.Instance.AddToTape(jsonDict);
		}
	}

	public void GetAppItems(bool withPurchases = true) {
		if(IsDataspinReady(DataspinRequestMethod.Dataspin_GetAppItems)) {
			Dictionary<string,object> jsonDict = new Dictionary<string,object>();

			jsonDict.Add("action", Action_GetAppItems);
			jsonDict.Add("uid", uid);
			jsonDict.Add("with_purchases", withPurchases ? "1" : "0");

			string encryptedData = CreateEncryptedData(jsonDict); 

			string url = CurrentConfiguration.BaseURL + encryptedData;
			new DataspinWebRequest(url, HttpRequestMethod.HttpMethod_Get, DataspinRequestMethod.Dataspin_GetAppItems);
		}
	}

	public void PurchaseCoinpack(string sku, string source, string order_id = null, string campaign_id = null, int timestamp = 0, string purchase_token = null, bool isVerified = false, string apple_receipt = null) {

		Dictionary<string,object> jsonDict = new Dictionary<string,object>();

		jsonDict.Add("action", Action_CoinpackPurchased);
		jsonDict.Add("uid", uid);
		jsonDict.Add("coinpack_id", sku);
		jsonDict.Add("source", source);
		jsonDict.Add("verified", isVerified ? "1" : "0");
		if(order_id != null) jsonDict.Add("order_id", order_id);
		if(campaign_id != null) jsonDict.Add("campaign_id", campaign_id);
		if(timestamp != 0) jsonDict.Add("timestamp", timestamp);
		if(purchase_token != null) jsonDict.Add("purchase_token", purchase_token);
		if(timestamp != 0) jsonDict.Add("timestamp", timestamp);

		Dictionary<string, string> appleReceipt = null;
		if(apple_receipt != null) {
			appleReceipt = new Dictionary<string,string>();
			appleReceipt.Add("apple_receipt", apple_receipt);
		}

		if(IsDataspinReady(DataspinRequestMethod.Dataspin_CoinpackPurchased)) {
			new DataspinWebRequest(jsonDict, HttpRequestMethod.HttpMethod_Get, DataspinRequestMethod.Dataspin_CoinpackPurchased, appleReceipt);
		}
		else {
			//Save request to tape
			DataspinTape.Instance.AddToTape(jsonDict);
		}
	}

	public void PurchaseItem(string sku, int quantity, int cid = 0, double price = 0) { 
		Dictionary<string,object> jsonDict = new Dictionary<string,object>();

		jsonDict.Add("action", Action_PurchaseItem);
		jsonDict.Add("uid", uid);
		jsonDict.Add("iid", sku);
		jsonDict.Add("quantity", quantity);
		if(cid != 0) jsonDict.Add("cid", cid);
		if(price != 0) jsonDict.Add("price", price);

		if(IsDataspinReady(DataspinRequestMethod.Dataspin_PurchaseItem)) {
			new DataspinWebRequest(jsonDict, HttpRequestMethod.HttpMethod_Get, DataspinRequestMethod.Dataspin_PurchaseItem);
		}
		else {
			//Save request to tape
			DataspinTape.Instance.AddToTape(jsonDict);
		}
	}

	public void PurchaseCoinpack(string sku, string source, bool isVerified) { //Simplfied version, only 3 parameters required
		Dictionary<string,object> jsonDict = new Dictionary<string,object>();

		jsonDict.Add("action", Action_CoinpackPurchased);
		jsonDict.Add("uid", uid);
		jsonDict.Add("coinpack_id", sku);
		jsonDict.Add("source", source);
		jsonDict.Add("verified", isVerified ? "1" : "0");

		if(IsDataspinReady(DataspinRequestMethod.Dataspin_CoinpackPurchased)) {
			new DataspinWebRequest(jsonDict, HttpRequestMethod.HttpMethod_Get, DataspinRequestMethod.Dataspin_CoinpackPurchased);
		}
		else {
			//Save request to tape
			DataspinTape.Instance.AddToTape(jsonDict);
		}
	}

	public void RegisterGooglePlusId(string gpuid) {
		if(IsDataspinReady(DataspinRequestMethod.Dataspin_RegisterGPlusId)) {
			Dictionary<string,object> jsonDict = new Dictionary<string,object>();

			jsonDict.Add("action", Action_RegisterGPlusID);
			jsonDict.Add("uid", uid);
			jsonDict.Add("gpuid", gpuid);

			string encryptedData = CreateEncryptedData(jsonDict); 

			string url = CurrentConfiguration.BaseURL + encryptedData;
			new DataspinWebRequest(url, HttpRequestMethod.HttpMethod_Get, DataspinRequestMethod.Dataspin_RegisterGPlusId);
		}
	}

	public void RegisterMail(string mail) {
		if(IsDataspinReady(DataspinRequestMethod.Dataspin_RegisterMail)) {
			Dictionary<string,object> jsonDict = new Dictionary<string,object>();

			jsonDict.Add("action", Action_RegisterMail);
			jsonDict.Add("email", mail);
			jsonDict.Add("device_id", SystemInfo.deviceUniqueIdentifier);

			string encryptedData = CreateEncryptedData(jsonDict); 

			string url = CurrentConfiguration.BaseURL + encryptedData;
			new DataspinWebRequest(url, HttpRequestMethod.HttpMethod_Get, DataspinRequestMethod.Dataspin_RegisterMail);
		}
	}

	public void RegisterNickname(string nick) {
		if(IsDataspinReady(DataspinRequestMethod.Dataspin_RegisterNickname)) {
			Dictionary<string,object> jsonDict = new Dictionary<string,object>();

			jsonDict.Add("action", Action_RegisterNickname);
			jsonDict.Add("uid", uid);
			jsonDict.Add("nick", nick);

			string encryptedData = CreateEncryptedData(jsonDict); 

			string url = CurrentConfiguration.BaseURL + encryptedData;
			new DataspinWebRequest(url, HttpRequestMethod.HttpMethod_Get, DataspinRequestMethod.Dataspin_RegisterNickname);
		}
	}

	public void CheckNickname() {
		if(IsDataspinReady(DataspinRequestMethod.Dataspin_RegisterNickname)) {
			Dictionary<string,object> jsonDict = new Dictionary<string,object>();

			jsonDict.Add("action", Action_RegisterNickname);
			jsonDict.Add("uid", uid);

			string encryptedData = CreateEncryptedData(jsonDict); 

			string url = CurrentConfiguration.BaseURL + encryptedData;
			new DataspinWebRequest(url, HttpRequestMethod.HttpMethod_Get, DataspinRequestMethod.Dataspin_RegisterNickname);
		}
	}

	public void CheckCoupon(string code) {
		if(IsDataspinReady(DataspinRequestMethod.Dataspin_CheckCoupon)) {
			Dictionary<string,object> jsonDict = new Dictionary<string,object>();

			jsonDict.Add("action", Action_CheckCoupon);
			jsonDict.Add("uid", uid);
			jsonDict.Add("code", code);

			string encryptedData = CreateEncryptedData(jsonDict); 

			string url = CurrentConfiguration.BaseURL + encryptedData;
			new DataspinWebRequest(url, HttpRequestMethod.HttpMethod_Get, DataspinRequestMethod.Dataspin_CheckCoupon);
		}
	}

	public void ReportNotification(DataspinNotificationDeliveryType type, int cid = 0) {
		if(IsDataspinReady(DataspinRequestMethod.Dataspin_ReportNotification)) {
			Dictionary<string,object> jsonDict = new Dictionary<string,object>();

			jsonDict.Add("action", Action_ReportNotification);
			jsonDict.Add("uid", uid);
			jsonDict.Add("type", type.ToString());
			if(cid != 0) jsonDict.Add("cid", cid);

			string encryptedData = CreateEncryptedData(jsonDict); 

			string url = CurrentConfiguration.BaseURL + encryptedData;
			new DataspinWebRequest(url, HttpRequestMethod.HttpMethod_Get, DataspinRequestMethod.Dataspin_ReportNotification);
		}
	}

	public void GetRandomGooglePlusIds(int count) {
		//if(IsDataspinReady(DataspinRequestMethod.Dataspin_GetGooglePlusIds)) {
        //Safety check removed
			Dictionary<string,object> jsonDict = new Dictionary<string,object>();

			jsonDict.Add("action", Action_GetGooglePlusIds);
			jsonDict.Add("id_no", count);

			string encryptedData = CreateEncryptedData(jsonDict); 

			string url = CurrentConfiguration.BaseURL + encryptedData;
			new DataspinWebRequest(url, HttpRequestMethod.HttpMethod_Get, DataspinRequestMethod.Dataspin_GetGooglePlusIds);
		//}
	}

	public void RegisterAdID(string adid) {
		if(IsDataspinReady(DataspinRequestMethod.Dataspin_GetGooglePlusIds)) {
			Dictionary<string,object> jsonDict = new Dictionary<string,object>();

			jsonDict.Add("action", Action_UpdateAdID);
			jsonDict.Add("uid", uid);
			jsonDict.Add("adid", adid);

			string encryptedData = CreateEncryptedData(jsonDict); 

			string url = CurrentConfiguration.BaseURL + encryptedData;
			new DataspinWebRequest(url, HttpRequestMethod.HttpMethod_Get, DataspinRequestMethod.Dataspin_GetGooglePlusIds);
		}
	}

	#endregion



	#region Connection Management

	public void RequestDidComplete (DataspinWebRequest req) {
		if(req.isError()) {
			Log("Connection Error! Method: "+req.DSMethod.ToString() +", Error Code: "+req.Error.Code.ToString() +", Message: "+req.Error.Message);
		}
		else {
			Dictionary<string,object> dict = null;
			try {
				dict = Json.Deserialize(req.Response) as Dictionary<string,object>;
			}
			catch {
				Log("Error! Couldn't parse JSON to dictionary! Method: "+req.DSMethod.ToString() + ", ResponseJson: "+req.Response);
			}

			if(dict != null) {
				if((string)dict["result"] == "OK") {
					Log("Request "+req.DSMethod.ToString() + " complete!");
					try {
						switch(req.DSMethod) {
							case DataspinRequestMethod.Dataspin_GetUid :
								try {
									uid = GetInt(dict["uid"]).ToString();
									DataspinUidRetrieved(uid);
									Log("Uid set - "+uid);
									if(CurrentConfiguration.ShouldStartSessionAuto) StartSession();
								}
								catch {
									Log("Server response error! Parameter 'uid' missing! Method: "+req.DSMethod.ToString() + ", ResponseJson: "+req.Response);
								}
							break;

							case DataspinRequestMethod.Dataspin_StartSession :
								sessionId = ((int)(long)dict["sessid"]).ToString();
								isSessionInProgress = true;
								DataspinTape.Instance.GetTape();
								DataspinSessionStarted();
								Log("Session started! Id: "+sessionId);
							break;

							case DataspinRequestMethod.Dataspin_EndSession :
								isSessionInProgress = false;
								DataspinSessionEnded();
								sessionId = null;
								Log("Session ended!");
							break;

							case DataspinRequestMethod.Dataspin_CustomEvent : 
								Log("Custom Event sent!");
							break;

							case DataspinRequestMethod.Dataspin_GetBalance : 
								DataspinBank.Instance.setDataspinBank(GetInt(dict["total_coins"]), GetInt(dict["total_coins_B"]), GetInt(dict["purchases_no"]));
								DataspinBankChanged();
								Log("Get Balance finished!");
							break;

							case DataspinRequestMethod.Dataspin_AddBalance : 

								DataspinBank.Instance.setDataspinBank(GetInt(dict["purse_a"]), GetInt(dict["purse_b"]));
								//I'm not sure about that. Still doesn't work on vr2 domain. I suppose Marcin changed it only in beta DS version.
							
								DataspinBankChanged();
								Log("Add Balance success!");
							break;

							case DataspinRequestMethod.Dataspin_GetAppItems : 
								try {
									DataspinBank.Instance.SetPurchasesList((List<object>)dict["purchases"]); 
								}
								catch {
									Log("Get App items requested without purchases.");
								}
								
								DataspinBank.Instance.SetItemsList((List<object>)dict["items"]);
								Log("Get App Items success!");
							break;

							case DataspinRequestMethod.Dataspin_PurchaseItem : //It would be purrrfect if server could return purchase or purchasesArray
								if((string)dict["message"] == "ok") {
									DataspinPurchaseSuccess();
									Log("Purchase successful!");
								}
								else {
									Log("Something is wrong with purchase!");
									DataspinPurchaseFailed((string)dict["message"]);
								}
							break;

							case DataspinRequestMethod.Dataspin_CheckCoupon : 
								if((string)dict["message"] == "BAD CODE") {
									DataspinCouponDeclined("BAD CODE");
									Log("Bad code entered!");
								}
								else if((string)dict["message"] == "CODE EXPIRED") {
									DataspinCouponDeclined("CODE EXPIRED");
									Log("Entered code is expired!");
								}
								else if((string)dict["message"] == "CODE NOT ACTIVE YET") {
									DataspinCouponDeclined("CODE NOT ACTIVE YET");
									Log("Entered code is not ready!");
								}
								else {
									Log("Good code!");
									if(GetInt(dict["used_already"]) != 0) {
										DataspinCouponDeclined("USED ALREADY");
										Log("But already used!");
									}
									else {
										Log("And code is valid!");
										DataspinCouponAccepted((string)dict["params"]);
									}
								}
							break;

							case DataspinRequestMethod.Dataspin_RegisterMail : 
								Log("Mail registered!"); 
								DataspinMailRegistered();
							break;

							case DataspinRequestMethod.Dataspin_RegisterNickname : 
								if(((string)dict["message"]).Contains("NOT UNIQUE")) {
									DataspinNicknameChangeFailed();
								}
								else {
									DataspinNicknameChanged((string)dict["nick"]);
								}
							break;

							case DataspinRequestMethod.Dataspin_RegisterGPlusId :
								DataspinGooglePlusIdRegistered();
							break;

							case DataspinRequestMethod.Dataspin_GetGooglePlusIds : 
								Log("Google plus ids retrieved!"); 
								DataspinGooglePlusIds((List<object>) dict["ids"]);
							break;

							case DataspinRequestMethod.Dataspin_PostRace : 
								Log("Info about race sent! Total distance: "+((long)dict["distance"]).ToString()); 
							break;

							case DataspinRequestMethod.Dataspin_UpdateAdID :
								Log("Ad id registered!");
							break;

							default :
								LogWarning("Unsupported response! Message: "+(string)dict["message"]);
							break;
						}
					}
					catch(Exception e) {
						//LogWarning("Response parameters couldn't be parsed.");
						DataspinError(new Error("Couldn't parse json. MSG: "+(string)dict["message"]+" Details: "+e.Message + ", "+e.StackTrace, req.DSMethod));
					}
				}
				else if((string)dict["result"] == "ERROR") {
					if(req.DSMethod == DataspinRequestMethod.Dataspin_PurchaseItem) {
						if(((string)dict["message"]).Contains("not enough B")) {
							DataspinPurchaseFailed("notEnoughCurrency_B");
						}
						else if(((string)dict["message"]).Contains("not enough A")) { 
							DataspinPurchaseFailed("notEnoughCurrency_A");
						}
						else {
							DataspinError(new Error(req.Response, req.DSMethod));
						}
					}
				}
				else {
					if(req.DSMethod == DataspinRequestMethod.Dataspin_RegisterNickname) { 
						if(((string)dict["message"]).Contains("NOT UNIQUE")) {
							DataspinNicknameChangeFailed();
						}
					}
					else {
						DataspinError(new Error(req.Response, req.DSMethod));
					}
				}
			}
			else {
				DataspinError(new Error("Dictionary is null!"));
			}
		}
	}

	#endregion



	#region Helpers

	public void StartChildCoroutine(IEnumerator coroutineMethod) {
		StartCoroutine(coroutineMethod);
	}

	private bool IsDataspinReady(DataspinRequestMethod req) {
		if(Application.internetReachability != NetworkReachability.NotReachable) {
			if(!isSessionInProgress || sessionId == null) {
				DataspinError(new Error("There is no session active. Before "+req.ToString() +" call Dataspin.StartSession before that!", req));
				return false;
			}
			else {
				if(uid == null) {
					DataspinError(new Error("Uid is null! Before "+req.ToString() +" call Dataspin.GetUid first!", req));
					return false;
				}
				else {
					return true;
				}
			}
		}
		else {
			DataspinError(new Error("There is no internet connection!", req));
			return false;
		}
	}

	public static int getInt(object o) {
		return Dataspin.Instance.GetInt(o);
	}

	public int GetInt(object o) { //Return int from anything capable of
		try {
			return (int)(long)o; //Case when server returns normal INT32 but MiniJSON threats it like an object. E.g. "total_coins":44
		}
		catch {
			try {
				return int.Parse(o.ToString()); //Case when server returns string but its parsable to INT32. E.g. "total_coins":"44"
			}
			catch {
				try {
					int i;
					float f = float.Parse(o.ToString(),System.Globalization.CultureInfo.InvariantCulture.NumberFormat); //Case when server returns string but its parsable to float and then INT32. E.g. "total_coins":"44.00"
					i = (int) Math.Floor(f);
					return i;
				}
				catch {
					Log("Couldn't parse object to int. Subject: "+o.ToString());
					return -1;
				}
			}
		}
	}

	public static double getDouble(object o) {
		return Dataspin.Instance.GetDouble(o);
	}

	public double GetDouble(object o) {
		try{
			return (double)o;
		}
		catch{
			try {
				return double.Parse(o.ToString());
			}
			catch {
				try {
					float f = float.Parse(o.ToString(),System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
					return (double)f;
				}
				catch {
					Log("Couldn't parse object to double. Subject: "+o.ToString());
					return -1;
				}
			}
		}
	}

	public void FireDataspinBankChanged() {
		DataspinBankChanged();
	}

	public static string CreateEncryptedData(Dictionary<string, object> dict) {
		string dataJson = Json.Serialize(dict);
		Dataspin.Instance.Log("Encrypting Json: "+dataJson);
		return DataspinEncryptionService.CreateEncryptedString(dataJson);
	}

	#endregion


	#region Logging

	private void Log(string logline) {
		if (CurrentConfiguration.LoggingOn) {
			Debug.Log("Dataspin: " + logline);
		}
	}

	private void LogWarning(string logline) {
		if (CurrentConfiguration.LoggingOn) {
			Debug.LogError("Warning - Dataspin: " + logline);
		}
	}

	#endregion
}

[Serializable]
public class ServerConfiguration
{
	public string AppId, AppSecret, BaseURL;
	public bool LoggingOn, ShouldStartSessionAuto;
	public string defaultPlatform = "a"; // 'i' for iOS and 'a' for Android

	public string CreateBaseUrl() { //Add to Base URL AppID and AppSecret
		return null; //Just temporary
	}
}

public enum HttpRequestMethod {
	HttpMethod_Get = 0,
	HttpMethod_Put = 1,
	HttpMethod_Post = 2,
	HttpMethod_Delete = -1
}

public enum DataspinRequestMethod {
	Dataspin_GetUid = 0,
	Dataspin_StartSession = 1,
	Dataspin_EndSession = 2,
	Dataspin_CustomEvent = 3,
	Dataspin_PostRace = 4,
	Dataspin_GetBalance = 5,
	Dataspin_AddBalance = 6,
	Dataspin_GetAppItems = 7,
	Dataspin_CoinpackPurchased = 8,
	Dataspin_RegisterGPlusId = 9,
	Dataspin_PurchaseItem = 10,
	Dataspin_CheckCoupon = 11,
	Dataspin_RegisterMail = 12,
	Dataspin_RegisterNickname = 13,
	Dataspin_ReportNotification = 14,
	Dataspin_GetGooglePlusIds = 15,
	Dataspin_UpdateAdID = 16
}

public enum DataspinType {
	Dataspin_Item,
	Dataspin_Coinpack,
	Dataspin_Purchase
}

public enum DataspinNotificationDeliveryType {
	tapped,
	received
}
