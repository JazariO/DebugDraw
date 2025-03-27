Shader "DebugDrawer/DebugDraw"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _ZTest ("ZTest", Float) = 4     // ZTest LessEqual
        _ZWrite ("ZWrite", Float) = 1   // ZWrite On
    }
    SubShader
    {
        Tags { "RenderType"="Overlay" "Queue"="Transparent" }
        
        Pass
        {
            Name "DebugDraw"
            Blend One Zero
            Cull Off

            // Use the properties for ZTest and ZWrite
            ZWrite [_ZWrite]
            ZTest [_ZTest]

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            UNITY_INSTANCING_BUFFER_START(PerInstanceData)
                UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
            UNITY_INSTANCING_BUFFER_END(PerInstanceData)

            struct appdata
            {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                fixed4 color = UNITY_ACCESS_INSTANCED_PROP(PerInstanceData, _Color);
                return fixed4(color.rgb, 1.0);
            }
            ENDCG
        }
    }

    Fallback "Diffuse"
}
