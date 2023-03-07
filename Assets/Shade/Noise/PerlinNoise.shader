Shader "Unlit/PerlinNoise"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Fade("Fade", Range(-1, 1)) = 1
		_Density("Density", Float) = 6
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

			fixed4 _Color;
			fixed _Fade;
			float _Density;

			v2f vert(a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord.xy;
				return o;
			}
		
			fixed2 randVec(fixed2 value)
			{
				fixed2 vec = fixed2(dot(value, fixed2(127.1, 337.1)), dot(value, fixed2(269.5, 183.3)));
				vec = -1 + 2 * frac(sin(vec) * 43758.5453123);
				return vec;
			}

			float perlinNoise(float2 uv)
			{
				float a, b, c, d;
				float x0 = floor(uv.x); 
				float x1 = ceil(uv.x); 
				float y0 = floor(uv.y); 
				float y1 = ceil(uv.y); 
				fixed2 pos = frac(uv);
				a = dot(randVec(fixed2(x0, y0)), pos - fixed2(0, 0));
				b = dot(randVec(fixed2(x0, y1)), pos - fixed2(0, 1));
				c = dot(randVec(fixed2(x1, y1)), pos - fixed2(1, 1));
				d = dot(randVec(fixed2(x1, y0)), pos - fixed2(1, 0));
				float2 st = 6 * pow(pos, 5) - 15 * pow(pos, 4) + 10 * pow(pos, 3);
				a = lerp(a, d, st.x);
				b = lerp(b, c, st.x);
				a = lerp(a, b, st.y);
				return a;
			}

			float fbm(float2 uv)
			{
				float f = 0;
				float a = 1;
				for(int i = 0; i < 3; i++)
				{
					f += a * perlinNoise(uv);
					uv *= 2;
					a /= 2;
				}
				return f;
			}
		
			fixed4 frag(v2f i) : SV_Target
			{
				i.uv *= _Density;
				clip(fbm(i.uv) - _Fade);
				return _Color;
			}
			ENDCG
		}      
    }
    FallBack "Diffuse"
}

