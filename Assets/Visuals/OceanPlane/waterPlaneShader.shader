Shader "Custom/waterPlaneShader" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MasksTex("Masks", 2D) = "black" {}
		_DispTex("Displacement Texture", 2D) = "black" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_Amount("Displacement Strength", Range(0,40)) = 0.0
		_DispTiling("Displacement Tiling", Range(0,20)) = 0.5
		_EbSpeed("Eb/Flow Speed", Range(0,5)) = 1.0

		_BaseColor("Base Water Color", Color) = (0,0.2,0.8,1)
		_FoamColor("Foam Color", Color) = (0.8,0.8,0.95)
		_FoamStrength("Foam Strength", Range(0,1)) = 0.5

		_FoamTilingLarge("Large Foam Tiling", Range(0,30)) = 1.0
		_FoamTilingSmall("Small Foam Tiling", Range(0,30)) = 1.0
		_FoamSpeedLarge("Large Foam Speed", Range(-3,3)) = 0
		_FoamSpeedSmall("Small Foam Speed", Range(-1,1)) = 0
	}
	SubShader{

		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert // Added vertex:vert
		// #pragma surface surf Lambert vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MasksTex;
		sampler2D _DispTex;

		struct Input {
			float2 uv_MasksTex;
			float2 uv_DispTex;
		};

		float _Amount;
		float _DispTiling;
		float _EbSpeed;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed4 _BaseColor;
		fixed4 _FoamColor;
		float _FoamStrength;

		float _FoamTilingLarge;
		float _FoamTilingSmall;
		float _FoamSpeedLarge;
		float _FoamSpeedSmall;

		void vert(inout appdata_full v) {
			float d = tex2Dlod(_DispTex, float4((v.texcoord.xy + ((_Time[0]) * _EbSpeed)*_DispTiling), 0, 0)).b * _Amount;
			v.vertex.xyz += v.normal * (d);
		}

		void surf(Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			//fixed4 c = tex2D(_MasksTex, IN.uv_MasksTex) * _Color;
			float foamAlpha = clamp(0,1,(tex2D(_MasksTex, IN.uv_MasksTex*_FoamTilingLarge+(_SinTime[0]*_FoamSpeedLarge)).r + tex2D(_MasksTex, IN.uv_MasksTex*_FoamTilingSmall+(_SinTime[0]*_FoamSpeedSmall)).g));
			//o.Albedo = lerp(_BaseColor, _FoamColor, 1);
			half4 c = lerp(_BaseColor, _FoamColor, foamAlpha);
			c = lerp(_BaseColor,_FoamColor,foamAlpha*_FoamStrength);
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			//o.Smoothness = _Glossiness;
			o.Smoothness = lerp(_Glossiness,1, foamAlpha);
			o.Alpha = 1.0;
	}
	ENDCG
	}
		FallBack "Diffuse"
}




/*Shader "Example/Normal Extrusion" {
Properties{
_MainTex("Texture", 2D) = "white" {}
_Amount("Extrusion Amount", Range(-1,1)) = 0.5
}
SubShader{
Tags{ "RenderType" = "Opaque" }
CGPROGRAM
#pragma surface surf Lambert vertex:vert
struct Input {
float2 uv_MainTex;
};
float _Amount;
void vert(inout appdata_full v) {
v.vertex.xyz += v.normal * _Amount;
}
sampler2D _MainTex;
void surf(Input IN, inout SurfaceOutput o) {
o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
}
ENDCG
}
Fallback "Diffuse"
}*/
