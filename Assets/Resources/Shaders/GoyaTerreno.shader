// Terreno y pasto del camino: color por vertice (ya trae la niebla y el tinte de la epoca) por una textura de pinceladas.
// Opaco y con profundidad, asi las lomas tapan lo que esta detras y el arbol o la roca quedan enterrados en el piso.
Shader "Goya/Terreno"
{
    Properties
    {
        _MainTex ("Textura", 2D) = "white" {}
        _Cutoff ("Recorte (pasto)", Range(0, 1)) = 0.0
    }
    SubShader
    {
        Tags { "Queue" = "AlphaTest" "RenderType" = "TransparentCutout" "IgnoreProjector" = "True" }
        Cull Off
        ZWrite On
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Cutoff;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; float4 color : COLOR; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; float4 color : COLOR; };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 t = tex2D(_MainTex, i.uv);
                clip(t.a - _Cutoff);
                return fixed4(t.rgb * i.color.rgb, 1);
            }
            ENDCG
        }
    }
}
