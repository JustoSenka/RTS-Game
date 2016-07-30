Shader "Custom/TeamedUnitColor" {
	Properties {
		_Color("Main Color", Color) = (0,1,0,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap("Normal Map", 2D) = "bump" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpMap;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
		};

		fixed4 _Color;
		half _Glossiness;
		half _Metallic;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 tex = tex2D (_MainTex, IN.uv_MainTex);
			tex = lerp(_Color, tex, tex.a);

			o.Albedo = tex.rgb;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = tex.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
