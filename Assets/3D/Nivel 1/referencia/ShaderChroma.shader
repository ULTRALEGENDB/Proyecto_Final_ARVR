Shader "UI/ChromaKeyGreen"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _KeyColor ("Color a eliminar", Color) = (0, 1, 0, 1)
        _Tolerance ("Tolerancia", Range(0, 1)) = 0.25
        _Softness ("Suavizado", Range(0, 1)) = 0.08
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 _KeyColor;
            float _Tolerance;
            float _Softness;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;

                float distanceToKey = distance(col.rgb, _KeyColor.rgb);

                float alpha = smoothstep(_Tolerance, _Tolerance + _Softness, distanceToKey);

                col.a *= alpha;

                return col;
            }
            ENDCG
        }
    }
}