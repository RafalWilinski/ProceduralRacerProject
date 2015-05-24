using UnityEngine;
using System.Collections;

public class SetActivePanel : MonoBehaviour {

	public PanelsManager.Panel panel;

	public void OnClick() {
		Debug.Log("OnClick :: Setting new active panel "+panel.ToString());
		PanelsManager.Instance.SetActivePanel(panel);
	}
}
