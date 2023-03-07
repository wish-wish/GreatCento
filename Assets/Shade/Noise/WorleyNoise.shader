Shader "Unlit/WorleyNoise"
{
	Properties
	{
		_Density("_Density", Float) = 6
	}
    SubShader
    {
        Tags { "RenderType"="Opaque" }
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct a2v
			{
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;         
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			float _Density;

			v2f vert(a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord.xy;
				return o;
			}
		
			fixed2 randPos(fixed2 value)
			{
				fixed2 pos = fixed2(dot(value, fixed2(127.1, 337.1)), dot(value, fixed2(269.5, 183.3)));
				pos = frac(sin(pos) * 43758.5453123);
				return pos;
			}

			float worleyNoise(float2 uv)
			{
				fixed2 index = floor(uv);
				float2 pos = frac(uv);
				float d = 1.5;
				for(int i = -1; i < 2; i++)
					for (int j = -1; j < 2; j++)
					{
						fixed2 p = randPos(index + fixed2(i, j));
						float dist = length(p + fixed2(i, j) - pos);
						d = min(dist, d);
					}
				return d;
			}
		
			float2 worleyNoise2(float2 uv)//泰森多边形
			{
				fixed2 index = floor(uv);
				float2 pos = frac(uv);
				float2 d = float2(1.5, 1.5);
				for (int i = -1; i < 2; i++)
					for (int j = -1; j < 2; j++)
					{
						fixed2 p = randPos(index + fixed2(i, j));
						float dist = length(p + fixed2(i, j) - pos);
						if (dist < d.x)
						{
							d.y = d.x;
							d.x = dist;
						}
						else
							d.y = min(dist, d.y);
					}
				return d;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				i.uv *= _Density;
				float2 d = worleyNoise2(i.uv);
				//return worleyNoise(i.uv);
				return d.y - d.x;
			}
			ENDCG
		}      
    }
    FallBack "Diffuse"
}
