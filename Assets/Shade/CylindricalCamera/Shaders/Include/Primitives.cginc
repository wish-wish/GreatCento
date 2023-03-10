#ifndef PRIMITIVES_H
#define PRIMITIVES_H

inline float Sphere(float3 pos, float radius)
{
    return length(pos) - radius;
}

inline float RoundBox(float3 pos, float3 size, float round)
{
    return length(max(abs(pos) - size, 0.0)) - round;
}

inline float Box(float3 pos, float3 size)
{
    // complete box (round = 0.0) cannot provide high-precision normals.
    return RoundBox(pos, size, 0.0001);
}

inline float Torus(float3 pos, float2 radius)
{
    float2 r = float2(length(pos.xy) - radius.x, pos.z);
    return length(r) - radius.y;
}

inline float Plane(float3 pos, float3 dir)
{
    return dot(pos, dir);
}

inline float Cylinder(float3 pos, float2 r)
{
    float2 d = abs(float2(length(pos.xy), pos.z)) - r;
    return min(max(d.x, d.y), 0.0) + length(max(d, 0.0)) - 0.1;
}

inline float HexagonalPrismX(float3 pos, float2 h)
{
    float3 p = abs(pos);
    return max(
        p.x - h.y, 
        max(
            (p.z * 0.866025 + p.y * 0.5),
            p.y
        ) - h.x
    );
}

inline float HexagonalPrismY(float3 pos, float2 h)
{
    float3 p = abs(pos);
    return max(
        p.y - h.y, 
        max(
            (p.z * 0.866025 + p.x * 0.5),
            p.x
        ) - h.x
    );
}

inline float HexagonalPrismZ(float3 pos, float2 h)
{
    float3 p = abs(pos);
    return max(
        p.z - h.y, 
        max(
            (p.x * 0.866025 + p.y * 0.5),
            p.y
        ) - h.x
    );
}

#endif
