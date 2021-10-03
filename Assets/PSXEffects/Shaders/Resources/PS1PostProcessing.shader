Shader "Hidden/PS1PostProcessing"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_DitherTex("", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth writing
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ DITHER_SKY
			#pragma multi_compile _ DITHER_FAST DITHER_SLOW DITHER_TEX
			#pragma multi_compile _ SCANLINES_ON
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			#ifdef SHADER_API_GLES3
			#define _SHADER_API_GLES3
			#endif
			#ifdef SHADER_API_METAL
			#define _SHADER_API_METAL
			#endif
			#define DITHER_COLORS 256

			sampler2D _MainTex;
			sampler2D _DitherTex;
			sampler2D _CameraDepthTexture;
			float4 _DitherTex_TexelSize;
			float _ColorDepth;
			float _Scanlines;
			float _ScanlineIntensity;
			float _Dithering;
			float _DitherThreshold;
			float _DitherIntensity;
			float _DitherSky;
			float _SubtractFade;
			float _FavorRed;
			float _SLDirection;
			float _ResX;
			float _ResY;
			float _DitherType;

			float GetDitherOld(float2 pos, float factor) {
				float DITHER_THRESHOLDS[16] =
				{
					1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
					13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
					4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
					16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
				};

				// Dynamic indexing isn't allowed in WebGL for some weird reason so here's this strange workaround
				uint i = (uint(pos.x) & 3) * 4 + uint(pos.y) & 3;
				#ifdef SHADER_API_GLES3
				for (int x = 0; x < 16; x++)
					if (x == i)
						return factor - DITHER_THRESHOLDS[x];
				return 0;
				#else
				return factor - DITHER_THRESHOLDS[i];
				#endif
			}

			float3 GetDither(float2 pos, float3 c, float intensity) {
				int DITHER_THRESHOLDS[16] =
				{
					-4, 0, -3, 1,
					2, -2, 3, -1,
					-3, 1, -4, 0,
					3, -1, 2, -2
				};
				int lut[DITHER_COLORS];
				uint j = (uint(pos.x) & 3) * 4 + uint(pos.y) & 3;
				for (int i = 0; i < DITHER_COLORS; i++) {
					int value;
					#if defined(_SHADER_API_GLES3) || defined(_SHADER_API_METAL)
					for (int x = 0; x < 16; x++)
						if (x == j)
							value = i + DITHER_THRESHOLDS[x] * (intensity * 100);
					#else
					value = i + DITHER_THRESHOLDS[j] * (intensity * 100);
					#endif
					value = clamp(value, 0, DITHER_COLORS - 1);
					lut[i] = value;
				}

				#if defined(_SHADER_API_GLES3) || defined(_SHADER_API_METAL)
				uint j = c.r * (DITHER_COLORS - 1);
				for (int x = 0; x < DITHER_COLORS; x++)
					if (x == j)
						c.r = lut[x];
				j = c.g * (DITHER_COLORS - 1);
				for (int x = 0; x < DITHER_COLORS; x++)
					if (x == j)
						c.g = lut[x];
				j = c.b * (DITHER_COLORS - 1);
				for (int x = 0; x < DITHER_COLORS; x++)
					if (x == j)
						c.b = lut[x];
				#else
				c.r = lut[c.r * (DITHER_COLORS-1)];
				c.g = lut[c.g * (DITHER_COLORS-1)];
				c.b = lut[c.b * (DITHER_COLORS-1)];
				#endif
				c /= DITHER_COLORS;
				return c;
			}

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float aspect = _ResY / _ResX;
				fixed4 col = tex2D(_MainTex, i.uv);
				#if !UNITY_COLORSPACE_GAMMA
				col.rgb = LinearToGammaSpace(col.rgb);
				#endif
				
				#ifdef UNITY_COLORSPACE_GAMMA
					half luma = LinearRgbToLuminance(GammaToLinearSpace(saturate(col.rgb)));
				#else
					half luma = LinearRgbToLuminance(col.rgb);
				#endif

				// Manipulate colors/saturate
				col.rgb -= (3 - col.rgb) * _SubtractFade;
				#if UNITY_COLORSPACE_GAMMA
					col.rgb -= _FavorRed * ((1 - col.rgb) * 0.25);
					col.r += _FavorRed * ((0.5 - col.rgb) * 0.1);
				#else
					col.rgb -= GammaToLinearSpace(_FavorRed * ((1 - col.rgb) * 0.25));
					col.r += GammaToLinearSpace(_FavorRed * ((0.5 - col.rgb) * 0.1));
				#endif

				// Scanlines
				#if defined(SCANLINES_ON)
					float sl = floor(i.uv.x * _ResX % 2) * _SLDirection + floor(i.uv.y * _ResY % 2) * (1 - _SLDirection);
					col.rgb += sl * _ScanlineIntensity;
					col.rgb = saturate(col.rgb);
				#endif

				// Calculate depth texture
				float depth = 1;
				#if !defined(DITHER_SKY)
					depth = 1 - floor(Linear01Depth(tex2D(_CameraDepthTexture, i.uv).r));
				#endif

				// Calculate dithering values based on _DitherTex
				float4 ditherTex = tex2D(_DitherTex, float2(i.uv.x, i.uv.y * aspect) * _DitherTex_TexelSize.x * _ResX);
				#if defined(DITHER_TEX)
					half dither = (ditherTex.a - 0.5) * 2 / _DitherThreshold;
					col.rgb *= 1.0f + (luma < dither ? (1 - luma) * (1 - (_ColorDepth / 24)) * _DitherIntensity * 10 : 0) * depth * _Dithering;
				#else
					//if (ditherTex.r == 1 && ditherTex.g == 1 && ditherTex.b == 1 && ditherTex.a == 1 && _DitherType < 2) {
					// Metal can't handle cool accurate dithering unfortunately
					#ifdef SHADER_API_METAL
						col.rgb += GetDitherOld(float2(i.uv.x, i.uv.y * aspect) * _ResX, 1) * _DitherIntensity * depth * _Dithering;
					#else
						#if defined(DITHER_SLOW)
							col.rgb = GetDither(float2(i.uv.x, i.uv.y * aspect) * _ResX, col.rgb, _DitherIntensity * depth * _Dithering / 3.0);
						#elif defined(DITHER_FAST)
							col.rgb += GetDitherOld(float2(i.uv.x, i.uv.y * aspect) * _ResX, 1) * _DitherIntensity * depth * _Dithering;
						#endif
					#endif
				#endif

				// Posterize
				col.rgb = saturate(floor(col.rgb * _ColorDepth) / _ColorDepth);

				#if !UNITY_COLORSPACE_GAMMA
					col.rgb = GammaToLinearSpace(col.rgb);
				#endif

				return col;
			}
			ENDCG
		}
	}
}
