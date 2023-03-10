#ifndef CAMERA_CGINC
#define CAMERA_CGINC

inline float3 GetCameraPosition()    { return _WorldSpaceCameraPos;      }
inline float3 GetCameraForward()     { return -UNITY_MATRIX_V[2].xyz;    }
inline float3 GetCameraUp()          { return UNITY_MATRIX_V[1].xyz;     }
inline float3 GetCameraRight()       { return UNITY_MATRIX_V[0].xyz;     }
inline float  GetCameraFocalLength() { return abs(UNITY_MATRIX_P[1][1]); }
inline float  GetCameraNearClip()    { return _ProjectionParams.y;       }
inline float  GetCameraFarClip()     { return _ProjectionParams.z;       }
inline bool   IsCameraPerspective()  { return any(UNITY_MATRIX_P[3].xyz); }
inline bool   IsCameraOrtho()        { return !IsCameraPerspective(); }
inline float  GetCameraMaxDistance() { return GetCameraFarClip() - GetCameraNearClip(); }

inline float3 _GetCameraDirection(float2 sp)
{
    float3 camDir      = GetCameraForward();
    float3 camUp       = GetCameraUp();
    float3 camSide     = GetCameraRight();
    float  focalLen    = GetCameraFocalLength();
    return normalize((camSide * sp.x) + (camUp * sp.y) + (camDir * focalLen));
}

inline float3 GetCameraDirection(float4 projPos)
{
    projPos.xy /= projPos.w;
    projPos.xy = (projPos.xy - 0.5) * 2.0;
    projPos.x *= _ScreenParams.x / _ScreenParams.y;
    return _GetCameraDirection(projPos.xy);
}

inline float GetDistanceFromCameraToNearClipPlane(float4 projPos)
{
    projPos.xy /= projPos.w;
    projPos.xy = (projPos.xy - 0.5) * 2.0;
    projPos.x *= _ScreenParams.x / _ScreenParams.y;
    float3 norm = normalize(float3(projPos.xy, GetCameraFocalLength()));
    return GetCameraNearClip() / norm.z;
}

inline float3 GetCameraDirectionForShadow(float4 screenPos)
{
#if UNITY_UV_STARTS_AT_TOP
    screenPos.y *= -1.0;
#endif
    screenPos.xy /= screenPos.w;

    return _GetCameraDirection(screenPos.xy);
}


#endif
