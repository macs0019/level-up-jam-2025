Shader"Custom/URPToon2Tint"
{
    Properties
    {
        _LightColor   ("Light Color", Color) = (1,1,1,1)
        _DarkColor    ("Dark Color",  Color) = (0,0,0,1)
        _Shades       ("Shades",       Range(1,20)) = 3
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }
LOD 100

        Pass
        {
Name"UniversalForward"
            Tags
{"LightMode"="UniversalForward"
}

Blend Off

Cull Back

ZWrite On

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

struct Attributes
{
    float3 positionOS : POSITION;
    float3 normalOS : NORMAL;
};

struct Varyings
{
    float4 positionCS : SV_POSITION;
    float3 normalWS : TEXCOORD0;
};

float4 _LightColor;
float4 _DarkColor;
float _Shades;

Varyings vert(Attributes IN)
{
    Varyings OUT;
                // posición
    OUT.positionCS = TransformObjectToHClip(IN.positionOS);
                // normal en mundo
    OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
    return OUT;
}

half4 frag(Varyings IN) : SV_Target
{
                // iluminación principal
    float3 L = _MainLightPosition.xyz; // viene de Core.hlsl
    float NdotL = dot(normalize(IN.normalWS), normalize(L));
    NdotL = saturate(NdotL);

                // toon-step
    float level = floor(NdotL * _Shades) / (_Shades - 1);

                // mezcla de dos colores
    return lerp(_DarkColor, _LightColor, level);
}
            ENDHLSL
        }
    }
}
