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
            #pragma multi_compile_fog

			#define c1 0xcc9e2d51u
			#define c2 0x1b873593u

            #include "UnityCG.cginc"

            struct VertexIn {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct VertexOut {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float _Density;
			int _NumCells;

			
            VertexOut VertShader(VertexIn i) {
                VertexOut output;
                
                output.pos = UnityObjectToClipPos(i.pos);
                output.uv = TRANSFORM_TEX(i.uv, _MainTex);

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

				rand = round(rand);

				if (rand > 0) {

				}
				else {
					discard;
				}

                return fixed4(rand, 0.0, 0.0, 1.0);
            }

            ENDCG
        }
    }
}
