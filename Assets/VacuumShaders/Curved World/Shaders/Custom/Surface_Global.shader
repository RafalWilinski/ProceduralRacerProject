#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'

// VacuumShaders 2014
// https://www.facebook.com/VacuumShaders

Shader "Hidden/VacuumShaders/Curved World/Custom/Surface"
{ 
	Properties 
	{			
		[CustomHeader] _V_CW_Header("Custom/One Directional Light", float) = 0		  		
		[CustomLabel] _V_CW_DO("Default Options", float) = 0

				
		//Add your properties below
		//...
		_Color("Main Color", color) = (1, 1, 1, 1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		 


		//Curved World properties
		//Do not modify

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
		_V_CW_IBL_Intensity("", float) = 1
		[HideInInspector] 
		_V_CW_IBL_Contrast("", float) = 1 
		[HideInInspector]   
		_V_CW_IBL_Cube("", cube ) = ""{}  		
		
	}

	SubShader 
	{
		Tags { "RenderType"="Opaque" 
		       "CurvedWorldTag"="Global/CurvedWorld_Custom" 
			 }
		LOD 100
		 
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
			#pragma multi_compile_fwdbase


			#pragma multi_compile V_CW_GLOBAL_FOG_OFF V_CW_GLOBAL_FOG_ON
			#pragma multi_compile V_CW_GLOBAL_IBL_OFF V_CW_GLOBAL_IBL_ON		
			#define V_CW_GLOBAL_ON

			#include "../cginc/CurvedWorld_Base.cginc" 



			fixed4 _Color;
			sampler2D _MainTex;
			half4 _MainTex_ST;
			fixed4 _LightColor0;


			struct vInput
			{
				float4 vertex : POSITION;    
				float4 texcoord : TEXCOORD0;

				float3 normal : NORMAL;
			};

			struct vOutput
			{
				float4 pos : SV_POSITION;
				half4 texcoord : TEXCOORD0;

				#ifdef V_CW_GLOBAL_FOG_ON
					half fog : TEXCOORD01;
				#endif

				#ifdef V_CW_GLOBAL_IBL_ON
					half3 normal : TEXCOORD02;
				#endif

				fixed vLightDiff : TEXCOORD03;

				LIGHTING_COORDS(4, 5)
			};

			vOutput vert(vInput v)
			{ 
				vOutput o = (vOutput)0;
		
		    	//Bending vertex
				V_CW_BEND(v.vertex)			
	  
				//MainTex uv
				o.texcoord.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;

				//Calculating world normal
				half3 worldNormal = normalize(mul((half3x3)_Object2World, v.normal * 1.0));
				
				//Calculationg light
				o.vLightDiff = max(0, dot (worldNormal, _WorldSpaceLightPos0.xyz));

				//IBL requires world space normal
				#ifdef V_CW_GLOBAL_IBL_ON
					o.normal = worldNormal;
				#endif

				//Generating fog
				#ifdef V_CW_GLOBAL_FOG_ON
					o.fog = V_CW_FOG
				#endif


				TRANSFER_VERTEX_TO_FRAGMENT(o);

				return o;
			}

			fixed4 frag(vOutput i) : SV_Target 
			{	
				fixed4 retColor = tex2D(_MainTex, i.texcoord.xy) * _Color;

				//Calculate light diffuse
				fixed3 diff = (_LightColor0.rgb * i.vLightDiff * LIGHT_ATTENUATION(i) + UNITY_LIGHTMODEL_AMBIENT.xyz) * 2;


				//IBL
				#ifdef V_CW_GLOBAL_IBL_ON
					fixed3 ibl = V_CW_IBL(i.normal);
					retColor.rgb = (diff + ibl) * retColor.rgb;	
				#else
					retColor.rgb *= diff;
				#endif

				//Fog
				#ifdef V_CW_GLOBAL_FOG_ON
					retColor.rgb = lerp(V_CW_FOG_COLOR.rgb, retColor.rgb, i.fog);
				#endif

				return retColor;
			}

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
			#pragma vertex vert
			#pragma fragment frag   
			 
			#pragma multi_compile_shadowcaster 
			#define UNITY_PASS_SHADOWCASTER
			#include "UnityCG.cginc"  
						  		
			#define V_CW_GLOBAL_ON			 
		    #include "../cginc/CurvedWorld_Base.cginc" 
 

			
			struct vOutput
			{
				V2F_SHADOW_CASTER;
			};

			vOutput vert(appdata_full v)
			{ 
				vOutput o;	

				//Bending vertex
				V_CW_BEND(v.vertex)


				TRANSFER_SHADOW_CASTER(o)
	 
				return o;
			} 

			half4 frag(vOutput i) : SV_Target 
			{	
				SHADOW_CASTER_FRAGMENT(i)
			}
			        
			ENDCG 
		}	//ShadowCaster   


		Pass 
		{  
			Name "ShadowCollector"
			Tags { "LightMode" = "ShadowCollector" }
			Fog {Mode Off}
			ZWrite On ZTest LEqual

			    
			CGPROGRAM   
			#pragma vertex vert
			#pragma fragment frag
			 
			#pragma multi_compile_shadowcollector
			#include "HLSLSupport.cginc"
			#include "UnityShaderVariables.cginc"
			#define UNITY_PASS_SHADOWCOLLECTOR
			#define SHADOW_COLLECTOR_PASS
			#include "UnityCG.cginc"
			#include "Lighting.cginc"			
					 
			#define V_CW_GLOBAL_ON
 		    #include "../cginc/CurvedWorld_Base.cginc" 



			struct vOutput
			{
				V2F_SHADOW_COLLECTOR;
			};

			vOutput vert(appdata_full v)
			{ 
				vOutput o;
	
				//Bending vertex
				V_CW_BEND(v.vertex)

				//Screen space shadows
				float4 wpos = mul(_Object2World, v.vertex); 
				o._WorldPosViewZ.xyz = wpos; 
				o._WorldPosViewZ.w = -mul( UNITY_MATRIX_MV, v.vertex ).z; 
				o._ShadowCoord0 = mul(unity_World2Shadow[0], wpos).xyz; 
				o._ShadowCoord1 = mul(unity_World2Shadow[1], wpos).xyz; 
				o._ShadowCoord2 = mul(unity_World2Shadow[2], wpos).xyz; 
				o._ShadowCoord3 = mul(unity_World2Shadow[3], wpos).xyz;

				return o;
			}

			half4 frag(vOutput i) : SV_Target 
			{
				SHADOW_COLLECTOR_FRAGMENT(i)
			}
			 
		 
 			ENDCG
		} //Shadow Collector
		
	}	//SubShader
	 
	CustomEditor "CurvedWorldCustomMaterial_Editor"

}	//Shader
