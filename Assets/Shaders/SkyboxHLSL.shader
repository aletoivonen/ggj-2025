Shader "Unlit/SkyboxHLSL"
{
    Properties
    {
        _GradientTex ("Gradient", 2D) = "white" {}
        _TimeSpeed ("Time speed", Float) = 10
        _PosDivider ("Pos Divider", Float) = 1000010
        _ColorRamp ("Color ramp", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 screenPos : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _GradientTex;
            float4 _GradientTex_ST;
            float _TimeSpeed;
            float _PosDivider;
            fixed _ColorRamp;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _GradientTex);
                o.screenPos = ComputeScreenPos(o.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed timeFactor = frac(_Time * _TimeSpeed);
                fixed posFactor = i.screenPos.y / _PosDivider;

                fixed2 uv = fixed2(timeFactor + posFactor, 0);
                
                // sample the texture
                fixed4 col = tex2D(_GradientTex, uv) * _ColorRamp;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
