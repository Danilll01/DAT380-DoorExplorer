Shader "Unlit/PortalUnlit"
{
    Properties
    {
        //_MainTex ("Texture", 2D) = "white" {}
        _InactiveColour ("Inactive Colour", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { 
            "RenderType"="Opaque" 
            "Queue" = "Geometry"
            "RenderPipeline" = "UniversalPipeline"
        }
        //LOD 100
        Cull Off
        
        HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        ENDHLSL
        
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // make fog work
            //#pragma multi_compile_fog
            //#include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                //float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                //float2 uv : TEXCOORD0;
                //UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD0;
            };

            sampler2D _MainTex;
            //float4 _MainTex_ST;
            float4 _InactiveColour;
            int displayMask; // set to 1 to display texture, otherwise will draw test colour

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.screenPos = ComputeScreenPos(o.vertex);
                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float2 uv = i.screenPos.xy / i.screenPos.w;
                float4 portalCol = tex2D(_MainTex, uv);
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
                return portalCol * displayMask + _InactiveColour * (1-displayMask);
            }
            ENDHLSL
        }
    }
}
