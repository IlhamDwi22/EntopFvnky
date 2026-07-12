Shader "Custom/KamuiDistortion"
{
    Properties
    {
        _Center ("Center (XY)", Vector) = (0.5, 0.5, 0, 0)
        _Radius ("Radius", Range(0, 1)) = 0.5
        _Angle ("Angle (Twist)", Range(-50, 50)) = 0.0
        _Progress ("Progress (Suck/Fade)", Range(0, 1)) = 0.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent+100" "RenderType"="Transparent" }
        
        GrabPass
        {
            "_GrabTexture"
        }

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
                float4 vertex : SV_POSITION;
                float4 grabPos : TEXCOORD0;
            };

            sampler2D _GrabTexture;
            float4 _Center;
            float _Radius;
            float _Angle;
            float _Progress;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.grabPos.xy / i.grabPos.w;
                float2 center = _Center.xy;
                float2 dir = uv - center;
                float dist = length(dir);

                if (dist < _Radius)
                {
                    // Menghitung putaran spiral (Kamui Twist)
                    float percent = (_Radius - dist) / _Radius;
                    float theta = percent * percent * _Angle;
                    
                    float s = sin(theta);
                    float c = cos(theta);
                    
                    float2 rotatedDir;
                    rotatedDir.x = dir.x * c - dir.y * s;
                    rotatedDir.y = dir.x * s + dir.y * c;
                    
                    // Efek hisapan masuk ke tengah
                    rotatedDir *= (1.0 - _Progress * percent);
                    uv = center + rotatedDir;
                }

                fixed4 color = tex2D(_GrabTexture, uv);
                
                // Perlahan menggelapkan bagian tengah yang terhisap menjadi hitam
                if (dist < _Radius * _Progress)
                {
                    float fade = dist / (_Radius * _Progress);
                    color = lerp(fixed4(0,0,0,1), color, saturate(fade));
                }

                return color;
            }
            ENDCG
        }
    }
}
