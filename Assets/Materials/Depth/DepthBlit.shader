Shader "Custom/DepthBlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [Toggle] _Invert ("Invert Depth", Float) = 0
        _NearClip ("Near Clip (meters)", Float) = 0.1
        _FarClip ("Far Clip (meters)", Float) = 20.0
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
            float _NearClip;
            float _FarClip;

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

            float3 JetColormap(float t)
            {
                float3 c;
                c.r = saturate(1.5 - abs(4.0 * t - 3.0));
                c.g = saturate(1.5 - abs(4.0 * t - 2.0));
                c.b = saturate(1.5 - abs(4.0 * t - 1.0));
                return c;
            }

            float4 Frag(Varyings IN) : SV_Target
            {
                float depth = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).r;

                // Reversed-Z: depth=1 at near, depth=0 at far
                // Linear eye depth = near * far / (near + depth * (far - near))
                float linearDepth = _NearClip * _FarClip / (_NearClip + depth * (_FarClip - _NearClip));

                // Map to 0-1 range within near/far, close=0 (blue), far=1 (red)
                float t = saturate((linearDepth - _NearClip) / (_FarClip - _NearClip));
                t = _Invert > 0.5 ? 1.0 - t : t;
                float3 color = JetColormap(t);
                return float4(color, 1.0);
            }
            ENDHLSL
        }
    }
}