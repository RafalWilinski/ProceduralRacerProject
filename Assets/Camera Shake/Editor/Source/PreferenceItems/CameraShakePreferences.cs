//
// CameraShakePreferences.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2012-2014 Thinksquirrel Software, LLC
//
using System.IO;
using Ionic.Zip;
using Thinksquirrel.CameraShakeInternal;
using UnityEditor;
using UnityEngine;

//! \cond PRIVATE
#if UNITY_3_5
using Thinksquirrel.CameraShakeEditor;
#else
using Thinksquirrel.Utilities;
namespace Thinksquirrel.CameraShakeEditor
{
#endif
    [InitializeOnLoad]
    static class CameraShakePreferences
    {
        static bool s_PrefsLoaded;
        static readonly GUIContent s_Label = new GUIContent();
        static DocumentationSource s_DocumentationSource = DocumentationSource.Online;
        static string s_DocumentationPath;
        static string s_ShortPath;
        static bool s_DownloadEveryUpdate;
        static WWW s_Www;
        static bool s_ShowProgress;
        static bool s_FinishedInstall;
        static bool s_Canceled;
        static string s_Progress;
        static Color s_ProgressColor = Color.white;

        static CameraShakePreferences()
        {
            // Check for first run
            bool firstRunAny = EditorPrefs.GetBool("Thinksquirrel.CameraShakeEditor.FirstRun_Any", true);
            bool firstRun = EditorPrefs.GetBool("Thinksquirrel.CameraShakeEditor.FirstRun_" + VersionInfo.version, true);

            // If first run, check to download docs, then open the first run window if it's the very first run
            if (firstRun || firstRunAny)
            {
                bool downloadDocs = EditorPrefs.GetBool("Thinksquirrel.CameraShakeEditor.DownloadDocsEveryUpdate", false);

                if (downloadDocs)
                {
                    LoadPreferences();

                    bool documentationExists = Directory.Exists(Path.Combine(s_DocumentationPath, VersionInfo.version));

                    if (!documentationExists)
                    {
                        DownloadDocumentation();
                    }
                }

                if (firstRunAny)
                {
                    EditorApplication.update += OpenFirstRunWindowDelayed;
                }
            }

            // Set first run flags
            EditorPrefs.SetBool("Thinksquirrel.CameraShakeEditor.FirstRun_" + VersionInfo.version, false);
            EditorPrefs.SetBool("Thinksquirrel.CameraShakeEditor.FirstRun_Any", false);
        }

        static void OpenFirstRunWindowDelayed()
        {
            EditorApplication.update -= OpenFirstRunWindowDelayed;
            OpenFirstRunWindow();
        }

        static void OpenFirstRunWindow()
        {
            float w = Screen.currentResolution.width;
            float h = Screen.currentResolution.height;

            CameraShakeFirstRunWindow window = EditorWindow.GetWindow<CameraShakeFirstRunWindow>(true, "Camera Shake Setup");

            var r = new Rect(
                (w * .5f) - (window._dimensions.x * .5f),
                (h * .5f) - (window._dimensions.y * .5f),
                window._dimensions.x,
                window._dimensions.y);

            window.position = r;
            window.minSize = new Vector2(r.width, r.height);
            window.maxSize = new Vector2(r.width, r.height);
        }

        public enum DocumentationSource
        {
            Online = 0,
            Local
        }

        internal static void DownloadDocumentation(bool fromFirstRunWindow)
        {
            if (fromFirstRunWindow)
            {
                s_DownloadEveryUpdate = true;
                s_DocumentationSource = DocumentationSource.Local;
                EditorPrefs.SetInt("Thinksquirrel.CameraShakeEditor.DocumentationSource", (int)s_DocumentationSource);
                EditorPrefs.SetString("Thinksquirrel.CameraShakeEditor.DocumentationPath", s_DocumentationPath);
                EditorPrefs.SetBool("Thinksquirrel.CameraShakeEditor.DownloadDocsEveryUpdate", s_DownloadEveryUpdate);
            }

            s_ShowProgress = true;
            s_FinishedInstall = false;
            s_Progress = "Downloading documentation...\n";
            s_ProgressColor = Color.white;
            s_Www = new WWW(VersionInfo.ArchiveUrl());
            s_Canceled = false;
            EditorApplication.update += UpdateProgress;
            EditorApplication.update += UpdateProgressBar;
        }

        internal static void DownloadDocumentation()
        {
            DownloadDocumentation(false);
        }

