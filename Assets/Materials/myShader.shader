// Upgrade NOTE: commented out 'float4x4 _Object2World', a built-in variable
// Upgrade NOTE: commented out 'float4x4 _World2Object', a built-in variable


Shader "Custom/myShaderTutorial" {
	Properties {
		_Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
	}

	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			// User variables
			uniform float4 _Color;


			// Unity Variables
			uniform float4 _LightColor0;
			// float4x4 _Object2World;
			// float4x4 _World2Object;
			// float4 _WorldSpaceLightPos0;

			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 col : COLOR;
			};

			vertexOutput vert(vertexInput v) {
				vertexOutput o;

				float3 normalDirection = normalize(mul(float4(v.normal, 0.0) , _World2Object).xyz);
				float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz );
				float atten = 1.0;

				float3 diffuseReflection = max(0.0, dot(normalDirection, lightDirection));


				o.col = float4(normalDirection, 1.0);
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

				return o;
			}

			float4 frag(vertexOutput i) : COLOR {
				return i.col;
			}

			ENDCG
		}
	}
	Fallback "Diffuse"
}