Shader "Custom/CamBlood" {
	Properties{
		_Color("Color", Color) = (1,0,0,1)
		_Fade("Fade",Range(0,1)) = 0.5
		_NoisePos("NoisePos", Vector) = (0,0,0,0)
		_NoiseOffsets("Noise",2D) = ""
		_NoiseSize("NoiseSize", Float) = 0.5
	}
	SubShader{

		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		
		Pass{
			ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off Lighting Off ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 vertex:SV_POSITION;
				float3 pos:TEXCOORD0;
				float3 uv:TEXCOORD1;
				fixed4 color : COLOR;
			};

			struct a2v
			{
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;  
				fixed4 color : COLOR;       
			};
			
			fixed4 _Color;
			fixed _Fade;
			sampler2D _NoiseOffsets;
			float4 _NoisePos;
			half _NoiseSize;

			float noise(float3 x)
			{
				x *= 4.0;
				float3 p = floor(x);
				float3 f = frac(x);
				f = f*f*(3.0 - 2.0*f);
				float2 uv = (p.xy + float2(37.0, 17.0)*p.z) + f.xy;
				float2 rg = tex2D(_NoiseOffsets, (uv + 0.5) / 256.0).yx;
				return lerp(rg.x, rg.y, f.z);
			}

			float noise_sum(float3 p)
			{
				float f = 0.0;
				f += (1.0000 * noise(p)); p = 2.0 * p;
				f += (0.5000 * noise(p)); p = 2.0 * p;
				f += (0.2500 * noise(p)); p = 2.0 * p;
				f += (0.1250 * noise(p)); p = 2.0 * p;
				
				//f += 0.06255 * noise(p); 
				return f;
			}


			v2f vert(a2v v)
			{
				v2f o;
				o.pos = v.vertex;
				o.vertex = fixed4(v.vertex.x * 2, v.vertex.y * 2, 0, 1);
				o.uv = v.texcoord;
				o.color = v.color;
				return o;
			}

			fixed4 frag(v2f i) :SV_Target
			{
				fixed4 c;
				c = _Color * noise_sum(i.pos * _NoiseSize + _NoisePos);
				if (c.a <= _Fade)
				{
					c.a = 0;
				}
				return c;
			}
			ENDCG
		}
	}
}
