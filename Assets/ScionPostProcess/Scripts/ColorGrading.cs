using UnityEngine;
using System.Collections;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ScionEngine
{
	public static class ColorGrading 
	{	
		private static Material s_ColorGradingMat;
		private static Material ColorGradingMat
		{
			get
			{
				if (s_ColorGradingMat == null)
				{
					s_ColorGradingMat = new Material(Shader.Find("Hidden/ScionColorGrading"));
					s_ColorGradingMat.hideFlags = HideFlags.HideAndDontSave;
				}
				return s_ColorGradingMat;
			}
		}

		private const int AmplifyPass = 0;
		private const int ChromaticaPass = 1;
		private const int UnityPass = 2;

		public static Texture2D Convert(Texture2D lut2D, ColorGradingCompatibility compatibilityMode) 
		{
			FilterMode prevFilterMode = lut2D.filterMode;
			Texture2D convertedTexture = null;

			switch (compatibilityMode)
			{
				case (ColorGradingCompatibility.Amplify):
					convertedTexture = ConvertAmplify(lut2D);
					break;
				case (ColorGradingCompatibility.Chromatica):
					convertedTexture = ConvertChromatica(lut2D);
					break;
				case (ColorGradingCompatibility.Unity):
					convertedTexture = ConvertUnity(lut2D);
					break;
			}

			lut2D.filterMode = prevFilterMode;
			return convertedTexture;
		}
		
		private static Texture2D ConvertUnity(Texture2D lut2D) 
		{			
			RenderTexture tex = RenderTexture.GetTemporary(lut2D.width, lut2D.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
			Graphics.Blit(lut2D, tex, ColorGradingMat, UnityPass);
			return ReadBackReleaseRenderTexture(tex);
		}
		
		
		private static void SaveLookupTexture(string relativePath, Texture2D lut)
		{
			#if UNITY_EDITOR
			string fullPath = Application.dataPath.Remove(Application.dataPath.Length - 6) + relativePath;
			byte[] textureBytes = lut.EncodeToPNG();
			
			File.WriteAllBytes(fullPath, textureBytes);
			
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

			Debug.Log ("saved at " + fullPath);
			#endif
		}

		private static Texture2D ConvertChromatica(Texture2D lut2D) 
		{				
			int height = lut2D.height / 8;
			int width = height * 64;
			UploadColorGradingParams(ColorGradingMat, height);

			Texture2D neutralLUT = GenerateNeutralLUT(height);
			ColorGradingMat.SetTexture("_NeutralLUT", neutralLUT);
			//SaveLookupTexture("Assets/TestPic.png", neutralLUT);

			RenderTexture tex = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
			Graphics.Blit(lut2D, tex, ColorGradingMat, ChromaticaPass);
			return ReadBackReleaseRenderTexture(tex);
		}
		
		private static Texture2D ConvertAmplify(Texture2D lut2D) 
		{				
			RenderTexture tex = RenderTexture.GetTemporary(lut2D.width, lut2D.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
			Graphics.Blit(lut2D, tex, ColorGradingMat, AmplifyPass);
			return ReadBackReleaseRenderTexture(tex);
		}

		private static Texture2D ReadBackReleaseRenderTexture(RenderTexture renderTex)
		{
			RenderTexture prevActive = RenderTexture.active;
			RenderTexture.active = renderTex;

			Texture2D newLUT = new Texture2D(renderTex.width, renderTex.height, TextureFormat.ARGB32, false, true);
			newLUT.hideFlags = HideFlags.HideAndDontSave;
			newLUT.ReadPixels(new Rect(0.0f, 0.0f, renderTex.width, renderTex.height), 0, 0);
			newLUT.Apply();

			RenderTexture.active = prevActive;
			RenderTexture.ReleaseTemporary(renderTex);
			return newLUT;
		}

		public static void UploadColorGradingParams(Material mat, float numSlices)
		{			
			float invNumSlices = 1.0f / numSlices;
			Vector2 LUTSize = new Vector2(numSlices*numSlices, numSlices);
			
			Vector4 colorGradingParams1 = new Vector4();
			colorGradingParams1.x = 1.0f * invNumSlices - 1.0f / LUTSize.x;
			colorGradingParams1.y = 1.0f - 1.0f * invNumSlices;
			colorGradingParams1.z = numSlices - 1.0f;
			colorGradingParams1.w = numSlices;
			
			Vector4 colorGradingParams2 = new Vector4();
			colorGradingParams2.x = 0.5f / LUTSize.x;
			colorGradingParams2.y = 0.5f / LUTSize.y;
			colorGradingParams2.z = 0.0f;
			colorGradingParams2.w = invNumSlices;
			
			mat.SetVector("_ColorGradingParams1", colorGradingParams1);
			mat.SetVector("_ColorGradingParams2", colorGradingParams2);
		}

		private static Texture2D GenerateNeutralLUT(int dim)
		{
			Texture2D neutral = new Texture2D(dim*dim, dim, TextureFormat.ARGB32, false, true);
			neutral.hideFlags = HideFlags.HideAndDontSave;
			Color[] clr = new Color[dim*dim*dim];
			
			for (int w = 0; w < dim; w++)
			{
				for (int v = 0; v < dim; v++)
				{
					for (int u = 0; u < dim; u++)
					{
						int index = u + w * dim + v * dim * dim;
						
						float r = u / (dim-1.0f);
						float g = v / (dim-1.0f);
						float b = w / (dim-1.0f);

						clr[index] = new Color(r, g, b, 1.0f);
					}
				}
			}

			neutral.SetPixels(clr);
			neutral.Apply(false, true);
			neutral.filterMode = FilterMode.Bilinear;
			neutral.wrapMode = TextureWrapMode.Clamp;
			return neutral;
		}
	}
}
