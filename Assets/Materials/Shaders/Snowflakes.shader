Shader "UI/Unlit/Snowflakes"
{
    Properties
    {
        _MainTex ("Background (optional)", 2D) = "white" {}
        _Color ("Snow Color", Color) = (1,1,1,1)
        _Density ("Density (0-1)", Range(0,1)) = 0.25
        _Speed ("Global Speed", Range(0,5)) = 1.0
        _Size ("Base Size", Range(0.002,0.2)) = 0.03
        _SizeVariation ("Size Variation", Range(0,1)) = 0.5
        _Softness ("Softness (edge)", Range(0.0,1.0)) = 0.35
        _Layers ("Layers", Range(1,8)) = 4
        _Seed ("Seed", Float) = 0.0
        _Blend ("Blend (0=alpha,1=add)", Range(0,1)) = 0.0
        _UseMainTex ("Use Background Texture", Float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" "PreviewType"="Plane" }
        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _Color;
            float _Density;
            float _Speed;
            float _Size;
            float _SizeVariation;
            float _Softness;
            int _Layers;
            float _Seed;
            float _Blend;
            float _UseMainTex;

            // Hash / random
            float hash21(float2 p) {
                p = frac(p * float2(123.34, 456.21) + _Seed);
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            // rotate 2D
            float2 rot(float2 p, float a) {
                float c = cos(a), s = sin(a);
                return float2(p.x*c - p.y*s, p.x*s + p.y*c);
            }

            // simple smooth circle for a flake
            float snowflakeShape(float2 uv, float radius, float softness) {
                float d = length(uv);
                float k = smoothstep(radius, radius * (1.0 - softness), d);
                return 1.0 - k;
            }

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            // Build one layer of snow: multiple flakes arranged by grid + jitter
            float layerSnow(float2 uv, float time, float layerIndex, out float alphaOut)
            {
                // scale grid per layer (farther layers have finer grid)
                float layerScale = lerp(10.0, 40.0, saturate(layerIndex / max(1.0, _Layers-1)));
                float2 g = uv * layerScale;

                // vertical offset (falling) depends on time & layerIndex
                float speedFactor = lerp(0.5, 1.6, layerIndex / max(1.0, _Layers-1));
                float yShift = time * _Speed * speedFactor;

                // move grid vertically and tile
                g.y += yShift;

                // cell base
                float2 cell = floor(g);
                float2 fracPos = frac(g);

                // jitter seed per cell
                float n = hash21(cell + layerIndex*13.13);

                // flake position inside cell
                float2 jitter = float2(hash21(cell + 0.17), hash21(cell + 2.71)) - 0.5;
                float2 flakePos = fracPos + jitter * 0.6;

                // size variation
                float sizeRand = lerp(_Size * 0.6, _Size * 1.6, hash21(cell + 7.3));
                sizeRand *= lerp(1.0, 1.0 + _SizeVariation, hash21(cell + 9.9));

                // rotation for shape
                float rotAngle = hash21(cell + 4.2) * 6.2831853;
                float2 p = flakePos - 0.5;
                p = rot(p, rotAngle);

                // shape and alpha
                float s = snowflakeShape(p, sizeRand, _Softness);
                // fade by vertical position inside cell (gives sense of depth / falling)
                float fade = smoothstep(0.0, 1.0, fracPos.y);
                s *= fade;

                // occasional flake missing based on density control
                float presence = step(1.0 - _Density * 1.2, n); // higher density -> more present (presence 1 when n >= threshold)
                s *= presence;

                alphaOut = s;
                return s;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // use time
                float t = _Time.y;

                // background
                fixed4 bg = tex2D(_MainTex, uv);
                if (_UseMainTex < 0.5) bg = fixed4(0,0,0,0);

                float accum = 0.0;
                float accumAlpha = 0.0;

                // iterate layers
                for (int li = 0; li < _Layers; ++li)
                {
                    float layerIndex = (float)li;
                    float alpha;
                    float s = layerSnow(uv, t + layerIndex * 0.13, layerIndex, alpha);

                    // layer weighting: farther layers are dimmer and slower
                    float layerWeight = lerp(1.0, 0.45, layerIndex / max(1.0, _Layers-1));
                    float layerAccum = s * layerWeight;

                    // compositing: accumulate alpha-like
                    accum += layerAccum;
                    accumAlpha = accum + accumAlpha * (1.0 - accum); // approximate over blending
                    // small optimization: break early if too dense
                    if (accum > 3.0) break;
                }

                // apply snow color
                fixed4 snowCol = _Color;
                snowCol.a = saturate(accumAlpha);

                // final blending mode
                fixed4 outCol;
                if (_Blend < 0.5)
                {
                    // alpha blend over background
                    outCol.rgb = lerp(bg.rgb, snowCol.rgb, snowCol.a);
                    outCol.a = max(bg.a, snowCol.a);
                }
                else
                {
                    // additive-like
                    outCol.rgb = bg.rgb + snowCol.rgb * snowCol.a;
                    outCol.a = min(1.0, bg.a + snowCol.a);
                }

                return outCol;
            }
            ENDCG
        }
    }
    FallBack "UI/Default"
}
