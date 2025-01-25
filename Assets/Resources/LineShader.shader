// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Torreng/LineShader"
{
    Properties
    {
        _IsRainbow ("IsRainbow ", Float) = 0
		_Color1 ("Color1", Color) = (1,1,1,1) 
        _Color2 ("Color2", Color) = (1,1,1,1) 
        _Color3 ("Color3", Color) = (1,1,1,1) 
        _Color4 ("Color4", Color) = (1,1,1,1) 
        _Color5 ("Color5", Color) = (1,1,1,1) 
        _Color6 ("Color6", Color) = (1,1,1,1) 
        _Color7 ("Color7", Color) = (1,1,1,1) 
        _Color8 ("Color8", Color) = (1,1,1,1)
        _Color9 ("Color9", Color) = (1,1,1,1) 
        _Color10 ("Color10", Color) = (1,1,1,1) 
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

            float4 colorArray[10];
            const float pi = 3.141592653589793238462;
            const float pi2 = 6.283185307179586476924;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            float _IsRainbow;
            float4 _Color1;
            float4 _Color2;
            float4 _Color3;
            float4 _Color4;
            float4 _Color5;
            float4 _Color6;
            float4 _Color7;
            float4 _Color8;
            float4 _Color9;
            float4 _Color10;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0)).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                colorArray[0] = _Color1;
                colorArray[1] = _Color2;
                colorArray[2] = _Color3;
                colorArray[3] = _Color4;
                colorArray[4] = _Color5;
                colorArray[5] = _Color6;
                colorArray[6] = _Color7;
                colorArray[7] = _Color8;
                colorArray[8] = _Color9;
                colorArray[9] = _Color10;

                if (_IsRainbow == 0) 
                {
                    return _Color1;
                } 
                else 
                {
                    float pi = 3.1415926;
                    float twoPi = 2 * pi;
                    float angleRad = acos(normalize(i.worldPos).x);
                    angleRad += _Time.z;

                    while (angleRad > twoPi) 
                    {
                        angleRad -= twoPi;
                    }

                    float indexFloat = (angleRad * 10) / twoPi;
                    int indexLower = floor(indexFloat) % 10;
                    int indexUpper = ceil(indexFloat) % 10;
                    float t = indexFloat - indexLower;
                    float4 col = lerp(colorArray[indexLower], colorArray[indexUpper], t);

                    return col;
                }
            }
            ENDCG
        }
    }
}
