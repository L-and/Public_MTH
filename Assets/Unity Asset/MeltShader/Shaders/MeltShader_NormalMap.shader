Shader "MeltShader/Built in (Normal Map)" {
	Properties {
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_BumpMap("Normal Map", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_SpreadPower("Spread Power", Float) = 1.0
		_MeltColor("Melt Color", Color) = (0.5,0.3,0.1,1)
		_MeltRate("Melt Rate", Range(0, 1)) = 0.5
		_TiltAngle("Tilt Angle", Float) = 10.0
		_ObjectHeight("Object Height", Float) = 100.0
		_ObjectMeltHeight("Object Melt Height", Float) = 2.0
		_ObjectBasePos("Object Base Position", Vector) = (0,0,0,0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpMap;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float4 packedData;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		half _MeltRate;
		half _ObjectMeltHeight;
		half _SpreadPower;
		fixed4 _MeltColor;
		half _ObjectHeight;
		float4 _ObjectBasePos;
		half _TiltAngle;

		void vert(inout appdata_full v, out Input data)
		{
			UNITY_INITIALIZE_OUTPUT(Input, data);
			float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
			//float meltRange = lerp(0.0, _ObjectMeltHeight, saturate(_MeltRate * 10.0));
			float meltRange = lerp(0.0, _ObjectHeight * 0.2, _MeltRate);
			float height = lerp(0.0, _ObjectHeight, _MeltRate);
			float verticalRate = saturate((worldPos.y - _ObjectBasePos.y) / _ObjectHeight);
			float minY = verticalRate * _ObjectMeltHeight + _ObjectBasePos.y;
			float meltRate = 1.0 - saturate((worldPos.y - (height + _ObjectBasePos.y)) / meltRange);
			float spreadPower = saturate((height + meltRange - (worldPos.y - _ObjectBasePos.y)) / (_ObjectHeight + meltRange));
			spreadPower = spreadPower * _SpreadPower;
			float tiltAngle = _TiltAngle * min(verticalRate, _MeltRate);
			float tiltSin = sin(tiltAngle / 180.0 * 3.141592);
			float tiltCos = cos(tiltAngle / 180.0 * 3.141592);
			float3x3 tiltMatrix = float3x3(float3(tiltCos, -tiltSin, 0.0), float3(tiltSin, tiltCos, 0.0), float3(0.0, 0.0, 1.0));
			worldPos.xyz -= _ObjectBasePos.xyz;
			worldPos.xyz = mul(tiltMatrix, worldPos.xyz);
			worldPos.xyz += _ObjectBasePos.xyz;
			worldPos.xz -= _ObjectBasePos.xz;
			worldPos.xz *= spreadPower * meltRate * meltRate + 1.0;
			worldPos.xz += _ObjectBasePos.xz;
			worldPos.y = max(minY, worldPos.y - height);
			v.vertex = mul(unity_WorldToObject, worldPos);

			data.packedData.z = meltRate;
			data.packedData.xw = UnityObjectToClipPos(v.vertex).xw;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			c.rgb = lerp(c.rgb, _MeltColor.rgb, saturate(IN.packedData.z * 2.0));
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		}
		ENDCG
	}
	FallBack "Diffuse"
}
