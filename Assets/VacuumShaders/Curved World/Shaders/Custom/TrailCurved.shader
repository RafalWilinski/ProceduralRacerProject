// VacuumShaders 2014
// https://www.facebook.com/VacuumShaders

Shader "VacuumShaders/Curved World/Custom/TrailCurved"
{ 
	Properties 
	{			
		[CustomHeader] _V_CW_Header("Custom/Unlit", float) = 0		  		
		[CustomLabel] _V_CW_DO("Default Options", float) = 0

				
		//Add your properties here
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
		       "CurvedWorldTag"="CurvedWorld_Custom" 
			  }
		LOD 100
		 
		Fog{Mode Off} 
		
		Pass 
	    {
			CGPROGRAM
			#pragma vertex vert 
	    	#pragma fragment frag
			
						
			#pragma multi_compile V_CW_FOG_OFF V_CW_FOG_ON
			#pragma multi_compile V_CW_IBL_OFF V_CW_IBL_ON			

			#include "../cginc/CurvedWorld_Base.cginc" 



			fixed4 _Color;
			sampler2D _MainTex;
			half4 _MainTex_ST;


			struct vInput
			{
				float4 vertex : POSITION;    
				float4 texcoord : TEXCOORD0;

				#ifdef V_CW_IBL_ON
					float3 normal : NORMAL;
				#endif
			};

			struct vOutput
			{
				float4 pos : SV_POSITION;
				half4 texcoord : TEXCOORD0;

				#ifdef V_CW_FOG_ON
					half fog : TEXCOORD01;
				#endif

				#ifdef V_CW_IBL_ON
					half3 normal : TEXCOORD02;
				#endif
			};

			vOutput vert(vInput v)
			{ 
				vOutput o;
		
				//Bending vertex
				V_CW_BEND(v.vertex)			
	  
				//MainTex uv
				o.texcoord.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				
				//IBL requires world space normal
				#ifdef V_CW_IBL_ON
					o.normal = normalize(mul((half3x3)_Object2World, v.normal * unity_Scale.w));
				#endif

				//Generating fog
				#ifdef V_CW_FOG_ON
					o.fog = V_CW_FOG
				#endif


				return o;
			}

			fixed4 frag(vOutput i) : SV_Target 
			{	
				fixed4 retColor = tex2D(_MainTex, i.texcoord.xy) * _Color;

				 
				//IBL
				#ifdef V_CW_IBL_ON
					fixed3 ibl = V_CW_IBL(i.normal);
					retColor.rgb = (UNITY_LIGHTMODEL_AMBIENT.xyz * 2 + ibl) * retColor.rgb;	
				#endif

				//Fog
				#ifdef V_CW_FOG_ON
					retColor.rgb = lerp(V_CW_FOG_COLOR.rgb, retColor.rgb, i.fog);
				#endif

				return retColor;
			}

			ENDCG

		}	//Pass

	}	//SubShader
	 
	CustomEditor "CurvedWorldCustomMaterial_Editor"

}	//Shader
