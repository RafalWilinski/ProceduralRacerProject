using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DistanceLabel : PreferencesSubscriber {

	public string formatString;
	private Text label;

	private void Start() {
		label = GetComponent<Text>();
		if(type == PreferencesManager.Type.T_Float) label.text = formatString.Replace("###", PlayerPrefs.GetFloat(key).ToString("f2"));
		else if(type == PreferencesManager.Type.T_Int) label.text = formatString.Replace("###", PlayerPrefs.GetInt(key).ToString());
	}

	private void OnEnable() {
		PreferencesManager.OnVariableChange += OnVariableChange;
	}

	private void OnDisable() {
		PreferencesManager.OnVariableChange -= OnVariableChange;
	}

	private void OnVariableChange(string key, PreferencesManager.Type type, object value) {
		if(this.key == key) {
			OnMyValueChange((float) value);
		}
	}

	public virtual void OnMyValueChange(float newValue) {
		Debug.Log("Distance Label changed!");
		label.text = formatString.Replace("###", newValue.ToString());
	}
}
