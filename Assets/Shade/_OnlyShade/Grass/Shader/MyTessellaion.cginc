#pragma hull hull
#pragma domain domain
#pragma vertex tessvert
#include "UnityCG.cginc"
#include "Tessellation.cginc"
#pragma only_renderers d3d9 d3d11 glcore gles 
#pragma target 5.0
uniform float _TessellationValue;
struct VertexInput {
float4 vertex : POSITION;
float3 normal : NORMAL;
float4 tangent : TANGENT;
};
struct VertexOutput {
float4 pos : SV_POSITION;
};

VertexOutput vert (VertexInput v) {
VertexOutput o = (VertexOutput)0;
o.pos = v.vertex;
//o.pos = UnityObjectToClipPos(v.vertex);
return o;
}

#ifdef UNITY_CAN_COMPILE_TESSELLATION
struct TessVertex {
    float4 vertex : INTERNALTESSPOS;
    float3 normal : NORMAL;
    float4 tangent : TANGENT;
};
struct OutputPatchConstant {
    float edge[3]         : SV_TessFactor;
    float inside          : SV_InsideTessFactor;
    float3 vTangent[4]    : TANGENT;
    float2 vUV[4]         : TEXCOORD;
    float3 vTanUCorner[4] : TANUCORNER;
    float3 vTanVCorner[4] : TANVCORNER;
    float4 vCWts          : TANWEIGHTS;
};
TessVertex tessvert (VertexInput v) {
    TessVertex o;
    o.vertex = v.vertex;
    o.normal = v.normal;
    o.tangent = v.tangent;
    return o;
}
float Tessellation(TessVertex v){
    return _TessellationValue;
}
float4 Tessellation(TessVertex v, TessVertex v1, TessVertex v2){
    float tv = Tessellation(v);
    float tv1 = Tessellation(v1);
    float tv2 = Tessellation(v2);
    return float4( tv1+tv2, tv2+tv, tv+tv1, tv+tv1+tv2 ) / float4(2,2,2,3);
}
OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {
    OutputPatchConstant o = (OutputPatchConstant)0;
    float4 ts = Tessellation( v[0], v[1], v[2] );
    o.edge[0] = ts.x;
    o.edge[1] = ts.y;
    o.edge[2] = ts.z;
    o.inside = ts.w;
    return o;
}
[domain("tri")]
[partitioning("fractional_odd")]
[outputtopology("triangle_cw")]
[patchconstantfunc("hullconst")]
[outputcontrolpoints(3)]
TessVertex hull (InputPatch<TessVertex,3> v, uint id : SV_OutputControlPointID) {
    return v[id];
}
[domain("tri")]
VertexOutput domain (OutputPatchConstant tessFactors, const OutputPatch<TessVertex,3> vi, float3 bary : SV_DomainLocation) {
    VertexInput v = (VertexInput)0;
    v.vertex = vi[0].vertex*bary.x + vi[1].vertex*bary.y + vi[2].vertex*bary.z;
    v.normal = vi[0].normal*bary.x + vi[1].normal*bary.y + vi[2].normal*bary.z;
    v.tangent = vi[0].tangent*bary.x + vi[1].tangent*bary.y + vi[2].tangent*bary.z;
    VertexOutput o = vert(v);
    return o;
}
#endif