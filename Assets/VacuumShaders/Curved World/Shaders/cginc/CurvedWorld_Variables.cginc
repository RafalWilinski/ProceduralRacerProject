#ifndef VACUUM_CURVEDWORLD_VARIABLES_CGINC
#define VACUUM_CURVEDWORLD_VARIABLES_CGINC


fixed4 _Color;
sampler2D _MainTex;
half4 _MainTex_ST;

#ifdef V_CW_DECAL
	sampler2D _DecalTex;
	half4 _DecalTex_ST;
#endif

#ifdef V_CW_DETAIL
	sampler2D _Detail;
	half4 _Detail_ST;
#endif

#ifdef V_CW_BLEND_BY_VERTEX
	fixed _VertexBlend;
	sampler2D _BlendTex;
	half4 _BlendTex_ST;
#endif

#ifdef LIGHTMAP_ON
	half4 unity_LightmapST;
	sampler2D unity_Lightmap;				
#endif


#ifdef V_CW_SPECULAR
	sampler2D _V_CW_Specular_Lookup;
#endif

#ifdef V_CW_REFLECTION
	samplerCUBE _Cube;
	fixed4 _ReflectColor;
#endif

#ifdef V_CW_CUTOUT
	half _Cutoff;
#endif

#if defined(UNITY_PASS_FORWARDBASE) 
	uniform half4 _LightColor0;
#endif

#ifdef V_CW_GLOBAL_ON	
	uniform float _V_CW_X_Bend_Size_GLOBAL;
	uniform float _V_CW_Y_Bend_Size_GLOBAL;
	uniform float _V_CW_Z_Bend_Size_GLOBAL;
	uniform float _V_CW_Z_Bend_Bias_GLOBAL;
	uniform float _V_CW_Camera_Bend_Offset_GLOBAL;
		

	#ifdef V_CW_GLOBAL_IBL_ON
		uniform half _V_CW_IBL_Intensity_GLOBAL;
		uniform half _V_CW_IBL_Contrast_GLOBAL;
		uniform samplerCUBE _V_CW_IBL_Cube_GLOBAL;	
	#endif

	#ifdef V_CW_GLOBAL_FOG_ON
		uniform fixed4 _V_CW_Fog_Color_GLOBAL;
		uniform fixed _V_CW_Fog_Density_GLOBAL;
		uniform half _V_CW_Fog_Start_GLOBAL;
		uniform half _V_CW_Fog_End_GLOBAL;
	#endif
#else
	float _V_CW_X_Bend_Size;
	float _V_CW_Y_Bend_Size;
	float _V_CW_Z_Bend_Size;
	float _V_CW_Z_Bend_Bias;
	float _V_CW_Camera_Bend_Offset;		


	#ifdef V_CW_IBL_ON
		half _V_CW_IBL_Intensity;
		half _V_CW_IBL_Contrast;
		samplerCUBE _V_CW_IBL_Cube;	
	#endif

	#ifdef V_CW_FOG_ON
		fixed4 _V_CW_Fog_Color;
		fixed _V_CW_Fog_Density;
		half _V_CW_Fog_Start;
		half _V_CW_Fog_End;
	#endif
#endif

#ifdef V_CW_RIM_ON
	fixed4 _V_CW_Rim_Color;
	fixed  _V_CW_Rim_Bias;
#endif

#ifdef V_CW_SELF_ILLUMINATED_ON
	half _V_CW_Emission_Strength;
#endif
	
#ifdef V_CW_FRESNEL_ON
	half _V_CW_Fresnel_Bias;
#endif


#endif