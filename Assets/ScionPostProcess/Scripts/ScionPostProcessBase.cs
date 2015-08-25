using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

namespace ScionEngine
{
	[ExecuteInEditMode, AddComponentMenu("Image Effects/Scion Post Process")]
	[RequireComponent(typeof(Camera))]
	public abstract class ScionPostProcessBase : MonoBehaviour 
	{					
		[Inspector.Decorations.Header(0, "Grain")]
		[Inspector.Toggle("Active", useProperty = "grain", tooltip = "Determines if grain is used")]
		public bool m_grain = true;
		protected bool ShowGrain() { return m_grain; }
		[Inspector.Slider("Intensity", useProperty = "grainIntensity", visibleCheck = "ShowGrain", minValue = 0.0f, maxValue = 1.0f, tooltip = "How strong the grain effect is")]
		public float m_grainIntensity = 0.12f;	
		
		[Inspector.Decorations.Header(0, "Vignette")]	
		[Inspector.Toggle("Active", useProperty = "vignette", tooltip = "Determines if vignette is used")]
		public bool m_vignette = true;
		protected bool ShowVignette() { return m_vignette; }
		[Inspector.Slider("Intensity", useProperty = "vignetteIntensity", visibleCheck = "ShowVignette", minValue = 0.0f, maxValue = 1.0f, tooltip = "How strong the vignette effect is")]
		public float m_vignetteIntensity = 0.7f;
		[Inspector.Slider("Scale", useProperty = "vignetteScale", visibleCheck = "ShowVignette", minValue = 0.0f, maxValue = 1.0f, tooltip = "How much of the screen is affected")]
		public float m_vignetteScale = 0.7f;
		[Inspector.Field("Color", useProperty = "vignetteColor", visibleCheck = "ShowVignette", tooltip = "What color the vignette effect has")]
		public Color m_vignetteColor = Color.black;
		
		[Inspector.Decorations.Header(0, "Chromatic Aberration")]
		[Inspector.Toggle("Active", useProperty = "chromaticAberration", tooltip = "Determines if chromatic aberration is used")]
		public bool m_chromaticAberration = true;
		protected bool ShowChromaticAberration() { return m_chromaticAberration; }
		[Inspector.Slider("Distortion Scale", useProperty = "chromaticAberrationDistortion", visibleCheck = "ShowChromaticAberration", minValue = 0.0f, maxValue = 1.0f, tooltip = "How much of the screen is affected")]
		public float m_chromaticAberrationDistortion = 0.5f;
		[Inspector.Slider("Intensity", useProperty = "chromaticAberrationIntensity", visibleCheck = "ShowChromaticAberration", minValue = -30.0f, maxValue = 30.0f, tooltip = "How strong the distortion effect is")]
		public float m_chromaticAberrationIntensity = 10.0f;
		
		[Inspector.Decorations.Header(0, "Bloom")]
		[Inspector.Toggle("Active", useProperty = "bloom", tooltip = "Determines if bloom is used")]
		public bool m_bloom = true;
		protected bool ShowBloom() { return bloom; }
		[Inspector.Slider("Intensity", useProperty = "bloomIntensity", visibleCheck = "ShowBloom", minValue = 0.0f, maxValue = 1.0f, tooltip = "How strong the bloom effect is")]
		public float m_bloomIntensity = 0.35f;
		[Inspector.Slider("Brightness", useProperty = "bloomBrightness", visibleCheck = "ShowBloom", minValue = 0.25f, maxValue = 4.0f, tooltip = "How bright the bloom effect is")]
		public float m_bloomBrightness = 1.2f;
		[Inspector.Slider("Range", useProperty = "bloomDistanceMultiplier", visibleCheck = "ShowBloom", minValue = 0.25f, maxValue = 1.25f, tooltip = "Modifies the range of the bloom")]
		public float m_bloomDistanceMultiplier = 1.0f;
		
		[Inspector.Slider("Downsamples", useProperty = "bloomDownsamples", visibleCheck = "ShowBloom", minValue = 2.0f, maxValue = 9.0f, tooltip = "Number of downsamples")]
		public int m_bloomDownsamples = 7;
		
