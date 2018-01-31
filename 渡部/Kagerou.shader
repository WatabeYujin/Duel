Shader "Custom/Kagerou" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_KagerouRange("DistanceLimit",Range(0,100))=0.0
	}

	SubShader{
		Pass{
			ZTest Always Cull Off ZWrite Off

			CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
#include "UnityCG.cginc"

			sampler2D _MainTex;
			uniform fixed4 _MainTex_ST;
			sampler2D_float _CameraDepthTexture;
			float _KagerouRange;
			float4  _MainTex_TexelSize;

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			fixed2 random2(fixed2 st) {
				st = fixed2(dot(st, fixed2(127.1, 311.7)),
					dot(st, fixed2(269.5, 183.3)));
				return -1.0 + 2.0*frac(sin(st)*43758.5453123);
			}

			float perlinNoise(fixed2 st)
			{
				fixed2 p = floor(st);
				fixed2 f = frac(st);
				fixed2 u = f*f*(3.0 - 2.0*f);

				float v00 = random2(p + fixed2(0, 0));
				float v10 = random2(p + fixed2(1, 0));
				float v01 = random2(p + fixed2(0, 1));
				float v11 = random2(p + fixed2(1, 1));

				return lerp(lerp(dot(v00, f - fixed2(0, 0)), dot(v10, f - fixed2(1, 0)), u.x),
					lerp(dot(v01, f - fixed2(0, 1)), dot(v11, f - fixed2(1, 1)), u.x),
					u.y) + 0.1f;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed2 uv = i.uv;
#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)uv.y = 1 - uv.y;
#endif
				float d = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UnityStereoScreenSpaceUVAdjust(uv.xy, _MainTex_ST));
				d = Linear01Depth(d);
				if (d > _KagerouRange) {
					float t = _SinTime.x;
					float noise = perlinNoise(float2 (i.uv.x * 12* t, i.uv.y *12 * t));
					uv.y += noise * 0.01;
				}
#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)uv.y = 1 - uv.y;
#endif
				return tex2D(_MainTex, uv);
			}
			ENDCG
		}
	}
	Fallback off
}