Shader "Custom/See-throughURP"
{
    Properties
    {
        _StencilMask("Stencil Mask", Int) = 1
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Base Map", 2D) = "white" {}
        _Smoothness("Smoothness", Range(0, 1)) = 0.5
        _Metallic("Metallic", Range(0, 1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100

        Stencil
        {
            Ref [_StencilMask]
            Comp Notequal
            Pass Keep
        }

        Pass
        {
            Name "MainPass"
            Tags { "LightMode" = "UniversalForward" }
            Cull Off
            ZWrite On

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

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

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _Color;
            float _Smoothness;
            float _Metallic;

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS);
                output.uv = input.uv;
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                // Sample the texture and apply the color
                float4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                float4 finalColor = texColor * _Color;

                // Return the final color (Albedo color and alpha)
                return half4(finalColor.rgb, finalColor.a);
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/InternalErrorShader"
}
