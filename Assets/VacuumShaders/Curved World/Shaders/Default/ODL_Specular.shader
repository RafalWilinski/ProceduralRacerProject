// VacuumShaders 2014
// https://www.facebook.com/VacuumShaders

Shader "VacuumShaders/Curved World/One Directional Light/Specular"
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
		_MainTex ("Base (RGB)  Gloss (A)", 2D) = "white" {}
		_V_CW_Specular_Lookup("Specular Lookup", 2D) = "black"{}
		 
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
		       "CurvedWorldTag"="One Directional Light/Specular" 
			   "CurvedWorldBakedKeywords"="" 
			 }
		LOD 200

		Fog{Mode Off}
		
		Pass
	    {
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase" } 

			CGPROGRAM
			#pragma vertex vert
	    	#pragma fragment frag
	    	#pragma fragmentoption ARB_precision_hint_fastest

			#define UNITY_PASS_FORWARDBASE  
            #include "UnityCG.cginc"
            #include "AutoLight.cginc" 
            #pragma multi_compile_fwdbase_fullshadows

			#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF

			#pragma multi_compile V_CW_UNITY_VERTEXLIGHT_OFF V_CW_UNITY_VERTEXLIGHT_ON
			#pragma multi_compile V_CW_LIGHT_PER_VERTEX V_CW_LIGHT_PER_PIXEL
			#pragma multi_compile V_CW_SELF_ILLUMINATED_OFF V_CW_SELF_ILLUMINATED_ON
			#pragma multi_compile V_CW_RIM_OFF V_CW_RIM_ON
			#pragma multi_compile V_CW_FOG_OFF V_CW_FOG_ON
			#pragma multi_compile V_CW_VERTEX_COLOR_OFF V_CW_VERTEX_COLOR_ON
			#pragma multi_compile V_CW_IBL_OFF V_CW_IBL_ON

			#define V_CW_SPECULAR

			#include "../cginc/CurvedWorld.cginc" 

			ENDCG

		}	//Pass

		Pass  
		{
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }
			Fog {Mode Off}
			ZWrite On ZTest LEqual Cull Off
			Offset 1, 1
	 
			CGPROGRAM
			#pragma vertex vert_ShadowCaster
			#pragma fragment frag_ShadowCaster   
			 
			#pragma multi_compile_shadowcaster 
			#define UNITY_PASS_SHADOWCASTER
			#include "UnityCG.cginc"  
						  
					 
		    #include "../cginc/CurvedWorld.cginc" 
 
			        
			ENDCG 
		}	//ShadowCaster   


		
		Pass 
		{  
			Name "ShadowCollector"
			Tags { "LightMode" = "ShadowCollector" }
			Fog {Mode Off}
			ZWrite On ZTest LEqual

			    
			CGPROGRAM   
			#pragma vertex vert_ShadowCollector
			#pragma fragment frag_ShadowCollector
			 
			#pragma multi_compile_shadowcollector
			#include "HLSLSupport.cginc"
			#include "UnityShaderVariables.cginc"
			#define UNITY_PASS_SHADOWCOLLECTOR
			#define SHADOW_COLLECTOR_PASS
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

					 
 		    #include "../cginc/CurvedWorld.cginc" 
			 
		 
 			ENDCG
		}
	}	//SubShader



	//Fallback - no shadows
	SubShader 
	{
		Tags { "RenderType"="Opaque" 
		       "CurvedWorldTag"="One Directional Light/Specular" 
			   "CurvedWorldBakedKeywords"="" 
			 }
		LOD 200
		
		Fog{Mode Off}

		Pass
	    {
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase" } 

			CGPROGRAM   
			#pragma vertex vert
	    	#pragma fragment frag
	    	#pragma fragmentoption ARB_precision_hint_fastest

			#define UNITY_PASS_FORWARDBASE  
            #include "UnityCG.cginc"

			#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF

			#pragma multi_compile V_CW_UNITY_VERTEXLIGHT_OFF V_CW_UNITY_VERTEXLIGHT_ON
			#pragma multi_compile V_CW_LIGHT_PER_VERTEX V_CW_LIGHT_PER_PIXEL
			#pragma multi_compile V_CW_SELF_ILLUMINATED_OFF V_CW_SELF_ILLUMINATED_ON
			#pragma multi_compile V_CW_RIM_OFF V_CW_RIM_ON
			#pragma multi_compile V_CW_FOG_OFF V_CW_FOG_ON
			#pragma multi_compile V_CW_VERTEX_COLOR_OFF V_CW_VERTEX_COLOR_ON
			#pragma multi_compile V_CW_IBL_OFF V_CW_IBL_ON


			#define V_CW_SPECULAR 
			#define V_NO_SHADOWS

			#include "../cginc/CurvedWorld.cginc" 

			ENDCG

		}	//Pass

	}	//SubShader
	 

	//Fallback - Unlit
	SubShader 
	{
		Tags { "RenderType"="Opaque" 
		       "CurvedWorldTag"="Unlit/Texture" 
			   "CurvedWorldBakedKeywords"="" 
			 }
		LOD 100

		Fog{Mode Off}
		
		Pass 
	    {
			CGPROGRAM
			#pragma vertex vert
	    	#pragma fragment frag
			
			#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF

			#pragma multi_compile V_CW_SELF_ILLUMINATED_OFF V_CW_SELF_ILLUMINATED_ON
			#pragma multi_compile V_CW_RIM_OFF V_CW_RIM_ON
			#pragma multi_compile V_CW_FOG_OFF V_CW_FOG_ON
			#pragma multi_compile V_CW_VERTEX_COLOR_OFF V_CW_VERTEX_COLOR_ON
			#pragma multi_compile V_CW_IBL_OFF V_CW_IBL_ON
			

			#include "../cginc/CurvedWorld.cginc" 

			ENDCG

		}	//Pass

	}	//SubShader

	CustomEditor "CurvedWorldMaterial_Editor"

}	//Shader
