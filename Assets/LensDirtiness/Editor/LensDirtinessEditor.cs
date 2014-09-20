/*=============================================================================
CHANGELOG 
			- april 2014
				* Fixed parameter update in editor mode
				* Added Scene Tints Bloom checker				

=============================================================================*/

using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(LensDirtiness))]

public class LensDirtinessEditor : Editor
{
    LensDirtiness _target;
    void Awake()
    {
         _target = (LensDirtiness)target;
    }
	public override void OnInspectorGUI ()
	{
		
		_target.gain = EditorGUILayout.Slider ("Gain", _target.gain, 1, 10);
		_target.threshold = EditorGUILayout.Slider ("Threshold", _target.threshold, 0, 10);
		_target.BloomSize = EditorGUILayout.Slider ("Bloom Size", _target.BloomSize, 0, 10);
		_target.Dirtiness = EditorGUILayout.Slider ("Dirtiness", _target.Dirtiness, 0, 10);
		_target.BloomColor = EditorGUILayout.ColorField ("Bloom Color", _target.BloomColor);
		_target.DirtinessTexture = (Texture2D)EditorGUILayout.ObjectField ("Dirtiness Texture", _target.DirtinessTexture, typeof(Texture2D), true);
        _target.SceneTintsBloom = EditorGUILayout.Toggle("Scene Tints Bloom", _target.SceneTintsBloom);
		_target.ShowScreenControls = EditorGUILayout.Toggle("Show Screen Controls", _target.ShowScreenControls);
        
        
		EditorGUILayout.HelpBox("v 1.05 April 2014.", MessageType.None);
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
        EditorUtility.SetDirty(target);
    }
    
}