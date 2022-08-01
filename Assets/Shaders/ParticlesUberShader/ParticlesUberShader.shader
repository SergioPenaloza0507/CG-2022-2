Shader "URP Custom/Particles Uber Unlit"
{
    
    Properties
    {
        [Header(Blend)]
        [Enum(UnityEngine.Rendering.BlendMode)]_SrcBlend("Source Blend Factor", float) = 4
        [Enum(UnityEngine.Rendering.BlendOp)]_BlendOperation("Source and destination Blend Operation", float) = 0
        [Enum(UnityEngine.Rendering.BlendMode)]_DstBlend("Destination Blend Factor", float) = 10
        [Toggle(SOFT_PARTICLES)]_SoftParticles("Soft Particles", float) = 1
        _SoftParticlesThreshold("Soft Particles Threshold", float) = 0.1
        _SoftParticlesFalloff("Soft Particles Faloff", float) = 0.01
//---------------------------------------------------------------------------------------------------------------
        [Header(Color tint)]
        _Color("Main Color", color) = (1.0, 1.0 ,1.0, 1.0)
        [KeywordEnum(Material, CustomVertexStreams)]EmissionMode("Emission Mode", float) = 0.0
        [Toggle(USE_VERTEX_STREAMS_ON_EMISSION)] _AnimatedEmissionMap("Use vertex streams on emission map animation", float) = 0.0
        _EmissionMap("Emission Map", 2D) = "white" {}
        [HDR]_EmissionColor("Emission Color", color) = (0.0, 0.0, 0.0, 0.0)
//---------------------------------------------------------------------------------------------------------------
        [Header(Main Texture Parameters)]
        [Toggle(USE_3D_TEXTURES)] _Use3DTextures ("Use 3D Textures", float) = 0.0
        _MainTex("Main Texture", 2D) = "white" {}
        _MainTex3D("Main 3D Texture", 3D) = "white" {}
        _3DSlice("3D Z coordinate", float) = 0.0
        [Toggle(CLAMP_MAIN_TEX)] _ClampMainTex("Clamp Main Texture", float) = 0.0
        [KeywordEnum(Time, CustomVertexStreams, TimePeriodic)] MainTexAnimMode("Main Texture Animation Mode", float) = 0.0
        //Float4 where:
        //X = Speed X
        //Y = Speed Y
        //Z = Periodic Frequency
        //W = Periodic Amplitude
        _MainTexAnimParams("Main Texture Animation Parameters", vector) = (1,1,1,1)
//--------------------------------------------------------------------------------------------------------------
        [Header(Main Texture Distortion)]
        _MainTexTwirlParams ("Main Texture Twirl Parameters", vector) = (-0.5,-0.5,0,0)
        _MainTexTwirlStrength ("Main Texture Twirl Strength", float) = 0.0
        _MainTexDistortNoise("Main Texture Distort Noise", 2D) = "black" {}
        [KeywordEnum(Time, CustomVertexStreams, TimePeriodic)] MainTexDistortNoiseAnimMode("Main Texture Distortion Animation Mode", float) = 0.0
        //Float4 where:
        //X = Speed X
        //Y = Speed Y
        //Z = Periodic Frequency
        //W = Periodic Amplitude
        _MainTexDistortAnimParams("Main Texture Distortion Animation Parameters", vector) = (0,0,1,1)
        _MainTexDistortStrength("Main Texture Distortion Strength", float) = 0.0
//--------------------------------------------------------------------------------------------------------------
        [Header(Alpha Masking)]
        [KeywordEnum(None, UV, Texture, Position)] AlphaMaskingMode("Alpha Masking Mode", float) = 0.0
        [KeywordEnum(Time, CustomVertexStreams, TimePeriodic)] AlphaMaskAnimMode("Alpha Mask Animation Mode", float) = 0.0
        _AlphaMaskAnimParams("Alpha Mask ANimation Parameters", vector) = (0,0,1,1)
        _AlphaMaskDirection("Alpha Mask Direction", vector) = (1,0,0,0)
        _AlphaMask("Alpha Mask", 2D) = "white" {}
//--------------------------------------------------------------------------------------------------------------
        [Header(Distortion)]
        [Toggle(DISTORTION)] _DistortionToggle("Distortion", float) = 0.0
        _DistortMap("Distortion Normal Map", 2D) = "Bump" {}
        _DistortionBlend("Distortion Blend", Range(0.0,1.0)) = 0.5
        _DistortionStrength("Distortion Strength", float) = 0.0
       
    }
    
    SubShader
    {
        Tags {"RenderType"="Transparent" "RenderPipeline"="UniversalRenderPipeline" "Queue"="Transparent"}
        Cull Off
        BlendOp [_BlendOperation]
        Blend [_SrcBlend] [_DstBlend]
        ZWrite Off
        Pass
        {
            HLSLPROGRAM
            #define DECLARE_TEXTURE_ANIMATION_FEATURES(TEXTURE_NAME)\
                #pragma shader_feature TEXTURE_NAME_TIME\
                #pragma shader_feature TEXTURE_NAME_CUSTOMVERTEXSTREAMS\
                #pragma shader_feature TEXTURE_NAME_TIMEPERIODIC
            
            //Tell the compiler which functions are supposed to be the vertex and the fragment shader
            #pragma vertex vert
            #pragma fragment frag

            //Create shader keywords for compile dependent conditions and shader variants
            //using shader_feature to allow unity to strip unused variants
            #pragma region Shader Keywords
            #pragma shader_feature SOFT_PARTICLES

            #pragma shader_feature EMISSIONMODE_MATERIAL
            #pragma shader_feature EMISSIONMODE_CUSTOMVERTEXSTREAMS
            #pragma shader_feature USE_VERTEX_STREAMS_ON_EMISSION

            #pragma shader_feature USE_3D_TEXTURES
            #pragma shader_feature CLAMP_MAIN_TEX
            #pragma shader_feature MAINTEXANIMMODE_TIME
            #pragma shader_feature MAINTEXANIMMODE_CUSTOMVERTEXSTREAMS
            #pragma shader_feature MAINTEXANIMMODE_TIMEPERIODIC

            #pragma shader_feature MAINTEXDISTORTNOISEANIMMODE_TIME
            #pragma shader_feature MAINTEXDISTORTNOISEANIMMODE_CUSTOMVERTEXSTREAMS
            #pragma shader_feature MAINTEXDISTORTNOISEANIMMODE_TIMEPERIODIC

            #pragma shader_feature ALPHAMASKINGMODE_NONE
            #pragma shader_feature ALPHAMASKINGMODE_UV
            #pragma shader_feature ALPHAMASKINGMODE_TEXTURE
            #pragma shader_feature ALPHAMASKINGMODE_POSITION

            #pragma shader_feature ALPHAMASKANIMMODE_TIME
            #pragma shader_feature ALPHAMASKANIMMODE_CUSTOMVERTEXSTREAMS
            #pragma shader_feature ALPHAMASKANIMMODE_TIMEPERIODIC

            #pragma shader_feature DISTORTION
            #pragma endregion

            //Define enums as macros so it's easier to compare int values
            #pragma region Macro Enums
            
            #pragma region BlendMode
            #define BLEND_ZERO 0
            #define BLEND_ONE 1
            #define BLEND_DST_COLOR 2
            #define BLEND_SRC_COLOR 3
            #define BLEND_ONE_MINUS_DST_COLOR 4
            #define BLEND_SRC_ALPHA 5
            #define BLEND_ONE_MINUS_SRC_COLOR 6
            #define BLEND_DST_ALPHA 7
            #define BLEND_ONE_MINUS_DST_ALPHA 8
            #define BLEND_SRC_ALPHA_SATURATE 9
            #define BLEND_ONE_MINUS_SRC_ALPHA 10
            #pragma endregion
            
            #pragma endregion 

            //Include core library from unity URP
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"   

            //Create vertex buffer struct, this will be passed by unity as the mesh's data
            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 texcoord1  : TEXCOORD0;
                float4 texcoord2  : TEXCOORD1;
                float4 color      : COLOR;
            };

            //Create interpolator struct, the struct that will pass data from the vertex shader to the fragment shader
            //and interpolate it based on semantics
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float4 texcoord1   : TEXCOORD0;
                float4 texcoord2   : TEXCOORD1;
                float3 positionOS  : TEXCOORD2;
                float4 texcoordSS  : TEXCOORD3;
                float4 color       : COLOR;
            };

            //Shader uniforms declaration, basically property mirrors from the property block above
            #pragma region Uniforms
            int _SrcBlend;
            int _DstBlend;

            float _SoftParticlesThreshold;
            float _SoftParticlesFalloff;

            half4 _Color;
            Texture2D<half4> _EmissionMap;
            float4 _EmissionMap_ST;
            half4 _EmissionColor;
            
            
            SamplerState  general_linear_repeat_sampler;
            SamplerState  general_linear_clamp_sampler;
            sampler2D _CameraDepthTexture;
            sampler2D _CameraOpaqueTexture;

            Texture2D<half4> _MainTex;
            sampler3D _MainTex3D;
            float4 _MainTex_ST;
            float4 _MainTexAnimParams;

            float4 _MainTexTwirlParams;
            float _MainTexTwirlStrength;
            Texture2D<half> _MainTexDistortNoise;
            float4 _MainTexDistortNoise_ST;
            float4 _MainTexDistortAnimParams;
            float _MainTexDistortStrength;

            float3 _AlphaMaskDirection;
            Texture2D<half> _AlphaMask;
            float4 _AlphaMask_ST;
            float4 _AlphaMaskAnimParams;

            Texture2D<half4> _DistortMap;
            half _DistortionBlend;
            float _DistortionStrength;
            #pragma endregion 

            /*
             *Compares a blend mode int with a macro enum to determine if the blend mode is dependent on alpha
             */
            bool IsBlendModeAlphaDependent(int blendMode)
            {
                return blendMode == BLEND_DST_ALPHA || blendMode == BLEND_SRC_ALPHA || blendMode == BLEND_ONE_MINUS_DST_ALPHA || blendMode == BLEND_SRC_ALPHA_SATURATE || blendMode == BLEND_ONE_MINUS_SRC_ALPHA;
            }

            float2 Twirl(float2 uv, float2 center, float strength, float2 offset)
            {
                float2 delta = uv - center;
                const float angle = strength * length(delta);
                const float x = cos(angle) * delta.x - sin(angle) * delta.y;
                const float y = sin(angle) * delta.x + cos(angle) * delta.y;
                return float2(x + center.x + offset.x, y + center.y + offset.y);
            }
            

            /*
             * Vertex shader
             */
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz); //Perform transformation from object space to NDC space
                OUT.texcoord1 = IN.texcoord1;//Interpolate UV channel 1
                OUT.texcoord2 = IN.texcoord2;//Interpolate UV channel 2
                OUT.positionOS = IN.positionOS;//Interpolate object space position
                OUT.texcoordSS = OUT.positionHCS;// Interpolate NDC space position
                OUT.texcoordSS.z = -TransformWorldToView(TransformObjectToWorld(IN.positionOS.xyz)).z; //Calculate View Space Depth
                OUT.color = IN.color; //Vertex colors support
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                const bool blendIsAlphaDependent = IsBlendModeAlphaDependent(_SrcBlend) && IsBlendModeAlphaDependent(_DstBlend);
                float2 mainTexUVs = IN.texcoord1.xy * _MainTex_ST.xy + _MainTex_ST.zw;

                float2 mainTexDistortUVs = IN.texcoord1.xy * _MainTexDistortNoise_ST.xy + _MainTexDistortNoise_ST.zw;

                float2 screenSpaceUvs = (IN.texcoordSS.xy / IN.texcoordSS.w) * 0.5 + 0.5; //Perform perspective division to solve Screen Space texture coordinates from NDC space position
                #if UNITY_UV_STARTS_AT_TOP
                screenSpaceUvs.y = 1 - screenSpaceUvs.y; //Flip Screen Space UVs Y axis for APIs that need it flipped
                #endif
                

                //Animate main texture distortion noise based on shader variants setup by user
                #ifdef MAINTEXDISTORTNOISEANIMMODE_TIME
                mainTexDistortUVs -= _MainTexDistortAnimParams.xy * _Time.y;
                #elif defined(MAINTEXDISTORTNOISEANIMMODE_CUSTOMVERTEXSTREAMS)
                mainTexDistortUVs -= IN.texcoord1.zw;
                #else
                mainTexDistortUVs -= sin(_Time.y * _MainTexDistortAnimParams.z) * _MainTexDistortAnimParams.w;
                #endif

                //Animate main texture based on shader variants setup by user
                #ifdef MAINTEXANIMMODE_CUSTOMVERTEXSTREAMS
                mainTexUVs += IN.texcoord1.zw;
                #elif defined(MAINTEXANIMMODE_TIME)
                mainTexUVs += _MainTexAnimParams.xy * _Time.y;
                #else
                mainTexUVs += sin(_Time.y * _MainTexAnimParams.z) * _MainTexAnimParams.xy * _MainTexAnimParams.w;
                #endif

                const float distortNoise = _MainTexDistortNoise.Sample(general_linear_repeat_sampler, mainTexDistortUVs) * _MainTexDistortStrength;
                //Solve normals from height using partial derivatives and displace uvs
                mainTexUVs += (float2(ddx(distortNoise), ddy(distortNoise)) * 2.0 - 1.0);
                mainTexUVs = Twirl(mainTexUVs, _MainTexTwirlParams.xy, _MainTexTwirlStrength, _MainTexTwirlParams.zw);
                
                //Sample texture 
                #ifdef CLAMP_TEXTURES
                    half4 color = _MainTex.Sample(general_linear_clamp_sampler, mainTexUVs);
                #else
                    half4 color = _MainTex.Sample(general_linear_repeat_sampler, mainTexUVs);
                #endif

                //Solve emission based on variants set up by user

                
                /////////////////////////////////////////////////
                // New code block start (Animated emission masks)
                float2 emissionMapUV = IN.texcoord1.xy * _EmissionMap_ST.xy + _EmissionMap_ST.zw;
                #if USE_VERTEX_STREAMS_ON_EMISSION
                emissionMapUV += IN.texcoord1.zw;
                #endif
                
                
                #ifdef CLAMP_TEXTURES
                half4 emission = _EmissionMap.Sample(general_linear_clamp_sampler, emissionMapUV);
                #else
                half4 emission = _EmissionMap.Sample(general_linear_repeat_sampler, emissionMapUV);
                #endif
                // New code block end (Animated emission masks)
                ////////////////////////////////////////////////
                
                #ifdef EMISSIONMODE_CUSTOMVERTEXSTREAMS
                emission *= IN.texcoord2;
                #else
                emission *= _EmissionColor;
                #endif

                color *= _Color;
                //Apply vertex colors
                color *= IN.color;
                //Apply solved emission
                color.rgb += emission.rgb * (blendIsAlphaDependent ? color.a : color.r);
                
                #ifdef ALPHAMASKINGMODE_UV
                half maskedAlpha = saturate(dot(normalize(_AlphaMaskDirection.xy), IN.texcoord1.xy));
                #elif defined(ALPHAMASKINGMODE_TEXTURE)
                float2 alphaMaskUV = IN.texcoord1.xy;
                
                #if ALPHAMASKANIMMODE_TIME
                alphaMaskUV += _Time.y * _AlphaMaskAnimParams.xy;
                #elif ALPHAMASKANIMMODE_TIMEPERIODIC
                alphaMaskUV += sin(_Time.y * _AlphaMaskAnimParams.z) * _AlphaMaskAnimParams.w;
                #endif
                
                half maskedAlpha = _AlphaMask.Sample(general_linear_repeat_sampler, alphaMaskUV);
                #elif defined(ALPHAMASKINGMODE_POSITION)
                half maskedAlpha = saturate(dot(normalize(_AlphaMaskDirection), IN.positionOS));
                #else
                half maskedAlpha = 1;
                #endif

                #ifdef SOFT_PARTICLES
                const float sceneEyeDepth = LinearEyeDepth(tex2D(_CameraDepthTexture, screenSpaceUvs),_ZBufferParams);
                maskedAlpha *= saturate(smoothstep(_SoftParticlesThreshold, _SoftParticlesThreshold + _SoftParticlesFalloff, sceneEyeDepth - IN.texcoordSS.z));
                #endif

                
                if(blendIsAlphaDependent)
                    color.a *= maskedAlpha;
                else
                    color *= maskedAlpha * color.a;
                
                #ifdef DISTORTION
                float3 backgroundDistortion = UnpackNormal(_DistortMap.Sample(general_linear_repeat_sampler, mainTexDistortUVs));
                half4 background = tex2D(_CameraOpaqueTexture, screenSpaceUvs + backgroundDistortion.xy * _DistortionStrength) + emission;
                if(blendIsAlphaDependent)
                    background.a *= maskedAlpha;
                else
                    background *= maskedAlpha * color.a;
                background *= IN.color;
                return lerp(color, background, _DistortionBlend);
                #else
                return color;
                #endif
            }
            ENDHLSL
        }
    }
    CustomEditor "ParticlesUberCustomEditor"
}