		[Inspector.Decorations.Header(0, "Lens Dirt", visibleCheck = "ShowBloom")]
		[Inspector.Toggle("Active", useProperty = "lensDirt", visibleCheck = "ShowBloom", tooltip = "Determines if lens dirt is used")]
		public bool m_lensDirt = false;
		protected bool ShowLensDirt() { return ShowBloom() && lensDirt; }
		[Inspector.Field("Dirt Texture", useProperty = "lensDirtTexture", visibleCheck = "ShowLensDirt", tooltip = "The texture used as lens dirt")]
		public Texture2D m_lensDirtTexture = null;
		protected bool ShowLensDirtSettings() { return lensDirtTexture != null && ShowLensDirt(); }
		[Inspector.Slider("Intensity", useProperty = "lensDirtIntensity", visibleCheck = "ShowLensDirtSettings", minValue = 0.0f, maxValue = 1.0f, tooltip = "How strong the lens dirt effect is")]
		public float m_lensDirtIntensity = 0.5f;
		[Inspector.Slider("Brightness", useProperty = "lensDirtBrightness", visibleCheck = "ShowLensDirtSettings", minValue = 0.5f, maxValue = 2.0f, tooltip = "How bright the lens dirt effect is")]
		public float m_lensDirtBrightness = 1.2f;

		protected virtual bool ShowTonemapping() { return false; }

		[Inspector.Decorations.Header(0, "Tonemapping", visibleCheck = "ShowTonemapping")]
		[Inspector.Field("Mode", useProperty = "tonemappingMode", visibleCheck = "ShowTonemapping", tooltip = "What type of tonemapping algorithm is used")]
		public TonemappingMode m_tonemappingMode = TonemappingMode.Filmic;
		[Inspector.Slider("White Point", useProperty = "whitePoint", visibleCheck = "ShowTonemapping", minValue = 0.5f, maxValue = 20.0f, tooltip = "At what intensity pixels will become white")]
		public float m_whitePoint = ScionUtility.DefaultWhitePoint;
		
		//protected bool ShowCameraMode() { return m_camera.hdr == true; }
		protected bool ShowCameraMode() { return true; }
		protected bool ShowExposureComp() { return cameraMode != CameraMode.Off; }
		protected bool ShowExposureAdaption() { return cameraMode != CameraMode.Off && cameraMode != CameraMode.Manual; }
		protected bool ShowDownsampleBloomExposure() { return ShowExposureAdaption() && ShowBloom(); }
		
		[Inspector.Decorations.Header(0, "Camera Mode")]
		[Inspector.Field("Camera Mode", useProperty = "cameraMode", visibleCheck = "ShowCameraMode", tooltip = "What camera mode is used")]
		public CameraMode m_cameraMode = CameraMode.AutoPriority;
		
		protected bool ShowFocalLength() { return m_userControlledFocalLength; }
		protected bool ShowFNumber() { return cameraMode == CameraMode.AperturePriority || cameraMode == CameraMode.Manual || (cameraMode == CameraMode.Off && depthOfField == true); }
		protected bool ShowISO() { return cameraMode == CameraMode.Manual; }
		//protected bool ShowShutterSpeed() { return cameraMode == CameraMode.ShutterPriority || cameraMode == CameraMode.Manual; }
		protected bool ShowShutterSpeed() { return cameraMode == CameraMode.Manual; }
		
		[Inspector.Slider("F Number", useProperty = "fNumber", visibleCheck = "ShowFNumber", minValue = 1.0f, maxValue = 22.0f, tooltip = "The F number of the camera")]
		public float m_fNumber = 4.0f;
		[Inspector.Slider("ISO", useProperty = "ISO", visibleCheck = "ShowISO", minValue = 100.0f, maxValue = 6400.0f, tooltip = "The ISO setting of the camera")]
		public float m_ISO = 100.0f;
		[Inspector.Slider("Shutter Speed", useProperty = "shutterSpeed", visibleCheck = "ShowShutterSpeed", minValue = 1.0f/4000.0f, maxValue = 1.0f/30.0f, tooltip = "The shutted speed of the camera")]
		public float m_shutterSpeed = 0.01f;
		[Inspector.Toggle("Custom Focal Length", useProperty = "userControlledFocalLength", tooltip = "If false the focal length will instead be derived from the camera's field of view")]
		public bool m_userControlledFocalLength = false;
		[Inspector.Slider("Focal Length", useProperty = "focalLength", visibleCheck = "ShowFocalLength", minValue = 10.0f, maxValue = 250.0f, tooltip = "The focal length of the camera in millimeters")]
		public float m_focalLength = 15.0f;
		
		[Inspector.Decorations.Header(0, "Exposure Settings")]
		[Inspector.Slider("Exposure Compensation", useProperty = "exposureCompensation", visibleCheck = "ShowExposureComp", minValue = -8.0f, maxValue = 8.0f, 
		                  tooltip = "Allows you to manually compensate towards the desired exposure")]
		public float m_exposureCompensation = 0.0f;
		[Inspector.MinMaxSlider("Min Max Exposure", -8.0f, 24.0f, useProperty = "minMaxExposure")]
		public Vector2 m_minMaxExposure = new Vector2(-8.0f, 24.0f);
		[Inspector.Slider("Adaption Speed", useProperty = "adaptionSpeed", visibleCheck = "ShowExposureAdaption", minValue = 0.1f, maxValue = 8.0f, tooltip = "How fast the exposure is allowed to change")]
		public float m_adaptionSpeed = 1.0f;
		
