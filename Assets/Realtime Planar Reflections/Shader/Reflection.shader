// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Realtime Planar Reflections/Reflection" {
	Properties {
		_MainTex            ("Albedo", 2D) = "white" {}
		_ReflectionTex      ("Reflection", 2D) = "black" {}
		_ReflectionTint     ("Reflection Tint", Color) = (1, 1, 1, 1)
		_ReflectionStrength ("Reflection Strength", Range(0,1)) = 1
		_BumpTex            ("Bump", 2D) = "bump" {}
		_BumpStrength       ("Bump Strength", Float) = 0.5
		_MaskTex            ("Mask", 2D) = "black" {}
	}
	SubShader {
		Tags { "RenderType" = "Opaque" "LightMode" = "ForwardBase" }
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ RPR_BUMP_REFLECTION
			#pragma multi_compile_fwdbase
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			struct v2f
			{
				float4 pos : POSITION;
				float4 posscr : TEXCOORD0;
				float2 uvMain : TEXCOORD1;
				float4 uvMaskBump : TEXCOORD2;
				float3 nor : TEXCOORD3;
				float3 lit : TEXCOORD4;
				LIGHTING_COORDS(5, 6)
			};
			sampler2D _MainTex, _ReflectionTex, _MaskTex, _BumpTex;
			float4 _MainTex_ST, _MaskTex_ST, _BumpTex_ST;
			fixed4 _ReflectionTint;
			fixed _BumpStrength, _ReflectionStrength;
			v2f vert (appdata_tan v)
			{
				TANGENT_SPACE_ROTATION;
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.posscr = ComputeScreenPos(o.pos);
				o.uvMain = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uvMaskBump.xy = TRANSFORM_TEX(v.texcoord, _MaskTex);
				o.uvMaskBump.zw = TRANSFORM_TEX(v.texcoord, _BumpTex);
				o.nor = mul(rotation, SCALED_NORMAL);
				o.lit = mul(rotation, ObjSpaceLightDir(v.vertex));
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				return o;
			}
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 albedo = tex2D(_MainTex, i.uvMain);
				float3 N = normalize(i.nor);
				float3 L = normalize(i.lit);
				albedo.rgb *= (0.5 * dot(N, L) + 0.5) * _LightColor0.rgb;
				
				fixed4 mask = tex2D(_MaskTex, i.uvMaskBump.xy);
				float4 scrpos = i.posscr;
#ifdef RPR_BUMP_REFLECTION
				float3 bump = UnpackNormal(tex2D(_BumpTex, i.uvMaskBump.zw)).xyz * _BumpStrength;
				scrpos.xyz += bump.xyz;
#endif
				half4 refl = tex2Dproj(_ReflectionTex, UNITY_PROJ_COORD(scrpos)) * _ReflectionTint;
				albedo.rgb = lerp(refl.rgb, albedo.rgb, _ReflectionStrength);
				fixed3 c = lerp(albedo.rgb, refl.rgb, mask.r) * _LightColor0 * LIGHT_ATTENUATION(i);
				return fixed4(c, 1);
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}