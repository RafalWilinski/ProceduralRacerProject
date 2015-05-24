//
// CameraShakeFirstRunWindow.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2012-2014 Thinksquirrel Software, LLC
//
using Thinksquirrel.CameraShakeInternal;
using UnityEditor;
using UnityEngine;

//! \cond PRIVATE
#if UNITY_3_5
using Thinksquirrel.CameraShakeEditor;
#else
namespace Thinksquirrel.CameraShakeEditor
{
#endif
    sealed class CameraShakeFirstRunWindow : EditorWindow
    {
        static GUIContent s_Logo;
        internal Vector2 _dimensions = new Vector2(450, 500);

        void OnGUI()
        {
            if (s_Logo == null)
            {
                s_Logo = new GUIContent(Resources.Load("camerashake_logo") as Texture);
            }

            GUILayout.BeginVertical();

            // Logo
            GUILayout.BeginHorizontal(GUILayout.Height(128));
            GUILayout.FlexibleSpace();
            GUILayout.Label(s_Logo);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            // Welcome message
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Thank you for installing Camera Shake!", EditorStyles.largeLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            GUILayout.Label("Please check out the following links for more information.");
            GUILayout.Label("This window can be re-opened from Preferences > Camera Shake.");
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();

            // Links
            if (DrawButton("Help", "Browse through the Reference Manual."))
                CameraShakeMenuItems.HelpWindow();
            if (DrawButton("Support Forum", "Open the Camera Shake support forum."))
                CameraShakeMenuItems.SupportForumWindow();

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            if (DrawButton("About Camera Shake", "More information about Camera Shake."))
            {
                AboutCameraShake.Open();
            }

            string contentLink = VersionInfo.ContentLink();
            bool e = GUI.enabled;
            GUI.enabled = e && !VersionInfo.isBeta && !string.IsNullOrEmpty(contentLink);
            if (DrawButton("Rate this package", "Rate this package in the Asset Store."))
                Application.OpenURL("com.unity3d.kharma:" + contentLink);
            GUI.enabled = e;

            if (DrawButton("Download Help Files", "Automatically download help files from the web."))
                CameraShakePreferences.DownloadDocumentation(true);

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();


            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            // Close window
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Close this Window", GUILayout.Width(175)))
                Close();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            // Version info
            GUILayout.Label("Camera Shake Version: " + VersionInfo.version);
            GUILayout.Label("Platform: Unity " + Application.unityVersion);
            GUILayout.Label("License: " + VersionInfo.license);
            GUILayout.EndVertical();

            // TODO: T.P.R
        }

        bool DrawButton(string label, string description)
        {
            GUILayout.BeginHorizontal();
            bool result = GUILayout.Button(label, GUILayout.Width(125));
            GUILayout.Label(description);
            GUILayout.EndHorizontal();

            return result;
        }
    }
#if !UNITY_3_5
}
#endif
//! \endcond

