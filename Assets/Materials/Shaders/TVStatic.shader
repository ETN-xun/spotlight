Shader "UI/Unlit/TVStatic_Dynamic"
{
    Properties
    {
        _Brightness("Brightness", Range(0,2)) = 1
        _Contrast("Contrast", Range(0,2)) = 1
        _Speed("Noise Speed", Range(0,10)) = 4
        _ScanlineStrength("Scanline Strength", Range(0,1)) = 0.3
        _Distortion("Horizontal Distortion", Range(0,0.5)) = 0.1
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Cull Off ZWrite Off Lighting Off
        Blend SrcAlpha OneMinusSrcAlpha

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
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float _Brightness;
            float _Contrast;
            float _Speed;
            float _ScanlineStrength;
            float _Distortion;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // 改进的随机函数：基于像素坐标 + 时间
            float random(float2 p)
            {
                return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // 模拟水平失真
                uv.x += sin(uv.y * 800.0 + _Time.y * 10.0) * _Distortion;

                // 转成更大范围坐标（保证每像素不同）
                float2 pixelUV = uv * _ScreenParams.xy;

                // 噪声，随时间变化
                float timeSeed = floor(_Time.y * _Speed * 60.0);
                float n = random(pixelUV + timeSeed);

                // 黑白对比
                n = (n - 0.5) * _Contrast + 0.5;
                n *= _Brightness;

                // 水平扫描线亮度 modulation
                float scanline = sin(uv.y * _ScreenParams.y * 1.0) * 0.5 + 0.5;
                n *= lerp(1.0, scanline, _ScanlineStrength);

                return fixed4(n, n, n, 1);
            }
            ENDCG
        }
    }
    FallBack "UI/Default"
}
