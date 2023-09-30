Shader "Unlit/VertexColorAlpha"
{
    Properties
    {
        _Fade ("Fade", Range(0.0,1.0)) = 1
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "IgnoreProjector" = "True"  "RenderType"="Transparent" }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float _Fade;

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
                o.vertexColor.a *= _Fade;

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
