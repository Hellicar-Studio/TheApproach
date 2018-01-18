Shader "Realtime Planar Reflections/Height Atten" {
	Properties {
		_MainTex    ("Albedo", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0, 1)) = 0.5
		_Metallic   ("Metallic", Range(0, 1)) = 0
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		CGPROGRAM
		#pragma surface surf Standard keepalpha
		sampler2D _MainTex;
		half _Glossiness, _Metallic;
		struct Input
		{
			float2 uv_MainTex;
			float3 worldPos;
		};
		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 tc = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = tc.rgb;
			o.Alpha = 0.2;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
		}
		ENDCG
	}
	FallBack "Diffuse"
}