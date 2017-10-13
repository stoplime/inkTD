Shader ".Demo/Texture"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Main Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 texcoord : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 color = _Color;
				float4 textColor = tex2D(_MainTex, i.texcoord);
				return color*textColor;
			}
			ENDCG
		}
		Pass
		{
			Tags { "RenderType" = "Opaque" }
            CGPROGRAM
            #pragma surface surf Lambert vertex:vert
            struct Input {
                float2 uv_MainTex;
                float3 customColor;
            };
            void vert (inout appdata_full v, out Input o) {
                UNITY_INITIALIZE_OUTPUT(Input,o);
                o.customColor = abs(v.normal);
            }
            sampler2D _MainTex;
            void surf (Input IN, inout SurfaceOutput o) {
                o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
                o.Albedo *= IN.customColor;
            }
            ENDCG
		}
	}
}
