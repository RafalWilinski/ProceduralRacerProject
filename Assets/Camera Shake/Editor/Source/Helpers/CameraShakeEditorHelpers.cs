//
// CameraShakeEditorHelpers.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2012-2014 Thinksquirrel Software, LLC
//
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

//! \cond PRIVATE
namespace Thinksquirrel.CameraShakeEditor
{
    [InitializeOnLoad]
    static class JavaScriptInstaller
    {
        static JavaScriptInstaller()
        {
            if (EditorPrefs.GetBool("CameraShake.InstallForJavaScript", false))
                EditorApplication.update += DoJavaScriptInstaller;
        }

        static void DoJavaScriptInstaller()
        {
            EditorApplication.update -= DoJavaScriptInstaller;
            EditorPrefs.DeleteKey("CameraShake.InstallForJavaScript");

            bool isWindows = Application.platform == RuntimePlatform.WindowsEditor;
            string dataPath, plugins, pluginsCameraShake, pluginsCameraShakeRel, cameraShakeMainRel, error;

            // Get paths
            dataPath = isWindows ? Application.dataPath.Replace("/", "\\") : Application.dataPath;
            plugins = Path.Combine(dataPath, "Plugins");
            pluginsCameraShake = Path.Combine(plugins, "Camera Shake");
            cameraShakeMainRel = "Assets/Camera Shake/_Main/";
            pluginsCameraShakeRel = "Assets/Plugins/Camera Shake/";

            // Delete any old CameraShake source files in the plugins folder
            if (Directory.Exists(pluginsCameraShake))
            {
                bool del = false;

                var dir = Path.Combine(pluginsCameraShake, "Source");

                if (Directory.Exists(dir))
                {
                    var meta = dir + ".meta";
                    del = true;
                    Directory.Delete(dir);
                    if (File.Exists(meta)) File.Delete(meta);
                }

                if (del)
                    AssetDatabase.Refresh();
            }

            // Check to see if the plugins folder exists
            if (!Directory.Exists(plugins))
            {
                AssetDatabase.CreateFolder("Assets", "Plugins");
            }

            // Check to see if the CameraShake folder exists under the plugins folder
            if (!Directory.Exists(pluginsCameraShake))
            {
                AssetDatabase.CreateFolder("Assets/Plugins", "Camera Shake");
            }

            error = AssetDatabase.MoveAsset(cameraShakeMainRel + "Source", pluginsCameraShakeRel + "Source");

            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError("Unable to move Camera Shake runtime source files: " + error);
            }

            AssetDatabase.Refresh();
        }
    }
}
#region For batch mode source code building only
static class CameraShakeBatchMode
{
    static bool CheckLog(bool clear)
    {
        Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
        System.Type logEntries = assembly.GetType("UnityEditorInternal.LogEntries");

        if (clear)
        {
            logEntries.GetMethod("Clear").Invoke(new object(), null);
        }

        int count = (int)logEntries.GetMethod("GetCount").Invoke(new object(), null);

        return count == 0;
    }

    static void ExportPackageInternal()
    {
        if (!CheckLog(true))
        {
            Debug.LogError(string.Format("[{0}] Compilation errors present! Aborting...", typeof(CameraShakeBatchMode).Name));
            EditorApplication.Exit(1);
            return;
        }

        string[] packagePaths =
        {
            "Assets/Camera Shake/_Main/Source",
            "Assets/Camera Shake/Editor/Source",
            "Assets/Camera Shake/Camera Shake Example Project"
        };

        string packageName;
        string asInstaller;
        string edition;

        packageName = "CameraShake";
        asInstaller = "ASInstaller_CameraShake";
        edition = "Camera Shake";

        #if CAMERASHAKE_BETA // TODO: This doesnt work with a source-only project
        edition += " Beta";
        #endif

        string name = string.Format("{0}-Unity-3.5.7.unitypackage", packageName);
        string finalProjectRoot = Path.Combine(Application.dataPath, "../../../AssetStoreProjects");
        string finalProjectPath = Path.Combine(finalProjectRoot, edition);
        string dataPathFinal = Path.Combine(finalProjectPath, "Assets");
        string destinationPath = Path.Combine(dataPathFinal, asInstaller);

        var destinationFile = Path.Combine(destinationPath, name);
        AssetDatabase.ExportPackage(packagePaths, destinationFile, ExportPackageOptions.Recurse);

        if (!File.Exists(destinationFile))
        {
            Debug.LogError(string.Format("[{0}] Package file was not created! Aborting...", typeof(CameraShakeBatchMode).Name));
            EditorApplication.Exit(1);
            return;
        }
    }
}
#endregion
//! \endcond

