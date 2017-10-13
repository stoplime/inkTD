// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/WaterShader3" 
{
	Properties 
	{
		_ShoreColor("Shoreline Color", Color) = (1,1,1,1)
		//_DeepColor("Deep Color", Color) = (1,1,1,1)
		_ReflectColor("Reflection Color", Color) = (1,1,1,0.5)
		_Cube("Reflection Cubemap", Cube) = "_Skybox"
		_BumpMap("Normalmap", 2D) = "bump" {}
		_WaveDepth("Wave Amplitude", Range(0,2)) = 0.25
		_WaveSpeed("Wave Speed", Range(-200,200)) = 30
		_WaveFreq("Wave Frequency", Range(0,2)) = 0.5
		_WaveDirection("Wave Direction", Range(0,5)) = 0
		_YPos("Y Position Offset", Float) = 0
	}
	SubShader 
		{
			Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
			Cull Off
			LOD 300

			CGPROGRAM
			#pragma surface surf Lambert alpha vertex:vert

			sampler2D _BumpMap;
			samplerCUBE _Cube;
			fixed4 _ShoreColor;
			//fixed4 _DeepColor;
			//fixed4 _MidColor;
			fixed4 _ReflectColor;
			float _WaveDepth;
			float _WaveSpeed;
			float _WaveFreq;
			float _WaveDirection;
			float _YPos;

			struct Input
			{
				float2 uv_BumpMap;
				float3 worldRefl;
				float3 worldPos;
				float4 Color : COLOR0;
				//float4 midColor : COLOR1;
				//float4 bottomColor : COLOR2;
				INTERNAL_DATA
			};

			//Applying the sin wave motion to the mesh
			void vert(inout appdata_full v)
			{
				float4 wavePos = mul(unity_ObjectToWorld, v.vertex);
				float yOffset = (wavePos.x + (wavePos.z * _WaveDirection)) * _WaveFreq;
				wavePos.y = (sin(_Time * _WaveSpeed + yOffset) * _WaveDepth) + _YPos;
				v.vertex = mul(unity_WorldToObject, wavePos);
			}

			//Applying the visual effects
			void surf(Input IN, inout SurfaceOutput o)
			{
				o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
				float3 worldRefl = WorldReflectionVector(IN, o.Normal);
				//o.Albedo =  lerp(_ShoreColor, _DeepColor, IN.wolrdPos.z);
				o.Albedo = _ShoreColor;
				fixed4 reflcol = texCUBE(_Cube, worldRefl);
				o.Emission = reflcol.rgb * _ReflectColor.rgb;
				o.Alpha = _ShoreColor.a;
			}
			ENDCG
	}
	FallBack "Reflective/VertexLit"
}
