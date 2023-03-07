Shader "Unlit/Grass2"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_TessellationValue("value",Range(1,10)) =1
		_grassWidth("width",Range(0.001,1)) =1
		_grassHeigth("heigth",Range(0.001,5)) =1
		_Color("Color",color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		Cull off
		Pass
		{
			CGPROGRAM
			//细分
			#pragma hull hull
			#pragma domain domain
			#pragma vertex tessvert
			#pragma fragment frag
			#pragma geometry geom
			#include "UnityCG.cginc"
			#include "MyTessellaion.cginc"

			#define SPLIT 10
			sampler2D _MainTex;
			float _grassWidth;
			float _grassHeigth;
			float4 _Color;
			fixed2 randPos(fixed2 value){
				fixed2 pos = fixed2(dot(value, fixed2(127.1, 337.1)), dot(value, fixed2(269.5, 183.3)));
				pos = frac(sin(pos) * 43758.5453123);
				return pos;
			}
			fixed rand(float x)
			{
				return frac(sin(x));
			}
			// https://gist.github.com/keijiro/ee439d5e7388f3aafc5296005c8c3f33
			//旋转矩阵
			float3x3 AngleAxis3x3(float angle, float3 axis)
			{
				float c, s;
				sincos(angle, s, c);

				float t = 1 - c;
				float x = axis.x;
				float y = axis.y;
				float z = axis.z;

				return float3x3(
					t * x * x + c, t * x * y - s * z, t * x * z + s * y,
					t * x * y + s * z, t * y * y + c, t * y * z - s * x,
					t * x * z - s * y, t * y * z + s * x, t * z * z + c
					);
			}
			struct g2f
			{
				float4 pos:SV_POSITION;
				float2 uv:TEXCOORD0;
				float4 col:COLOR0;
			};

			void Grass(float3 center,inout TriangleStream<g2f> triStream)
			{
				float width = _grassWidth;

				float height =  _grassHeigth*1.0/SPLIT + rand((center.x+center.z)*45.4535)*0.05*_grassHeigth;

				float angle = radians(rand((center.x+center.z)*15.23545)*360);
				float Windstrength = radians( sin(center.x+center.z+_Time.z)*20 );
				for(int i=0;i<SPLIT;i++)
				{
					float bendStrength =pow(i*1.0/(SPLIT-1),2)*0.2;

					g2f o = (g2f)0;
					float3 localPos = float3(width*0.5,i*height,bendStrength);
					localPos = mul(AngleAxis3x3(angle,float3(0,1,0)),localPos);
					localPos = mul( AngleAxis3x3(Windstrength,float3(1,0,0)),localPos);
					o.pos = UnityObjectToClipPos( localPos+ center);
					o.uv = float2(0,i*1.0 / (SPLIT - 1));
					triStream.Append(o);

					g2f o2 = (g2f)0;
					localPos = float3(-width*0.5,i*height,bendStrength);
					localPos = mul(AngleAxis3x3(angle,float3(0,1,0)),localPos);
					localPos = mul( AngleAxis3x3(Windstrength,float3(1,0,0)),localPos);
					o2.pos = UnityObjectToClipPos(localPos+center);
					o2.uv = float2(1,i*1.0 / (SPLIT - 1));
					triStream.Append(o2);
				}
				triStream.RestartStrip();
			}

			[maxvertexcount(SPLIT*2)]
            void geom(point VertexOutput p[1], inout TriangleStream<g2f> triStream)
			{
				float3 center = p[0].pos ;
				center.xz +=randPos(p[0].pos.xz);
				Grass(center,triStream);
			}

			fixed4 frag (g2f i) : SV_Target
			{
				float4 col = tex2D(_MainTex,i.uv);
				clip(col.a -0.001f);
				return col*_Color;
			}
			ENDCG
		}
	}
}
