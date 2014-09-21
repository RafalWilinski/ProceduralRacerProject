using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MeshGenerator))]
public class MeshGeneratorEditor : Editor
{
	public float sizeMultiplier;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        MeshGenerator myScript = (MeshGenerator)target;
        if(GUILayout.Button("Re-Generate")) {
            myScript.Generate();
        }
    }
}