		protected bool ShowDepthOfField() { return depthOfField; }
		protected bool ShowPointAverage() { return m_depthFocusMode == DepthFocusMode.PointAverage && ShowDepthOfField(); }
		protected bool ShowFocalDistance() { return (m_depthFocusMode == DepthFocusMode.ManualDistance || m_depthFocusMode == DepthFocusMode.ManualRange) && ShowDepthOfField(); }
		protected bool ShowFocalRange() { return m_depthFocusMode == DepthFocusMode.ManualRange && ShowDepthOfField(); }
		
		[Inspector.Decorations.Header(0, "Depth of Field")]
		[Inspector.Toggle("Active", useProperty = "depthOfField", tooltip = "Determines if depth of field is used")]
		public bool m_depthOfField = true;	
		[Tooltip("Excludes layers from the depth of field")]
		public LayerMask m_exclusionMask;
		[Inspector.Slider("Max Radius", useProperty = "maxCoCRadius", visibleCheck = "ShowDepthOfField", minValue = 10.0f, maxValue = 20.0f, tooltip = "The maximum radius the blur can be. Lower values might have less artifacts")]
		public float m_maxCoCRadius = 20.0f;
		[Inspector.Field("Quality Level", useProperty = "depthOfFieldQuality", visibleCheck = "ShowDepthOfField", tooltip = "Dictates how many samples the algorithm does")]
		public DepthOfFieldQuality m_depthOfFieldQuality = DepthOfFieldQuality.Normal;
		[Inspector.Field("Depth Focus Mode", useProperty = "depthFocusMode", visibleCheck = "ShowDepthOfField", tooltip = "How the depth focus point is chosen")]
		public DepthFocusMode m_depthFocusMode = DepthFocusMode.PointAverage;
		[Inspector.Field("Point Center", useProperty = "pointAveragePosition", visibleCheck = "ShowPointAverage", tooltip = "Where the center of focus is on the screen." +
		                 " [0,0] is the bottom left corner and [1,1] is the top right")]
		public Vector2 m_pointAveragePosition = new Vector2(0.5f, 0.5f); 	
		[Inspector.Decorations.Space(0, 1)]
		[Inspector.Slider("Point Range", useProperty = "pointAverageRange", visibleCheck = "ShowPointAverage", minValue = 0.01f, maxValue = 1.0f, tooltip = "How far the point average calculation reaches")]
		public float m_pointAverageRange = 0.2f;
		[Inspector.Toggle("Visualize", useProperty = "visualizePointFocus", visibleCheck = "ShowPointAverage", tooltip = "Show the area of influence on the main screen for visualizaiton")]
		public bool m_visualizePointFocus = false;
		[Inspector.Slider("Adaption Speed", useProperty = "depthAdaptionSpeed", visibleCheck = "ShowPointAverage", minValue = 1.0f, maxValue = 30.0f, tooltip = "Dictates how fast the focal distance changes")]
		public float m_depthAdaptionSpeed = 10.0f;
		[Inspector.Field("Focal Distance", useProperty = "focalDistance", visibleCheck = "ShowFocalDistance", tooltip = "The focal distance in meters")]
		public float m_focalDistance = 10.0f;
		[Inspector.Slider("Depth Range", useProperty = "focalRange", visibleCheck = "ShowFocalRange", minValue = 0.0f, maxValue = 50.0f, tooltip = "The length of the range that is 100% in focus")]
		public float m_focalRange = 10.0f;
		
		protected bool ShowCCTex1() { return colorGradingMode == ColorGradingMode.On || colorGradingMode == ColorGradingMode.Blend; }
		protected bool ShowCCTex2() { return colorGradingMode == ColorGradingMode.Blend; }
		
		[Inspector.Decorations.Header(0, "Color Correction")]
		[Inspector.Field("Mode", useProperty = "colorGradingMode", tooltip = "Which color correction mode is currently active")]
		public ColorGradingMode m_colorGradingMode = ColorGradingMode.Off;
		[Inspector.Field("Lookup Texture", useProperty = "colorGradingTex1", visibleCheck = "ShowCCTex1", tooltip = "The lookup texture used for color correction")]
		public Texture2D m_colorGradingTex1 = null;
		[Inspector.Field("Blend Lookup Texture", useProperty = "colorGradingTex2", visibleCheck = "ShowCCTex2", tooltip = "The lookup texture blended in as the blend factor increases")]
		public Texture2D m_colorGradingTex2 = null;
		[Inspector.Slider("Blend Factor", useProperty = "colorGradingBlendFactor", visibleCheck = "ShowCCTex2", minValue = 0.0f, maxValue = 1.0f, tooltip = "Interpolates between the original color correction texture and the blend target color correction texture")]
		public float m_colorGradingBlendFactor = 0.0f;
		
		
		protected bool m_isFirstRender;
		protected float prevCamFoV;
		
