#ifndef __PSXEFFECTS_CGINC__
#define __PSXEFFECTS_CGINC__

sampler2D _MainTex;
sampler2D _EmissionMap;
sampler2D _NormalMap;
sampler2D _SpecularMap;
sampler2D _MetalMap;
sampler2D _LODTex;
sampler2D _DetailAlbedoMap;
float4 _DetailAlbedoMap_ST;
float4 _MainTex_ST;
float4 _MainTex_TexelSize;
float _VertexSnappingDetail;
float _VertexInaccuracy;
float _AffineMapping;
float _AffineBounds;
float _DrawDistance;
float _Specular;
float4 _Color;
float _DarkMax;
float _Unlit;
float _SkyboxLighting;
float _WorldSpace;
float3 _Emission;
float _Metallic;
float _Smoothness;
float _Triplanar;
float _DrawDist;
float _SpecModel;
float _DiffModel;
float _RenderMode;
float _CamPos;
float _LODAmt;
float _ShadowType;
samplerCUBE _Cube;
float _NormalMapDepth;
float _CutoutThreshold;

// This function snaps a vertex to a screen-space approximation
float4 PixelSnap(float4 pos)
{
	float2 hpc = _ScreenParams.xy * 0.75f;
	_VertexInaccuracy /= 8;
	float4 pixelPos = pos;
	pixelPos.xy = pos.xy / pos.w;
	pixelPos.xy = floor(pixelPos.xy * hpc / _VertexInaccuracy) * _VertexInaccuracy / hpc;
	pixelPos.xy *= pos.w;
	return pixelPos;
}

// Unused
float GetDither(float2 pos, float factor) {
	float DITHER_THRESHOLDS[16] =
	{
		1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
		13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
		4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
		16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
	};

	// Dynamic indexing isn't allowed in WebGL for some weird reason so here's this strange workaround
	#ifdef SHADER_API_GLES3
	int i = (int(pos.x) % 4) * 4 + int(pos.y) % 4;
	for (int x = 0; x < 16; x++)
		if (x == i)
			return factor - DITHER_THRESHOLDS[x];
	return 0;
	#else
	return factor - DITHER_THRESHOLDS[(uint(pos.x) % 4) * 4 + uint(pos.y) % 4];
	#endif
}

float2 PerformAffineMapping(float4 inp, float4 st) {
	float2 rtn = inp.xy;
	#if defined(AFFINE_MAPPING)
		rtn = (inp / inp.z + st.zw) * st.xy;
	#else
		rtn = (inp + st.zw) * st.xy;
	#endif
	return rtn;
}

float2 PerformAffineMapping(float4 inp, float4 st, float4 origUv, float4 ts, float clampAmt) {
	float2 rtn = inp.xy;
	#if defined(AFFINE_MAPPING)
		float2 bounds = ts * clampAmt;
		rtn = float2(clamp(inp.x / inp.z, origUv.x - bounds.x, origUv.x + ts.x + bounds.x) + st.z, clamp(inp.y / inp.z, origUv.y - bounds.y, origUv.y + ts.y + bounds.y) + st.w) * st.xy;
	#else
		rtn = (inp + st.zw) * st.xy;
	#endif
	return rtn;
}

#endif