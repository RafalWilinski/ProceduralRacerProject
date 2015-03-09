// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

#ifndef VACUUM_CURVEDWORLD_FRAGMENT_CGINC
#define VACUUM_CURVEDWORLD_FRAGMENT_CGINC


#if !defined(UNITY_PASS_SHADOWCASTER) && !defined(UNITY_PASS_SHADOWCOLLECTOR)
fixed4 frag(vOutput i) : SV_Target 
{		
	half4 mainTex = tex2D(_MainTex, i.texcoord.xy);

	#ifdef V_CW_DETAIL
		fixed4 retColor = mainTex * _Color;
		retColor.rgb *= tex2D(_Detail, i.texcoord.zw).rgb * 2;
	#elif defined(V_CW_DECAL)
		fixed4 decal = tex2D(_DecalTex, i.texcoord.zw);
		fixed4 retColor = fixed4(lerp(mainTex.rgb, decal.rgb, decal.a), mainTex.a) * _Color;
	#elif defined(V_CW_BLEND_BY_VERTEX)
		fixed vBlend = clamp(_VertexBlend + i.color.a, 0, 1);
		fixed4 retColor = lerp(mainTex, tex2D(_BlendTex, i.texcoord.zw), vBlend);
	#else
		fixed4 retColor = mainTex * _Color;
	#endif


	#ifdef V_CW_VERTEX_COLOR_ON
		retColor *= i.color;
	#endif 


	#ifdef V_CW_CUTOUT
		#ifdef V_CW_COUTOUT_SOFTEDGE
			clip(-(retColor.a - _Cutoff));
		#else
			clip(retColor.a - _Cutoff);
		#endif
	#endif

	#if defined(V_CW_IBL_ON) || defined(V_CW_GLOBAL_IBL_ON) || defined(V_CW_SELF_ILLUMINATED_ON)
		fixed3 albedo = retColor.rgb;
	#endif


	#if defined(LIGHTMAP_ON) && !defined(PASS_FORWARD_ADD)
		fixed4 lmtex = UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap.xy);
		fixed3 diff = V_DecodeLightmap (lmtex);
	#endif


	#ifndef LIGHTMAP_ON
		#ifdef UNITY_PASS_FORWARDBASE		
			
			#ifdef V_NO_SHADOWS
				fixed atten = 1;
			#else
				fixed atten = LIGHT_ATTENUATION(i);
			#endif

			#ifdef V_CW_LIGHT_PER_PIXEL
				
				i.normal = normalize(i.normal);
				
				fixed3 diff = (_LightColor0.rgb * max(0, dot(i.normal, _WorldSpaceLightPos0.xyz)) * atten + UNITY_LIGHTMODEL_AMBIENT.xyz) * 2;
					
				#ifdef V_CW_UNITY_VERTEXLIGHT_ON
					diff += i.vLight.rgb;
				#endif
					
				#ifdef V_CW_SPECULAR 
					half nh = max (0, dot (i.normal, normalize (_WorldSpaceLightPos0.xyz + normalize(i.viewDir.xyz))));
					fixed3 specular = tex2D(_V_CW_Specular_Lookup, half2(nh, 0.5)).rgb * mainTex.a * _LightColor0.rgb * atten;
				#endif

			#else
				fixed3 diff = (_LightColor0.rgb * i.vLight.a * atten + UNITY_LIGHTMODEL_AMBIENT.xyz) * 2;
				
				#ifdef V_CW_UNITY_VERTEXLIGHT_ON
					diff += i.vLight.rgb;
				#endif

				#ifdef V_CW_SPECULAR
					fixed3 specular = tex2D(_V_CW_Specular_Lookup, half2(i.viewDir.w, 0.5)).rgb * mainTex.a * _LightColor0.rgb * atten;
				#endif	
							
			#endif
		#endif	
	#endif


	//IBL
	#ifdef UNITY_PASS_FORWARDBASE
		#if defined(V_CW_IBL_ON) || defined(V_CW_GLOBAL_IBL_ON)
			diff += V_CW_IBL(i.normal);									
		#endif	
				
		retColor.rgb = diff * retColor.rgb;		

		#if !defined(LIGHTMAP_ON) && defined(V_CW_SPECULAR)
			retColor.rgb += specular;
		#endif
	#endif
	#ifdef UNITY_PASS_UNLIT
		#ifdef LIGHTMAP_ON
			//retColor.rgb = (diff + V_CW_IBL(i.normal)) * retColor.rgb;	
		#else
			#if defined(V_CW_IBL_ON) || defined(V_CW_GLOBAL_IBL_ON)
				retColor.rgb = (UNITY_LIGHTMODEL_AMBIENT.xyz * 2 + V_CW_IBL(i.normal)) * retColor.rgb;									
			#endif	
		#endif
	#endif


	#ifdef V_CW_SELF_ILLUMINATED_ON
		retColor.rgb += albedo * mainTex.a * _V_CW_Emission_Strength;
	#endif


	#ifdef V_CW_REFLECTION
		fixed4 reflTex = texCUBE( _Cube, i.refl ) * _ReflectColor;

		#ifdef V_CW_FRESNEL_ON
			retColor.rgb += reflTex.rgb * mainTex.a * i.refl.w;
		#else
			retColor.rgb += reflTex.rgb * mainTex.a;
		#endif
	#endif

	
	#if defined(V_CW_RIM_ON) || defined(V_CW_GLOBAL_RIM_ON)
		retColor.rgb = lerp(_V_CW_Rim_Color.rgb, retColor.rgb, i.vfx.x);
	#endif

	#if defined(V_CW_FOG_ON) || defined(V_CW_GLOBAL_FOG_ON)
		retColor.rgb = lerp(V_CW_FOG_COLOR.rgb, retColor.rgb, i.vfx.y);
	#endif


	return retColor; 
}
#endif


#ifdef UNITY_PASS_SHADOWCASTER
half4 frag_ShadowCaster(vOutput i) : SV_Target 
{	
	#ifdef V_CW_CUTOUT
		clip(tex2D(_MainTex, i.texcoord.xy).a * _Color.a - _Cutoff);
	#endif

	SHADOW_CASTER_FRAGMENT(i)
}
#endif

#ifdef UNITY_PASS_SHADOWCOLLECTOR
half4 frag_ShadowCollector(vOutput i) : SV_Target 
{
	#ifdef V_CW_CUTOUT
		clip(tex2D(_MainTex, i.texcoord.xy).a * _Color.a - _Cutoff);
	#endif

	SHADOW_COLLECTOR_FRAGMENT(i)
}
#endif
	
#endif