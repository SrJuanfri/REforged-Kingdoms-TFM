Shader "Custom/MangaToonShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _MangaPattern ("Manga Pattern", 2D) = "white" {}
        _Threshold ("Threshold", Range(0,1)) = 0.5
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            Tags { "LightMode"="UniversalForward" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include <UnityCG.cginc>

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
                float3 normalOS     : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float3 normalWS     : TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D _MangaPattern;
            float4 _MainTex_ST;
            float _Threshold;
            float4 _Color;

            Varyings vert (Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.normalWS = normalize(TransformObjectToWorldNormal(input.normalOS));
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                // Sample the main texture
                half4 mainTexColor = tex2D(_MainTex, input.uv) * _Color;

                // Compute lighting
                half3 lightDir = normalize(UnityWorldSpaceLightDir(input.positionHCS));
                half NdotL = dot(input.normalWS, lightDir);
                half toonShade = step(_Threshold, NdotL);

                // Sample the manga pattern texture
                half2 screenUV = input.positionHCS.xy / input.positionHCS.w;
                screenUV = screenUV * 0.5 + 0.5;
                half4 mangaPattern = tex2D(_MangaPattern, screenUV);

                // Combine the toon shading with the manga pattern
                half4 color = mainTexColor * toonShade * mangaPattern;

                return color;
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}