// Obra viva (tela de particulas): imagen por color de vertice, y el fondo de la pintura mas transparente
// que las figuras (mascara = el recorte de Assets/Resources/Cortes), asi las figuras quedan paradas en el paisaje.
Shader "Goya/ObraViva"
{
    Properties
    {
        _MainTex ("Obra", 2D) = "white" {}
        _Mask ("Recorte de las figuras", 2D) = "white" {}
        _HasMask ("Usa recorte", Float) = 0
        _Bg ("Opacidad del fondo", Range(0, 1)) = 0.3
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _Mask;
            float _HasMask;
            float _Bg;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; float2 uv2 : TEXCOORD1; float4 color : COLOR; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; float2 uv2 : TEXCOORD1; float4 color : COLOR; };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.uv2 = v.uv2;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, i.uv) * i.color;
                float m = _HasMask > 0.5 ? tex2D(_Mask, i.uv2).a : 1.0;
                c.a *= lerp(_Bg, 1.0, m);
                return c;
            }
            ENDCG
        }
    }
}
