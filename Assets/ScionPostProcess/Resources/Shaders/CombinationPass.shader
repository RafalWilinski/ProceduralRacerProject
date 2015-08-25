Shader "Hidden/ScionCombinationPass" 
{	
 	Properties 
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    
	CGINCLUDE
	#include "UnityCG.cginc" 
	#include "../ShaderIncludes/ScionCommon.cginc" 
	#include "../ShaderIncludes/ColorGradingCommon.cginc" 
    
	struct v2f
	{
	    float4 pos : SV_POSITION;
	    float2 uv : TEXCOORD0;	    
	};	
	
	#define CHROMATIC_ABERRATION_SAMPLES 5
	
	uniform sampler2D _MainTex;
	uniform float4 _MainTex_TexelSize;
	
	uniform sampler2D _VirtualCameraResult;
	uniform float _ManualExposure;
	
	uniform sampler2D _BloomTexture;
	uniform sampler2D _LensDirtTexture;
	uniform sampler2D _BlurTexture;

	uniform float4 _BloomParameters;
	#define BloomIntensity 	_BloomParameters.x
	#define BloomBrightness _BloomParameters.y
	
	uniform float4 _LensDirtParameters;
	#define DirtIntensity 	_LensDirtParameters.x	
	#define DirtBrightness 	_LensDirtParameters.y

	uniform float4 _PostProcessParams1;
	#define GrainIntensity 				_PostProcessParams1.x
	#define VignetteIntensity 			_PostProcessParams1.y
	#define VignetteScale				_PostProcessParams1.z
	#define ChromaticDistortionScale 	_PostProcessParams1.w

	uniform float4 _PostProcessParams2;
	#define VignetteColor 				_PostProcessParams2.xyz
	#define ChromaticIntensity 			_PostProcessParams2.w

	uniform float4 _PostProcessParams3;
	#define GrainSeed 					_PostProcessParams3.x
	#define WhitePointMult				_PostProcessParams3.y
	#define InvWhitePoint				_PostProcessParams3.z	
	
	//Removes grain from the shader completely, making it cheaper
	//#define FORCE_GRAIN_OFF
		
	v2f vert (appdata_img v)
	{
		v2f o;
		
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord.xy;
		#if UNITY_UV_STARTS_AT_TOP
        if (_MainTex_TexelSize.y < 0.0f) o.uv.y = 1.0f - o.uv.y; 
		#endif

		return o;
	}
	
	float3 ColorGrading(float3 clr)
	{		
		#ifdef SC_COLOR_CORRECTION_OFF
		return clr;
		#endif
		
		//Convert from linear to sRGB
		clr = AccurateLinearToSRGB(saturate(clr));
		
		#ifdef SC_COLOR_CORRECTION_1_TEX
		clr = ApplyColorGrading(clr, _ColorGradingLUT1);
		#endif
		
		#ifdef SC_COLOR_CORRECTION_2_TEX
		clr = ApplyColorGrading(clr, _ColorGradingLUT1, _ColorGradingLUT2, _ColorGradingBlendFactor);
		#endif
		
		return clr;
	}
	
	float3 SampleInputTexture(float2 uv)
	{	
		#ifdef SC_CHROMATIC_ABERRATION_ON
		float3 samplesSum = 0.0f;
		float3 weightsSum = 0.0f;
		
		SCION_UNROLL for (int k = 0; k < CHROMATIC_ABERRATION_SAMPLES; k++)
		{
			float iterStep = k / (CHROMATIC_ABERRATION_SAMPLES - 1.0f);
			float3 spectrumWeights = SpectrumOffset(iterStep);			
			
			samplesSum += spectrumWeights * tex2Dlod(_MainTex, float4(VignettedDistortion(uv, iterStep * ChromaticIntensity, ChromaticDistortionScale), 0.0f, 0.0f)).xyz;
			weightsSum += spectrumWeights;
		}
			
		return samplesSum / weightsSum;
		#endif
		
		#ifdef SC_CHROMATIC_ABERRATION_OFF
		return tex2Dlod(_MainTex, float4(uv, 0.0f, 0.0f)).xyz;
		#endif
	}
	
	float3 VariousPostProcessing(float3 clr, float2 uv)
	{
		clr = Vignette(clr, uv, VignetteScale, VignetteIntensity, VignetteColor);	
		
		#ifndef FORCE_GRAIN_OFF			
		clr = Grain(clr, uv, GrainIntensity, GrainSeed);
		#endif
		
		return clr;
	}
	
	float3 FilmicTonemapping(float3 clr)
	{
		clr = (clr * (0.22 * clr + 0.03) + 0.002) / (clr * (0.22 * clr + 0.3) + 0.06f) - 0.033334;		
		return clr;
	}
	
	float3 LumaFilmicTonemapping(float3 clr)
	{
		float luma = Luma(clr);
		float tonemappedLuma = (luma * (0.22 * luma + 0.03) + 0.002) / (luma * (0.22 * luma + 0.3) + 0.06f) - 0.033334;	
		clr = clr * tonemappedLuma / luma;
		return clr;
	}
	
	float3 PhotographicTonemapping(float3 clr)
	{
		return 1.0f - exp2(-clr);
	}

	float3 ReinhardTonemapping(float3 clr)
	{
		clr = clr / (clr + float3(1.0f, 1.0f, 1.0f));
		return clr;
	}

	float3 LumaReinhardTonemapping(float3 clr)
	{
		float luma = Luma(clr);
		float toneMappedLuma = luma * (1.0f + luma * InvWhitePoint) / (1.0f + luma);
		clr *= toneMappedLuma / luma;
		return clr;
	}
	
	float3 Tonemapping(float3 clr)
	{		
		//Clamp to avoid artifacts
		clr = min(clr, 1000.0f);	
							
		#ifdef SC_TONEMAPPING_REINHARD
		clr = ReinhardTonemapping(clr) * WhitePointMult;
		#endif		
			
		#ifdef SC_TONEMAPPING_LUMAREINHARD
		clr = LumaReinhardTonemapping(clr);
		#endif	
				
		#ifdef SC_TONEMAPPING_FILMIC
		clr = FilmicTonemapping(clr) * WhitePointMult;
		#endif
		
		#ifdef SC_TONEMAPPING_PHOTOGRAPHIC
		clr = PhotographicTonemapping(clr) * WhitePointMult;
		#endif
	
		return clr;
	}
	
	float3 GetBloomSamples(float2 screenUV)
	{	
		const float2 texel = InvHalfResSize;			
		const float weights[9] = { 1.0f/16.0f, 2.0f/16.0f, 1.0f/16.0f, 2.0f/16.0f, 4.0f/16.0f, 2.0f/16.0f, 1.0f/16.0f, 2.0f/16.0f, 1.0f/16.0f };
		
		float4 outColor = float4(0.0f, 0.0f, 0.0f, 0.0f);		
		int weightIndex = 0;
		
		SCION_UNROLL for (int x = -1; x < 2; x++)
		{		
			SCION_UNROLL for (int y = -1; y < 2; y++)
			{
				float2 uvOffset = texel * float2(x, y);
				float2 uv = screenUV + uvOffset;
				outColor = outColor + tex2Dlod(_BloomTexture, float4(uv, 0.0f, 0.0f)) * weights[weightIndex];
				weightIndex++;
			}
		}
		
		return outColor;	
	}
	
	float3 SampleBloomNoDirt(float3 clr, float2 screenUV)
	{
		//float3 bloomTexture = tex2D(_BloomTexture, screenUV).xyz;
		float3 bloomTexture = GetBloomSamples(screenUV);
		
		clr = lerp(clr, bloomTexture * BloomBrightness, BloomIntensity);	
				
		return clr;
	}
	
	float3 SampleBloomDirt(float3 clr, float2 screenUV)
	{
		float3 bloomTexture = tex2D(_BloomTexture, screenUV).xyz;	
		float3 dirtMask = saturate(tex2D(_LensDirtTexture, screenUV).xyz * DirtIntensity);
		
		clr = lerp(clr, bloomTexture * BloomBrightness, BloomIntensity);					
		clr = lerp(clr, bloomTexture * DirtBrightness, dirtMask);		
		return clr; 
	}
	
	float3 ApplyExposure(float3 clr)
	{			
		#ifdef SC_EXPOSURE_AUTO			
		float4 virtualCameraResult = tex2Dlod(_VirtualCameraResult, float4(0.5f, 0.5f, 0.0f, 0.0f));
		float exposure = virtualCameraResult.x;			
		return clr * exposure;
		#endif
		
		#ifdef SC_EXPOSURE_MANUAL
		return clr * _ManualExposure;	
		#endif
	}
	
	float4 NoTonemappingBloomNoDirt(v2f i) : SV_Target0
	{
		float3 sample = SampleInputTexture(i.uv);
		sample = SampleBloomNoDirt(sample, i.uv);	
		sample = VariousPostProcessing(sample, i.uv);		
		sample = ApplyExposure(sample);	
		sample = ColorGrading(sample);
				
		return float4(sample, 1.0f);
	}
	
	float4 NoTonemappingBloomDirt(v2f i) : SV_Target0
	{
		float3 sample = SampleInputTexture(i.uv);
		sample = SampleBloomDirt(sample, i.uv);	
		sample = VariousPostProcessing(sample, i.uv);	
		sample = ApplyExposure(sample);	
		sample = ColorGrading(sample);
				
		return float4(sample, 1.0f);
	}
	
	float4 TonemappingNoBloomNoDirt(v2f i) : SV_Target0
	{
		float3 sample = SampleInputTexture(i.uv);		
		sample = VariousPostProcessing(sample, i.uv);	
		sample = ApplyExposure(sample);			
		sample = Tonemapping(sample);
		sample = ColorGrading(sample);
		
		return float4(sample, 1.0f);
	}
	
	float4 TonemappingBloomNoDirt(v2f i) : SV_Target0
	{
		float3 sample = SampleInputTexture(i.uv);
		sample = SampleBloomNoDirt(sample, i.uv);	
		sample = VariousPostProcessing(sample, i.uv);	
		sample = ApplyExposure(sample);	
		sample = Tonemapping(sample);
		sample = ColorGrading(sample);
		
		return float4(sample, 1.0f);
	}
	
	float4 TonemappingBloomDirt(v2f i) : SV_Target0
	{
		float3 sample = SampleInputTexture(i.uv);
		sample = SampleBloomDirt(sample, i.uv);	
		sample = VariousPostProcessing(sample, i.uv);	
		sample = ApplyExposure(sample);		
		sample = Tonemapping(sample);	
		sample = ColorGrading(sample);	
		
		return float4(sample, 1.0f);
	}
	
	float4 NoTonemappingNoBloomNoDirt(v2f i) : SV_Target0
	{
		float3 sample = SampleInputTexture(i.uv);
		sample = VariousPostProcessing(sample, i.uv);	
		sample = ApplyExposure(sample);	
		sample = ColorGrading(sample);
		
		return float4(sample, 1.0f);
	}
	
	ENDCG
	
	Subshader 
	{
    ZTest Off
    Cull Off
    ZWrite Off
    Blend Off
    Fog { Mode off }
		Pass 
		{
			Name "TonemappingBloomDirt" //Pass 0

			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest 
			#pragma vertex vert
			#pragma fragment TonemappingBloomDirt
			#pragma target 3.0
			#pragma multi_compile SC_TONEMAPPING_REINHARD SC_TONEMAPPING_LUMAREINHARD SC_TONEMAPPING_FILMIC SC_TONEMAPPING_PHOTOGRAPHIC
			#pragma multi_compile SC_EXPOSURE_MANUAL SC_EXPOSURE_AUTO
			#pragma multi_compile SC_CHROMATIC_ABERRATION_ON SC_CHROMATIC_ABERRATION_OFF
			#pragma multi_compile SC_COLOR_CORRECTION_OFF SC_COLOR_CORRECTION_1_TEX SC_COLOR_CORRECTION_2_TEX
			
			ENDCG
		}
		Pass 
		{
			Name "TonemappingBloomNoDirt" //Pass 1

			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest 
			#pragma vertex vert
			#pragma fragment TonemappingBloomNoDirt
			#pragma target 3.0
			#pragma multi_compile SC_TONEMAPPING_REINHARD SC_TONEMAPPING_LUMAREINHARD SC_TONEMAPPING_FILMIC SC_TONEMAPPING_PHOTOGRAPHIC
			#pragma multi_compile SC_EXPOSURE_MANUAL SC_EXPOSURE_AUTO
			#pragma multi_compile SC_CHROMATIC_ABERRATION_ON SC_CHROMATIC_ABERRATION_OFF
			#pragma multi_compile SC_COLOR_CORRECTION_OFF SC_COLOR_CORRECTION_1_TEX SC_COLOR_CORRECTION_2_TEX
			
			ENDCG
		}		
		Pass 
		{
			Name "TonemappingNoBloomNoDirt" //Pass 2

			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest 
			#pragma vertex vert
			#pragma fragment TonemappingNoBloomNoDirt
			#pragma target 3.0
			#pragma multi_compile SC_TONEMAPPING_REINHARD SC_TONEMAPPING_LUMAREINHARD SC_TONEMAPPING_FILMIC SC_TONEMAPPING_PHOTOGRAPHIC
			#pragma multi_compile SC_EXPOSURE_MANUAL SC_EXPOSURE_AUTO
			#pragma multi_compile SC_CHROMATIC_ABERRATION_ON SC_CHROMATIC_ABERRATION_OFF
			#pragma multi_compile SC_COLOR_CORRECTION_OFF SC_COLOR_CORRECTION_1_TEX SC_COLOR_CORRECTION_2_TEX
			
			ENDCG
		}
		Pass 
		{
			Name "NoTonemappingBloomDirt" //Pass 3

			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest 
			#pragma vertex vert
			#pragma fragment NoTonemappingBloomDirt
			#pragma target 3.0
			#pragma multi_compile SC_TONEMAPPING_REINHARD SC_TONEMAPPING_LUMAREINHARD SC_TONEMAPPING_FILMIC SC_TONEMAPPING_PHOTOGRAPHIC
			#pragma multi_compile SC_EXPOSURE_MANUAL SC_EXPOSURE_AUTO
			#pragma multi_compile SC_CHROMATIC_ABERRATION_ON SC_CHROMATIC_ABERRATION_OFF
			#pragma multi_compile SC_COLOR_CORRECTION_OFF SC_COLOR_CORRECTION_1_TEX SC_COLOR_CORRECTION_2_TEX
			
			ENDCG
		}
		Pass 
		{
			Name "NoTonemappingBloomNoDirt" //Pass 4

			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest 
			#pragma vertex vert
			#pragma fragment NoTonemappingBloomNoDirt
			#pragma target 3.0
			#pragma multi_compile SC_TONEMAPPING_REINHARD SC_TONEMAPPING_LUMAREINHARD SC_TONEMAPPING_FILMIC SC_TONEMAPPING_PHOTOGRAPHIC
			#pragma multi_compile SC_EXPOSURE_MANUAL SC_EXPOSURE_AUTO
			#pragma multi_compile SC_CHROMATIC_ABERRATION_ON SC_CHROMATIC_ABERRATION_OFF
			#pragma multi_compile SC_COLOR_CORRECTION_OFF SC_COLOR_CORRECTION_1_TEX SC_COLOR_CORRECTION_2_TEX
			
			ENDCG
		}
		Pass 
		{
			Name "NoTonemappingNoBloomNoDirt" //Pass 5

			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest 
			#pragma vertex vert
			#pragma fragment NoTonemappingNoBloomNoDirt
			#pragma target 3.0
			#pragma multi_compile SC_TONEMAPPING_REINHARD SC_TONEMAPPING_LUMAREINHARD SC_TONEMAPPING_FILMIC SC_TONEMAPPING_PHOTOGRAPHIC
			#pragma multi_compile SC_EXPOSURE_MANUAL SC_EXPOSURE_AUTO
			#pragma multi_compile SC_CHROMATIC_ABERRATION_ON SC_CHROMATIC_ABERRATION_OFF
			#pragma multi_compile SC_COLOR_CORRECTION_OFF SC_COLOR_CORRECTION_1_TEX SC_COLOR_CORRECTION_2_TEX
			
			ENDCG
		}
	}	
	Fallback Off	
}
