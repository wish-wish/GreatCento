Shader "Unlit/ABlood"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile _ WIPE
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 scrPos : TEXCOORD0;
			};

			sampler2D _MainTex;
			sampler2D _SrcTex;
			sampler2D _WipeCanvasTex;

			uniform half _MoreRainAmount;

			float3 N13(float p) {
				float3 p3 = frac(float3(p,p,p) * float3(.1031,.11369,.13787));
				p3 += dot(p3, p3.yzx + 19.19);
				return frac(float3((p3.x + p3.y)*p3.z, (p3.x+p3.z)*p3.y, (p3.y+p3.z)*p3.x));
			}
			float4 N14(float t) {
				return frac(sin(t*float4(123., 1024., 1456., 264.))*float4(6547., 345., 8799., 1564.));
			}
			float N(float t) {
				return frac(sin(t*12345.564)*7658.76);
			}
			float Saw(float b, float t) {
				return smoothstep(0., b, t)*smoothstep(1., b, t);
			}
			float2 DropLayer2(float2 uv, float t) {
				float2 UV = uv;
				
				uv.y += t*0.75;
				float2 a = float2(6., 1.);
				float2 grid = a*2.;
				float2 id = floor(uv*grid);
				
				float colShift = N(id.x); 
				uv.y += colShift;
				
				id = floor(uv*grid);
				float3 n = N13(id.x*35.2+id.y*2376.1);
				float2 st = frac(uv*grid)-float2(.5, 0);
				
				float x = n.x-.5;
				
				float y = UV.y*20.;
				float wiggle = sin(y+sin(y));
				x += wiggle*(.5-abs(x))*(n.z-.5);
				x *= .7;
				float ti = frac(t+n.z);
				y = (Saw(.85, ti)-.5)*.9+.5;
				float2 p = float2(x, y);
				
				float d = length((st-p)*a.yx);
				
				float mainDrop = smoothstep(.4, .0, d);
				
				float r = sqrt(smoothstep(1., y, st.y));
				float cd = abs(st.x-x);
				float trail = smoothstep(.23*r, .15*r*r, cd);
				float trailFront = smoothstep(-.02, .02, st.y-y);
				trail *= trailFront*r*r;
				
				y = UV.y;
				float trail2 = smoothstep(.2*r, .0, cd);
				float droplets = max(0., (sin(y*(1.-y)*120.)-st.y))*trail2*trailFront*n.z;
				y = frac(y*10.)+(st.y-.5);
				float dd = length(st-float2(x, y));
				droplets = smoothstep(.3, 0., dd);
				float m = mainDrop+droplets*r*trailFront; 
				
				return float2(m, trail);
			}
			float StaticDrops(float2 uv, float t) {
				uv *= 40.;
				
				float2 id = floor(uv);
				uv = frac(uv)-.5;
				float3 n = N13(id.x*107.45+id.y*3543.654);
				float2 p = (n.xy-.5)*.7;
				float d = length(uv-p);
				
				float fade = Saw(.025, frac(t+n.z));
				float c = smoothstep(.3, 0., d)*frac(n.z*10.)*fade;
				return c;
			}
			float2 Drops(float2 uv, float t, float l0, float l1, float l2) {
				float s = StaticDrops(uv, t)*l0; 
				float2 m1 = DropLayer2(uv, t)*l1;
				float2 m2 = DropLayer2(uv*1.85, t)*l2;
				
				float c = s+m1.x+m2.x;
				c = smoothstep(.3, 1., c);
				
				return float2(c, max(m1.y*l0, m2.y*l1));
			}

			float2 DropsDynamic(float2 uv, float t, float l1, float l2)
			{
				float2 m1 = DropLayer2(uv, t)*l1;
				float2 m2 = DropLayer2(uv*1.75, t)*l2;
				
				float c = m1.x+m2.x;
				c = smoothstep(.4, 1., c);
				
				return float2(c, max(0, m2.y*l1));
			}

			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.scrPos = ComputeScreenPos(o.pos);
				return o;
			}

			fixed4 frag (v2f _iParam) : SV_Target
			{
				float2 fragCoord = (_iParam.scrPos.xy / _iParam.scrPos.w) * _ScreenParams.xy;
				float4 fragColor = 0;
        		
				float2 uv = (fragCoord.xy - .5*_ScreenParams.xy) / _ScreenParams.y;
				float2 UV = fragCoord.xy / _ScreenParams.xy;
				float3 M = 2;
				float T = _Time.y + M.x*2.;

				float t = T*(.2+0.1*_MoreRainAmount);

				float rainAmount = M.y;

				uv *= 0.5;

				float staticDrops = smoothstep(-.5, 1., rainAmount)*2.;
				float layer1 = smoothstep(.25, .75, rainAmount);
				float layer2 = smoothstep(.0, .5, rainAmount);

				float2 n = float2(0, 0);
				float2 c = Drops(uv, t, staticDrops, layer1, layer2);
				float2 e = float2(.001, 0.);
				float cx = Drops(uv + e, t, staticDrops, layer1, layer2).x;
				float cy = Drops(uv + e.yx, t, staticDrops, layer1, layer2).x;
				n += float2(cx - c.x, cy - c.x);
				float moreRainAmount = 1.25 + 1.25 * _MoreRainAmount;
				for(float i = 1.25; i < moreRainAmount; i+=0.25)
				{
					float2 _c = DropsDynamic(uv, t*i, layer1, layer2);
					float _cx = DropsDynamic(uv + e, t*i, layer1, layer2).x;
					float _cy = DropsDynamic(uv + e.yx, t*i, layer1, layer2).x;
					n += float2(_cx - _c.x, _cy - _c.x);
				}

				float blend = (n.x + n.y)*(1.75 + _MoreRainAmount);

#if defined(WIPE)
				float wipe = tex2D(_WipeCanvasTex, UV).r;
				wipe = saturate(pow(wipe, 0.2));
				float3 col = tex2D(_MainTex, UV + n * (1 - wipe)).rgb;
#else
				float3 col = tex2D(_MainTex, UV + n).rgb;
#endif

				fragColor = float4(col, blend);

				return fragColor;
			}
			ENDCG
		}
    }
}
