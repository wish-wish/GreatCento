#include "Common.hlsl"
#include "UnityGBuffer.cginc"
#include "UnityStandardUtils.cginc"
#include "SimplexNoise3D.hlsl"

// Cube map shadow caster; Used to render point light shadows on platforms
// that lacks depth cube map support.
#if defined(SHADOWS_CUBE) && !defined(SHADOWS_CUBE_IN_DEPTH_TEX)
#define PASS_CUBE_SHADOWCASTER
#endif

// Material properties
half4 _Color;
half _Metallic;
half _Glossiness;
half _Deform;

// Vertex attributes
struct Attributes
{
    float4 position : POSITION;
    float3 normal : NORMAL;
};

// Fragment varyings
struct Varyings
{
    float4 position : SV_POSITION;

#if defined(PASS_CUBE_SHADOWCASTER)
    // Cube map shadow caster pass
    float3 shadow : TEXCOORD0;

#elif defined(UNITY_PASS_SHADOWCASTER)
    // Default shadow caster pass

#else
    // GBuffer construction pass
    float3 normal : NORMAL;
    half3 ambient : TEXCOORD0;
    float3 wpos : TEXCOORD1;

#endif
};

//
// Vertex stage
//

void Vertex(inout Attributes input)
{
    // Only do object space to world space transform.
    input.position = mul(unity_ObjectToWorld, input.position);
    input.normal = UnityObjectToWorldNormal(input.normal);
}

//
// Geometry stage
//

Varyings VertexOutput(float3 wpos, half3 wnrm)
{
    Varyings o;

#if defined(PASS_CUBE_SHADOWCASTER)
    // Cube map shadow caster pass: Transfer the shadow vector.
    o.position = UnityWorldToClipPos(float4(wpos, 1));
    o.shadow = wpos - _LightPositionRange.xyz;

#elif defined(UNITY_PASS_SHADOWCASTER)
    // Default shadow caster pass: Apply the shadow bias.
    float scos = dot(wnrm, normalize(UnityWorldSpaceLightDir(wpos)));
    wpos -= wnrm * unity_LightShadowBias.z * sqrt(1 - scos * scos);
    o.position = UnityApplyLinearShadowBias(UnityWorldToClipPos(float4(wpos, 1)));

#else
    // GBuffer construction pass
    o.position = UnityWorldToClipPos(float4(wpos, 1));
    o.normal = wnrm;
    o.ambient = ShadeSHPerVertex(wnrm, 0);
    o.wpos = wpos;

#endif
    return o;
}

[maxvertexcount(3)]
void Geometry(
    triangle Attributes input[3],
    inout TriangleStream<Varyings> outStream
)
{
    // Input vertices
    float3 p1 = input[0].position.xyz;
    float3 p2 = input[1].position.xyz;
    float3 p3 = input[2].position.xyz;

    float3 n1 = input[0].normal;
    float3 n2 = input[1].normal;
    float3 n3 = input[2].normal;

    float3 offs = float3(0, 0, _Time.y);
    float freq = 5;
    float amp = _Deform * 2;

    float4 d1 = snoise_grad(p1 * freq + offs);
    float4 d2 = snoise_grad(p2 * freq + offs);
    float4 d3 = snoise_grad(p3 * freq + offs);

    p1 += d1.xyz * pow(d1.w, 3) * amp;
    p2 += d2.xyz * pow(d2.w, 3) * amp;
    p3 += d3.xyz * pow(d3.w, 3) * amp;

    float3 n = normalize(cross(p2 - p1, p3 - p1));
    float np = smoothstep(0, 0.1, _Deform);

    outStream.Append(VertexOutput(p1, lerp(n1, n, np)));
    outStream.Append(VertexOutput(p2, lerp(n2, n, np)));
    outStream.Append(VertexOutput(p3, lerp(n3, n, np)));
    outStream.RestartStrip();
}

//
// Fragment phase
//

#if defined(PASS_CUBE_SHADOWCASTER)

// Cube map shadow caster pass
half4 Fragment(Varyings input) : SV_Target
{
    float depth = length(input.shadow) + unity_LightShadowBias.x;
    return UnityEncodeCubeShadowDepth(depth * _LightPositionRange.w);
}

#elif defined(UNITY_PASS_SHADOWCASTER)

// Default shadow caster pass
half4 Fragment() : SV_Target { return 0; }

#else

// GBuffer construction pass
void Fragment(
    Varyings input,
    out half4 outGBuffer0 : SV_Target0,
    out half4 outGBuffer1 : SV_Target1,
    out half4 outGBuffer2 : SV_Target2,
    out half4 outEmission : SV_Target3
)
{
    // PBS workflow conversion (metallic -> specular)
    half3 c_diff, c_spec;
    half not_in_use;

    c_diff = DiffuseAndSpecularFromMetallic(
        _Color.rgb, _Metallic, // input
        c_spec, not_in_use     // output
    );

    // Update the GBuffer.
    UnityStandardData data;
    data.diffuseColor = c_diff;
    data.occlusion = 1;
    data.specularColor = c_spec;
    data.smoothness = _Glossiness;
    data.normalWorld = input.normal;
    UnityStandardDataToGbuffer(data, outGBuffer0, outGBuffer1, outGBuffer2);

    // Output ambient light to the emission buffer.
    half3 sh = ShadeSHPerPixel(data.normalWorld, input.ambient, input.wpos);
    outEmission = half4(sh * data.diffuseColor, 1);
}

#endif
