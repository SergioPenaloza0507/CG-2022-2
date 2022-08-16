Shader "URPCustoms/VariableFov"
{
    Properties
    {
        _Color ("Main Color", color) = (1,0.5,0,1)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"            

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float3 normalWS     : NORMAL;
            };

            half4 _Color;
            float4x4 _AltProjectionMatrix;
            
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = mul(_AltProjectionMatrix,float4(TransformWorldToView(TransformObjectToWorld(IN.positionOS.xyz)),1.0));
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                return _Color * dot(IN.normalWS, float3(-1,1,-1));
            }
            ENDHLSL
        }
    }
}