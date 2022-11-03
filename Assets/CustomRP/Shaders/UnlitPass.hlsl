#ifndef CUSTOM_UNLIT_PASS_INCLUDED
#define CUSTOM_UNLIT_PASS_INCLUDED

#include "../ShaderLibrary/Common.hlsl"

float4 _BaseColor;

float4 UnlitPassVertex(float3 positionOS : POSITION) : SV_POSITION
{
    return TransformWorldToHClip(TransformObjectToWorld(positionOS));
}

float4 UnlitPassFragment() : SV_TARGET
{
     return _BaseColor;
}

#endif