		protected Camera m_camera;
		protected Transform m_cameraTransform;
		protected Bloom m_bloomClass;
		protected VirtualCamera m_virtualCamera;
		protected CombinationPass m_combinationPass;
		protected Downsampling m_downsampling;
		protected DepthOfField m_depthOfFieldClass;
		
		protected PostProcessParameters postProcessParams = new PostProcessParameters();
		
		protected ScionDebug m_scionDebug;
		public static ScionDebug ActiveDebug;
		
		
		
		
		public CameraMode cameraMode 
		{
			get { return m_cameraMode; }
			set 
			{ 
				m_cameraMode = value; 
				postProcessParams.cameraParams.cameraMode = value;
				postProcessParams.exposure = value != CameraMode.Off ? true : false;
			}
		}
		public bool bloom 
		{
			get { return m_bloom; }
			set { m_bloom = value; postProcessParams.bloom = value; }
		}
		public bool lensDirt 
		{
			get { return m_lensDirt; }
			set { m_lensDirt = value; postProcessParams.lensDirt = value; }
		}
		public Texture2D lensDirtTexture 
		{
			get { return m_lensDirtTexture; }
			set { m_lensDirtTexture = value; postProcessParams.lensDirtTexture = value; }
		}
		public bool depthOfField 
		{
			get { return m_depthOfField; }
			set 
			{ 
				m_depthOfField = value; 
				postProcessParams.depthOfField = value; 
				PlatformCompatibility();
			}
		}
		
		public float bloomIntensity 
		{
			get { return m_bloomIntensity; }
			set { m_bloomIntensity = value; postProcessParams.bloomParams.intensity = ScionUtility.Square(value); }
		}
		public float bloomBrightness 
		{
			get { return m_bloomBrightness; }
			set { m_bloomBrightness = value; postProcessParams.bloomParams.brightness = value; }
		}
		public float bloomDistanceMultiplier
		{
			get { return m_bloomDistanceMultiplier; }
			set { m_bloomDistanceMultiplier = value; postProcessParams.bloomParams.distanceMultiplier = value; }
		}
		public int bloomDownsamples 
		{
			get { return m_bloomDownsamples; }
			set { m_bloomDownsamples = value; postProcessParams.bloomParams.downsamples = value; }
		}
		
		public float lensDirtIntensity 
		{
			get { return m_lensDirtIntensity; }
			set { m_lensDirtIntensity = value; postProcessParams.lensDirtParams.intensity = ScionUtility.Square(value); }
		}
		public float lensDirtBrightness 
		{
			get { return m_lensDirtBrightness; }
			set { m_lensDirtBrightness = value; postProcessParams.lensDirtParams.brightness = value; }
		}

		public TonemappingMode tonemappingMode
		{
			get { return m_tonemappingMode; }
			set { m_tonemappingMode = value; }
		}
		public float whitePoint 
		{
			get { return m_whitePoint; }
			set { m_whitePoint = value; postProcessParams.commonPostProcess.whitePoint = value; }
		}
		
		public LayerMask exclusionMask
		{
			get { return m_exclusionMask; }
			set { m_exclusionMask = value; postProcessParams.DoFParams.depthOfFieldMask = value; }
		}
		public DepthFocusMode depthFocusMode 
		{
			get { return m_depthFocusMode; }
			set { m_depthFocusMode = value; postProcessParams.DoFParams.depthFocusMode = value; }
		}
		public float maxCoCRadius 
		{
			get { return m_maxCoCRadius; }
			set { m_maxCoCRadius = value; postProcessParams.DoFParams.maxCoCRadius = value; }
		}
		public DepthOfFieldQuality depthOfFieldQuality 
		{
			get { return m_depthOfFieldQuality; }
			set { m_depthOfFieldQuality = value; postProcessParams.DoFParams.quality = SystemInfo.graphicsShaderLevel < 40 ? DepthOfFieldQuality.Normal : value; }
		}
		public Vector2 pointAveragePosition 
		{
			get { return m_pointAveragePosition; }
			set { m_pointAveragePosition = value; postProcessParams.DoFParams.pointAveragePosition = value; }
		}
		public float pointAverageRange 
		{
			get { return m_pointAverageRange; }
			set { m_pointAverageRange = value; postProcessParams.DoFParams.pointAverageRange = value; }
		}
		public bool visualizePointFocus 
		{
			get { return m_visualizePointFocus; }
			set { m_visualizePointFocus = value; postProcessParams.DoFParams.visualizePointFocus = value; }
		}
		public float depthAdaptionSpeed 
		{
			get { return m_depthAdaptionSpeed; }
			set { m_depthAdaptionSpeed = value; postProcessParams.DoFParams.depthAdaptionSpeed = value; }
		}
		public float focalDistance 
		{
			get { return m_focalDistance; }
			set { m_focalDistance = value; postProcessParams.DoFParams.focalDistance = value; }
		}
		public float focalRange 
		{
			get { return m_focalRange; }
			set { m_focalRange = value; postProcessParams.DoFParams.focalRange = value; }
		}
		
