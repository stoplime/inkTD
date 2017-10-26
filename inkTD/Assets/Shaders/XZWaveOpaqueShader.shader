// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/XZWaveOpaqueShader"
{
	Properties
	{
		_Color("Primary Color", Color) = (1,1,1,1)
		_Color2("Secondary Color", Color) = (1,1,1,1)
		_Value("Value", float) = 0
		_WaveLength("Wave Length", float) = 0.1
		_Speed("Speed", float) = 8
		_Amplitude("Amplitude", float) = 0.001
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Tags{"RenderType" = "Opaque" }
		Tags{ "LightMode" = "ForwardBase" }
		LOD 100

	Pass
	{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"

		struct appdata
		{
			float4 vertex : POSITION;
			float2 texcoord : TEXCOORD0;
		};

		struct v2f
		{
			float4 pos : SV_POSITION;
			fixed4 diffuse : COLOR0; //For diffuse lighting
			float2 texcoord : TEXCOORD0;
		};

		sampler2D _MainTex;
		float4 _MainTex_ST;
		fixed4 _Color;
		fixed4 _Color2;
		float _Value;
		float _Speed;
		float _WaveLength;
		float _Amplitude;

		v2f vert(appdata_base  v)
		{
			v2f o;
			float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			v.vertex.x += sin((_Time * _Speed + v.vertex.y + (worldPos.x + worldPos.z)) * (1 / _WaveLength)) / (1 / _Amplitude);
			v.vertex.z += sin((_Time * _Speed - v.vertex.y * 2 + (worldPos.x + worldPos.z)) * (1 / _WaveLength)) / (1 / _Amplitude);

			o.pos = UnityObjectToClipPos(v.vertex);
			o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

			//Getting the vertex normal in world space
			half3 worldNormal = UnityObjectToWorldNormal(v.normal);
			half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
			o.diffuse = nl * _LightColor0; //Applying light color

			//Adding illumination from ambient/light probes:
			o.diffuse.rgb += ShadeSH9(half4(worldNormal, 1));

			return o;
		}

		fixed4 frag(v2f i) : SV_Target
		{
			float4 textColor;

			if (i.texcoord.y > _Value)
			{
				textColor = tex2D(_MainTex, i.texcoord) * _Color;
			}
			else
			{
				textColor = tex2D(_MainTex, i.texcoord) * _Color2;

			}

			return textColor * i.diffuse;
			}
			ENDCG
		}



			UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
			//PAsses go here
	}
}