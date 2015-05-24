//
// AboutCameraShake.cs
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
#if !UNITY_3_5
namespace Thinksquirrel.CameraShakeEditor
{
#else
using Thinksquirrel.CameraShakeEditor;
#endif
    sealed class AboutCameraShake : EditorWindow
    {
        static Vector2 s_MinSize = new Vector2(350, 280);
        static Vector2 s_MaxSize = new Vector2(350, 280);
        static GUIContent s_Logo;
        double m_LastTime;
        double m_DeltaTime;
        Vector2 m_ScrollPosition = Vector2.zero;
        static GUIStyle s_HorizontalScrollbarStyle;
        static GUIStyle s_VerticalScrollbarStyle;

        [MenuItem (CameraShakeMenuItems.menuWindowLocation + "/Camera Shake/About Camera Shake", false, 1202)]
        public static void Open()
        {
            EditorWindow.GetWindow<AboutCameraShake>(true, "About Camera Shake");
        }

        void OnGUI()
        {
            if (s_Logo == null)
            {
                s_Logo = new GUIContent(Resources.Load("camerashake_logo") as Texture);
                s_HorizontalScrollbarStyle = new GUIStyle(GUI.skin.horizontalScrollbar);
                s_VerticalScrollbarStyle = new GUIStyle(GUI.skin.verticalScrollbar);
                s_HorizontalScrollbarStyle.fixedWidth = 0;
                s_HorizontalScrollbarStyle.fixedHeight = 0;
                s_VerticalScrollbarStyle.fixedWidth = 0;
                s_VerticalScrollbarStyle.fixedHeight = 0;
            }
            GUILayout.BeginHorizontal(GUILayout.Height(128));
            GUILayout.Label(s_Logo);
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Camera Shake", EditorStyles.largeLabel);
            GUILayout.Label("Version " + VersionInfo.version, EditorStyles.miniLabel);
            if (VersionInfo.isBeta)
            {
                GUILayout.Label("Beta Release", EditorStyles.miniLabel);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition, false, false, s_HorizontalScrollbarStyle, s_VerticalScrollbarStyle, GUILayout.Height(40));
            CreditsLabel("");
            CreditsLabel("");
            CreditsLabel("Developed by Josh Montoute");
            CreditsLabel(
                "Special thanks to:\n" +
                "Alpha and Beta Testers (you guys rock!)\n" +
                ""
            );
            CreditsLabel("");
            CreditsLabel("Uses the DotNetZip library: http://dotnetzip.codeplex.com");
            CreditsLabel("");
            CreditsLabel("");
            CreditsLabel("");
            CreditsLabel("");
            GUILayout.EndScrollView();
            string contentLink = VersionInfo.ContentLink();
            if (!VersionInfo.isBeta && !string.IsNullOrEmpty(contentLink))
            {
                if (GUILayout.Button("Sign up for update notifications"))
                {
                    CameraShakeMenuItems.RegisterCameraShake();
                }
                if (GUILayout.Button("Rate this package! (Unity Asset Store)"))
                {
                    Application.OpenURL("com.unity3d.kharma:" + contentLink);
                }
            }
            GUI.color = new Color(0, .5f, .75f, 1);
            if (GUILayout.Button("https://www.thinksquirrel.com/", EditorStyles.whiteLabel))
            {
                Application.OpenURL("https://www.thinksquirrel.com/");
            }
            GUI.color = Color.white;
            GUILayout.Label("License: " + VersionInfo.license);
            GUILayout.Label(VersionInfo.copyright, EditorStyles.miniLabel);

            GUILayout.Space(14);

            minSize = s_MinSize;
            maxSize = s_MaxSize;
        }

        void Update()
        {
            m_DeltaTime = EditorApplication.timeSinceStartup - m_LastTime;
            m_LastTime = EditorApplication.timeSinceStartup;

            m_ScrollPosition = new Vector2(0, m_ScrollPosition.y + 5 * (float)m_DeltaTime);
            if (m_ScrollPosition.y >= 150)
            {
                m_ScrollPosition = Vector2.zero;
            }

            Repaint();
        }

        void CreditsLabel(string text)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(text, EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
#if !UNITY_3_5
}
#endif
//! \endcond

