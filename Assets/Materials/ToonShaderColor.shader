Shader "Unlit/ToonShader_2Tint"
{
    Properties
    {
        _Albedo    ("Light Color", Color) = (1,1,1,1)
        _InkColor  ("Dark Color",  Color) = (0,0,0,1)
        _Shades    ("Shades",       Range(1,20)) = 3
        _InkSize   ("Ink Size",     float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos         : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
            };

            float4 _Albedo;
            float4 _InkColor;
            float  _Shades;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // cálculo del ángulo luz-superficie
                float NdotL = dot(normalize(i.worldNormal), normalize(_WorldSpaceLightPos0.xyz));
                NdotL = saturate(NdotL);

                // cuantización toon
                float level = floor(NdotL * _Shades) / (_Shades - 1);

                // interpolación entre color oscuro y claro
                return lerp(_InkColor, _Albedo, level);
            }
            ENDCG
        }
    }
}
