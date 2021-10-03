#ifndef __PSXEFFECTS_LIGHTMAPPING_CGINC__
#define __PSXEFFECTS_LIGHTMAPPING_CGINC__

#include "UnityPBSLighting.cginc"
#include "UnityMetaPass.cginc"
#include "PSXEffects.cginc"

struct appdata {
	float4 vertex : POSITION;
	float2 uv : TEXCOORD0;
	float4 uv1 : TEXCOORD1;
	float4 uv2 : TEXCOORD2;
};

struct v2f {
	float4 pos : SV_POSITION;
	float4 uv : TEXCOORD0;
};

float3 GetAlbedo(v2f i) {
	return tex2D(_MainTex, i.uv);
}

float GetMetallic(v2f i) {
	#if defined(_METAL_MAP)
		return tex2D(_MetalMap, i.uv).rgb * _Metallic;
	#else
		return _Metallic;
	#endif
}

float GetSmoothness(v2f i) {
	#if defined(_METAL_MAP)
		return tex2D(_MetalMap, i.uv).a * _Smoothness;
	#else
		return _Smoothness;
	#endif
}

float3 GetEmission(v2f i) {
	#if defined(_EMISSION_MAP)
		return tex2D(_EmissionMap, i.uv) * _Emission;
	#else
		return _Emission;
	#endif
}

v2f vert(appdata v) {
	v2f o;
	o.pos = UnityMetaVertexPosition(v.vertex, v.uv1.xy, v.uv2.xy, unity_LightmapST, unity_DynamicLightmapST);

	o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
	o.uv.zw = TRANSFORM_TEX(v.uv, _DetailAlbedoMap);
	return o;
}

float4 frag(v2f i) : SV_TARGET {
	UnityMetaInput surfaceData;
	UNITY_INITIALIZE_OUTPUT(UnityMetaInput, surfaceData);
	surfaceData.Emission = GetEmission(i);
	float oneMinusReflectivity;
	surfaceData.Albedo = DiffuseAndSpecularFromMetallic(GetAlbedo(i) * _Color, GetMetallic(i), surfaceData.SpecularColor, oneMinusReflectivity);

	float roughness = SmoothnessToRoughness(GetSmoothness(i)) * 0.5;
	surfaceData.Albedo += surfaceData.SpecularColor * roughness;
	
	return UnityMetaFragment(surfaceData);
}

#endif