Shader "Custom/PlayerWaveShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

		_NormalMap("Normal Map", 2D) = "bump" {}

		_BaseColor("Base Water Color", Color) = (0,0.2,0.8,1)
		_FoamTex("Foam Texture", 2D) = "white" {}
		_FoamColor("Foam Color", Color) = (0.8,0.8,0.95)
		_FoamStrength("Foam Strength", Range(0,1)) = 0.5
		_FoamSpeed("Foam Speed", Range(-3,3)) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _FoamTex;
		sampler2D _NormalMap;

		struct Input {
			float2 uv_MainTex;
			float2 uv_FoamTex;
			float2 uv_NormalMap;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		fixed4 _BaseColor;
		fixed4 _FoamColor;
		float _FoamStrength;
		float _FoamSpeed;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			// Get offset foam UVs
			float2 pannedFoamUVs = { IN.uv_FoamTex[0], IN.uv_FoamTex[1] + (_Time[1] * _FoamSpeed) };
			fixed4 foamc = tex2D(_FoamTex, pannedFoamUVs);
			// lerp between c and foam texture based on alpha of foam texture
			c = lerp(c,foamc,foamc.a*_FoamStrength);
			//c = foamc;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
			o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap));
		}
		ENDCG
	}
	FallBack "Diffuse"
}
