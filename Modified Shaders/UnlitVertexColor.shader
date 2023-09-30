Shader "Unlit/VertexColor"
{
    Properties
    {
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
                fixed4 color : COLOR;

                UNITY_VERTEX_INPUT_INSTANCE_ID // required to render properly in virtual reality
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 vertexColor : COLOR;

                UNITY_VERTEX_OUTPUT_STEREO // required to render properly in virtual reality
            };

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v); // required to render properly in virtual reality
                UNITY_INITIALIZE_OUTPUT(v2f, o); // required to render properly in virtual reality
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); // required to render properly in virtual reality

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.vertexColor = v.color;
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return i.vertexColor;
            }
            ENDCG
        }
    }
}
