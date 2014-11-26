using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataspinExample : MonoBehaviour {


	#region Public variables

	public bool IsLogging = true;
    public bool shouldShowGUI = true;

	//Example values
	public int customEventId = 105;
	public int raceScore = 436634;
	public int raceDistance = 46336;
	public int addBalanceAmount = 1234;
	public string addBalanceSource = "GAME";
	public string purchaseItemSku = "headstart_1";
	public int purchaseItemQuantity = 2;
	public string googlePlusId = "6574762353";
	public string exampleMail = "enter@yourmail.here";
	public string couponString = "<enter here>";
	public string nickname = "nickname";
	public string productSKU = "sku";
	public string productSource = "GOOG";
	#endregion



	#region Private variables

	private int buttonWidth = 100;
	private int buttonHeight = 30;
	private int spacing = 10;

	#endregion



	#region Subscriptions and Unsubscriptions

	void OnEnable() {
		Dataspin.DataspinUidRetrieved += OnUidRetrieved;
		Dataspin.DataspinSessionStarted += OnSessionStarted;
		Dataspin.DataspinSessionEnded += OnSessionEnded;
		Dataspin.DataspinBankChanged += OnBankChanged;
		Dataspin.DataspinPurchaseFailed += OnPurchaseFailed;
		Dataspin.DataspinPurchaseSuccess += OnPurchaseSuccess;
		Dataspin.DataspinCouponDeclined += OnCouponDeclined;
		Dataspin.DataspinCouponAccepted	+= OnCouponAccepted;
		Dataspin.DataspinMailRegistered += OnMailRegistered;
		Dataspin.DataspinNicknameChanged += OnNicknameChanged;
		Dataspin.DataspinNicknameChangeFailed += OnNicknameChangeFailed;
		Dataspin.DataspinGooglePlusIds += OnGooglePlusIdsGot;
		Dataspin.DataspinError += OnError;
		Dataspin.DataspinGooglePlusIdRegistered += OnGooglePlusRegistered; 
	}

	void OnDisable() {
		Dataspin.DataspinUidRetrieved -= OnUidRetrieved;
		Dataspin.DataspinSessionStarted -= OnSessionStarted;
		Dataspin.DataspinSessionEnded -= OnSessionEnded;
		Dataspin.DataspinBankChanged -= OnBankChanged;
		Dataspin.DataspinPurchaseFailed -= OnPurchaseFailed;
		Dataspin.DataspinPurchaseSuccess -= OnPurchaseSuccess;
		Dataspin.DataspinCouponDeclined -= OnCouponDeclined;
		Dataspin.DataspinCouponAccepted	-= OnCouponAccepted;
		Dataspin.DataspinMailRegistered -= OnMailRegistered;
		Dataspin.DataspinNicknameChanged -= OnNicknameChanged;
		Dataspin.DataspinNicknameChangeFailed -= OnNicknameChangeFailed;
		Dataspin.DataspinGooglePlusIds -= OnGooglePlusIdsGot;
		Dataspin.DataspinError -= OnError;
		Dataspin.DataspinGooglePlusIdRegistered -= OnGooglePlusRegistered; 
	}

	#endregion



	#region Helpers

	private void Log(string msg) {
		if(IsLogging) Debug.Log("DataspinExample: "+msg);
	}

	private void BeginNewColumn() {

	}

	#endregion



	#region Example Functions

	void Start () {
		Dataspin.Instance.GetUid();
	}

	void OnGUI() {
	    if (shouldShowGUI) {
	        if (GUI.Button(new Rect(spacing, 10, buttonWidth, buttonHeight), "Start Session"))
	            Dataspin.Instance.StartSession();

	        if (GUI.Button(new Rect(spacing, 50, buttonWidth, buttonHeight), "End Session"))
	            Dataspin.Instance.EndSession();

	        if (GUI.Button(new Rect(spacing, 90, buttonWidth, buttonHeight), "Get Balance"))
	            Dataspin.Instance.GetBalance();

	        if (GUI.Button(new Rect(spacing, 130, buttonWidth, buttonHeight), "Get App Items"))
	            Dataspin.Instance.GetAppItems();

	        if (GUI.Button(new Rect(spacing, 170, buttonWidth, buttonHeight), "Custom Event"))
	            Dataspin.Instance.CustomEvent(customEventId);

	        if (GUI.Button(new Rect(spacing, 210, buttonWidth, buttonHeight), "Post Race"))
	            Dataspin.Instance.PostRace(raceScore, raceDistance);

	        if (GUI.Button(new Rect(spacing, 250, buttonWidth, buttonHeight), "Report Nofif"))
	            Dataspin.Instance.ReportNotification(DataspinNotificationDeliveryType.tapped);

	        if (GUI.Button(new Rect(spacing, 290, buttonWidth, buttonHeight), "Add to tape")) {
	            Dictionary<string, object> jsonDict = new Dictionary<string, object>();

	            jsonDict.Add("action", "scores");
	            jsonDict.Add("uid", Dataspin.Instance.Uid);
	            jsonDict.Add("s", 452345);

	            DataspinWebRequest req = new DataspinWebRequest(jsonDict, HttpRequestMethod.HttpMethod_Get,
	                DataspinRequestMethod.Dataspin_PostRace);
	            DataspinTape.Instance.AddToTape(req);
	        }

	        if (GUI.Button(new Rect(spacing, 330, buttonWidth, buttonHeight), "Register Ad Id"))
	            Dataspin.Instance.RegisterAdID("abcd");

	        if (GUI.Button(new Rect(spacing*2 + buttonWidth, 10, buttonWidth, buttonHeight), "Add Balance"))
	            Dataspin.Instance.AddBalance(addBalanceAmount, addBalanceSource);

	        if (GUI.Button(new Rect(spacing*2 + buttonWidth, 50, buttonWidth, buttonHeight), "Purchase Item"))
	            Dataspin.Instance.PurchaseItem(purchaseItemSku, purchaseItemQuantity);

	        if (GUI.Button(new Rect(spacing*2 + buttonWidth, 90, buttonWidth, buttonHeight), "Register G+ ID"))
	            Dataspin.Instance.RegisterGooglePlusId(googlePlusId);

	        if (GUI.Button(new Rect(spacing*2 + buttonWidth, 130, buttonWidth, buttonHeight), "Register Mail"))
	            Dataspin.Instance.RegisterMail(exampleMail);

	        if (GUI.Button(new Rect(spacing*2 + buttonWidth, 170, buttonWidth, buttonHeight), "Register Nick"))
	            Dataspin.Instance.RegisterNickname(nickname);

	        if (GUI.Button(new Rect(spacing*2 + buttonWidth, 210, buttonWidth, buttonHeight), "Get G+ Ids"))
	            Dataspin.Instance.GetRandomGooglePlusIds(5);

	        if (GUI.Button(new Rect(spacing*2 + buttonWidth, 250, buttonWidth + 25, buttonHeight), "Purchase coinpack"))
	            Dataspin.Instance.PurchaseCoinpack(productSKU, productSource, true);

	        if (GUI.Button(new Rect(spacing*2 + buttonWidth, 290, buttonWidth, buttonHeight), "Test encrypt"))
	            DataspinTape.Instance.TestAES();

	        if (GUI.Button(new Rect(spacing*2 + buttonWidth, 330, buttonWidth, buttonHeight), "Delete Tape"))
	            DataspinTape.Instance.DeleteTape();
	    }
	}

	#endregion




	#region Event Delegates

	private void OnUidRetrieved(string uid) {
		Log("DataspinUidRetrieved, Uid: "+uid);
	}

	private void OnSessionStarted() {
		Log("DataspinSessionStarted");
	}

	private void OnSessionEnded() {
		Log("DataspinSessionEnded");
	}

	private void OnBankChanged() {
		Log("DataspinBankChanged");
	}

	private void OnPurchaseFailed(string reason) {
		Log("DataspinPurchaseFailed, Reason: "+reason);
	}

	private void OnPurchaseSuccess() {
		Log("DataspinPurchaseSuccess");
	}

	private void OnCouponDeclined(string reason) {
		Log("DataspinCouponDeclined, Reason: "+reason);
	}

	private void OnCouponAccepted(string parameters) {
		Log("DataspinCouponAccepted, Parameters: "+parameters);
	}

	private void OnMailRegistered() {
		Log("DataspinMailRegistered");
	}

	private void OnNicknameChanged(string nickname) {
		Log("DataspinNicknameChanged, Nick: "+nickname);
	}

	private void OnGooglePlusRegistered() {
		Log("OnGooglePlusRegistered");
	}

	private void OnNicknameChangeFailed() {
		Log("DataspinNicknameChangeFailed");
	}

	private void OnError(Error err) {
		Debug.LogWarning("Dataspin Example: DataspinError: "+err.ToString());
	}

	private void OnGooglePlusIdsGot(List<object> googlePlusIds) {
		Log("DataspinGooglePlusIds");
		foreach(Dictionary<string,object> item in googlePlusIds) {
			foreach(KeyValuePair<string,object> kvp in item) {
				Log(kvp.Key + " : " + kvp.Value);
			}
		}
	}

	#endregion
}
