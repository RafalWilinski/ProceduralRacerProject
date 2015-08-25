using UnityEngine;
using System.Collections;

namespace ScionEngine
{
	public class Bloom
	{
		private Material m_bloomMat;
		private RenderTexture[] m_bloomTextures;		
		//public const int numDownsamples = 7;
		private int numDownsamples = -1;
		private float distanceMultiplier;
		
		public Bloom()
		{
			//m_bloomTextures = new RenderTexture[numDownsamples];
			m_bloomMat = new Material(Shader.Find("Hidden/ScionBloom"));
			m_bloomMat.hideFlags = HideFlags.HideAndDontSave;
		}
		
		public void ReleaseResources()
		{
			if (m_bloomMat != null)
			{
				#if UNITY_EDITOR
				Object.DestroyImmediate(m_bloomMat);
				#else
				Object.Destroy(m_bloomMat);
				#endif
				m_bloomMat = null;
			}
		}	

		public bool PlatformCompatibility()
		{
			if (Shader.Find("Hidden/ScionBloom").isSupported == false) return false;
			return true;
		}

		public RenderTexture TryGetSmallBloomTexture(int minimumReqPixels, out int numSearches)
		{
			numSearches = 1;
			for (int i = numDownsamples-1; i >= 0; i--)
			{
				int size = m_bloomTextures[i].width > m_bloomTextures[i].height ? m_bloomTextures[i].height : m_bloomTextures[i].width;
				if (size >= minimumReqPixels) return m_bloomTextures[i];
				numSearches++;
			}

			return null;
		}

		public float GetEnergyNormalizer(int forNumDownsamples)
		{
			float rangeDivisor = 1.0f;			

			for (int i = forNumDownsamples-1; i > 0; i--) rangeDivisor = rangeDivisor * distanceMultiplier + 1.0f;

			return 1.0f / rangeDivisor;
		}	

		public void EndOfFrameCleanup()
		{
			if (m_bloomTextures == null) return;

			for (int i = 0; i < numDownsamples; i++) 
			{
				RenderTexture.ReleaseTemporary(m_bloomTextures[i]);
				m_bloomTextures[i] = null;
			}
		}
		
		public RenderTexture CreateBloomTexture(RenderTexture halfResSource, BloomParameters bloomParams)
		{			
			distanceMultiplier = bloomParams.distanceMultiplier;
			if (numDownsamples != bloomParams.downsamples)
			{
				numDownsamples = bloomParams.downsamples;
				m_bloomTextures = new RenderTexture[numDownsamples];
			}

			halfResSource.filterMode = FilterMode.Bilinear;
			RenderTextureFormat format = halfResSource.format;
			
			int width = halfResSource.width;
			int height = halfResSource.height;	

			for (int i = 0; i < numDownsamples; i++)
			{		
				m_bloomTextures[i] = RenderTexture.GetTemporary(width, height, 0, format);
				m_bloomTextures[i].filterMode = FilterMode.Bilinear;		
				m_bloomTextures[i].wrapMode = TextureWrapMode.Clamp;	

				width /= 2;
				height /= 2;	
			}
			
			halfResSource.filterMode = FilterMode.Bilinear;
			RenderTexture input = halfResSource;
			m_bloomMat.SetFloat("_BloomDistanceMultiplier", bloomParams.distanceMultiplier);

			for (int i = 1; i < numDownsamples; i++)
			{
				Graphics.Blit(input, m_bloomTextures[i], m_bloomMat, 0);		
				input = m_bloomTextures[i];
			}
			
			//ScionPostProcess.ActiveDebug.RegisterTextureForVisualization(halfResSource, false);		

			for (int i = numDownsamples-1; i > 1; i--)
			{
				Graphics.Blit(m_bloomTextures[i], m_bloomTextures[i-1], m_bloomMat, 1);	
			}

			m_bloomMat.SetFloat("_EnergyNormalizer", GetEnergyNormalizer(numDownsamples));
			Graphics.Blit(m_bloomTextures[1], m_bloomTextures[0], m_bloomMat, 2);	

			return m_bloomTextures[0];
		}
	}
}