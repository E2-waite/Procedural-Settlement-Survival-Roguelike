Shader "Custom/Cheap Instanced Toon"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _ShadowColor ("Shadow Color", Color) = (0.45,0.45,0.45,1)
        _LightDir ("Light Direction", Vector) = (0.3, 1, 0.4, 0)
        _Steps ("Steps", Range(1, 4)) = 2
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }

        Pass
        {
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
            float _Steps;

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

                float ndotl = dot(normal, lightDir);
                ndotl = ndotl * 0.5 + 0.5;

                float toon = floor(ndotl * _Steps) / max(1, _Steps - 1);
                toon = saturate(toon);

                return lerp(_ShadowColor, _Color, toon);
            }

            ENDCG
        }
    }
}