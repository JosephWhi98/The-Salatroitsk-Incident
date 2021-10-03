using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class PS1ShaderEditor : ShaderGUI
{

	public enum RenderModes
	{
		Opaque,
		Transparent,
		Cutout
	}

	public enum DiffuseModels
	{
		Vertex,
		Fragment
	}

	public enum SpecularModels
	{
		Gouraud,
		Phong
	}

	public enum BlendOperations
	{
		Add,
		Subtract,
		ReverseSubtract
	}

	public enum UVMapping
	{
		NoOverride,
		AffineMapping,
		Default
	}

	private static class Styles
	{
		public static GUIContent mainTexText = EditorGUIUtility.TrTextContent("Main Texture");
		public static GUIContent normalMapText = EditorGUIUtility.TrTextContent("Normal Map");
		public static GUIContent specularMapText = EditorGUIUtility.TrTextContent("Specular Map");
		public static GUIContent metalMapText = EditorGUIUtility.TrTextContent("Metal Map");
		public static GUIContent emissionMapText = EditorGUIUtility.TrTextContent("Emission Map");
		public static GUIContent cubeMapText = EditorGUIUtility.TrTextContent("Cubemap");
		public static GUIContent lodTexText = EditorGUIUtility.TrTextContent("LOD Texture");

		public static string renderModeText = "Render Mode";
		public static string blendOpText = "Blend Operation";
		public static string diffuseModeText = "Diffuse Mode";
		public static string specularModeText = "Specular Mode";
		public static string unlitText = "Unlit";
		public static string drawDistText = "Draw Distance Influence";
		public static string texturesText = "Textures";
		public static string settingsText = "Settings";
		public static string vertInnText = "Override Vertex Inaccuracy";
		public static string uvMapText = "Override UV Mapping";
		public static string cutoutThresholdText = "Alpha Cutoff";
		public static readonly string[] blendNames = Enum.GetNames(typeof(RenderModes));
		public static readonly string[] diffuseNames = Enum.GetNames(typeof(DiffuseModels));
		public static readonly string[] specularNames = Enum.GetNames(typeof(SpecularModels));
		public static readonly string[] blendOpNames = Enum.GetNames(typeof(BlendOperations));
		public static readonly string[] uvNames = Enum.GetNames(typeof(UVMapping));
	}

	MaterialProperty _MainTex = null;
	MaterialProperty _Color = null;
	MaterialProperty _RenderMode = null;
	MaterialProperty _BlendOp = null;
	MaterialProperty _Unlit = null;
	MaterialProperty _DrawDist = null;
	MaterialProperty _VertexInaccuracy = null;
	MaterialProperty _DiffModel = null;
	MaterialProperty _NormalMap = null;
	MaterialProperty _NormalMapDepth = null;
	MaterialProperty _SpecModel = null;
	MaterialProperty _SpecularMap = null;
	MaterialProperty _Specular = null;
	MaterialProperty _MetalMap = null;
	MaterialProperty _Metallic = null;
	MaterialProperty _Smoothness = null;
	MaterialProperty _EmissionMap = null;
	MaterialProperty _Emission = null;
	MaterialProperty _Cube = null;
	MaterialProperty _LODTex = null;
	MaterialProperty _LODAmt = null;
	MaterialProperty _CutoutThreshold = null;

	public void FindProperties(MaterialProperty[] props) {
		_MainTex = FindProperty("_MainTex", props);
		_Color = FindProperty("_Color", props);
		_RenderMode = FindProperty("_RenderMode", props);
		_BlendOp = FindProperty("_BlendOp", props);
		_Unlit = FindProperty("_Unlit", props);
		_DrawDist = FindProperty("_DrawDist", props);
		_VertexInaccuracy = FindProperty("_VertexInaccuracy", props);
		_DiffModel = FindProperty("_DiffModel", props);
		_NormalMap = FindProperty("_NormalMap", props);
		_NormalMapDepth = FindProperty("_NormalMapDepth", props);
		_SpecModel = FindProperty("_SpecModel", props);
		_SpecularMap = FindProperty("_SpecularMap", props);
		_Specular = FindProperty("_Specular", props);
		_MetalMap = FindProperty("_MetalMap", props);
		_Metallic = FindProperty("_Metallic", props);
		_Smoothness = FindProperty("_Smoothness", props);
		_EmissionMap = FindProperty("_EmissionMap", props);
		_Emission = FindProperty("_Emission", props);
		_Cube = FindProperty("_Cube", props);
		_LODTex = FindProperty("_LODTex", props);
		_LODAmt = FindProperty("_LODAmt", props);
		_CutoutThreshold = FindProperty("_CutoutThreshold", props);
	}

	public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties) {
		FindProperties(properties);
		Material material = materialEditor.target as Material;

		bool culling = Array.IndexOf(material.shaderKeywords, "BFC") != -1;
		bool depthWrite = Array.IndexOf(material.shaderKeywords, "DEPTH_WRITE") != -1;

		EditorGUI.BeginChangeCheck();
		EditorGUILayout.LabelField(Styles.settingsText, EditorStyles.boldLabel);
		_RenderMode.floatValue = EditorGUILayout.Popup(Styles.renderModeText, (int)_RenderMode.floatValue, Styles.blendNames);
		if (_RenderMode.floatValue == 2) {
			_CutoutThreshold.floatValue = EditorGUILayout.Slider(Styles.cutoutThresholdText, _CutoutThreshold.floatValue, 0.0f, 1.0f);
		}
		_BlendOp.floatValue = EditorGUILayout.Popup(Styles.blendOpText, (int)_BlendOp.floatValue, Styles.blendOpNames);
		depthWrite = EditorGUILayout.Toggle("Ignore Depth Buffer", depthWrite);
		culling = EditorGUILayout.Toggle("Backface Culling", culling);
		_Unlit.floatValue = EditorGUILayout.Toggle(Styles.unlitText, _Unlit.floatValue > 0.0f) ? 1.0f : 0.0f;
		_DrawDist.floatValue = EditorGUILayout.Toggle(Styles.drawDistText, _DrawDist.floatValue > 0.0f) ? 1.0f : 0.0f;
		_VertexInaccuracy.floatValue = EditorGUILayout.FloatField(Styles.vertInnText, _VertexInaccuracy.floatValue);
		EditorGUILayout.Separator();

		EditorGUILayout.LabelField(Styles.texturesText, EditorStyles.boldLabel);
		materialEditor.TexturePropertySingleLine(Styles.mainTexText, _MainTex, _Color);
		if (_NormalMap.textureValue == null)
			_DiffModel.floatValue = EditorGUILayout.Popup(Styles.diffuseModeText, (int)_DiffModel.floatValue, Styles.diffuseNames);
		materialEditor.TexturePropertySingleLine(Styles.normalMapText, _NormalMap, _NormalMapDepth);
		materialEditor.TexturePropertySingleLine(Styles.specularMapText, _SpecularMap, _Specular);
		_SpecModel.floatValue = EditorGUILayout.Popup(Styles.specularModeText, (int)_SpecModel.floatValue, Styles.specularNames);
		if (_MetalMap.textureValue == null) {
			materialEditor.TexturePropertySingleLine(Styles.metalMapText, _MetalMap, _Metallic);
		} else {
			materialEditor.TexturePropertySingleLine(Styles.metalMapText, _MetalMap);
		}
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Smoothness");
		_Smoothness.floatValue = EditorGUILayout.Slider(_Smoothness.floatValue, 0.0f, 1.0f);
		EditorGUILayout.EndHorizontal();
		materialEditor.TexturePropertySingleLine(Styles.emissionMapText, _EmissionMap, _Emission);
		materialEditor.TexturePropertySingleLine(Styles.cubeMapText, _Cube);
		materialEditor.TexturePropertySingleLine(Styles.lodTexText, _LODTex, _LODAmt);

		EditorGUILayout.Separator();

		EditorGUI.BeginChangeCheck();
		materialEditor.TextureScaleOffsetProperty(_MainTex);
		if (EditorGUI.EndChangeCheck()) {
			_Emission.textureScaleAndOffset = _MainTex.textureScaleAndOffset;
		}



		if (EditorGUI.EndChangeCheck()) {
			if (_RenderMode.floatValue == 1) {
				material.SetOverrideTag("RenderType", "Transparent");
				material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
				material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				material.DisableKeyword("_ALPHATEST_ON");
				material.EnableKeyword("_ALPHABLEND_ON");
				material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				material.EnableKeyword("TRANSPARENT");
				material.DisableKeyword("OPAQUE");
				material.DisableKeyword("CUTOUT");
				material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
			} else {
				material.SetOverrideTag("RenderType", "Opaque");
				material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
				material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
				material.DisableKeyword("_ALPHATEST_ON");
				material.DisableKeyword("_ALPHABLEND_ON");
				material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				material.DisableKeyword("TRANSPARENT");
				if (_RenderMode.floatValue == 0) {
					material.EnableKeyword("OPAQUE");
					material.DisableKeyword("CUTOUT");
				} else if (_RenderMode.floatValue == 2) {
					material.EnableKeyword("CUTOUT");
					material.DisableKeyword("OPAQUE");
				}
				material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;

			}
			if (depthWrite) {
				material.SetInt("_ZWrite", 0);
				if (_RenderMode.floatValue != 1)
					material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry + 1;
				material.EnableKeyword("DEPTH_WRITE");
			} else {
				material.SetInt("_ZWrite", 1);
				if (_RenderMode.floatValue != 1)
					material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
				material.DisableKeyword("DEPTH_WRITE");
			}
			material.SetShaderPassEnabled("ForwardAdd", _Unlit.floatValue == 0);
			material.SetShaderPassEnabled("Meta", true);
			material.EnableKeyword("_EMISSION");
			if (culling) {
				material.SetInt("_Cul", (int)UnityEngine.Rendering.CullMode.Back);
				material.EnableKeyword("BFC");
			} else {
				material.SetInt("_Cul", (int)UnityEngine.Rendering.CullMode.Off);
				material.DisableKeyword("BFC");
			}
			if (_NormalMap.textureValue != null)
				_DiffModel.floatValue = 1.0f;

			if (_Unlit.floatValue == 1) {
				material.EnableKeyword("UNLIT");
			} else {
				material.DisableKeyword("UNLIT");
			}

			if (_DiffModel.floatValue == 0) {
				material.EnableKeyword("DIFF_VERTEX");
				material.DisableKeyword("DIFF_FRAGMENT");
			} else if (_DiffModel.floatValue == 1) {
				material.DisableKeyword("DIFF_VERTEX");
				material.EnableKeyword("DIFF_FRAGMENT");
			}

			if (_SpecModel.floatValue == 0) {
				material.EnableKeyword("SPEC_GOURAUD");
				material.DisableKeyword("SPEC_PHONG");
			} else if (_SpecModel.floatValue == 1) {
				material.DisableKeyword("SPEC_GOURAUD");
				material.EnableKeyword("SPEC_PHONG");
			}

			foreach (Material m in materialEditor.targets) {
				m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;
			}

			SetKeyword(material, "_METAL_MAP", material.GetTexture("_MetalMap"));
			SetKeyword(material, "_NORMAL_MAP", material.GetTexture("_NormalMap"));
			SetKeyword(material, "_EMISSION", (material.globalIlluminationFlags & MaterialGlobalIlluminationFlags.EmissiveIsBlack) == 0);
			SetKeyword(material, "_EMISSION_MAP", material.GetTexture("_EmissionMap"));
			SetKeyword(material, "_CUBE_MAP", material.GetTexture("_Cube"));
			SetKeyword(material, "_LOD_TEX", material.GetTexture("_LODTex"));
		}

		void SetKeyword(Material m, string keyword, bool state) {
			if (state)
				m.EnableKeyword(keyword);
			else
				m.DisableKeyword(keyword);
		}
	}
}
