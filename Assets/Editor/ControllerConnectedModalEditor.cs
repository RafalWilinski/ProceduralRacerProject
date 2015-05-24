using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ControllerConnectedModal))]
public class ControllerConnectedModalEditor : Editor {

	public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ControllerConnectedModal myScript = (ControllerConnectedModal)target;
        if(GUILayout.Button("Animate")) {
            myScript.Show();
        }
    }
}
