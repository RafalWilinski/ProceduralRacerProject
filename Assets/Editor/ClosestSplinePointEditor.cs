using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ClosestSplinePoint))]
public class ClosestSplinePointEditor : Editor {

	public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ClosestSplinePoint myScript = (ClosestSplinePoint)target;
        if(GUILayout.Button("Simulate!")) {
            myScript.Simulate();
        }
    }
}
