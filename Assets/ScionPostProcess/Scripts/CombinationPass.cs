using UnityEngine;
using System.Collections;

namespace ScionEngine
{
	public class CombinationPass
	{
		private Material m_combinationMat;
		private const float MinValue = 1e-4f;
		
		public CombinationPass()
		{
			m_combinationMat = new Material(Shader.Find("Hidden/ScionCombinationPass"));
			m_combinationMat.hideFlags = HideFlags.HideAndDontSave;
		}
		
		public void ReleaseResources()
		{
			if (m_combinationMat != null)
			{
				#if UNITY_EDITOR
				Object.DestroyImmediate(m_combinationMat);
				#else
				Object.Destroy(m_combinationMat);
				#endif
				m_combinationMat = null;
			}
		}	
		
		public bool PlatformCompatibility()
		{
			if (Shader.Find("Hidden/ScionCombinationPass").isSupported == false) return false;
			return true;
		}
		
		private void PrepareBloomSampling(RenderTexture bloomTexture, BloomParameters bloomParams)
		{ 		
			m_combinationMat.SetTexture("_BloomTexture", bloomTexture);			

			Vector4 shaderParams = new Vector4();
			shaderParams.x = bloomParams.intensity > MinValue ? bloomParams.intensity : MinValue;	
			shaderParams.y = bloomParams.brightness;			
			m_combinationMat.SetVector("_BloomParameters", shaderParams);
		}		
		
		private void PrepareLensDirtSampling(Texture lensDirtTexture,LensDirtParameters lensDirtParams)
		{ 				
			m_combinationMat.SetTexture("_LensDirtTexture", lensDirtTexture);

			Vector4 shaderParams = new Vector4();
			shaderParams.x = lensDirtParams.intensity > MinValue ? lensDirtParams.intensity : MinValue;	
			shaderParams.y = lensDirtParams.brightness;			
			m_combinationMat.SetVector("_LensDirtParameters", shaderParams);
		}

		private void PrepareExposure(CameraParameters cameraParams, VirtualCamera virtualCamera)
		{
			if (cameraParams.cameraMode == CameraMode.Off)
			{
				m_combinationMat.SetFloat("_ManualExposure", 1.0f);
			}
			else if (cameraParams.cameraMode != CameraMode.Manual) 
			{
				virtualCamera.BindExposureTexture(m_combinationMat);
			}
			else 
			{
				m_combinationMat.SetFloat("_ManualExposure", virtualCamera.CalculateManualExposure(cameraParams));
			}
		}

		private void UploadVariables(CommonPostProcess commonPostProcess)
		{
			Vector4 postProcessParams1 = new Vector4();
			postProcessParams1.x = commonPostProcess.grainIntensity;
			postProcessParams1.y = commonPostProcess.vignetteIntensity;
			postProcessParams1.z = commonPostProcess.vignetteScale;
			postProcessParams1.w = commonPostProcess.chromaticAberrationDistortion;
			m_combinationMat.SetVector("_PostProcessParams1", postProcessParams1);
			
			Vector4 postProcessParams2 = new Vector4();
			postProcessParams2.x = commonPostProcess.vignetteColor.r;
			postProcessParams2.y = commonPostProcess.vignetteColor.g;
			postProcessParams2.z = commonPostProcess.vignetteColor.b;
			postProcessParams2.w = commonPostProcess.chromaticAberrationIntensity;
			m_combinationMat.SetVector("_PostProcessParams2", postProcessParams2);
			
			Vector4 postProcessParams3 = new Vector4();
			postProcessParams3.x = Random.value;
			postProcessParams3.y = ScionUtility.GetWhitePointMultiplier(commonPostProcess.whitePoint);
			postProcessParams3.z = 1.0f / commonPostProcess.whitePoint;
			m_combinationMat.SetVector("_PostProcessParams3", postProcessParams3);
		}

		private void PrepareColorGrading(ColorGradingParameters colorGradingParams)
		{
			if (colorGradingParams.colorGradingMode == ColorGradingMode.Off) return;

			m_combinationMat.SetTexture("_ColorGradingLUT1", colorGradingParams.colorGradingTex1);
			ColorGrading.UploadColorGradingParams(m_combinationMat, colorGradingParams.colorGradingTex1.height);

			if (colorGradingParams.colorGradingMode == ColorGradingMode.On) return;

			m_combinationMat.SetTexture("_ColorGradingLUT2", colorGradingParams.colorGradingTex2);
			m_combinationMat.SetFloat("_ColorGradingBlendFactor", colorGradingParams.colorGradingBlendFactor);
		}
		
		public void Combine(RenderTexture source, RenderTexture dest, PostProcessParameters postProcessParams, VirtualCamera virtualCamera)
		{			
			if (postProcessParams.bloom == true) PrepareBloomSampling(postProcessParams.bloomTexture, postProcessParams.bloomParams);
			if (postProcessParams.lensDirt == true) PrepareLensDirtSampling(postProcessParams.lensDirtTexture, postProcessParams.lensDirtParams);
			PrepareExposure(postProcessParams.cameraParams, virtualCamera);
			PrepareColorGrading(postProcessParams.colorGradingParams);
			UploadVariables(postProcessParams.commonPostProcess);

			int passIndex = 0;
			if (postProcessParams.tonemapping == false) 	passIndex += 3;
			if (postProcessParams.bloom == false)  			passIndex += 2;
			else if (postProcessParams.lensDirt == false)  	passIndex += 1;

			source.filterMode = FilterMode.Bilinear;
			source.wrapMode = TextureWrapMode.Clamp;
			Graphics.Blit(source, dest, m_combinationMat, passIndex);

			//Graphics.Blit(postProcessParams.blurTexture, dest);
			//Graphics.Blit(postProcessParams.halfResSource, dest);
			//Graphics.Blit(source, dest);
		}
	}
}