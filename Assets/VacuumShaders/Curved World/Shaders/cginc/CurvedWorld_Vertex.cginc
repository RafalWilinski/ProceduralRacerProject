// Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'

#ifndef VACUUM_CURVEDWORLD_VERTEX_CGINC
#define VACUUM_CURVEDWORLD_VERTEX_CGINC



					 

#if !defined(UNITY_PASS_SHADOWCASTER) && !defined(UNITY_PASS_SHADOWCOLLECTOR)
vOutput vert(vInput v)
{ 
	vOutput o = (vOutput)0;
		
	V_CW_BEND(v.vertex)			
	  

	o.texcoord.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
	#ifdef V_CW_DECAL
		o.texcoord.zw = v.texcoord.xy * _DecalTex_ST.xy + _DecalTex_ST.zw;
	#elif defined(V_CW_DETAIL)
		o.texcoord.zw = v.texcoord.xy * _Detail_ST.xy + _Detail_ST.zw;
	#elif defined(V_CW_BLEND_BY_VERTEX)
		o.texcoord.zw = v.texcoord.xy * _BlendTex_ST.xy + _BlendTex_ST.zw;
	#endif
	
	#ifdef LIGHTMAP_ON
		o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
	#endif

	
	#ifdef NEED_V_CALC_VIEWDIR_WS
		float3 viewDir_WS = V_WorldSpaceViewDir(v.vertex);
	#endif
	#ifdef NEED_V_CALC_VIEWDIR_OS
		float3 viewDir_OS = V_ObjSpaceViewDir(v.vertex.xyz);
	#endif
	
	#ifdef NEED_V_CALC_POS_WS
		half3 pos_WS = mul(_Object2World, v.vertex).xyz;
	#endif

	#ifdef NEED_V_CALC_NORMAL_WS
		float3 normal_WS = normalize(mul((half3x3)_Object2World, v.normal * 1.0));
	#endif


	#ifdef NEED_P_NORMAL_WS
		o.normal = normal_WS;
	#endif
	#ifdef NEED_P_VIEWDIR_WS
		o.viewDir.xyz = viewDir_WS;
	#endif
	#ifdef NEED_P_REFLECTION_WS		
		o.refl.xyz = reflect(-viewDir_WS, normal_WS);

		#ifdef V_CW_FRESNEL_ON
			half fresnel = dot(v.normal, viewDir_OS);
			o.refl.w = max(0, _V_CW_Fresnel_Bias - fresnel * fresnel);
		#endif
	#endif

	#ifdef NEED_P_LIGHTDIR_WS
		o.vLight.xyz = lightDir_WS;
	#endif

	#if defined(V_CW_RIM_ON) || defined(V_CW_GLOBAL_RIM_ON)
		half rim = saturate(dot (v.normal, viewDir_OS) + _V_CW_Rim_Bias);
		o.vfx.x = rim * rim;
	#endif
	
	#if defined(V_CW_VERTEX_COLOR_ON) || defined(V_CW_GLOBAL_VERTEX_COLOR_ON) || defined(V_CW_BLEND_BY_VERTEX)
		o.color = v.color;
	#endif

	#ifdef UNITY_PASS_FORWARDBASE
		#ifndef LIGHTMAP_ON
		 
			#if defined(VERTEXLIGHT_ON) && defined(V_CW_UNITY_VERTEXLIGHT_ON)		
				o.vLight.rgb = Shade4PointLights ( unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
			  				 				        unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
												    unity_4LightAtten0, pos_WS, normal_WS );
			#endif

			#ifndef V_CW_LIGHT_PER_PIXEL

				o.vLight.a = (max(0, dot (normal_WS, _WorldSpaceLightPos0.xyz)));	
				 
				#ifdef V_CW_SPECULAR
				//	half3 h = normalize (_WorldSpaceLightPos0.xyz + normalize(viewDir_WS));	
				//	float nh = max (0, dot (normal_WS, h));
				//	o.vLight.w = pow (nh, _Shininess * 128.0);

				o.viewDir.w = max (0, dot(normal_WS, normalize(_WorldSpaceLightPos0.xyz + normalize(viewDir_WS))));
				
				#endif
			#endif
		#endif
	#endif
	

	#if defined(V_CW_FOG_ON) || defined(V_CW_GLOBAL_FOG_ON)
		o.vfx.y = saturate((V_CW_FOG_END.x - length(mv.xyz) * V_CW_FOG_DENSITY) / (V_CW_FOG_END.x - V_CW_FOG_START.x));
	#endif

	
	#if defined(UNITY_PASS_FORWARDBASE) || defined(UNITY_PASS_FORWARDADD)
		#ifndef V_NO_SHADOWS
			TRANSFER_VERTEX_TO_FRAGMENT(o);
		#endif
	#endif

	return o;
}

vOutput vert_particles(vInput v)
{ 
	vOutput o = (vOutput)0;
	
	V_CW_BEND(v.vertex)


	o.texcoord.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;

	#if defined(V_CW_VERTEX_COLOR_ON) || defined(V_CW_GLOBAL_VERTEX_COLOR_ON)
		o.color = v.color;
	#endif

	#if defined(V_CW_FOG_ON) || defined(V_CW_GLOBAL_FOG_ON)
		o.vfx.y = saturate((V_CW_FOG_END.x - length(mv.xyz) * V_CW_FOG_DENSITY) / (V_CW_FOG_END.x - V_CW_FOG_START.x));
	#endif	


	return o;
}

vOutput vert_text(vInput v)
{ 
	vOutput o = (vOutput)0;
	
	V_CW_BEND(v.vertex)
	

	o.texcoord.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;

	#if defined(V_CW_VERTEX_COLOR_ON) || defined(V_CW_GLOBAL_VERTEX_COLOR_ON)
		o.color = v.color * _Color;
	#endif

	#if defined(V_CW_FOG_ON) || defined(V_CW_GLOBAL_FOG_ON)
		o.vfx.y = saturate((V_CW_FOG_END.x - length(mv.xyz) * V_CW_FOG_DENSITY) / (V_CW_FOG_END.x - V_CW_FOG_START.x));
	#endif

	return o;
}

#endif

#ifdef UNITY_PASS_SHADOWCASTER
vOutput vert_ShadowCaster(appdata_full v)
{ 
	vOutput o;	

	V_CW_BEND(v.vertex)


	#ifdef V_CW_CUTOUT
		o.texcoord = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
	#endif

	TRANSFER_SHADOW_CASTER(o)
	 
	return o;
} 
#endif
 

#ifdef UNITY_PASS_SHADOWCOLLECTOR
vOutput vert_ShadowCollector(appdata_full v)
{ 
	vOutput o;
	
	V_CW_BEND(v.vertex)

	 
	#ifdef V_CW_CUTOUT
		o.texcoord = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
	#endif
	
	float4 wpos = mul(_Object2World, v.vertex); 
	o._WorldPosViewZ.xyz = wpos; 
	o._WorldPosViewZ.w = -mul( UNITY_MATRIX_MV, v.vertex ).z; 
	o._ShadowCoord0 = mul(unity_World2Shadow[0], wpos).xyz; 
	o._ShadowCoord1 = mul(unity_World2Shadow[1], wpos).xyz; 
	o._ShadowCoord2 = mul(unity_World2Shadow[2], wpos).xyz; 
	o._ShadowCoord3 = mul(unity_World2Shadow[3], wpos).xyz;

	return o;
}
#endif
	
#endif