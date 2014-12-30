// VacuumShaders 2014
// https://www.facebook.com/VacuumShaders

Shader "Hidden/VacuumShaders/Curved World/Particles/Alpha Blended"
{
	Properties 
	{
		//Tag
		[Tag]
		V_CW_TAG("", float) = 0

		//Default Options
		[DefaultOptions]
		V_CW_D_OPTIONS("", float) = 0

		_Color("Tint Color", color) = (1, 1, 1, 1)
		_MainTex ("Particle Texture", 2D) = "white" {}

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
		_V_CW_Fog_Color("", color) = (1, 1, 1, 1)
		[HideInInspector]
		_V_CW_Fog_Density("", Range(0.0, 1.0)) = 1
		[HideInInspector]
		_V_CW_Fog_Start("", float) = 0
		[HideInInspector]
		_V_CW_Fog_End("", float) = 100

		[HideInInspector]
		_V_CW_Emission_Color("", color) = (1, 1, 1, 1)
		[HideInInspector]
		_V_CW_Emission_Strength("", float) = 1
	}

	SubShader 
	{
		Tags { "Queue"="Transparent" 
		       "IgnoreProjector"="True" 
			   "RenderType"="Transparent" 
			   "CurvedWorldTag"="Global/Particles/Alpha Blended" 
			   "CurvedWorldBakedKeywords"="" 
			 }
		LOD 100

		Blend SrcAlpha OneMinusSrcAlpha
	    AlphaTest Greater .01
	    ColorMask RGB
	    Cull Off Lighting Off ZWrite Off Fog { Mode Off }

		Pass
	    {
			CGPROGRAM
			#pragma vertex vert_particles
	    	#pragma fragment frag_particle
	    	#pragma multi_compile_particles
			
			#pragma multi_compile V_CW_GLOBAL_FOG_OFF V_CW_GLOBAL_FOG_ON

			#define V_CW_GLOBAL_VERTEX_COLOR_ON
			#define V_CW_GLOBAL_ON

			#include "../cginc/CurvedWorld.cginc" 

			fixed4 frag_particle(vOutput i) : SV_Target 
			{
				fixed4 retColor = 2.0f * i.color * _Color * tex2D(_MainTex, i.texcoord.xy);
			   
			    #if defined(V_CW_FOG_ON) || defined(V_CW_GLOBAL_FOG_ON)
					retColor.rgb = lerp(V_CW_FOG_COLOR.rgb, retColor.rgb, i.vfx.y);
					//retColor.a *= i.vfx.y;
				#endif 

				return retColor;
			}

			ENDCG

		}	//Pass
	}	//SubShader
	 
	CustomEditor "CurvedWorldMaterial_Editor"

}	//Shader
