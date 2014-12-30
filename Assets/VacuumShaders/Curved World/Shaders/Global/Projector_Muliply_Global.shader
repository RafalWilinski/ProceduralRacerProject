Shader "Hidden/VacuumShaders/Curved World/Projector/Multiply" 
{ 
	Properties 
	{
	//Tag
		[Tag]
		V_CW_TAG("", float) = 0
		  
		//Default Options
		[DefaultOptions]
		V_CW_D_OPTIONS("", float) = 0

		_ShadowTex ("Cookie", 2D) = "gray" {}
		_FalloffTex ("FallOff", 2D) = "white" {}


		//CurvedWorld Options
		[CurvedWorldOptions]
		V_CW_W_OPTIONS("", float) = 0
		[HideInInspector]
		_V_CW_Z_Bend_Size("_V_CW_Z_Bend_Size", float) = 0
		[HideInInspector]
		_V_CW_Z_Bend_Bias("_V_CW_Z_Bend_Bias", float) = 0
		[HideInInspector]
		_V_CW_Y_Bend_Size("_V_CW_Y_Bend_Size", float) = 0
		[HideInInspector]
		_V_CW_X_Bend_Size("_V_CW_X_Bend_Size", float) = 0
		[HideInInspector]
		_V_CW_Camera_Bend_Offset("_V_CW_Camera_Bend_Offset", float) = 0

		[HideInInspector]
		_V_CW_Fog_Color("", color) = (1, 1, 1, 1)
		[HideInInspector]
		_V_CW_Fog_Density("", Range(0.0, 1.0)) = 1
		[HideInInspector]
		_V_CW_Fog_Start("", float) = 0
		[HideInInspector]
		_V_CW_Fog_End("", float) = 100
	}

	Subshader 
	{
		Tags { "Queue"="Transparent"
			   "CurvedWorldTag"="Global/Projector/Multiply" 
			   "CurvedWorldBakedKeywords"="" 
			 }

		Pass 
		{
			ZWrite Off
			Fog{Mode Off}
			AlphaTest Greater 0
			ColorMask RGB
			Blend DstColor Zero
			Offset -1, -1
 
			CGPROGRAM
			#pragma vertex vert_proj
			#pragma fragment frag_proj

			#pragma multi_compile V_CW_GLOBAL_FOG_OFF V_CW_GLOBAL_FOG_ON

			#define V_CW_GLOBAL_ON
			
			#include "../cginc/CurvedWorld.cginc" 
			
			struct v2f 
			{
				float4 uvShadow : TEXCOORD0;
				float4 uvFalloff : TEXCOORD1;
				float4 pos : SV_POSITION;

				half fog : TEXCOORD2;
			};
			
			float4x4 _Projector;
			float4x4 _ProjectorClip;
			sampler2D _ShadowTex;
			sampler2D _FalloffTex;

			
			v2f vert_proj (float4 vertex : POSITION)
			{
				v2f o;
				
				V_CW_BEND(vertex)

				o.uvShadow = mul (_Projector, vertex);
				o.uvFalloff = mul (_ProjectorClip, vertex);

				#if defined(V_CW_FOG_ON) || defined(V_CW_GLOBAL_FOG_ON)
					o.fog = saturate((V_CW_FOG_END.x - length(mv.xyz) * V_CW_FOG_DENSITY) / (V_CW_FOG_END.x - V_CW_FOG_START.x));
				#endif

				return o;
			}
					
			
			fixed4 frag_proj (v2f i) : SV_Target
			{
				fixed4 texS = tex2Dproj (_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
				texS.a = 1.0-texS.a;
 
				fixed4 texF = tex2Dproj (_FalloffTex, UNITY_PROJ_COORD(i.uvFalloff));
				

				#if defined(V_CW_FOG_ON) || defined(V_CW_GLOBAL_FOG_ON)
					texF.a *= i.fog;
				#endif

				fixed4 res = lerp(fixed4(1,1,1,0), texS, texF.a);

				return res;
			}
			ENDCG
		}
	}

	CustomEditor "CurvedWorldMaterial_Editor"
}
