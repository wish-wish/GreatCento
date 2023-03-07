// Normal Mapping for a Triplanar Shader - Ben Golus 2017
// Reconstructed Tangents example shader

// Calculate plausible tangents and binormals in the fragment shader. Similar to how mesh tangents would be calculated.
// Slightly faster than the screen space partial derivatives method.

Shader "Triplanar/Reconstructed Tangents"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [NoScaleOffset] _BumpMap ("Normal Map", 2D) = "bump" {}
    }
    SubShader
    {
        Tags { "LightMode"="ForwardBase" "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            // flip UVs horizontally to correct for back side projection
            #define TRIPLANAR_CORRECT_PROJECTED_U

            // offset UVs to prevent obvious mirroring
            // #define TRIPLANAR_UV_OFFSET

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                half3 worldNormal : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _BumpMap;

            fixed4 _LightColor0;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);

                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // calculate triplanar blend
                half3 triblend = saturate(pow(i.worldNormal, 4));
                triblend /= max(dot(triblend, half3(1,1,1)), 0.0001);

                // preview blend
                // return fixed4(triblend.xyz, 1);

                // calculate triplanar uvs
                // applying texture scale and offset values ala TRANSFORM_TEX macro
                float2 uvX = i.worldPos.zy * _MainTex_ST.xy + _MainTex_ST.zw;
                float2 uvY = i.worldPos.xz * _MainTex_ST.xy + _MainTex_ST.zw;
                float2 uvZ = i.worldPos.xy * _MainTex_ST.xy + _MainTex_ST.zw;

                // offset UVs to prevent obvious mirroring
            #if defined(TRIPLANAR_UV_OFFSET)
                uvY += 0.33;
                uvZ += 0.67;
            #endif

                // minor optimization of sign(). prevents return value of 0
                half3 axisSign = i.worldNormal < 0 ? -1 : 1;

                // flip UVs horizontally to correct for back side projection
            #if defined(TRIPLANAR_CORRECT_PROJECTED_U)
                uvX.x *= axisSign.x;
                uvY.x *= axisSign.y;
                uvZ.x *= -axisSign.z;
            #endif

                // albedo textures
                fixed4 colX = tex2D(_MainTex, uvX);
                fixed4 colY = tex2D(_MainTex, uvY);
                fixed4 colZ = tex2D(_MainTex, uvZ);
                fixed4 col = colX * triblend.x + colY * triblend.y + colZ * triblend.z;

                // tangent space normal maps
                half3 tnormalX = UnpackNormal(tex2D(_BumpMap, uvX));
                half3 tnormalY = UnpackNormal(tex2D(_BumpMap, uvY));
                half3 tnormalZ = UnpackNormal(tex2D(_BumpMap, uvZ));

                // flip normal maps' x axis to account for flipped UVs
            #if defined(TRIPLANAR_CORRECT_PROJECTED_U)
                tnormalX.x *= axisSign.x;
                tnormalY.x *= axisSign.y;
                tnormalZ.x *= -axisSign.z;
            #endif

                // construct tangent to world matrices for each axis
                half3 tangentX = normalize(cross(i.worldNormal, half3(0, axisSign.x, 0)));
                half3 bitangentX = normalize(cross(tangentX, i.worldNormal)) * axisSign.x;
                half3x3 tbnX = half3x3(tangentX, bitangentX, i.worldNormal);

                half3 tangentY = normalize(cross(i.worldNormal, half3(0, 0, axisSign.y)));
                half3 bitangentY = normalize(cross(tangentY, i.worldNormal)) * axisSign.y;
                half3x3 tbnY = half3x3(tangentY, bitangentY, i.worldNormal);

                half3 tangentZ = normalize(cross(i.worldNormal, half3(0, -axisSign.z, 0)));
                half3 bitangentZ = normalize(-cross(tangentZ, i.worldNormal)) * axisSign.z;
                half3x3 tbnZ = half3x3(tangentZ, bitangentZ, i.worldNormal);

                // Apply tangent to world matricies and blend
                half3 worldNormal = normalize(
                    clamp(mul(tnormalX, tbnX), -1, 1) * triblend.x +
                    clamp(mul(tnormalY, tbnY), -1, 1) * triblend.y +
                    clamp(mul(tnormalZ, tbnZ), -1, 1) * triblend.z
                    );

                // preview world normals
                // return fixed4(worldNormal * 0.5 + 0.5, 1);

                // calculate lighting
                fixed ndotl = saturate(dot(worldNormal, _WorldSpaceLightPos0.xyz));
                half3 ambient = ShadeSH9(half4(worldNormal, 1));
                half3 lighting = _LightColor0.rgb * ndotl + ambient;

                // preview directional lighting
                // return fixed4(ndotl.xxx, 1);

                return fixed4(col.rgb * lighting, 1);
            }
            ENDCG
        }
    }
}
