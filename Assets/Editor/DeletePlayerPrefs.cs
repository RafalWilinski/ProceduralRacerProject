using UnityEngine;
using UnityEditor;
using System.Collections;

public class DeletePlayerPrefs : EditorWindow {

	[MenuItem ("Window/Delete PlayerPrefs")]
	public static void Clickeer () {
		PlayerPrefs.DeleteAll();	
	}
}
