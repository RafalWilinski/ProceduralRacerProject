using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using ScionEngine;
//using System.Reflection;
using System.IO;

namespace ScionGUI
{
	#if UNITY_EDITOR
	[Serializable]
	public class ScionWindow : EditorWindow
	{
		private ScionPostProcess activeContext { get; set; }

		const int width = 500;
		const int height = 350;
		
		[MenuItem("Window/Scion")]
		public static void Initialize()
		{
			EditorWindow editorWindow = EditorWindow.GetWindow<ScionWindow>();
			editorWindow.titleContent = new GUIContent("Scion");
			editorWindow.minSize = new Vector2(width, height);
			editorWindow.Show();
		}

		private static Texture2D s_ScionHeader;
		private static Texture2D ScionHeader 
		{
			get
			{
				if (s_ScionHeader == null) s_ScionHeader = ScionGUIUtil.LoadPNG("SCIONLogo1000x300");
				return s_ScionHeader;
			}
		}
		
		private void OnDisable()
		{
			GUI.FocusControl(null);
		}

		private void OnInspectorUpdate()
		{
			Repaint();
		}
		
//		private void UpdateSelected()
//		{
//			GameObject selectedGO = Selection.activeGameObject;
//			if (selectedGO == null) { activeContext = null; return; }
//
//			ScionPostProcess selectedScion = selectedGO.GetComponent<ScionPostProcess>();
//			if (selectedScion == null) { activeContext = null; return; }
//
//			if (activeContext != selectedScion)
//			{
//				activeContext = selectedScion;
//			}
//		}
				
		private void OnGUI()
		{
			DrawHeader();
			DrawColorGrading();
		}

		private float headerWidth { get { return width; } }
		private float headerHeight { get { return width * 3.0f / 10.0f; } }

		private void DrawHeader()
		{
			Rect headerRect = new Rect(0, 0, headerWidth, headerHeight);
			GUI.DrawTexture(headerRect, ScionHeader, ScaleMode.ScaleToFit, false);
		}
		
		private Texture2D inputLUT;
		private string inputPath = "";
		private const string DefaultHelpMessage = "Supply a color grading lookup texture and choose the program it is compatible with";
		private string helpBoxMessage = DefaultHelpMessage;
		private MessageType messageType = MessageType.None;
		private ColorGradingCompatibility compatibilityMode;

		private void ResetHelpBox()
		{
			helpBoxMessage = DefaultHelpMessage;
			messageType = MessageType.None;
		}

		private void DrawColorGrading()
		{			
			GUILayout.Space(headerHeight + 5);
			GUILayout.Label("Color Grading", EditorStyles.largeLabel);
			DrawColorGradingInput();
			DrawColorGradingConvertButton();

			if (helpBoxMessage == "") helpBoxMessage = DefaultHelpMessage;
			EditorGUILayout.HelpBox(helpBoxMessage, messageType, true);
		}
		
		private void DrawColorGradingInput()
		{
			GUILayout.Label("Input Lookup Texture", EditorStyles.boldLabel);
			
			Texture2D newInput = EditorGUILayout.ObjectField(inputLUT, typeof(Texture2D), false) as Texture2D;
			if (newInput != inputLUT)
			{
				inputLUT = newInput;
				if (newInput != null) 
				{
					inputPath = AssetDatabase.GetAssetPath(inputLUT);
					inputPath = inputPath.Substring(0, inputPath.LastIndexOf(".")); //Remove file extension
				}
				else inputPath = "";

				ResetHelpBox();
			}			
			
			GUILayout.Label("Compatibility", EditorStyles.boldLabel);
			compatibilityMode = (ColorGradingCompatibility)EditorGUILayout.EnumPopup(compatibilityMode);
		}
		
		private void DrawColorGradingConvertButton()
		{
			GUILayout.Space(10);
			if (GUILayout.Button("Convert") == false) return;

			if (inputLUT == null) 
			{
				messageType = MessageType.Error;
				helpBoxMessage = "Please supply an input lookup texture";
				return;
			}

			Texture2D converted = ColorGrading.Convert(inputLUT, compatibilityMode);
			string newPath = inputPath + "_Scion.png";

			SaveLookupTexture(newPath, converted);

			helpBoxMessage = "Saved converted lookup texture: " + newPath;
			messageType = MessageType.Info;
			Debug.Log(helpBoxMessage);
		}

		private void SaveLookupTexture(string relativePath, Texture2D lut)
		{
			string fullPath = Application.dataPath.Remove(Application.dataPath.Length - 6) + relativePath;
			byte[] textureBytes = lut.EncodeToPNG();
			
			try { File.WriteAllBytes(fullPath, textureBytes); }
			catch (Exception e)
			{
				Debug.LogError("Error saving lookup texture: " + e.StackTrace);
				return;
			}

			AssetDatabase.Refresh();
			TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(relativePath);
			importer.textureFormat = TextureImporterFormat.AutomaticTruecolor;
			importer.anisoLevel = 0;
			importer.mipmapEnabled = false;
			importer.linearTexture = true;
			importer.filterMode = FilterMode.Bilinear;
			importer.wrapMode = TextureWrapMode.Repeat;
			importer.isReadable = false;
			AssetDatabase.ImportAsset(relativePath);
			AssetDatabase.Refresh();
		}
	}
	#endif
}
