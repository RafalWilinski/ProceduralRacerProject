using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

public class DataspinPurchase : MonoBehaviour {

	private int iid; //Numerical id, for us it's useless
	private string purchase_date;
	private int price;
	private int campaign_id;
	private string internal_id; //IAB id and/or Dataspin console id
	private string _name;
	private int coinsAdded; //0 for item
	private int coinsAdded_B; //0 for item
	private int amount;

	private const string JsonKey_Iid = "iid";
	private const string JsonKey_InternalName = "internal_id";
	private const string JsonKey_Name = "name";
	private const string JsonKey_CoinsAdded = "coins_added";
	private const string JsonKey_CoinsAddedB = "coins_B_added";
	private const string JsonKey_Price = "price";
	private const string JsonKey_CampaignId = "cid";
	private const string JsonKey_Amount = "amount";
	private const string JsonKey_PurchaseDate = "purchase_date";

	public DataspinPurchase(string json) {
		JsonToDict(json);
	}

	private void JsonToDict (string json) {
		var dict = Json.Deserialize(json) as Dictionary<string,object>;
		InitializeFromDict(dict);
	}

	public DataspinPurchase(Dictionary <string, object> dict) {
		InitializeFromDict(dict);
	}

	private void InitializeFromDict(Dictionary<string, object> dict) {

		/*
		foreach(KeyValuePair<string, object> p in dict) {
			Debug.Log(p.Key + " : " + p.Value); //Just help.
		}
		*/
		
		iid = (dict.ContainsKey (JsonKey_Iid)) ? Dataspin.getInt(dict[JsonKey_Iid]) : -1;
		internal_id = (dict.ContainsKey (JsonKey_InternalName)) ? (string)dict[JsonKey_InternalName] : null;
		_name = (dict.ContainsKey (JsonKey_Name)) ? (string)dict[JsonKey_Name] : null;
		price = (dict.ContainsKey (JsonKey_Price)) ? Dataspin.getInt(dict[JsonKey_Price]) : -1;
		campaign_id = (dict.ContainsKey (JsonKey_CampaignId)) ? Dataspin.getInt(dict[JsonKey_CampaignId]) : -1;
		purchase_date = (dict.ContainsKey (JsonKey_PurchaseDate)) ? (string)dict[JsonKey_Name] : null;
		coinsAdded = (dict.ContainsKey (JsonKey_CoinsAdded)) ? Dataspin.getInt(dict[JsonKey_Name]) : -1;
		coinsAdded_B = (dict.ContainsKey (JsonKey_CoinsAddedB)) ? Dataspin.getInt(dict[JsonKey_Name]) : -1;
		amount = (dict.ContainsKey (JsonKey_Amount)) ? Dataspin.getInt(dict[JsonKey_Name]) : -1;

	}

	public string Identifier {
		get { return internal_id; }
	}

	public string Name {
		get { return _name; }
	}

	public int Price {
		get { return price; }
	}

	public int Amount {
		get { return amount; }
	}

	public string PurchaseDate {
		get { return purchase_date; }
	}

	public int Coins_A_Added {
		get { return coinsAdded; }
	}

	public int Coins_B_Added {
		get { return coinsAdded_B; }
	}

	public int CampaignId {
		get { return campaign_id; }
	}

	public override string ToString() { //Get an item information in a nutshell
		return internal_id + ", ID: "+iid.ToString()+", Name: "+name+ ", Purchase Date: "+purchase_date+", Price: "+price.ToString()+", Amount: "+amount.ToString();
	}

}
