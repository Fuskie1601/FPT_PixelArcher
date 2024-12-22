Shader "Custom/EnhancedPixelizeShader"
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/Colors.hlsl"
        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/Sampling.hlsl"

    
        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        float4 _MainTex_TexelSize;
        int _PixelSample;

        TEXTURE2D_SAMPLER2D(_CameraNormalsTexture, sampler_CameraNormalsTexture);
        TEXTURE2D_SAMPLER2D(_CameraNormalsTexture2, sampler_CameraNormalsTexture2);
        TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);
        float _Scale;
        float4 _Color;
        float4 _NormalColor;

		float _NormalThreshold;
		float _NormalEdgeBias;
		bool _Highlight;

		float3 normal_edge_bias = (float3(1., 1., 1.));

        // This matrix is populated in PostProcessOutline.cs.
		float4x4 _ClipToView;
		float4x4 _InvProjectionMatrix;

		float4 alphaBlend(float4 top, float4 bottom)
		{
			float3 color = (top.rgb * top.a) + (bottom.rgb * (1 - top.a));
			float alpha = top.a + bottom.a * (1 - top.a);

			return float4(color, alpha);
		}
        struct v2f
        {
            float2 uv : TEXCOORD0;
            float4 vertex : SV_POSITION;
		    float2 texcoord : TEXCOORD0; //might remove
		    float3 viewSpaceDir : TEXCOORD1;
            float3 screen_pos : TEXCOORD2;
        };

        inline float4 ComputeScreenPos(float4 pos)
        {
            float4 o = pos * 0.5f;
            o.xy = float2(o.x, o.y * _ProjectionParams.x) + o.w;
            o.zw = pos.zw;
            return o;
        }

        v2f Vert (AttributesDefault v)
        {
            v2f o;
            o.vertex = float4(v.vertex.x, -v.vertex.y, 0.0, 1.0);
            //o.vertex = float4(v.vertex.xy, 0.0, 1.0);
            o.uv = TransformTriangleVertexToUV(v.vertex.xy);
            o.screen_pos = ComputeScreenPos(o.vertex);
            
            o.texcoord = TransformTriangleVertexToUV(v.vertex.xy);
            o.viewSpaceDir = mul(_InvProjectionMatrix, o.vertex).xyz;
        	
            return o;
        }

        float2 pixelArt(float2 uv, float pixelSample)
        {
			float pixelScale = pixelSample * _Scale;
            float2 quantizedUV = floor(uv * pixelScale) / pixelScale;
            return quantizedUV;
        }
        float2 pixelArtSobel(float2 uv, float pixelSample, float pixelOffsetX, float pixelOffsetY)
        {
			float pixelScale = pixelSample * _Scale;
            float2 quantizedUV = floor(uv * pixelScale) / pixelScale;
            quantizedUV -= float2(pixelOffsetX / 2, pixelOffsetY) / pixelScale;
            return quantizedUV;
        }
		float normalIndicator(float3 normalEdgeBias, float3 baseNormal, float3 newNormal, float depth_diff)
		{
		    float normalDiff = dot(baseNormal - newNormal, normalEdgeBias);
		    float normalIndicator = clamp(smoothstep(-0.01, 0.01, normalDiff), 0.0, 1.0);
		    float depthIndicator = clamp(sign(depth_diff * 0.25 + 0.0025), 0.0, 1.0);
		    return (1.0 - dot(baseNormal, newNormal)) * depthIndicator * normalIndicator;
		}

		float GetDepth(float2 screenUV)
		{
		    // Sample the depth texture
		    float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, screenUV).r;
		    float3 ndc = float3(screenUV * 2.0 - 1.0, depth);
		    float4 view = mul(_InvProjectionMatrix, float4(ndc, 1.0));
		    view.xyz /= view.w;
		    return -view.z;
		}
		float3 GetNormal(float2 uv)
        {
	        return SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, uv).rgb;
        }

		float3 GetNormalPlayer2(float2 uv)
        {
	        return SAMPLE_TEXTURE2D(_CameraNormalsTexture2, sampler_CameraNormalsTexture2, uv).rgb;
        }
    
        
        float4 Frag(v2f input) : SV_Target
		{
		     // Pixelize the UV coordinates
		    float2 pixeledUV = pixelArt(input.uv, _PixelSample);
			float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, pixeledUV);
		     // Calculate the offsets for Sobel filter
		    float2 pixeledUVOffset[4];
		    pixeledUVOffset[0] = pixelArtSobel(input.uv, _PixelSample, 1, 0); // right
		    pixeledUVOffset[1] = pixelArtSobel(input.uv, _PixelSample, -1, 0); // left
		    pixeledUVOffset[2] = pixelArtSobel(input.uv, _PixelSample, 0, 1); // up
		    pixeledUVOffset[3] = pixelArtSobel(input.uv, _PixelSample, 0, -1); // down
			
		     // Get depth value for the pixelized UV coordinates
		    float depth = GetDepth(pixeledUV);
		     // Calculate depth differences for the outline
		    float depth_diff = 0.0;
			float depths[4];
		    for (int j = 0; j < 4; j++)
		    {
		        depths[j] = GetDepth(pixeledUVOffset[j]);
		    	depth_diff += depths[j] - depth;
		    }

			float depthThreshold = 0.256 * depth;
            float depthFiniteDifference1 = depths[1] - depths[0];
            float depthFiniteDifference2 = depths[3] - depths[2];

			float edgeDepth = sqrt(pow(depthFiniteDifference1, 2) + pow(depthFiniteDifference2, 2)) * 1000;
        	edgeDepth = edgeDepth > depthThreshold ? 1 : 0;
			
			
		     // Get normal value for the pixelized UV coordinates
		    float3 normal = GetNormal(pixeledUV).rgb * 2.0 - 1.0;
		     // Calculate normal differences for the outline
		    float normal_diff = 0.0;
		    float normal2_diff = 0.0;
		    for (int i = 0; i < 4; i++)
		    {
		        float3 n = GetNormal(pixeledUVOffset[i]).rgb * 2.0 - 1.0;
		        float3 n2 = GetNormalPlayer2(pixeledUVOffset[i]).rgb * 2.0 - 1.0;
		        normal_diff += normalIndicator(normal_edge_bias, normal, n, depth_diff);
		    }
		    normal_diff = smoothstep(0.15, 0.75, normal_diff);

			//return normal_diff; //test
			//return edgeDepth; //test
			//return edgeDepth + normal_diff; //test
			
			// Combine the colors
		    float4 finalColor = color; // Start with the original color

		    // Blend in the edge color based on edgeDepth
		    if (edgeDepth > 0.0)
		        finalColor = lerp(finalColor, _Color, edgeDepth * 1.5);
		    else
			    finalColor = lerp(finalColor, _NormalColor, normal_diff * 0.3);

		    return finalColor;
		}
    
    ENDHLSL
    
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM

                #pragma vertex Vert
                #pragma fragment Frag

            ENDHLSL
        }
    }
}
