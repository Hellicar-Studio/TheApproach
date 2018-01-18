using UnityEngine;
using System.Collections;

[ExecuteInEditMode]   // available in editor mode
public class PlanarReflection : MonoBehaviour
{
	public Camera m_Camera;
	[Range(1, 1024)] public int m_TextureSize = 512;
	public float m_ClipPlaneOffset = 0.07f;
	[Range(0f, 1f)] public float m_ReflectionStrength = 0f;
	public LayerMask m_ReflectLayers = -1;
	[Header("Blur")]
	[Range(0, 8)] public int m_Iterations = 1;
	[Range(0f, 1f)]	public float m_Interpolation = 1f;
	public Material m_BlurMat;
	[Header("Bump")]
	public Texture2D m_BumpTex;
	[Range(0.1f, 1f)] public float m_BumpStrength = 0.5f;
	[Range(1f, 16f)] public float m_BumpTexScale = 1f;
	[Header("Mask")]
	public Texture2D m_MaskTex;
	
	private Camera m_ReflectionCamera = null;
	private RenderTexture m_RTReflectionColor = null;
	private int m_OldReflectionTextureSize = 0;
	private static bool s_InsideRendering = false;

	void OnWillRenderObject ()
	{
		Renderer rd = GetComponent<Renderer> ();
		if (!enabled || !rd || !rd.sharedMaterial || !rd.enabled)
			return;
		Camera cam = m_Camera;
		if (cam == null)
			return;
		if (s_InsideRendering)   // safeguard from recursive reflections.
			return;

		s_InsideRendering = true;
		CreateMirrorObjects ();
        UpdateCameraModes (cam, m_ReflectionCamera);
		
		// plane reflection render texture building
		Vector3 pos = transform.position;
		Vector3 normal = transform.up;
		float d = -Vector3.Dot (normal, pos) - m_ClipPlaneOffset;
		Vector4 reflectionPlane = new Vector4 (normal.x, normal.y, normal.z, d);

		Matrix4x4 reflection = Matrix4x4.zero;
		CalculateReflectionMatrix (ref reflection, reflectionPlane);
		Vector3 oldpos = cam.transform.position;
		Vector3 newpos = reflection.MultiplyPoint (oldpos);
		m_ReflectionCamera.worldToCameraMatrix = cam.worldToCameraMatrix * reflection;

		// setup oblique projection matrix to clip everything below/above it for free.
		Vector4 clipPlane = CameraSpacePlane (m_ReflectionCamera, pos, normal, 1f);
		Matrix4x4 projection = cam.projectionMatrix;
		CalculateObliqueMatrix (ref projection, clipPlane);
		m_ReflectionCamera.projectionMatrix = projection;
        
		m_ReflectionCamera.cullingMask = ~(1 << 4) & m_ReflectLayers.value;
		m_ReflectionCamera.targetTexture = m_RTReflectionColor;
		GL.invertCulling = true;
		m_ReflectionCamera.transform.position = newpos;
		Vector3 euler = cam.transform.eulerAngles;
		m_ReflectionCamera.transform.eulerAngles = new Vector3 (0, euler.y, euler.z);
		m_ReflectionCamera.Render ();
		m_ReflectionCamera.transform.position = oldpos;
		GL.invertCulling = false;
		
		// blur if necessary
		if (m_Iterations != 0)
		{
			RenderTexture rt = RenderTexture.GetTemporary (m_TextureSize, m_TextureSize, 0, m_RTReflectionColor.format);
			for (int i = 0; i < m_Iterations; i++)
			{
				float radius = (float)i * m_Interpolation + m_Interpolation;
				m_BlurMat.SetFloat ("_Radius", radius);
				Graphics.Blit (m_RTReflectionColor, rt, m_BlurMat, 0);
				Graphics.Blit (rt, m_RTReflectionColor, m_BlurMat, 1);
			}
			RenderTexture.ReleaseTemporary (rt);
		}
		
		// setup reflection map
		Material[] materials;
		if (Application.isEditor)
			materials = rd.sharedMaterials;
		else
			materials = rd.materials;
		foreach (Material mat in materials)
		{
			if (m_BumpTex)   // enable bump if necessary
			{
				mat.EnableKeyword ("RPR_BUMP_REFLECTION");
				mat.SetFloat ("_BumpStrength", m_BumpStrength);
				mat.SetTexture ("_BumpTex", m_BumpTex);
				mat.SetTextureScale ("_BumpTex", new Vector2 (m_BumpTexScale, m_BumpTexScale));
			}
			else
			{
				mat.DisableKeyword ("RPR_BUMP_REFLECTION");
			}
			mat.SetTexture ("_MaskTex", m_MaskTex);
			mat.SetTexture ("_ReflectionTex", m_RTReflectionColor);
			mat.SetFloat ("_ReflectionStrength", 1f - m_ReflectionStrength);
		}
		s_InsideRendering = false;
	}
	void OnDisable ()
	{
		if (m_RTReflectionColor) 
		{
			DestroyImmediate (m_RTReflectionColor);
			m_RTReflectionColor = null;
			DestroyImmediate (m_ReflectionCamera.gameObject);
			m_ReflectionCamera = null;
		}
	}
	void UpdateCameraModes (Camera src, Camera dest)
	{
		if (dest == null)
			return;

		dest.clearFlags = src.clearFlags;
		dest.backgroundColor = src.backgroundColor;        
		if (src.clearFlags == CameraClearFlags.Skybox)
		{
			Skybox sky = src.GetComponent<Skybox> ();
			Skybox mysky = dest.GetComponent<Skybox> ();
			if (!sky || !sky.material)
			{
				mysky.enabled = false;
			}
			else
			{
				mysky.enabled = true;
				mysky.material = sky.material;
			}
		}
		dest.farClipPlane = src.farClipPlane;
		dest.nearClipPlane = src.nearClipPlane;
		dest.orthographic = src.orthographic;
		dest.fieldOfView = src.fieldOfView;
		dest.aspect = src.aspect;
		dest.orthographicSize = src.orthographicSize;
	}
	void CreateMirrorObjects ()
	{
		// camera for reflection
		if (m_ReflectionCamera == null)
		{
			GameObject go = new GameObject ("RPR_Camera_" + GetInstanceID (), typeof (Camera), typeof (Skybox));
			m_ReflectionCamera = go.GetComponent<Camera> ();
			m_ReflectionCamera.enabled = false;
			m_ReflectionCamera.transform.position = transform.position;
			m_ReflectionCamera.transform.rotation = transform.rotation;
			go.hideFlags = HideFlags.DontSave;
		}
		// reflection render texture
		if (null == m_RTReflectionColor || m_OldReflectionTextureSize != m_TextureSize)
		{
			if (m_RTReflectionColor)
				DestroyImmediate (m_RTReflectionColor);
			m_RTReflectionColor = new RenderTexture (m_TextureSize, m_TextureSize, 16);
			m_RTReflectionColor.name = "RPR_RT_" + GetInstanceID();
			m_RTReflectionColor.isPowerOfTwo = true;
			m_RTReflectionColor.hideFlags = HideFlags.DontSave;
			m_OldReflectionTextureSize = m_TextureSize;
		}
	}    
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	Vector4 CameraSpacePlane (Camera cam, Vector3 pos, Vector3 normal, float sideSign)
	{
		Vector3 offsetPos = pos + normal * m_ClipPlaneOffset;
		Matrix4x4 m = cam.worldToCameraMatrix;
		Vector3 cpos = m.MultiplyPoint (offsetPos);
		Vector3 cnormal = m.MultiplyVector (normal).normalized * sideSign;
		return new Vector4 (cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot (cpos,cnormal));
	}
	static float sgn (float a)
	{
		if (a > 0f) return 1f;
		if (a < 0f) return -1f;
		return 0f;
	}
	static void CalculateObliqueMatrix (ref Matrix4x4 projection, Vector4 clipPlane)
	{
		Vector4 q = projection.inverse * new Vector4 (sgn (clipPlane.x), sgn (clipPlane.y), 1f, 1f);
		Vector4 c = clipPlane * (2f / (Vector4.Dot (clipPlane, q)));
		projection[2] = c.x - projection[3];
		projection[6] = c.y - projection[7];
		projection[10] = c.z - projection[11];
		projection[14] = c.w - projection[15];
	}
	static void CalculateReflectionMatrix (ref Matrix4x4 reflectionMat, Vector4 plane)
	{
		reflectionMat.m00 = (1f - 2f * plane[0] * plane[0]);
		reflectionMat.m01 = (   - 2f * plane[0] * plane[1]);
		reflectionMat.m02 = (   - 2f * plane[0] * plane[2]);
		reflectionMat.m03 = (   - 2f * plane[3] * plane[0]);

		reflectionMat.m10 = (   - 2f * plane[1] * plane[0]);
		reflectionMat.m11 = (1f - 2f * plane[1] * plane[1]);
		reflectionMat.m12 = (   - 2f * plane[1] * plane[2]);
		reflectionMat.m13 = (   - 2f * plane[3] * plane[1]);

		reflectionMat.m20 = (   - 2f * plane[2] * plane[0]);
		reflectionMat.m21 = (   - 2f * plane[2] * plane[1]);
		reflectionMat.m22 = (1f - 2f * plane[2] * plane[2]);
		reflectionMat.m23 = (   - 2f * plane[3] * plane[2]);

		reflectionMat.m30 = 0f;
		reflectionMat.m31 = 0f;
		reflectionMat.m32 = 0f;
		reflectionMat.m33 = 1f;
	}
}
