using UnityEngine;
using System.Collections;

public class PreferencesSubscriber : MonoBehaviour {

	public string key;
	public PreferencesManager.Type type;

	public delegate void delegateMethod();
	public event delegateMethod call;

	private void OnEnable() {
		PreferencesManager.OnVariableChange += OnVariableChange;
	}

	private void OnDisable() {
		PreferencesManager.OnVariableChange -= OnVariableChange;
	}

	private void OnVariableChange(string key, PreferencesManager.Type type, object value) {
		if(this.key == key) {
			call();
		}
	}
}
