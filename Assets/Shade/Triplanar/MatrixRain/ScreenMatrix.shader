// for seek a job : 13860624608 lin
Shader "Unlit/ScreenMatrix"
{
    Properties
    {
        //_white_noise ("noise", 2D) = "white" {}
        _font_texture ("font", 2D) = "red" { }
        _screen_height ("height", int) = 1334
        _screen_width ("width", int) = 750
        _color ("color", int) = 0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            uint _screen_width;
            uint _screen_height;
            sampler2D _white_noise;
            sampler2D _font_texture;
            uint _session_rand_seed;
            bool _color;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float text(float2 coord)//获取文字

            {
                float2 uv = frac(coord.xy / 16.0);//uv  // Geting the fract part of the block, this is the uv map for the blocl
                float2 block = floor(coord.xy / 16.0);//获取blockid，左下角(0,0)，右上角(15,15) // Getting the id for the block. The first blocl is (0,0) to its right (1,0), and above it (0,1)
                uv = uv * 0.7 + 0.1;//scale           // Zooming a bit in each block to have larger ltters
                float2 rand = tex2D(_white_noise, block.xy / float2(512.0, 512.0)).xy;//每个block对应噪声贴图的一个像素  // This texture contains animated white noise. The white noise is animated in compute shaders
                // 512 is the white noise texture width. This division ensures that each block samples exactly one pixel of the noise texture
                rand = floor(rand * 16.0);          // Each random value is used for the block to sample one of the 16 columns of the font texture. This rand offset is what picks the letter, the animated white noise is what changes it
                uv += rand;//uv随机偏移           // The random texture has a different value und the xy channels. This ensures that randomly one member of the texture is picked
                uv *= 0.0625;//0-16映射成0~1      // So far the uv value is between 0-16. To sample the font texture we need to normalize this to 0-1. hence a divid by 16
                uv.x = -uv.x;
                return tex2D(_font_texture, uv).r;
            }

            float3 rain(float2 fragCoord)
            {
                fragCoord.x = floor(fragCoord.x / 16.);//block 列的id
                float offset = sin(fragCoord.x * 15);//
                float speed = cos(fragCoord.x * 3.) * 0.15 + .35;
                float y = frac((fragCoord.y / _screen_height) + _Time.y * speed + offset);
                float3 col = float3(0.1, 1.0, 0.35) / (y * 20.0);
                return col;
            }

            #include "LabColorspace.cginc"

            // The below macro is used to get a random number which varies across different generations.
            #define rnd(seed, constant)  wang_rnd(seed + triple32(_session_rand_seed) * constant)
            #define scale 0.6

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

            float3 MatrixTri(float2 coord)
            {
                float col = float3(0.0, 0.0, 0.0);
                float2 crd = coord * float2(_screen_width, _screen_height) * scale;
                float3 rain_col = rain(crd);
                return text(crd) * rain_col;
            }

            float3 rain_color(float2 fragCoord)
            {
                fragCoord.x = floor(fragCoord.x / 16.);                  // This is the exact replica of the calculation in text function for getting the cell ids. Here we want the id for the columns
                
                float offset = rnd(fragCoord.x * 521., 612);              // Each drop of rain needs to start at a different point. The column id  plus a sin is used to generate a different offset for each columm
                float speed = rnd(fragCoord.x * 612., 951) * .15 + .35;    // Same as above, but for speed. Since we dont want the columns travelling up, we are adding the 0.7. Since the cos *0.3 goes between -0.3 and 0.3 the 0.7 ensures that the speed goes between 0.4 mad 1.0. This is also control parameters for min and max speed
                speed *= 0.4;
                float y = frac((fragCoord.y / _screen_height)       // This maps the screen again so that top is 1 and button is 0. The addition with time and frac would cause an entire bar moving from button to top
                + _Time.y * speed + offset);        // the speed and offset would cause the columns to move down at different speeds. Which causes the rain drop effect
                
                
                int randomSeed = (fragCoord.x +
                floor((fragCoord.y / _screen_height) + _Time.y * speed + offset) * 200.) * 5551.;

                float3 col = float3(rnd(randomSeed, 21),
                rnd(randomSeed, 712),
                rnd(randomSeed, 61));
                col = lab2rgb(col);
                return col / (y * 20.);                                    // adjusting the retun color based on the columns calculations.

            }

            float3 gradual_column(float2 fragCoord)
            {
                float y = frac(fragCoord.y / _screen_height + _Time.y * 0.5);
                return float3(0.1, 1.0, 0.35) / (y * 20.0);
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = float4(0.0, 0.0, 0.0, 1.0);
                float2 coord = i.uv * float2(_screen_width, _screen_height) * scale;
                //col.xyz = text(coord);
                //col.xyz=text(coord);
                if (_color == 1)
                    col.xyz = rain_color(coord) * text(coord);
                else
                    col.xyz = rain(coord) * text(coord);
                return col;
            }

            ENDCG

        }
    }
}
