// for seek a job : 13860624608 lin
Shader "Unlit/TriPlannarMatrix"
{
    //这里需要用到triplaner mapping三平面映射的技术。简单来说就是利用世界空间坐标在三个方向上采样纹理，最后通过法线计算权重混合结果。
    Properties { }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "IgnoreProjector" = "True" }

        // LOD 100
        // Blend SrcColor OneMinusSrcColor
        // Blend SrcFactor DstFactor
        // Blend One One
        // Cull Off
        // ZWrite Off
        
        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
                float3 normal : NORMAL;
                float4 screenPos : TEXCOORD2;
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = v.uv
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.screenPos = ComputeScreenPos(o.vertex);

                return o;
            }

            sampler2D white_noise;
            sampler2D font_texture;
            uint colored;

            float text(float2 coord)//获取文字

            {
                float2 uv = frac(coord.xy / 16.0);//uv
                float2 block = floor(coord.xy / 16.0);//获取blockid，左下角(0,0)，右上角(15,15)
                uv = uv * 0.7 + 0.1;//scale
                float2 rand = tex2D(white_noise, block.xy / float2(512.0, 512.0)).xy;//每个block对应噪声贴图的一个像素
                rand = floor(rand * 16.0);
                uv += rand;//uv随机偏移
                uv *= 0.0625;//0-16映射成0~1
                uv.x = -uv.x;
                return tex2D(font_texture, uv).r;
            }
            #define dropLength 512
            #define scale 0.2
            #define sharpness 10.0

            float3 rain(float2 fragCoord)
            {
                fragCoord.x = floor(fragCoord.x / 16.0);//block 列的id
                float offset = sin(fragCoord.x * 15.0);//
                float speed = cos(fragCoord.x * 3.0) * 0.15 + .35;
                float y = frac((fragCoord.y / dropLength) + _Time.y * speed + offset);
                float3 col = float3(0.1, 1.0, 0.35) / (y * 20.0);
                return col;
            }
            uint _session_rand_seed; // required by the RandomLi Include

            
            #include "LabColorspace.cginc"

            // The below macro is used to get a random number which varies across different generations.
            #define rnd(seed, constant)  wang_rnd(seed + triple32(_session_rand_seed) * constant)

            uint triple32(uint x)
            {
                x ^= x >> 17;
                x *= 0xed5ad4bbU;
                x ^= x >> 11;
                x *= 0xac4c1b51U;
                x ^= x >> 15;
                x *= 0x31848babU;
                x ^= x >> 14;
                return x;
            }

            float wang_rnd(uint seed)
            {
                uint rndint = triple32(seed);
                return((float)rndint) / float(0xFFFFFFFF);             // 0xFFFFFFFF is max unsigned integer in hexa decimal

            }

            float _Global_Transition_value;
            float3 _Global_Effect_center;

            float spread_from_center(float3 center, float3 coord, float transition, float shift)
            {
                float distance_to_center = distance(coord, center);
                float control_value = saturate(transition - shift);
                if (control_value * 60.0f < distance_to_center) return 0;
                else return 1.0f;
            }

            float split_from_midle(float coordx, float transition, float shift)
            {

                float distance_to_center = abs(coordx - 0.5) * 2.0;
                float control_value = saturate(transition - shift);
                if (control_value < distance_to_center) return 0;
                else return 1.0f;
            }


            float3 rain_color(float2 fragCoord)
            {
                fragCoord.x = floor(fragCoord.x / 16.);                  // This is the exact replica of the calculation in text function for getting the cell ids. Here we want the id for the columns
                
                float offset = rnd(fragCoord.x * 521., 612);              // Each drop of rain needs to start at a different point. The column id  plus a sin is used to generate a different offset for each columm
                float speed = rnd(fragCoord.x * 612., 951) * .15 + .35;    // Same as above, but for speed. Since we dont want the columns travelling up, we are adding the 0.7. Since the cos *0.3 goes between -0.3 and 0.3 the 0.7 ensures that the speed goes between 0.4 mad 1.0. This is also control parameters for min and max speed
                speed *= 0.4;
                float y = frac((fragCoord.y / dropLength)       // This maps the screen again so that top is 1 and button is 0. The addition with time and frac would cause an entire bar moving from button to top
                + _Time.y * speed + offset);        // the speed and offset would cause the columns to move down at different speeds. Which causes the rain drop effect
                
                
                int randomSeed = (fragCoord.x +
                floor((fragCoord.y / dropLength) + _Time.y * speed + offset) * 200.) * 5551.;

                float3 col = float3(rnd(randomSeed, 21),
                rnd(randomSeed, 712),
                rnd(randomSeed, 61));
                col = lab2rgb(col);
                return col / (y * 40.);                                    // adjusting the retun color based on the columns calculations.

            }

            float3 MatrixTri(float2 coord)
            {
                float col = float3(0.0, 0.0, 0.0);
                float2 crd = coord * float2(dropLength, dropLength) * scale;
                float3 rain_col = float3(0.0, 0.0, 0.0);
                if (colored == 1)
                    rain_col = rain_color(crd);
                else
                    rain_col = rain(crd);
                return text(crd) * rain_col;
            }    

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = float4(0., 0., 0., 1.);
                
                float3 colFront = MatrixTri(i.worldPos.xy + sin(i.worldPos.zz));//面向Z,xy
                float3 colSide = MatrixTri(i.worldPos.zy + sin(i.worldPos.xx));//面向X,yz
                float3 colTop = MatrixTri(i.worldPos.xz + sin(i.worldPos.yy));//面向Y,xz
                
                //权重//面向xy45°,采样xz，yz共同混合的颜色，以此类推
                float3 blendWeight = pow(normalize(abs(i.normal)), sharpness);
                blendWeight /= (blendWeight.x + blendWeight.y + blendWeight.z);
                col.xyz = colFront * blendWeight.z +
                colSide * blendWeight.x +
                colTop * blendWeight.y;
                
                // float distance_to_center = distance(i.worldPos.xyz, _Global_Effect_center.xyz);//到攝像機的距離
                // float control_value      = saturate(_Global_Transition_value);//將轉變時間規範在0-1之間
                // if (control_value * 60.0f < distance_to_center) col = col * 0.0f;//轉變效果，隨著control_value的增大，逐漸轉變為矩陣效果
                
                // float2 screenPos = i.screenPos.xy / i.screenPos.w;//得到视口空间坐标
                // col      *= split_from_midle(screenPos.x, _Global_Transition_value, 0.0f);//判断是否有该转变的矩阵效果
                // col       = min(1.5,col);

                return col;
            }

            ENDCG

        }
    }
}
