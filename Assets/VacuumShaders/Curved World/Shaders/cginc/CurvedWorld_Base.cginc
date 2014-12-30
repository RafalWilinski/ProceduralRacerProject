#ifndef VACUUM_CURVEDWORLD_BASE_CGINC
#define VACUUM_CURVEDWORLD_BASE_CGINC

////////////////////////////////////////////////////////////////////////////
//																		  //
//Variables 															  //
//																		  //
////////////////////////////////////////////////////////////////////////////

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


////////////////////////////////////////////////////////////////////////////
//																		  //
//Defines    															  //
//																		  //
////////////////////////////////////////////////////////////////////////////

#ifdef V_CW_GLOBAL_ON
	#define V_CW_X_BEND_SIZE        _V_CW_X_Bend_Size_GLOBAL
	#define V_CW_Y_BEND_SIZE        _V_CW_Y_Bend_Size_GLOBAL
	#define V_CW_Z_BEND_SIZE        _V_CW_Z_Bend_Size_GLOBAL
	#define V_CW_Z_BEND_BIAS        _V_CW_Z_Bend_Bias_GLOBAL
	#define V_CW_CAMERA_BEND_OFFSET _V_CW_Camera_Bend_Offset_GLOBAL
	
	#ifdef V_CW_GLOBAL_FOG_ON
		#define V_CW_FOG_COLOR _V_CW_Fog_Color_GLOBAL
		#define V_CW_FOG_DENSITY _V_CW_Fog_Density_GLOBAL
		#define V_CW_FOG_START _V_CW_Fog_Start_GLOBAL
		#define V_CW_FOG_END _V_CW_Fog_End_GLOBAL

		#define V_CW_FOG saturate((_V_CW_Fog_End_GLOBAL - length(mv.xyz) * _V_CW_Fog_Density_GLOBAL) / (_V_CW_Fog_End_GLOBAL - _V_CW_Fog_Start_GLOBAL));
	#endif

	#ifdef V_CW_GLOBAL_IBL_ON
		#define V_CW_IBL_INTENSITY _V_CW_IBL_Intensity_GLOBAL
		#define V_CW_IBL_CONTRAST _V_CW_IBL_Contrast_GLOBAL
		#define V_CW_IBL_CUBE _V_CW_IBL_Cube_GLOBAL

		#define V_CW_IBL(n) ((texCUBE(_V_CW_IBL_Cube_GLOBAL, n).rgb - 0.5) * _V_CW_IBL_Contrast_GLOBAL + 0.5) * _V_CW_IBL_Intensity_GLOBAL
	#endif
#else
	#define V_CW_X_BEND_SIZE        _V_CW_X_Bend_Size
	#define V_CW_Y_BEND_SIZE        _V_CW_Y_Bend_Size
	#define V_CW_Z_BEND_SIZE        _V_CW_Z_Bend_Size
	#define V_CW_Z_BEND_BIAS        _V_CW_Z_Bend_Bias
	#define V_CW_CAMERA_BEND_OFFSET _V_CW_Camera_Bend_Offset
	
	#ifdef V_CW_FOG_ON
		#define V_CW_FOG_COLOR _V_CW_Fog_Color
		#define V_CW_FOG_DENSITY _V_CW_Fog_Density
		#define V_CW_FOG_START _V_CW_Fog_Start
		#define V_CW_FOG_END _V_CW_Fog_End

		#define V_CW_FOG saturate((_V_CW_Fog_End - length(mv.xyz) * _V_CW_Fog_Density) / (_V_CW_Fog_End - _V_CW_Fog_Start));
	#endif

	#ifdef V_CW_IBL_ON
		#define V_CW_IBL_INTENSITY _V_CW_IBL_Intensity
		#define V_CW_IBL_CONTRAST _V_CW_IBL_Contrast
		#define V_CW_IBL_CUBE _V_CW_IBL_Cube

		#define V_CW_IBL(n) ((texCUBE(_V_CW_IBL_Cube, n).rgb - 0.5) * _V_CW_IBL_Contrast + 0.5) * _V_CW_IBL_Intensity
	#endif
#endif

#define V_CW_BEND(v)  half4 mv = mul(UNITY_MATRIX_MV, v); \
				      half zOff = min(0, mv.z + V_CW_CAMERA_BEND_OFFSET); \
				      zOff = zOff * zOff * 0.001; \
				      half xOff = max(0, abs(mv.x) - V_CW_Z_BEND_BIAS) * sign(mv.x); \
				      half4 pos = mv + half4(V_CW_Y_BEND_SIZE * zOff, V_CW_X_BEND_SIZE * zOff + (xOff * xOff * V_CW_Z_BEND_SIZE) * 0.001, 0, 0);	o.pos = mul(UNITY_MATRIX_P, pos);

#endif 
