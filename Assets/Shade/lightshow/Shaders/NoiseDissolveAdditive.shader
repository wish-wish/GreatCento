Shader "Custom/NoiseDissolveAdditive"
{
	Properties
	{
		_MainTex ("Noise", 2D) = "white" {}
		_GradientTex ("Gradients", 2D) = "" {}	
		_Alpha ("Alpha", Float) = 1


		[Header(Color Correction)]
		_Gamma ("Gamma", Float) = 1
		_Brightness ("Brightness", Float) = 0
		_Contrast ("Contrast", Float) = 1
		_Saturation ("Saturation", Float) = 1
		_Hue ("Hue", Float) = 0	
		_SpeedX("SpeedX", Float) = 1
		_TimeValue1("_TimeValue1", Float) = 0.1
		_TimeValue2("_TimeValue2", Float) = 0.1
		_TimeValue3("_TimeValue3", Float) = 0.1

	}
	SubShader
	{
		Cull Off
		// ZTest Off ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		//Blend One One

		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#include "Common.cginc"
			#pragma vertex vert
			#pragma fragment frag
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _GradientTex;

			float4 _Tint;
			float _Alpha;

			float _Gamma;
			float _Brightness;
			float _Contrast;
			float _Saturation;
			float _Hue;
			float _SpeedX;
			float _TimeValue1;
			float _TimeValue2;
			float _TimeValue3;

			struct v2f
			{
				TRI_COMMON
				float2 pos : TEXCOORD2; 
			};

			v2f vert(appdata v) {
				v2f o;
				TRI_INITIALIZE(o);
				// o.pos = o.vertex * 0.5 + 0.5;
				float2 s = _Resolution.zw * _TexelSize.zw;
				// o.pos = (v.uv2 + (_Resolution.xy + 0.5) / _Resolution.zw) * s;
				float fx = _Resolution.y % 2 == 0;
				o.pos = (v.uv2 + (_Resolution.xy - float2(fx, 0)) / _Resolution.zw) * s;
				o.pos += _Group.xy / _Group.zw;
				o.pos = o.pos * _MainTex_ST.xy + _MainTex_ST.zw;
				// o.pos = v.uv2;
				// o.pos += _Resolution.xy * _TexelSize.xy;
				// o.pos += _Resolution.xy;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float2 uv = i.pos;
				
				//uv.x += _Time.y * _SpeedX;
				// return float4(uv, 0, 1);
				float r = tex2D(_MainTex, uv + _Time.xx * _TimeValue1).r;
				float z = tex2D(_MainTex, r*0.25 + _Time.xy * _TimeValue2);
				//float z = tex2D(_MainTex, uv * 2.0f + _Time.xy * _TimeValue2);
				// float z = tex2D(_MainTex, r.xx + _Time.xy * 0.0625);
				// z = pow(z + 0.25, 8);
				float4 col = tex2D(_GradientTex, float2(z, _Time.x * _TimeValue3));
				// float z2 = tex2D(_MainTex, z);
				float4 col2 = tex2D(_GradientTex, float2(z, _Time.x * _TimeValue3 - 0.667));
				col = lerp(col, col2, r);
				col.a *= _Alpha;

				col.rgb = pow(col.rgb, _Gamma);
				col.rgb = (col.rgb - 0.5) * _Contrast + 0.5 + _Brightness;
				col.rgb = lerp(dot(col.rgb, float3(0.2126, 0.7152, 0.0722)), col.rgb, _Saturation);
				col.rgb = RGBtoHSV(col.rgb);
				col.r = fmod(col.r + _Hue, 1);
				col.rgb = HSVtoRGB(col.rgb);

				//return (col * _Tint).rgba;
				return col.rgba;
			}
			ENDCG
		}
	}
}
// Gamma - 1
// Brightness - 0
// Contrast - 1
// Saturation - 1
// Hue - 0