Shader "Custom/Instanced Toon Outline"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _ShadowColor ("Shadow Color", Color) = (0.45,0.45,0.45,1)
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Float) = 0.03
        _LightDir ("Fake Light Direction", Vector) = (0.3, 1, 0.4, 0)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }

        // OUTLINE PASS
        Pass
        {
            Name "Outline"
            Cull Front
            ZWrite Off
            ZTest LEqual

            CGPROGRAM
            #include "UnityCG.cginc"

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            float _OutlineWidth;
            fixed4 _OutlineColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                UNITY_SETUP_INSTANCE_ID(v);

                v2f o;

                float3 expanded = v.vertex.xyz + v.normal * _OutlineWidth;
                o.pos = UnityObjectToClipPos(float4(expanded, 1));

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _OutlineColor;
            }

            ENDCG
        }

        // MAIN TOON PASS
        Pass
        {
            Name "Toon"
            Cull Back
            ZWrite On
            ZTest LEqual

            CGPROGRAM
            #include "UnityCG.cginc"

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            fixed4 _Color;
            fixed4 _ShadowColor;
            float4 _LightDir;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                UNITY_SETUP_INSTANCE_ID(v);

                v2f o;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 normal = normalize(i.worldNormal);
                float3 lightDir = normalize(_LightDir.xyz);

                float lightAmount = dot(normal, lightDir);
                lightAmount = lightAmount * 0.5 + 0.5;

                // hard cartoon step
                float toon = lightAmount > 0.55 ? 1.0 : 0.45;

                fixed4 col = lerp(_ShadowColor, _Color, toon);
                return col;
            }

            ENDCG
        }
    }
}