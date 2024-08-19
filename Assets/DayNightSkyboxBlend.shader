Shader "Custom/DayNightSkyboxBlend"
{
    Properties
    {
        _DayTex ("Day Cubemap", CUBE) = "" {}
        _NightTex ("Night Cubemap", CUBE) = "" {}
        _Blend ("Blend", Range(0,1)) = 0.0  // Comienza completamente de día
    }
    SubShader
    {
        Tags { "Queue" = "Background" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 texcoord : TEXCOORD0;
            };

            samplerCUBE _DayTex;
            samplerCUBE _NightTex;
            float _Blend;

            v2f vert (float4 vertex : POSITION)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(vertex);
                o.texcoord = mul(unity_ObjectToWorld, vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 dayColor = texCUBE(_DayTex, i.texcoord);
                float4 nightColor = texCUBE(_NightTex, i.texcoord);
                return lerp(dayColor, nightColor, _Blend);
            }
            ENDCG
        }
    }
    Fallback "RenderType"
}
