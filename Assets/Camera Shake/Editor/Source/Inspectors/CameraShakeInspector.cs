//
// CameraShakeEditor.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2012-2014 Thinksquirrel Software, LLC
//
using UnityEngine;
using UnityEditor;
#if !(UNITY_3_3 || UNITY_3_4 || UNITY_3_5)
using Thinksquirrel.Utilities;
#endif

//!\cond PRIVATE
#if !(UNITY_3_3 || UNITY_3_4 || UNITY_3_5)
namespace Thinksquirrel.CameraShakeEditor
{
#endif
    #if !UNITY_3_3 && !UNITY_3_4
    [CanEditMultipleObjects]
    #endif
    [CustomEditor(typeof(CameraShake))]
    class CameraShakeInspector : CameraShakeInspectorBase
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
#if !(UNITY_3_3 || UNITY_3_4 || UNITY_3_5)
}
#endif
//!\endcond
