using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class PSXEffects : MonoBehaviour {

	public System.Version version = new System.Version("1.18.2");
	public string cfuStatus = "PSXEffects";
	public bool[] sections = { true, true, true, false };

	#region Properties
	public Vector2Int customRes = new Vector2Int(620, 480);
	public int resolutionFactor = 1;
	public int limitFramerate = -1;
	public int skipFrames = 0;
	public bool affineMapping = true;
	public bool clampAffine = false;
	public float affineBounds = 0;
	public float polygonalDrawDistance = 0f;
	public int vertexInaccuracy = 30;
	public int polygonInaccuracy = 2;
	public int colorDepth = 5;
	public bool scanlines = false;
	public int scanlineIntensity = 5;
	public Texture2D ditherTexture;
	public bool dithering = true;
	public float ditherThreshold = 1;
	public int ditherIntensity = 12;
	public int maxDarkness = 20;
	public int subtractFade = 0;
	public float favorRed = 1.0f;
	public bool worldSpaceSnapping = false;
	public bool postProcessing = true;
	public bool verticalScanlines = true;
	public float shadowIntensity = 0.5f;
	public bool downscale = false;
	public bool snapCamera = false;
	public float camInaccuracy = 0.05f;
	public bool camSnapping = false;
	public int shadowType = 0;
	public bool ditherSky = false;
	public int ditherType = 1;
	#endregion

	private Material postProcessingMat;
	private RenderTexture rt;
	private Vector2 prevCustomRes;
	private int[] propIds;

	private void Awake() {
		if (Application.isPlaying) {
			QualitySettings.vSyncCount = 0;
		}

		QualitySettings.antiAliasing = 0;
		cfuStatus = "PSXEffects v" + version.ToString();

		Camera cam = GetComponent<Camera>();
		cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.Depth;

		if (!downscale) {
			customRes = new Vector2Int(Screen.width / resolutionFactor, Screen.height / resolutionFactor);
		}
		rt = new RenderTexture(customRes.x, customRes.y, 16, RenderTextureFormat.ARGB32);
		rt.filterMode = FilterMode.Point;

		string[] props = new string[] {
			"_AffineMapping",
			"_DrawDistance",
			"_VertexSnappingDetail",
			"_Offset",
			"_DarkMax",
			"_SubtractFade",
			"_WorldSpace",
			"_CamPos",
			"_ShadowType",
			"_AffineBounds"
		};
		propIds = new int[props.Length];
		for (int i = 0; i < props.Length; i++) {
			propIds[i] = Shader.PropertyToID(props[i]);
		}

		UpdateProperties();
		CheckForUpdates();
	}

	private void Start() {
		UpdateProperties();
	}

	private void Update() {
		if (!downscale) {
			customRes = new Vector2Int(Screen.width / resolutionFactor, Screen.height / resolutionFactor);
		}

		if (prevCustomRes != customRes) {
			rt = new RenderTexture(customRes.x, customRes.y, 16, RenderTextureFormat.ARGB32);
			rt.filterMode = FilterMode.Point;
		}

		if (Application.isEditor && !Application.isPlaying) {
			UpdateProperties();
		}

		Application.targetFrameRate = limitFramerate;

		prevCustomRes = customRes;
	}

	void LateUpdate() {
		if (snapCamera && Application.isPlaying) {
			// Handles the camera position snapping
			if (transform.parent == null || !transform.parent.name.Contains("CameraRealPosition")) {
				GameObject newParent = new GameObject("CameraRealPosition");
				newParent.transform.position = transform.position;
				if (transform.parent)
					newParent.transform.SetParent(transform.parent);
				transform.SetParent(newParent.transform);
			}

			Vector3 snapPos = transform.parent.position;
			snapPos /= camInaccuracy;
			snapPos = new Vector3(Mathf.Round(snapPos.x), Mathf.Round(snapPos.y), Mathf.Round(snapPos.z));
			snapPos *= camInaccuracy;
			transform.position = snapPos;
		} else if(transform.parent != null && transform.parent.name.Contains("CameraRealPosition")) {
			Destroy(transform.parent.gameObject);
		}
	}

	// Updates shader properties with the properties set in PSXEffects
	public void UpdateProperties() {
		// Set mesh shader variables
		if (affineMapping)
			Shader.EnableKeyword("AFFINE_MAPPING");
		else
			Shader.DisableKeyword("AFFINE_MAPPING");
		if (clampAffine)
			Shader.EnableKeyword("CLAMP_AFFINE");
		else
			Shader.DisableKeyword("CLAMP_AFFINE");
		Shader.SetGlobalFloat(propIds[1], polygonalDrawDistance);
		Shader.SetGlobalInt(propIds[2], (int)(vertexInaccuracy * 0.5f));
		Shader.SetGlobalInt(propIds[3], polygonInaccuracy);
		Shader.SetGlobalFloat(propIds[5], subtractFade * 0.01f);
		Shader.SetGlobalFloat("_AffineBounds", affineBounds);
		if (worldSpaceSnapping)
			Shader.EnableKeyword("WORLD_SPACE_SNAPPING");
		else
			Shader.DisableKeyword("WORLD_SPACE_SNAPPING");
		Shader.SetGlobalFloat(propIds[7], camSnapping ? 1.0f : 0.0f);
		if (shadowType == 0) {
			Shader.EnableKeyword("SHADOW_DEFAULT");
			Shader.DisableKeyword("SHADOW_PSX");
		} else {
			Shader.DisableKeyword("SHADOW_DEFAULT");
			Shader.EnableKeyword("SHADOW_PSX");
		}

		if (postProcessing) {
			// Handles all post processing variables
			if (postProcessingMat == null) {
				postProcessingMat = new Material(Shader.Find("Hidden/PS1PostProcessing"));
			} else {
				postProcessingMat.SetFloat("_ColorDepth", Mathf.Pow(2, colorDepth));
				postProcessingMat.SetFloat("_Scanlines", scanlines ? 1 : 0);
				postProcessingMat.SetFloat("_ScanlineIntensity", scanlineIntensity * 0.01f);
				postProcessingMat.SetTexture("_DitherTex", ditherTexture);
				postProcessingMat.SetFloat("_Dithering", dithering ? 1 : 0);
				postProcessingMat.SetFloat("_DitherThreshold", ditherThreshold);
				postProcessingMat.SetFloat("_DitherIntensity", ditherIntensity * 0.01f);
				postProcessingMat.SetFloat("_ResX", customRes.x);
				postProcessingMat.SetFloat("_ResY", customRes.y);
				postProcessingMat.SetFloat("_FavorRed", favorRed);
				postProcessingMat.SetFloat("_SLDirection", verticalScanlines ? 1 : 0);
				if (ditherSky) {
					postProcessingMat.EnableKeyword("DITHER_SKY");
				} else {
					postProcessingMat.DisableKeyword("DITHER_SKY");
				}
				if (ditherType == 0) {
					postProcessingMat.EnableKeyword("DITHER_SLOW");
					postProcessingMat.DisableKeyword("DITHER_FAST");
					postProcessingMat.DisableKeyword("DITHER_TEX");
				} else if (ditherType == 1) {
					postProcessingMat.EnableKeyword("DITHER_FAST");
					postProcessingMat.DisableKeyword("DITHER_SLOW");
					postProcessingMat.DisableKeyword("DITHER_TEX");
				} else if (ditherType == 2) {
					postProcessingMat.EnableKeyword("DITHER_TEX");
					postProcessingMat.DisableKeyword("DITHER_SLOW");
					postProcessingMat.DisableKeyword("DITHER_FAST");
				}
				if (scanlines) {
					postProcessingMat.EnableKeyword("SCANLINES_ON");
				} else {
					postProcessingMat.DisableKeyword("SCANLINES_ON");
				}
			}
		}
	}

	public void SnapCamera() {
		Vector3 snapPos = transform.position;
		snapPos /= camInaccuracy;
		snapPos = new Vector3(Mathf.Round(snapPos.x), Mathf.Round(snapPos.y), Mathf.Round(snapPos.z));
		snapPos *= camInaccuracy;
		transform.position = snapPos;
	}

	// Draw a transparent red circle around the camera to show its
	// real position
	private void OnDrawGizmos() {
		if (snapCamera) {
			Gizmos.color = new Color(1, 0, 0, 0.5f);
			if(transform.parent != null)
				Gizmos.DrawSphere(transform.parent.position, 0.5f);
		}
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dst) {
		if (postProcessing) {
			if (customRes.x > 2 && customRes.y > 2) {
				// Renders scene to downscaled render texture using the post processing shader
				if (src != null)
					src.filterMode = FilterMode.Point;
				if (skipFrames < 1 || Time.frameCount % skipFrames == 0)
					Graphics.Blit(src, rt);
				Graphics.Blit(rt, dst, postProcessingMat);
			} else {
				customRes.x = 2;
				customRes.y = 2;
			}
		} else {
			// Renders scene to downscaled render texture
			if (src != null)
				src.filterMode = FilterMode.Point;
			if(skipFrames < 1 || Time.frameCount % skipFrames == 0)
				Graphics.Blit(src, rt);
			Graphics.Blit(rt, dst);
		}
	}

	private void OnDestroy() {
		Shader.EnableKeyword("AFFINE_MAPPING");
		Shader.SetGlobalFloat("_DrawDistance", 0);
		Shader.SetGlobalInt("_VertexSnappingDetail", 30);
		Shader.SetGlobalInt("_Offset", 2);
		Shader.SetGlobalFloat("_SubtractFade", 0);
		Shader.EnableKeyword("WORLD_SPACE_SNAPPING");
		Shader.SetGlobalFloat("_CamPos", 1);
		Shader.EnableKeyword("SHADOW_DEFAULT");
		Shader.DisableKeyword("SHADOW_PSX");
	}

	public void CheckForUpdates() {
		StartCoroutine("CheckForUpdate");
	}

	IEnumerator CheckForUpdate() {
		cfuStatus = "Checking for updates...";
		UnityWebRequest www = UnityWebRequest.Get("https://ckosmic.github.io/psfxredir.html");
		yield return www.SendWebRequest();

		if (www.isNetworkError || www.isHttpError) {
			Debug.Log(www.error);
		} else {
			www = UnityWebRequest.Get(www.downloadHandler.text);
			yield return www.SendWebRequest();

			if (www.isNetworkError || www.isHttpError) {
				Debug.Log(www.error);
			} else {
				System.Version onlineVer = new System.Version(www.downloadHandler.text);
				int comparison = onlineVer.CompareTo(version);

				if (comparison < 0) {
					cfuStatus = "PSXEffects v" + version.ToString() + " - version ahead?!";
				} else if (comparison == 0) {
					cfuStatus = "PSXEffects v" + version.ToString() + " - up to date.";
				} else {
					cfuStatus = "PSXEffects v" + version.ToString() + " - update available (click to update).";
				}
			}
		}
	}
}
