Shader "Unlit/UVDebug"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Resolution ("Resolution", Vector) = (10, 5, 10, 5)
		_Highlight ("Highlight", Range(0, 50)) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		Cull Off
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float4 color : COLOR;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.uv1 = v.uv1;
				o.color = v.color;
				return o;
			}
			
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float4 _Resolution;
			int _Highlight;

			fixed4 frag (v2f i) : SV_Target
			{
				int2 uvi = i.uv.xy * _Resolution.zw;
				int idx = uvi.x + uvi.y * _Resolution.z;
				if(idx == _Highlight)
					return float4(1,0,1,1);
				// return i.color;
				// return float4(i.uv1, 0, 1);
				return float4(i.uv, 0, 1);
			}
			ENDCG
		}
	}
}
