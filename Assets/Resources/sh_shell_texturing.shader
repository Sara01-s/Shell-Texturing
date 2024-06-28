Shader "Custom/Unlit/sh_shell_texturing" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
	}
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }

        LOD 100 
		Cull Off

        Pass {
            CGPROGRAM
            #pragma vertex VertShader
            #pragma fragment FragShader

            #include "UnityCG.cginc"

            struct VertexIn {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
				float3 tanget : TANGENT;
            };

            struct VertexOut {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
				float3 normalWorld : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
			int _NumCells;
			int _NumShells;
			int _ShellIndex;
			float _ShellsSeparation;

			float _HeightAttenuation;
			float _ShellLength;
			float _CellThickness;

			float4 _Color;
			float4 _ShadowColor;
			float4 _SecondaryShadowColor;
			float _LightAttenuation;
			float _LightSmooth;
			float _ShadowIntensity;

			
            VertexOut VertShader(VertexIn i) {
                VertexOut output;

				float height = (float)_ShellIndex / (float)_NumShells;
				height *= _ShellsSeparation;
				height = pow(height, _HeightAttenuation);

				i.pos.xyz += i.normal * _ShellLength * height;

                output.pos = UnityObjectToClipPos(i.pos);
                output.uv = TRANSFORM_TEX(i.uv, _MainTex);
				output.normalWorld = normalize(mul((float3x3)unity_WorldToObject, i.normal));

                return output;
            }


			// Thanks to Hugo Elias
			float hash(uint n) {
				n = (n << 13u) ^ n;
				n = n * (n * n * 15731u + 0x789221u) + 0x1376312589u;
				
				return float(n & uint(0x7fffffffu)) / float(0x7fffffff);
			}

			uint seed(uint cellX, uint cellY, uint t) {
				return cellX + (cellY * t);
			}

            fixed4 FragShader(VertexOut i) : SV_Target {
				i.uv *= _NumCells;

				uint2 cell = uint2(i.uv);
				float rand = hash(seed(cell.x, cell.y, _NumCells + 1));

				float height = (float)_ShellIndex / (float)_NumShells;

				float2 cellUv = frac(i.uv) * 2.0 - 1.0;

				if (length(cellUv) > _CellThickness * (rand - height) && _ShellIndex > 0) {
					discard;
				}

				float diffuse = dot(i.normalWorld, normalize(_WorldSpaceLightPos0.xyz));
				float halfLambert = diffuse * 0.5 + 0.5;
				float clampedHalfLambert = max(_ShadowIntensity, halfLambert);
				

				float diffuse2 = dot(i.normalWorld, normalize(_WorldSpaceLightPos0.xyz));
				float smoothedDiffuse2 = smoothstep(0.0, 0.2, diffuse2);


				float smoothedLight = smoothstep(0.0, _LightSmooth, clampedHalfLambert);
				float4 primaryLightColor = lerp(_ShadowColor, _Color, smoothedLight);

				float4 secondaryLightColor = lerp(primaryLightColor, _SecondaryShadowColor, smoothedLight);

				return fixed4(primaryLightColor);
            }

            ENDCG
        }
    }
}
