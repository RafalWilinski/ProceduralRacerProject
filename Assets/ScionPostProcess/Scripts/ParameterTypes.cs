﻿using UnityEngine;
using System.Collections;

namespace ScionEngine
{	
	public enum CameraMode
	{
		Off					= 0,
		Manual				= 1,
		AutoPriority		= 2,
		//ShutterPriority		= 3,
		AperturePriority	= 4,
	}

	public enum TonemappingMode
	{
		Reinhard		= 0,
		LumaReinhard	= 1,
		Filmic			= 2,
		Photographic	= 3,
	}

	public enum DepthFocusMode
	{
		ManualDistance	= 0,
		ManualRange 	= 1,
		PointAverage 	= 2,
	}
	
	public enum DepthOfFieldQuality
	{
		Normal		= 0,
		High_DX11 	= 1,
	}
	
	public enum ColorGradingMode
	{
		Off		= 0,
		On 		= 1,
		Blend 	= 2,
	}
	
	public enum ColorGradingCompatibility
	{
		Unity	= 0,
		Chromatica = 1,
		Amplify	= 2,
	}

	public struct BloomParameters
	{
		public float intensity;
		public float brightness;
		public float distanceMultiplier;
		public int downsamples;
	}
	
	public struct LensDirtParameters
	{
		public float intensity;
		public float brightness;
	}

	public struct CameraParameters
	{
		public CameraMode cameraMode;
		public float focalLength;
		public float apertureDiameter;
		public float fNumber;
		public float ISO;
		public float shutterSpeed;
		public Vector2 minMaxExposure;
		public float exposureCompensation;
		public float fieldOfView;		
		public float adaptionSpeed;	//Exposure
		public float aspect;

		public float nearPlane;
		public float farPlane;
	}

	public struct PreCalcValues
	{
		public float tanHalfFoV;
	}

	public struct DepthOfFieldParameters
	{		
		public LayerMask depthOfFieldMask;
		public bool useMedianFilter;
		public DepthOfFieldQuality quality;
		public DepthFocusMode depthFocusMode;
		public float maxCoCRadius;

		public Vector2 pointAveragePosition; 	//Used for PointAverage
		public float pointAverageRange;			//Used for PointAverage
		public bool visualizePointFocus;		//Used for PointAverage
		public float depthAdaptionSpeed;		//Used for PointAverage

		public float focalDistance;
		public float focalRange;
	}

	public struct CommonPostProcess
	{		
		public float grainIntensity;

		public float vignetteIntensity;
		public float vignetteScale;
		public Color vignetteColor;
		
		public bool chromaticAberration;
		public float chromaticAberrationDistortion;
		public float chromaticAberrationIntensity;

		public float whitePoint;
	}

	public struct ColorGradingParameters
	{		
		public ColorGradingMode colorGradingMode;
		public Texture2D colorGradingTex1;
		public Texture2D colorGradingTex2;
		public float colorGradingBlendFactor;
	}

	public class PostProcessParameters
	{
		public Camera camera;
		public Transform cameraTransform;

		public bool tonemapping;
		public bool bloom;
		public bool lensDirt;
		public bool exposure;
		public bool depthOfField;
		public bool isFirstRender;

		public int width;
		public int height;
		public int halfWidth;
		public int halfHeight;

		public RenderTexture halfResSource;
		public RenderTexture halfResDepth;
		public RenderTexture bloomTexture;
		public RenderTexture dofTexture;
		public Texture lensDirtTexture;
		
		public BloomParameters bloomParams;
		public LensDirtParameters lensDirtParams;
		public CameraParameters cameraParams;
		public DepthOfFieldParameters DoFParams;
		public ColorGradingParameters colorGradingParams;
		public PreCalcValues preCalcValues;
		public CommonPostProcess commonPostProcess;

		public PostProcessParameters()
		{
			bloomParams = new BloomParameters();
			lensDirtParams = new LensDirtParameters();
			cameraParams = new CameraParameters();
			DoFParams = new DepthOfFieldParameters();
			colorGradingParams = new ColorGradingParameters();
			preCalcValues = new PreCalcValues();
			commonPostProcess = new CommonPostProcess();
		}

		public void Fill(ScionPostProcessBase postProcess)
		{
			bloom 				= postProcess.bloom;
			lensDirt 			= postProcess.lensDirt && (postProcess.lensDirtTexture != null);
			lensDirtTexture 	= postProcess.lensDirtTexture;
			bloomTexture		= null;
			dofTexture			= null;
			exposure			= postProcess.cameraMode != CameraMode.Off ? true : false;
			depthOfField		= postProcess.depthOfField;
			halfResSource 		= null; //Done later

			bloomParams.intensity 				= ScionUtility.Square(postProcess.bloomIntensity);
			bloomParams.brightness 				= postProcess.bloomBrightness;
			bloomParams.distanceMultiplier		= postProcess.bloomDistanceMultiplier;
			bloomParams.downsamples				= postProcess.bloomDownsamples;

			lensDirtParams.intensity			= ScionUtility.Square(postProcess.lensDirtIntensity);
			lensDirtParams.brightness 			= postProcess.lensDirtBrightness;

			DoFParams.depthFocusMode			= postProcess.depthFocusMode;
			DoFParams.maxCoCRadius				= postProcess.maxCoCRadius;
			DoFParams.quality					= SystemInfo.graphicsShaderLevel < 40 ? DepthOfFieldQuality.Normal : postProcess.depthOfFieldQuality;
			DoFParams.pointAveragePosition		= postProcess.pointAveragePosition;
			DoFParams.pointAverageRange			= postProcess.pointAverageRange;
			DoFParams.visualizePointFocus		= postProcess.visualizePointFocus;
			DoFParams.depthAdaptionSpeed		= postProcess.depthAdaptionSpeed;
			DoFParams.focalDistance 			= postProcess.focalDistance;
			DoFParams.focalRange 				= postProcess.focalRange;

			colorGradingParams.colorGradingMode 			= postProcess.colorGradingTex1 == null ? ColorGradingMode.Off : postProcess.colorGradingMode;
			colorGradingParams.colorGradingTex1 			= postProcess.colorGradingTex1;
			colorGradingParams.colorGradingTex2		 		= postProcess.colorGradingTex2;
			colorGradingParams.colorGradingBlendFactor 		= postProcess.colorGradingBlendFactor;

			cameraParams.cameraMode 			= postProcess.cameraMode;
			cameraParams.fNumber 				= postProcess.fNumber;
			cameraParams.ISO 					= postProcess.ISO;
			cameraParams.shutterSpeed 			= postProcess.shutterSpeed;
			cameraParams.adaptionSpeed 			= postProcess.adaptionSpeed;
			cameraParams.minMaxExposure			= postProcess.minMaxExposure;
			cameraParams.exposureCompensation 	= postProcess.exposureCompensation;

			commonPostProcess.grainIntensity 				= postProcess.grain == true ? postProcess.grainIntensity : 0.0f;
			commonPostProcess.vignetteIntensity 			= postProcess.vignette == true ? postProcess.vignetteIntensity : 0.0f;
			commonPostProcess.vignetteScale 				= postProcess.vignetteScale;
			commonPostProcess.vignetteColor 				= postProcess.vignetteColor;
			commonPostProcess.chromaticAberration 			= postProcess.chromaticAberration;
			commonPostProcess.chromaticAberrationDistortion = postProcess.chromaticAberrationDistortion;
			commonPostProcess.chromaticAberrationIntensity 	= postProcess.chromaticAberrationIntensity;
		}
	}
}
