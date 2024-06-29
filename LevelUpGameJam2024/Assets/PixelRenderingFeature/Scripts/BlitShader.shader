Shader "Unlit/BlitShader"
{
    Properties
    {
        _MainTex("Base (RGB)", 2D) = "white" {}
        _PixelDensity("Pixel Density", float) = 10
        _Power("OutlineStrength", float) = 50
        _PosterizationCount("Count", int) = 8

        _Palette("Palette", 2D) = "white" {}

        [Toggle(SAMPLEPALETTE)]
        _ActivePalette("Activate Palette", Float) = 0
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200

            Pass
            {

                Blend SrcAlpha OneMinusSrcAlpha

                ZWrite On

                HLSLPROGRAM
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

                TEXTURE2D(_DepthTex);
                SAMPLER(sampler_DepthTex);

                TEXTURE2D(_CameraDepthTexture);
                SAMPLER(sampler_CameraDepthTexture);

                TEXTURE2D(_CameraColorTexture);
                SAMPLER(sampler_CameraColorTexture);

                TEXTURE2D(_MainTex);
                SAMPLER(sampler_MainTex);

                float _PixelDensity;
                int _PosterizationCount;
                float _Power;

                float _SamplePalette;
                sampler2D _Palette;

                struct Attributes
                {
                    float4 positionOS       : POSITION;
                    float2 uv               : TEXCOORD0;
                };

                struct Varyings
                {
                    float2 uv        : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    UNITY_VERTEX_OUTPUT_STEREO
                };

                float SampleDepth(float2 uv)
                {
                    return SAMPLE_DEPTH_TEXTURE(_DepthTex, sampler_DepthTex, uv);
                }

                half4 Posterization(half4 col) 
                {
                    float3 c = RgbToHsv(col);
                    c.z = round(c.z * _PosterizationCount) / _PosterizationCount;
                    return float4(HsvToRgb(c), col.a);
                }

                float2 SobelfeldmanOperator(float2 uv)
                {
                    float2 delta = float2(_PixelDensity / _ScreenParams.x, _PixelDensity / _ScreenParams.y);

                    float north = SampleDepth(uv + float2(0.0, 1.0) * delta);
                    float south = SampleDepth(uv + float2(0.0, -1.0) * delta);
                    float west = SampleDepth(uv + float2(1.0, 0.0) * delta);
                    float east = SampleDepth(uv + float2(-1.0, 0.0) * delta);
                    float middle = SampleDepth(uv);

                    float depth = max(max(north, south), max(west, east));
                    return float2(clamp(north - middle, 0, 1) + clamp(south - middle, 0, 1) + clamp(west - middle, 0, 1) + clamp(east - middle, 0, 1), depth);
                }

                float2 SobelFeldman(float2 uv, half4 d) 
                {
                    float2 sobelData = SobelfeldmanOperator(uv);
                    sobelData.x = pow(abs(1 - saturate(sobelData.x)), _Power);

                    sobelData.x = floor(sobelData.x + 0.2);
                    sobelData.x = lerp(1.0, sobelData.x, ceil(sobelData.y - d.x));

                    return sobelData;
                }

                half4 ColourPalette(half4 col) 
                {
                    float3 c = RgbToHsv(col);
                    float lerpValue = clamp(c.z, 0.1f, 1.0f);//lerp(col.r, col.g, col.b);
                    float lerpY = 1 - c.x;

                    float2 append = (float2(lerpValue, lerpY));
                    col.r = (half4)(tex2D(_Palette, append).r);
                    col.g = (half4)(tex2D(_Palette, append).g);
                    col.b = (half4)(tex2D(_Palette, append).b);// , col.g, col.b, 1.0);

                    return col;
                }

                Varyings vert(Attributes input)
                {
                    Varyings output = (Varyings)0;
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                    output.vertex = vertexInput.positionCS;
                    output.uv = input.uv;

                    return output;
                }

                half4 frag(Varyings input, out float depth : SV_Depth) : SV_Target//, out float depth : SV_Depth) : SV_Target
                {
                    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                    half4 col = Posterization((half4)SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv));
                    half4 d = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, input.uv);
                    float x = ceil(SampleDepth(input.uv) - d.x);

                    float2 sobelData = SobelFeldman(input.uv, d);
                    depth = lerp(sobelData.y, SampleDepth(input.uv), sobelData.x);
                    
                    #ifdef SAMPLEPALETTE
                        col = ColourPalette(col);
                    #endif
                                        
                    col.rgb *= sobelData.x;
                    col.a += 1 - sobelData.x;

                    return col;
                }

                #pragma vertex vert
                #pragma fragment frag

                #pragma shader_feature SAMPLEPALETTE

                ENDHLSL
            }

        }
            FallBack "Diffuse"
}
