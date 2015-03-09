using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;
using System;

public class DataspinBank : MonoBehaviour {

	private int coin_A_balance;
	private int coin_B_balance;
	private int purchasesCount;
	private List<DataspinItem> itemsList;
	private List<DataspinPurchase> purchasesList;


	#region Singleton
	private static DataspinBank _instance;

	public static DataspinBank Instance {
		get {
			if(_instance == null) CreateBankGameObject();
			return _instance;
		}
	}

	static void CreateBankGameObject() {
		if (_instance == null || _instance.gameObject == null) {
			GameObject ds = new GameObject("_DataspinBank");
			ds.AddComponent<DataspinBank>();
		}
	}

	void Awake() {
		if (_instance == null) {
			_instance = this;
		}
		DontDestroyOnLoad(this.gameObject);
	}
	#endregion

	public void setDataspinBank(int a, int b, int p) {
		coin_A_balance = a;
		coin_B_balance = b;
		purchasesCount = p;
	}

	public void setDataspinBank(int a, int b) {
		coin_A_balance = a;
		coin_B_balance = b;
	}

	public List<DataspinPurchase> PurchasesList {
		get {
			return purchasesList;
		}
	}

	public List<DataspinItem> ItemsList {
		get {
			return itemsList;
		}
	}

	public void SetPurchasesList(List<object> list) {
		int i = 0;
		purchasesList = new List<DataspinPurchase>();

		foreach(object o in list) {
			purchasesList.Add(new DataspinPurchase((Dictionary<string,object>) o));
			i++;
		}
		Log("Dataspin_Items serialization complete!");

		/* 
		foreach(DataspinItem b in itemsList) {
			Log(b.ToString()); //Just for debug purpouses.
		}
		*/
	}

	public void SetItemsList(List<object> list) {
		int i = 0;
		itemsList = new List<DataspinItem>();

		foreach(object o in list) {
			itemsList.Add(new DataspinItem((Dictionary<string,object>) o));
			i++;
		}
		Log("Dataspin_Items serialization complete!");
		Dataspin.Instance.FireDataspinBankChanged();
		/* 
		foreach(DataspinItem b in itemsList) {
			Log(b.ToString()); //Just for debug purpouses.
		}
		*/
	}

	public DataspinItem GetDataspinItemByName(string name) {
		foreach(DataspinItem item in itemsList) {
			if(item.Name == name) return item;
		}

		Log("Item not found!");
		return null;
	}

	public DataspinItem GetDataspinItemByIdentifier(string id) {
		foreach(DataspinItem item in itemsList) {
			if(item.Identifier == id) return item;
		}

		Log("Item not found!");
		return null;
	}

	public DataspinPurchase GetPurchaseByName(string name) {
		foreach(DataspinPurchase item in purchasesList) {
			if(item.Name == name) return item;
		}

		Log("Item not found!");
		return null;
	}

	public DataspinPurchase GetPurchaseByIdentifier(string id) {
		foreach(DataspinPurchase item in purchasesList) {
			if(item.Identifier == id) return item;
		}

		Log("Item not found!");
		return null;
	}

	public int Coins_A_Balance {
		get {
			return coin_A_balance;
		}
	}

	public int Coins_B_Balance {
		get {
			return coin_B_balance;
		}
	}

	public int PurchasesCount {
		get {
			return purchasesCount;
		}
	}

	#region Helpers

	private void Log(string msg) {
		if(Dataspin.Instance.CurrentConfiguration.LoggingOn) Debug.Log("Dataspin_Bank: "+msg);
	}

	public override string ToString() {
		return "Coins_A: "+Coins_A_Balance.ToString() + ", Coins_B: " + Coins_B_Balance.ToString() + ", Purchases Count: "+PurchasesCount.ToString();
	}

	#endregion
}
