Shader "Custom/UnitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineThickness ("Thickness", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }

        Cull Off
        ZWrite Off
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
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            float4 _Color;
            float4 _OutlineColor;
            float _OutlineThickness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv) * i.color;

                float alpha = col.a;

                // Only calculate outline where sprite is transparent
                if (alpha > 0.01)
                    return col;

                float2 texel = _MainTex_TexelSize.xy * _OutlineThickness;

                float outline =
                    tex2D(_MainTex, i.uv + float2(texel.x, 0)).a +
                    tex2D(_MainTex, i.uv - float2(texel.x, 0)).a +
                    tex2D(_MainTex, i.uv + float2(0, texel.y)).a +
                    tex2D(_MainTex, i.uv - float2(0, texel.y)).a;

                if (outline > 0)
                    return _OutlineColor;

                return float4(0,0,0,0);
            }
            ENDCG
        }
    }
}