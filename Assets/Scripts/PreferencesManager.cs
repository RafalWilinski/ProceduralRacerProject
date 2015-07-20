using UnityEngine;
using System.Collections;

public class PreferencesManager : MonoBehaviour {

	public delegate void ChangeVariable(string key, Type type, object value);
	public static event ChangeVariable OnVariableChange;

	public enum Type {
		T_Int, T_Float, T_String
	}

	private static PreferencesManager _instance;

	public static PreferencesManager Instance {
		get { 
			if(_instance == null) {
				GameObject temp = new GameObject();
				temp.name = "PreferencesManager";
				temp.AddComponent<PreferencesManager>();
				_instance = temp.GetComponent<PreferencesManager>();
			}

			return _instance;
		}
	}

	private void Awake() {
		_instance = this;
	}

	public void Increment(string key, float add) {
		float newValue = PlayerPrefs.GetFloat(key) + add;
		PlayerPrefs.SetFloat(key, newValue);
		if(OnVariableChange != null) OnVariableChange(key, Type.T_Float, (object) newValue);

		Debug.Log(key + "Incremented, new value: "+newValue);
	}

	public void Increment(string key, int add) {
		int newValue = PlayerPrefs.GetInt(key) + add;
		PlayerPrefs.SetInt(key, newValue);
		if(OnVariableChange != null) OnVariableChange(key, Type.T_Int, (object) newValue);

		Debug.Log(key + "Incremented, new value: "+newValue);
	}

	public void Set(string key, int newValue) {
		PlayerPrefs.SetInt(key, newValue);
		if(OnVariableChange != null) OnVariableChange(key, Type.T_Int, (object) newValue);
	}

	public void Set(string key, float newValue) {
		PlayerPrefs.SetFloat(key, newValue);
		if(OnVariableChange != null) OnVariableChange(key, Type.T_Float, (object) newValue);
	}
}
