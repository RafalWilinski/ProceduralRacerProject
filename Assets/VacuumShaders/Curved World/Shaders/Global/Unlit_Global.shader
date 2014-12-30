// VacuumShaders 2014
// https://www.facebook.com/VacuumShaders

Shader "Hidden/VacuumShaders/Curved World/Unlit/Texture"
{
	Properties 
	{
		//Tag
		[Tag]
		V_CW_TAG("", float) = 0
		 
		//Default Options
		[DefaultOptions]
		V_CW_D_OPTIONS("", float) = 0

		_Color("Main Color", color) = (1, 1, 1, 1)
		_MainTex ("Base (RGB)", 2D) = "white" {}

		[HideInInspector]
		_ReflectColor("Reflection Color", color) = (1, 1, 1, 1)
		[HideInInspector]
		_Cube("Reflection Cube", Cube) = "_Skybox"{}

		//CurvedWorld Options
		[CurvedWorldOptions]
		V_CW_W_OPTIONS("", float) = 0


		[HideInInspector]
		_V_CW_Z_Bend_Size("", float) = 0
		[HideInInspector]
		_V_CW_Z_Bend_Bias("", float) = 0
		[HideInInspector]
		_V_CW_Y_Bend_Size("", float) = 0
		[HideInInspector]
		_V_CW_X_Bend_Size("", float) = 0
		[HideInInspector] 
		_V_CW_Camera_Bend_Offset("", float) = 0
		 

		[HideInInspector]
		_V_CW_Rim_Color("", color) = (1, 1, 1, 1)
		[HideInInspector]
		_V_CW_Rim_Bias("", Range(-1, 1)) = 0.2

		[HideInInspector]
		_V_CW_Fog_Color("", color) = (1, 1, 1, 1)
		[HideInInspector]
		_V_CW_Fog_Density("", Range(0.0, 1.0)) = 1
		[HideInInspector]
		_V_CW_Fog_Start("", float) = 0
		[HideInInspector]
		_V_CW_Fog_End("", float) = 100	
		
		[HideInInspector]
		_V_CW_IBL_Intensity("", float) = 1
		[HideInInspector] 
		_V_CW_IBL_Contrast("", float) = 1 
		[HideInInspector]   
		_V_CW_IBL_Cube("", cube ) = ""{}
		
		[HideInInspector]
		_V_CW_Emission_Color("", color) = (1, 1, 1, 1)
		[HideInInspector]
		_V_CW_Emission_Strength("", float) = 1		
	}

	SubShader 
	{
		Tags { "RenderType"="Opaque" 
		       "CurvedWorldTag"="Global/Unlit/Texture" 
			   "CurvedWorldBakedKeywords"="" 
			 }
		LOD 100

		Fog{Mode Off}
		
		Pass 
	    {
			CGPROGRAM
			#pragma vertex vert
	    	#pragma fragment frag
			

			#pragma multi_compile V_CW_SELF_ILLUMINATED_OFF V_CW_SELF_ILLUMINATED_ON
			#pragma multi_compile V_CW_RIM_OFF V_CW_RIM_ON
			#pragma multi_compile V_CW_GLOBAL_FOG_OFF V_CW_GLOBAL_FOG_ON
			#pragma multi_compile V_CW_VERTEX_COLOR_OFF V_CW_VERTEX_COLOR_ON
			#pragma multi_compile V_CW_GLOBAL_IBL_OFF V_CW_GLOBAL_IBL_ON

			#define V_CW_GLOBAL_ON
			

			#include "../cginc/CurvedWorld.cginc" 

			ENDCG

		}	//Pass

	}	//SubShader
	 
	CustomEditor "CurvedWorldMaterial_Editor"

}	//Shader