		//		DepthOfFieldParameters DoFParams 	= new DepthOfFieldParameters();
		//		DoFParams.useMedianFilter			= true; //This could technically be a user choice, but its a lot of quality for a low price
		//		DoFParams.depthFocusMode			= depthFocusMode;
		//		DoFParams.maxCoCRadius				= maxCoCRadius;
		//		DoFParams.quality					= SystemInfo.graphicsShaderLevel < 40 ? DepthOfFieldQuality.Normal : depthOfFieldQuality;
		//		DoFParams.pointAveragePosition		= pointAveragePosition;
		//		DoFParams.pointAverageRange			= pointAverageRange;
		//		DoFParams.visualizePointFocus		= visualizePointFocus;
		//		DoFParams.depthAdaptionSpeed		= depthAdaptionSpeed;
		//		DoFParams.focalDistance 			= focalDistance;
		//		DoFParams.focalRange 				= focalRange;
		//		postProcessParams.DoFParams			= DoFParams;
		
		public ColorGradingMode colorGradingMode 
		{
			get { return m_colorGradingMode; }
			set 
			{ 
				m_colorGradingMode = value; 
				postProcessParams.colorGradingParams.colorGradingMode = colorGradingTex1 == null ? ColorGradingMode.Off : value; 
			}
		}
		public Texture2D colorGradingTex1 
		{
			get { return m_colorGradingTex1; }
			set 
			{ 
				m_colorGradingTex1 = value; 
				postProcessParams.colorGradingParams.colorGradingTex1 = value; 
				colorGradingMode = colorGradingMode; //Rerun this property to update state based on texture
			}
		}
		public Texture2D colorGradingTex2 
		{
			get { return m_colorGradingTex2; }
			set 
			{ 
				m_colorGradingTex2 = value; 
				postProcessParams.colorGradingParams.colorGradingTex2 = value; 
			}
		}
		public float colorGradingBlendFactor 
		{
			get { return m_colorGradingBlendFactor; }
			set 
			{ 
				float clampedValue = Mathf.Clamp01(value);
				m_colorGradingBlendFactor = clampedValue; 
				postProcessParams.colorGradingParams.colorGradingBlendFactor = clampedValue; 
			}
		}
		
		public bool userControlledFocalLength 
		{
			get { return m_userControlledFocalLength; }
			set { m_userControlledFocalLength = value; }
		}
		public float focalLength 
		{
			get { return m_focalLength; }
			set { m_focalLength = value; postProcessParams.cameraParams.focalLength = value; }
		}
		public float fNumber 
		{
			get { return m_fNumber; }
			set { m_fNumber = value; postProcessParams.cameraParams.fNumber = value; }
		}
		public float ISO 
		{
			get { return m_ISO; }
			set { m_ISO = value; postProcessParams.cameraParams.ISO = value; }
		}
		public float shutterSpeed 
		{
			get { return m_shutterSpeed; }
			set { m_shutterSpeed = value; postProcessParams.cameraParams.shutterSpeed = value; }
		}
		public float adaptionSpeed 
		{
			get { return m_adaptionSpeed; }
			set { m_adaptionSpeed = value; postProcessParams.cameraParams.adaptionSpeed = value; }
		}
		public Vector2 minMaxExposure 
		{
			get { return m_minMaxExposure; }
			set { m_minMaxExposure = value; postProcessParams.cameraParams.minMaxExposure = value; }
		}
		public float exposureCompensation 
		{
			get { return m_exposureCompensation; }
			set { m_exposureCompensation = value; postProcessParams.cameraParams.exposureCompensation = value; }
		}
		
