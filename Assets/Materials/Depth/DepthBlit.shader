Shader "Custom/DepthBlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [Toggle] _Invert ("Invert Depth", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Name "DepthBlit"
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float _Invert;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = IN.uv;
                return OUT;
            }

            float4 Frag(Varyings IN) : SV_Target
            {
                float depth = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).r;
                float linearDepth = Linear01Depth(depth, _ZBufferParams);
                float result = _Invert > 0.5 ? 1.0 - linearDepth : linearDepth;
                return float4(result, result, result, 1.0);
            }
            ENDHLSL
        }
    }
}