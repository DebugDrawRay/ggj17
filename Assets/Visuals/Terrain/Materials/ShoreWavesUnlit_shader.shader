﻿Shader "Unlit/ShoreWavesUnlit_shader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_EbSpeed("Eb Speed", Range(-2,2)) = -0.5
		_Dummy("Dummy", Range(0,1)) = 0
	}
	SubShader
	{
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert alpha
			#pragma fragment frag alpha
			// make fog work
			#pragma multi_compile_fog alpha
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _EbSpeed;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed2 pannedUVs = { i.uv.x,i.uv.y + _SinTime[3] * _EbSpeed };
				fixed4 col = tex2D(_MainTex, pannedUVs);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				col.a = 0;
				return col;
			}
			ENDCG
		}
	}
}