        internal static void DisplayDocumentationProgress()
        {
            if (s_ShowProgress)
            {
                Color c = GUI.color;
                GUI.color = s_ProgressColor;
                GUILayout.Label(s_Progress, EditorStyles.wordWrappedLabel);
                GUI.color = c;
            }
        }

        static void UpdateProgress()
        {
            if (!string.IsNullOrEmpty(s_Www.error))
            {
                s_ProgressColor = Color.red;
                s_Progress = "Error downloading documentation\n\n" + s_Www.error;
                s_FinishedInstall = true;
                EditorApplication.update -= UpdateProgress;
            }
            else
            if (s_Www.isDone)
            {
                if (s_Canceled)
                {
                    s_ProgressColor = Color.red;
                    s_Progress = "Error extracting documentation\n\nCanceled by user";
                    s_FinishedInstall = true;
                    EditorApplication.update -= UpdateProgress;
                    return;
                }

                try
                {
                    using (var inputStream = new MemoryStream(s_Www.bytes))
                    {
                        using (ZipFile zip = ZipFile.Read(inputStream))
                        {
                            string archivePath = Path.Combine(s_DocumentationPath, VersionInfo.version);
                            if (!Directory.Exists(archivePath))
                                Directory.CreateDirectory(archivePath);

                            zip.ExtractAll(archivePath, ExtractExistingFileAction.OverwriteSilently);
                        }
                    }

                    s_ProgressColor = Color.green;
                    s_Progress = "Download complete.";
                    s_FinishedInstall = true;
                    EditorApplication.update -= UpdateProgress;
                }
                catch (System.Exception e)
                {
                    s_ProgressColor = Color.red;
                    s_Progress = string.Format("Error extracting documentation\n\nAn exception was thrown ({0}): {1} at {2}", e.GetType(), e.Message, e.StackTrace);
                    s_FinishedInstall = true;
                    EditorApplication.update -= UpdateProgress;
                }
            }
            else
            {
                if (s_Canceled)
                {
                    s_ProgressColor = Color.red;
                    s_Progress = "Error extracting documentation\n\nCanceled by user";
                    s_FinishedInstall = true;
                    EditorApplication.update -= UpdateProgress;
                    return;
                }
                s_Progress += ".";
            }
        }

        static void UpdateProgressBar()
        {
            string infoString;
            float progress;

            if (!string.IsNullOrEmpty(s_Www.error))
            {
                EditorUtility.ClearProgressBar();
                CameraShakeBase.LogError("Error downloading Camera Shake documentation: " + s_Www.error, "Camera Shake", "Preferences");
                EditorApplication.update -= UpdateProgressBar;
                return;
            }
            if (s_Www.isDone)
            {
                infoString = "Extracting documentation";
                progress = 1;

                EditorUtility.DisplayProgressBar("Installing documentation", infoString, progress);

                if (s_FinishedInstall)
                {
                    EditorUtility.ClearProgressBar();
                    EditorApplication.update -= UpdateProgressBar;
                    return;
                }
            }
            else
            {
                infoString = "Downloading documentation";
                progress = s_Www.progress;

                if (EditorUtility.DisplayCancelableProgressBar("Installing documentation", infoString, progress))
                {
                    EditorUtility.ClearProgressBar();
                    CameraShakeBase.LogError("Error downloading Camera Shake documentation: Canceled by user", "Camera Shake", "Preferences");
                    s_Canceled = true;
                    s_FinishedInstall = true;
                    EditorApplication.update -= UpdateProgressBar;
                }
            }
        }

        internal static string ReferenceManualUrl()
        {
            if (!s_PrefsLoaded)
                LoadPreferences();

            switch (s_DocumentationSource)
            {
                case DocumentationSource.Local:
                    return new System.Uri(
                    Path.Combine(Path.Combine(s_DocumentationPath, VersionInfo.version), "index.html")).AbsoluteUri;
                default:
                    return VersionInfo.ReferenceManualUrl();
            }
        }

        internal static string ComponentUrl(System.Type type)
        {
            if (!s_PrefsLoaded)
                LoadPreferences();

            switch (s_DocumentationSource)
            {
                case DocumentationSource.Local:
                    return new System.Uri(
                    Path.Combine(Path.Combine(s_DocumentationPath, VersionInfo.version), VersionInfo.ComponentUrl(type, false))).AbsoluteUri;
                default:
                    return VersionInfo.ComponentUrl(type);
            }
        }

        internal static string ScriptingUrl(System.Type type)
        {
            if (!s_PrefsLoaded)
                LoadPreferences();

            switch (s_DocumentationSource)
            {
                case DocumentationSource.Local:
                    return new System.Uri(
                    Path.Combine(Path.Combine(s_DocumentationPath, VersionInfo.version), VersionInfo.ScriptingUrl(type, false))).AbsoluteUri;
                default:
                    return VersionInfo.ScriptingUrl(type);
            }
        }