		public bool grain 
		{
			get { return m_grain; }
			set { m_grain = value; postProcessParams.commonPostProcess.grainIntensity = m_grain == true ? grainIntensity : 0.0f; }
		}
		public float grainIntensity 
		{
			get { return m_grainIntensity; }
			set { m_grainIntensity = value; postProcessParams.commonPostProcess.grainIntensity = m_grain == true ? grainIntensity : 0.0f; }
		}
		public bool vignette 
		{
			get { return m_vignette; }
			set { m_vignette = value; postProcessParams.commonPostProcess.vignetteIntensity = m_vignette == true ? vignetteIntensity : 0.0f; }
		}
		public float vignetteIntensity 
		{
			get { return m_vignetteIntensity; }
			set { m_vignetteIntensity = value; postProcessParams.commonPostProcess.vignetteIntensity = m_vignette == true ? vignetteIntensity : 0.0f; }
		}
		public float vignetteScale 
		{
			get { return m_vignetteScale; }
			set { m_vignetteScale = value; postProcessParams.commonPostProcess.vignetteScale = value;}
		}
		public Color vignetteColor 
		{
			get { return m_vignetteColor; }
			set { m_vignetteColor = value; postProcessParams.commonPostProcess.vignetteColor = value; }
		}
		public bool chromaticAberration 
		{
			get { return m_chromaticAberration; }
			set { m_chromaticAberration = value; postProcessParams.commonPostProcess.chromaticAberration = value; }
		}
		public float chromaticAberrationDistortion 
		{
			get { return m_chromaticAberrationDistortion; }
			set { m_chromaticAberrationDistortion = value; postProcessParams.commonPostProcess.chromaticAberrationDistortion = value; }
		}
		public float chromaticAberrationIntensity 
		{
			get { return m_chromaticAberrationIntensity; }
			set { m_chromaticAberrationIntensity = value; postProcessParams.commonPostProcess.chromaticAberrationIntensity = value; }
		}
		
		protected void OnEnable() 
		{			
			m_camera 				= GetComponent<Camera>();
			m_cameraTransform		= m_camera.transform;
			m_bloomClass			= new Bloom();
			m_combinationPass 		= new CombinationPass();
			m_downsampling			= new Downsampling();
			m_virtualCamera 		= new VirtualCamera();
			m_depthOfFieldClass		= new DepthOfField();
			m_scionDebug			= new ScionDebug();
			m_isFirstRender			= true;
			
			if (PlatformCompatibility() == false) this.enabled = false;

			InitializePostProcessParams();
		}

		protected virtual void InitializePostProcessParams()
		{			
			postProcessParams.Fill(this);
		}
		
		protected void OnDisable() 
		{
			if (m_bloomClass != null) m_bloomClass.ReleaseResources();
		}
		
		protected void OnPreRender()
		{
			m_camera.depthTextureMode |= DepthTextureMode.Depth;
		}
				
		protected bool PlatformCompatibility()
		{			
			if (SystemInfo.supportsImageEffects == false)
			{
				Debug.LogWarning("Image Effects are not supported on this platform");
				return false;
			}
			
			if (SystemInfo.supportsRenderTextures == false)
			{
				Debug.LogWarning("RenderTextures are not supported on this platform");
				return false;
			}
			
			if (m_bloomClass.PlatformCompatibility() == false) 
			{
				Debug.LogWarning("Bloom shader not supported on this platform");
				return false;
			}
			
			if (m_combinationPass.PlatformCompatibility() == false) 
			{
				Debug.LogWarning("Combination shader not supported on this platform");
				return false;
			}
			
			if (m_virtualCamera.PlatformCompatibility() == false) 
			{
				Debug.LogWarning("Virtual camera shader not supported on this platform");
				return false;
			}
			
			if (m_depthOfFieldClass.PlatformCompatibility() == false && depthOfField == true)
			{
				return false;
			}
			
			return true;
		}
		
