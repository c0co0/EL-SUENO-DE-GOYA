// Las Pinturas Negras por dentro (GoyaDiorama) con volumen: la pintura sobre una forma inflada.
// El contorno sale de la transparencia de la imagen, la luz y sombra del color de cada punto,
// y la aparicion, la niebla y el desvanecerse al atravesarlas del _Color.
// _DetailMap: mapa de normales (rg, el relieve de cada pincelada) y de rugosidad (a, donde el oleo brilla),
// que GoyaDiorama saca de la misma pintura. La luz viene de arriba a la izquierda, de adelante.
Shader "Goya/Volumen"
{
    Properties
    {
        _MainTex ("Obra", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _DetailMap ("Normales (rg) y rugosidad (a)", 2D) = "gray" {}
        _Bump ("Relieve de las pinceladas", Float) = 1
        _DetailLight ("Cuanto marca la luz el relieve", Float) = 0.9
        _Shine ("Brillo del oleo", Float) = 0.35
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
            sampler2D _MainTex, _DetailMap;
            fixed4 _Color;
            float _Bump, _DetailLight, _Shine;
            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; float4 color : COLOR; float3 normal : NORMAL; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; fixed4 color : COLOR; float3 n : TEXCOORD1; float3 v : TEXCOORD2; };
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color * _Color;
                o.n = v.normal;                      // todo en el espacio de la capa (la forma da la luz grande; el mapa, la fina)
                o.v = ObjSpaceViewDir(v.vertex);
                return o;
            }
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 t = tex2D(_MainTex, i.uv);
                clip(t.a - 0.35);   // el borde exacto de la figura
                float4 d = tex2D(_DetailMap, i.uv);
                float3 L = normalize(float3(-0.5, 0.6, -0.6));
                float3 V = normalize(i.v);
                float3 Ng = normalize(i.n);
                if (dot(Ng, V) < 0) Ng = -Ng;
                // La normal de la forma, inclinada por el relieve de la pincelada (las capas miran de frente: x e y de la capa)
                float3 Nd = normalize(Ng + float3(d.rg * 2 - 1, 0) * _Bump);
                float diff = 1 + (saturate(dot(Nd, L)) - saturate(dot(Ng, L))) * _DetailLight;
                // Rugosidad: lo liso brilla con un reflejo chico y fuerte; lo aspero casi no brilla
                float rough = d.a;
                float spec = pow(saturate(dot(Nd, normalize(L + V))), lerp(80, 6, rough)) * (1 - rough) * _Shine;
                return fixed4(t.rgb * i.color.rgb * diff + spec * i.color.rgb, i.color.a);
            }
            ENDCG
        }
    }
}
