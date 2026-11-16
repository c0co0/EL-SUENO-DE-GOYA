// Las Pinturas Negras por dentro (GoyaDiorama) con volumen: la pintura sobre una forma inflada.
// El contorno sale de la transparencia de la imagen, la luz y sombra del color de cada punto,
// y la aparicion, la niebla y el desvanecerse al atravesarlas del _Color.
Shader "Goya/Volumen"
{
    Properties
    {
        _MainTex ("Obra", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        [HideInInspector] _ZWrite ("ZWrite", Float) = 1
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
        Cull Back
        ZWrite [_ZWrite]
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex;
            fixed4 _Color;
            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; float4 color : COLOR; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; fixed4 color : COLOR; };
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color * _Color;
                return o;
            }
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 t = tex2D(_MainTex, i.uv);
                clip(t.a - 0.35);   // el borde exacto de la figura
                return fixed4(t.rgb * i.color.rgb, i.color.a);
            }
            ENDCG
        }
    }
}