		protected void SetupPostProcessParameters(PostProcessParameters postProcessParams, RenderTexture source)
		{
			focalDistance = focalDistance < m_camera.nearClipPlane + 0.3f ? m_camera.nearClipPlane + 0.3f : focalDistance;
			
			postProcessParams.camera = m_camera;
			postProcessParams.cameraTransform = m_cameraTransform;
			
			//Done later
			postProcessParams.halfResSource 	= null; 
			postProcessParams.halfResDepth		= m_downsampling.DownsampleDepthTexture(source.width, source.height);
			
			postProcessParams.width 			= source.width;
			postProcessParams.height 			= source.height;
			postProcessParams.halfWidth 		= source.width / 2;
			postProcessParams.halfHeight 		= source.height / 2;
			
			if (prevCamFoV != m_camera.fieldOfView)
			{
				postProcessParams.preCalcValues.tanHalfFoV = Mathf.Tan(m_camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
				prevCamFoV = m_camera.fieldOfView;
			}
			
			postProcessParams.DoFParams.useMedianFilter = true; //This could technically be a user choice, but its a lot of quality for a low price
			
			if (userControlledFocalLength == false) postProcessParams.cameraParams.focalLength = ScionUtility.GetFocalLength(postProcessParams.preCalcValues.tanHalfFoV);
			else postProcessParams.cameraParams.focalLength = focalLength * 0.001f; //Millimeter to meter
			postProcessParams.cameraParams.apertureDiameter 		= ScionUtility.ComputeApertureDiameter(fNumber, postProcessParams.cameraParams.focalLength);
			
			postProcessParams.cameraParams.fieldOfView	= m_camera.fieldOfView;
			postProcessParams.cameraParams.aspect		= m_camera.aspect;
			postProcessParams.cameraParams.nearPlane	= m_camera.nearClipPlane;
			postProcessParams.cameraParams.farPlane		= m_camera.farClipPlane;
			
			postProcessParams.isFirstRender = m_isFirstRender;
			m_isFirstRender 				= false;
		}
		
		protected void SetGlobalParameters(PostProcessParameters postProcessParams)
		{
			Vector4 nearFarParams = new Vector4();
			nearFarParams.x = postProcessParams.cameraParams.nearPlane;
			nearFarParams.y = postProcessParams.cameraParams.farPlane;
			nearFarParams.z = 1.0f / nearFarParams.y;
			nearFarParams.w = nearFarParams.x * nearFarParams.z;
			Shader.SetGlobalVector("_ScionNearFarParams", nearFarParams);
			
			Vector4 resolutionParams1 = new Vector4();
			resolutionParams1.x = postProcessParams.halfWidth;
			resolutionParams1.y = postProcessParams.halfHeight;
			resolutionParams1.z = postProcessParams.width;
			resolutionParams1.w = postProcessParams.height;
			Shader.SetGlobalVector("_ScionResolutionParameters1", resolutionParams1);
			
			Vector4 resolutionParams2 = new Vector4();
			resolutionParams2.x = 1.0f / postProcessParams.halfWidth;
			resolutionParams2.y = 1.0f / postProcessParams.halfHeight;
			resolutionParams2.z = 1.0f / postProcessParams.width;
			resolutionParams2.w = 1.0f / postProcessParams.height;
			Shader.SetGlobalVector("_ScionResolutionParameters2", resolutionParams2);			
			
			Vector4 cameraParams1 = new Vector4();
			cameraParams1.x = postProcessParams.cameraParams.apertureDiameter;
			cameraParams1.y = postProcessParams.cameraParams.focalLength;
			cameraParams1.z = postProcessParams.cameraParams.aspect;
			Shader.SetGlobalVector("_ScionCameraParams1", cameraParams1);
			
			Shader.SetGlobalTexture("_HalfResDepthTexture", postProcessParams.halfResDepth);
		}
		
		protected virtual void SetShaderKeyWords(PostProcessParameters postProcessParams)
		{
			if (postProcessParams.cameraParams.cameraMode == CameraMode.Off ||
			    postProcessParams.cameraParams.cameraMode == CameraMode.Manual)
			{
				ShaderSettings.ExposureSettings.SetIndex(1);
			}
			else ShaderSettings.ExposureSettings.SetIndex(0);
			
			switch (postProcessParams.DoFParams.depthFocusMode)
			{
			case (DepthFocusMode.ManualDistance):
				ShaderSettings.DepthFocusSettings.SetIndex(0);
				break;
			case (DepthFocusMode.ManualRange):
				ShaderSettings.DepthFocusSettings.SetIndex(1);
				break;
			case (DepthFocusMode.PointAverage):
				ShaderSettings.DepthFocusSettings.SetIndex(2);
				break;
			}

			if (postProcessParams.DoFParams.depthOfFieldMask != 0) ShaderSettings.DepthOfFieldMask.SetIndex(1);
			else ShaderSettings.DepthOfFieldMask.SetIndex(0);
			
			switch (postProcessParams.colorGradingParams.colorGradingMode)
			{
			case (ColorGradingMode.Off):
				ShaderSettings.ColorGradingSettings.SetIndex(0);
				break;
			case (ColorGradingMode.On):
				ShaderSettings.ColorGradingSettings.SetIndex(1);
				break;
			case (ColorGradingMode.Blend):
				ShaderSettings.ColorGradingSettings.SetIndex(2);
				break;
			}
			
			if (postProcessParams.commonPostProcess.chromaticAberration == true) ShaderSettings.ChromaticAberrationSettings.SetIndex(1);
			else ShaderSettings.ChromaticAberrationSettings.SetIndex(0);
		}
		
		protected void SRGBSettings(PostProcessParameters postProcessParams)
		{
			//If color grading is on the sRGB conversion is handled manually instead
			if (postProcessParams.colorGradingParams.colorGradingMode != ColorGradingMode.Off)
			{
				GL.sRGBWrite = false;
			}
		}

		protected virtual void OnRenderImage (RenderTexture source, RenderTexture dest)
		{
			ActiveDebug = m_scionDebug;
			
			SetupPostProcessParameters(postProcessParams, source);
			
			//TODO: REMOVE REMOVE REMOVE when inspector attributes are updated
			postProcessParams.DoFParams.depthOfFieldMask = exclusionMask;
			
			SetGlobalParameters(postProcessParams);
			SRGBSettings(postProcessParams);
			SetShaderKeyWords(postProcessParams);
			PerformPostProcessing(source, dest, postProcessParams);
			
			ActiveDebug = null;
		}
		
		protected void PerformPostProcessing(RenderTexture source, RenderTexture dest, PostProcessParameters postProcessParams)
		{	
			//Graphics.Blit(source, dest);
			source = DepthOfFieldStep(postProcessParams, source);
			
			//Do this after DoF so DoF gets included (if active)
			postProcessParams.halfResSource = m_downsampling.DownsampleFireflyRemoving(source);
			//postProcessParams.halfResSource = m_downsampling.Downsample(source);
			
			if (postProcessParams.bloom == true) 
			{
				postProcessParams.bloomTexture = m_bloomClass.CreateBloomTexture(postProcessParams.halfResSource, postProcessParams.bloomParams);
				if (postProcessParams.exposure == true) 
				{
					const int minimumReqPixels = 100;
					int numSearches;
					RenderTexture textureToAverage = m_bloomClass.TryGetSmallBloomTexture(minimumReqPixels, out numSearches);
					float energyNormalizer = m_bloomClass.GetEnergyNormalizer(numSearches);
					if (textureToAverage == null) { textureToAverage = postProcessParams.halfResSource; energyNormalizer = 1.0f; }
					m_virtualCamera.CalculateVirtualCamera(postProcessParams.cameraParams, textureToAverage, postProcessParams.halfWidth, postProcessParams.preCalcValues.tanHalfFoV, 
					                                       energyNormalizer, postProcessParams.DoFParams.focalDistance, postProcessParams.isFirstRender);
				}
			}
			else if (postProcessParams.exposure == true)
			{
				m_virtualCamera.CalculateVirtualCamera(postProcessParams.cameraParams, postProcessParams.halfResSource, postProcessParams.halfWidth, 
				                                       postProcessParams.preCalcValues.tanHalfFoV, 1.0f, postProcessParams.DoFParams.focalDistance, postProcessParams.isFirstRender);
			}
			
			//Graphics.Blit(source, dest);
			m_combinationPass.Combine(source, dest, postProcessParams, m_virtualCamera);
			m_scionDebug.VisualizeDebug(dest);
			
			RenderTexture.ReleaseTemporary(postProcessParams.halfResSource);
			RenderTexture.ReleaseTemporary(postProcessParams.halfResDepth);
			RenderTexture.ReleaseTemporary(postProcessParams.dofTexture);
			
			m_bloomClass.EndOfFrameCleanup();
			m_virtualCamera.EndOfFrameCleanup();
			m_depthOfFieldClass.EndOfFrameCleanup();
			
			if (postProcessParams.depthOfField == true) RenderTexture.ReleaseTemporary(source);
		}
		
		//This function is also responsible for downsampling the depth buffer and binding it
		protected RenderTexture DepthOfFieldStep(PostProcessParameters postProcessParams, RenderTexture source)
		{		
			if (postProcessParams.depthOfField == false) return source;
			
			RenderTexture exclusionMask = null;
			if (postProcessParams.DoFParams.depthOfFieldMask != 0) //If objects are masked out
			{
				exclusionMask = m_depthOfFieldClass.RenderExclusionMask(postProcessParams.width, postProcessParams.height, postProcessParams.camera, 
				                                                        postProcessParams.cameraTransform, postProcessParams.DoFParams.depthOfFieldMask);
				//ScionPostProcess.ActiveDebug.RegisterTextureForVisualization(exclusionMask, false, false, false);
				RenderTexture downsampledExclusionMask = m_downsampling.DownsampleMinFilter(source.width, source.height, exclusionMask);
				RenderTexture.ReleaseTemporary(exclusionMask);
				exclusionMask = downsampledExclusionMask;
			}
			
			//postProcessParams.halfResSource = m_downsampling.DownsampleFireflyRemoving(source);
			postProcessParams.halfResSource = m_downsampling.DownsampleFireflyRemovingBilateral(source, postProcessParams.halfResDepth);
			
			source = m_depthOfFieldClass.RenderDepthOfField(postProcessParams, source, m_virtualCamera, exclusionMask);	
			
			//Downsample scene again, this time with DoF applied
			RenderTexture.ReleaseTemporary(postProcessParams.halfResSource);
			if (exclusionMask != null) RenderTexture.ReleaseTemporary(exclusionMask);
			
			return source;
		}
	}
}