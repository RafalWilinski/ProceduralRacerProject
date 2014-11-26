using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using MiniJSON;
//using System.Date;

[Serializable]
public class DataspinItem {

	private int iid; //Numerical id, for us it's useless
	private string internal_id; //IAB id and/or Dataspin console id
	private string state;
	private string name;
	private string v_from;
	private string v_to;
	private int max_amount;
	private int regular_price;
	private int price;
	private double usd_value;
	private int is_in_B_currency;
	private int campaign_id;
	private string campaign_name;
	private int type; //1 is coinpack, 0 for item
//	private string params = null;

	private const string JsonKey_Iid = "iid";
	private const string JsonKey_InternalName = "internal_id";
	private const string JsonKey_State = "state";
	private const string JsonKey_Name = "name";
	private const string JsonKey_ValidFrom = "v_from";
	private const string JsonKey_ValidTo = "v_to";
	private const string JsonKey_MaxAmount = "max_amount";
	private const string JsonKey_RegularPrice = "rprice";
	private const string JsonKey_Price = "price";
	private const string JsonKey_USDValue = "usd_value";
	private const string JsonKey_IsBCurrency = "is_in_B_currency";
	private const string JsonKey_CampaignId = "campaign_id";
	private const string JsonKey_CampaignName = "campaign_name";
	private const string JsonKey_Type = "type";
	private const string JsonKey_Params = "params";

	public DataspinItem(string json) {
		JsonToDict(json);
	}

	public DataspinItem(Dictionary <string, object> dict) {
		InitializeFromDict(dict);
	}

	private void JsonToDict (string json) {
		var dict = Json.Deserialize(json) as Dictionary<string,object>;
		InitializeFromDict(dict);
	}

	private void InitializeFromDict(Dictionary<string, object> dict) {

		/*
		foreach(KeyValuePair<string, object> p in dict) {
			Debug.Log(p.Key + " : " + p.Value); //Just help.
		}
		*/

		iid = (dict.ContainsKey (JsonKey_Iid)) ? (int)(long)dict[JsonKey_Iid] : -1;
		internal_id = (dict.ContainsKey (JsonKey_InternalName)) ? (string)dict[JsonKey_InternalName] : null;
		state = (dict.ContainsKey (JsonKey_State)) ? (string)dict[JsonKey_State] : null;
		name = (dict.ContainsKey (JsonKey_Name)) ? (string)dict[JsonKey_Name] : null;
		v_from = (dict.ContainsKey (JsonKey_ValidFrom)) ? (string)dict[JsonKey_ValidFrom] : null;
		v_to = (dict.ContainsKey (JsonKey_ValidTo)) ? (string)dict[JsonKey_ValidTo] : null;
		max_amount = (dict.ContainsKey (JsonKey_MaxAmount)) ? Dataspin.getInt(dict[JsonKey_MaxAmount]) : -1;
		regular_price = (dict.ContainsKey (JsonKey_RegularPrice)) ? Dataspin.getInt(dict[JsonKey_RegularPrice]) : -1;
		price = (dict.ContainsKey (JsonKey_Price)) ? Dataspin.getInt(dict[JsonKey_Price]) : -1;
		usd_value = (dict.ContainsKey (JsonKey_USDValue)) ? Dataspin.getDouble(dict[JsonKey_USDValue]) : -1;
		is_in_B_currency = (dict.ContainsKey (JsonKey_IsBCurrency)) ? Dataspin.getInt(dict[JsonKey_IsBCurrency]) : -1;
		campaign_id = (dict.ContainsKey (JsonKey_CampaignId)) ? Dataspin.getInt(dict[JsonKey_CampaignId]) : -1;
		campaign_name = (dict.ContainsKey (JsonKey_CampaignName)) ? (string)dict[JsonKey_CampaignName] : null;
		type = (dict.ContainsKey (JsonKey_Type)) ? Dataspin.getInt(dict[JsonKey_Type]) : -1;
	}

	public string Identifier {
		get { return internal_id; }
	}

	public string Name {
		get { return name; }
	}

	public int Price {
		get { return price; }
	}

	public int RegularPrice {
		get { return regular_price; }
	}

	public bool IsInBCurrency {
		get { 
			if(is_in_B_currency == 1) return true;
			return false; 
		}
	}

	public string ValidFrom {
		get { return v_from; }
	}

	public string ValidTo {
		get { return v_to; }
	}

	public string CampaignName {
		get { return campaign_name; }
	}

	public int CampaignId {
		get { return campaign_id; }
	}

	public int MaxAmount {
		get { return max_amount; }
	}

	public DataspinType Type {
		get { 
			if(type == 1) return DataspinType.Dataspin_Coinpack;
			else return DataspinType.Dataspin_Item;
		}
	}

	public bool IsItemValid() {
		IFormatProvider culture = new System.Globalization.CultureInfo("en-GB", true);
		DateTime dt_from = DateTime.Parse(v_from, culture, System.Globalization.DateTimeStyles.AdjustToUniversal);
		DateTime dt_to = DateTime.Parse(v_to, culture, System.Globalization.DateTimeStyles.AdjustToUniversal);

//		if(DateTime.op_GreaterThan(DateTime.UtcNow,dt_from) && DateTime.op_GreaterThan(dt_to, DateTime.UtcNow)) { //Somehow it doesn't work in Mono. According to C# MSDN it is correct...
		if(DateTime.UtcNow > dt_from && dt_to > DateTime.UtcNow) {
			return true;
		}
		return false;
	}

	public override string ToString() { //Get an item information in a nutshell
		return internal_id + ", ID: "+iid+", State: "+state+", Regular Price: "+regular_price+", Price: "+price+", In in B currency? "+is_in_B_currency+", Type: "+type+", USD Value: "+usd_value;
	}
}
