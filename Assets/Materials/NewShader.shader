Shader "Custom/NewShader" {
	Properties {
		 _Color ("Main Color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		 float4 _Color;
 
        struct Input {
            float4 color: Color; // Vertex color
        };

		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = _Color.rgb + IN.color.rgb; 
			o.Alpha = IN.color.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