        static void LoadPreferences()
        {
            s_DocumentationSource = (DocumentationSource)EditorPrefs.GetInt("Thinksquirrel.CameraShakeEditor.DocumentationSource", 0);
            s_DocumentationPath = EditorPrefs.GetString("Thinksquirrel.CameraShakeEditor.DocumentationPath");
            s_DownloadEveryUpdate = EditorPrefs.GetBool("Thinksquirrel.CameraShakeEditor.DownloadDocsEveryUpdate", false);

            if (string.IsNullOrEmpty(s_DocumentationPath))
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.OSXEditor:
                        s_DocumentationPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "Thinksquirrel/Camera Shake/Documentation");
                        if (!Directory.Exists(s_DocumentationPath))
                            Directory.CreateDirectory(s_DocumentationPath);
                        break;
                    case RuntimePlatform.WindowsEditor:
                        s_DocumentationPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "Thinksquirrel/Camera Shake/Documentation");
                        if (!Directory.Exists(s_DocumentationPath))
                            Directory.CreateDirectory(s_DocumentationPath);
                        break;
                }
            }

            s_ShortPath = new DirectoryInfo(s_DocumentationPath).Name;
            s_PrefsLoaded = true;
        }

        [PreferenceItem("Camera Shake")]
        public static void PreferncesGUI()
        {
            if (!s_PrefsLoaded)
                LoadPreferences();

            s_Label.text = "Open First-Time Setup";
            s_Label.tooltip = "Opens the Camera Shake first-time setup window.";
            if (GUILayout.Button(s_Label))
                OpenFirstRunWindow();

            // Documentation source
            s_Label.text = "Documentation Source";
            s_Label.tooltip = "The source for all documentation links.";
            s_DocumentationSource = (DocumentationSource)EditorGUILayout.EnumPopup(s_Label, s_DocumentationSource);

            if (s_DocumentationSource == DocumentationSource.Local)
            {
                // Documentation path
                s_Label.text = "Documentation Path";
                s_Label.tooltip = "The path where local documentation should be saved.";
                GUILayout.BeginHorizontal();
                EditorGUILayout.TextField(s_Label, s_ShortPath);
                bool e = GUI.enabled;
                GUI.enabled = !(s_ShowProgress && !s_FinishedInstall);
                if (GUILayout.Button("...", GUILayout.ExpandWidth(false)))
                {
                    string newPath = EditorUtility.OpenFolderPanel("Select folder", s_DocumentationPath, string.Empty);

                    if (!string.IsNullOrEmpty(newPath))
                    {
                        s_DocumentationPath = newPath;
                        s_ShortPath = new DirectoryInfo(s_DocumentationPath).Name;
                        s_ShowProgress = false;
                    }
                }
                GUI.enabled = e;
                GUILayout.EndHorizontal();

                // Download documentation with every update
                s_Label.text = "Download with Every Update";
                s_Label.tooltip = "If enabled, documentation will be downloaded with every Camera Shake update.";
                bool downloadEveryUpdate = EditorGUILayout.Toggle(s_Label, s_DownloadEveryUpdate);
                if (downloadEveryUpdate != s_DownloadEveryUpdate)
                {
                    s_DownloadEveryUpdate = downloadEveryUpdate;
                }

                Color c = GUI.color;

                // Download documentation
                bool documentationExists = Directory.Exists(Path.Combine(s_DocumentationPath, VersionInfo.version));
                if (!documentationExists)
                    GUI.color = Color.red;
                s_Label.text = documentationExists ? "Update" : "Download";
                s_Label.tooltip = documentationExists ? "Update the local documentation from the server" : "Download local documentation from the server";
                if (GUILayout.Button(s_Label))
                    DownloadDocumentation();

                GUI.color = c;

                DisplayDocumentationProgress();

                if (GUI.changed)
                {
                    EditorPrefs.SetInt("Thinksquirrel.CameraShakeEditor.DocumentationSource", (int)s_DocumentationSource);
                    EditorPrefs.SetString("Thinksquirrel.CameraShakeEditor.DocumentationPath", s_DocumentationPath);
                    EditorPrefs.SetBool("Thinksquirrel.CameraShakeEditor.DownloadDocsEveryUpdate", s_DownloadEveryUpdate);
                }
            }
        }
    }
#if !UNITY_3_5
}
#endif
//! \endcond
