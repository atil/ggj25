Shader "Torreng/Background"
{
    Properties
    {
        _Pattern ("Pattern", 2D) = "white" {}
        _Color1 ("Color1", Color) = (1, 1, 1, 1)
        _Color2 ("Color2", Color) = (1, 1, 1, 1)
        _SpeedCoeff("Speed Coeff", Float) = 1.0
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

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _Pattern;
            float4 _Pattern_ST;

            float4 _Color1;
            float4 _Color1_ST;

            float4 _Color2;
            float4 _Color2_ST;

            float _SpeedCoeff;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _Pattern);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 time = float2(-_Time.x, _Time.x) * _SpeedCoeff;
                fixed4 patternColor = tex2D(_Pattern, i.uv * 4 + time);
                float isColor1 = step(0.5, patternColor.a);
                return lerp(_Color1, _Color2, isColor1);
            }
            ENDCG
        }
    }
}
