Shader "CustomRP/Unlit"
{
    Properties
    {
        _BaseColor("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Pass
        {
            HLSLPROGRAM
            #include "UnlitPass.hlsl"
            #pragma vertex UnlitPassVertex
            #pragma fragment UnlitPassFragment
            ENDHLSL
        }
    }
}
