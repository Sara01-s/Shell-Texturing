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

			
            VertexOut VertShader(VertexIn i) {
                VertexOut output;
                
                output.pos = UnityObjectToClipPos(i.pos);
                output.uv = TRANSFORM_TEX(i.uv, _MainTex);

                return output;
            }


			// Thanks to Hugo Elias
			float hash(uint n) {
				n = (n << 13U) ^ n;
				n = n * (n * n * 15731U + 0x789221U) + 0x1376312589U;
				
				return float(n & uint(0x7fffffffU)) / float(0x7fffffff);
			}

            fixed4 FragShader(VertexOut i) : SV_Target {
				i.uv *= _Density;
				// tid = Thread Id, aquí se utiliza para describir una hilera de shell
				uint2 tid = uint2(i.uv);
				uint seed = 1;
				//uint seed = tid.x + (tid.y * 2000U);
				float rand = hash(seed);

                return fixed4(float3(rand, rand, rand), 1.0);
            }

            ENDCG
        }
    }
}
