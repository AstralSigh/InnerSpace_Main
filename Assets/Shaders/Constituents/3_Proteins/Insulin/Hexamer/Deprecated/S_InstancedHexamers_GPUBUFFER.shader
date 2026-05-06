Shader "DEPRECATED/S_InstancedHexamers_GPUBUFFER" {
	Properties{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Color("Color (RGBA)", Color) = (1, 1, 1, 1) // add _Color property
		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5

	}
		SubShader{

			Tags {"Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout"}
			LOD 100
			//ZWrite Off
			//Blend SrcAlpha OneMinusSrcAlpha
			Cull back

			Pass {

				Tags {"LightMode" = "ForwardBase"}

				CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag alpha

				#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
				#pragma target 4.5

				#include "UnityCG.cginc"
				#include "UnityLightingCommon.cginc"
				#include "AutoLight.cginc"

				sampler2D _MainTex;
				float4 _Color;
				fixed _Cutoff;


			#if SHADER_TARGET >= 45
				StructuredBuffer<float4> positionBuffer;
				StructuredBuffer<float4x4> matrixBuffer;
			#endif

				struct appdata_t {
					float4 vertex   : POSITION;
					float4 color    : COLOR;
				};

				struct v2f
				{
					float4 pos : SV_POSITION;
					float2 uv_MainTex : TEXCOORD0;
					float3 ambient : TEXCOORD1;
					float3 diffuse : TEXCOORD2;
					float3 color : TEXCOORD3;
					SHADOW_COORDS(4)
				};


				v2f vert(appdata_full v, appdata_t i, uint instanceID : SV_InstanceID)
				{
				#if SHADER_TARGET >= 45
					float4 data = positionBuffer[instanceID];
				#else
					float4 data = 0;
				#endif
					float3 localPosition = v.vertex.xyz * data.w;
					float3 worldPosition = mul(matrixBuffer[instanceID], i.vertex) + localPosition;
					float3 worldNormal = v.normal;

					float3 ndotl = saturate(dot(worldNormal, _WorldSpaceLightPos0.xyz));
					float3 ambient = ShadeSH9(float4(worldNormal, 1.0f));
					float3 diffuse = (ndotl * _LightColor0.rgb);
					float3 color = v.color;

					v2f o;
					o.pos = mul(UNITY_MATRIX_VP, float4(worldPosition, 1.0f));
					o.uv_MainTex = v.texcoord;
					o.ambient = ambient;
					o.diffuse = diffuse;
					o.color = color;
					TRANSFER_SHADOW(o)
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed shadow = SHADOW_ATTENUATION(i);
					fixed4 albedo = tex2D(_MainTex, i.uv_MainTex) * _Color;
					float3 lighting = i.diffuse * shadow + i.ambient;
					fixed4 output = fixed4(albedo.rgb * i.color * lighting, albedo.w);
					clip(output.a - _Cutoff);
					UNITY_APPLY_FOG(i.fogCoord, output);
					return output;
				}

				ENDCG
			}
		}
}