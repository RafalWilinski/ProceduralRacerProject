/*=============================================================================
CHANGELOG 
			- april 2014
				* Fixed UV coordinates
				* Fixed offset. It was wrong. Its now proportional to screen size
				* Merged downsample with Kawase filter
				* Added bloom tint option

=============================================================================*/

Shader "Hidden/LensDirtiness" {
	
	Properties {
	_MainTex ("Base (RGB)", 2D) = "black" {}
}

	CGINCLUDE
	
	#include "UnityCG.cginc"
	
	//Receive parameters
	uniform sampler2D		_MainTex, 
							_Bloom, 
							_DirtinessTexture;
	
	float					_Gain,
							_Threshold,  
							_Dirtiness;
	
	float4					_BloomColor, 
							_Offset,
							_MainTex_TexelSize;	
	
	struct v2f {
		float4 pos : POSITION;
		float2 uv[2] : TEXCOORD0;
		
	};
	
	//Common Vertex Shader
	v2f vert( appdata_img v )
	{
	v2f o;
	o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
	
	o.uv[0] = v.texcoord.xy;
	 
	
		#if UNITY_UV_STARTS_AT_TOP
		if(_MainTex_TexelSize.y<0.0)
			o.uv[0].y = 1.0-o.uv[0].y;
		#endif
		
		o.uv[1] =  v.texcoord.xy;	

	
	return o;
	
	} 

	half4 Threshold(v2f IN) : COLOR
	{
		float2 coords = IN.uv[0];	
		float4 Scene = tex2D(_MainTex ,coords ) * _Gain;
		float thresh =  Scene - _Threshold;
		return max((float4)0.0f,(lerp((float4)0.0f ,Scene , thresh )));	
	  
	}		
	
	//http://developer.amd.com/wordpress/media/2012/10/Oat-ScenePostprocessing.pdf
	half4 Kawase(v2f IN) : COLOR
	{
		float2 texCoord = IN.uv[0];	
		float2 texCoordSample = 0;

		float2 dUV=_Offset.xy;
		
		float4 cOut;
		
		// Sample top left pixel
		texCoordSample.x = texCoord.x - dUV.x;
		texCoordSample.y = texCoord.y + dUV.y;
		cOut = tex2D (_MainTex, texCoordSample);
		
		// Sample top right pixel
		texCoordSample.x = texCoord.x + dUV.x;
		texCoordSample.y = texCoord.y + dUV.y;
		cOut += tex2D (_MainTex, texCoordSample);
		
		// Sample bottom right pixel
		texCoordSample.x = texCoord.x + dUV.x;
		texCoordSample.y = texCoord.y - dUV.y;
		cOut += tex2D (_MainTex, texCoordSample);
		
		// Sample bottom left pixel
		texCoordSample.x = texCoord.x - dUV.x;
		texCoordSample.y = texCoord.y - dUV.y;
		cOut += tex2D (_MainTex, texCoordSample);
		// Average
		cOut *= 0.25f;
		return cOut;
	
	}
	
	half4 Compose(v2f IN) : COLOR
	{		
		half2 DirtnessUV = IN.uv[0];
		half2 ScreenUV = IN.uv[1];
		float4 Final;
		
		#ifdef _SCENE_TINTS_BLOOM
		float4 BloomSample = tex2D(_Bloom, ScreenUV);
		#else
		float4 BloomSample = tex2D(_Bloom, ScreenUV);
		BloomSample.rgb = BloomSample.r + BloomSample.g + BloomSample.b;
		BloomSample.rgb /=3;
		#endif

		half4 DirtinessSample = tex2D(_DirtinessTexture, DirtnessUV);

		float4 Scene = tex2D(_MainTex, ScreenUV);
		Final = Scene + BloomSample * DirtinessSample * _Dirtiness + BloomSample * _BloomColor;;
	  
		return Final;
	}

	ENDCG 
	
Subshader {

	ZTest Off
	Cull Off
	ZWrite Off
	Fog { Mode off }
	
//Pass 0 Threshold
 Pass 
 {
 Name "Threshold"

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest 
      #pragma vertex vert
      #pragma fragment Threshold
      ENDCG
  }
  
 
  
   //Pass 1 _Bloom
 Pass 
 {
 Name "Kawase"

      CGPROGRAM
	  #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment Kawase
      ENDCG
  }
  
  //Pass 2 Done, compose everything
 Pass 
 {
 Name "Compose"

      CGPROGRAM
	  #pragma fragmentoption ARB_precision_hint_fastest
	  #pragma multi_compile _ _SCENE_TINTS_BLOOM 
      #pragma vertex vert
      #pragma fragment Compose
      ENDCG
  }
}

Fallback off
	
}