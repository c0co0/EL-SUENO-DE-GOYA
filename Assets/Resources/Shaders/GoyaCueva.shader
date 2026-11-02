// Paredes y piso de la cueva: oscuros, salvo donde llega la luz de la antorcha (_GoyaTorch, la pone GoyaCave).
Shader "Goya/Cueva"
{
    Properties { _MainTex ("Textura", 2D) = "white" {} }
    SubShader
    {
        Tags { "Queue" = "Geometry" "RenderType" = "Opaque" }
        Cull Off
        ZWrite On
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex; float4 _MainTex_ST;
            float4 _GoyaTorch;        // xyz = posicion, w = alcance (metros)
            float4 _GoyaTorchColor;   // color por intensidad
            float4 _GoyaCaveAmbient;  // luz que hay sin antorcha
            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; float4 color : COLOR; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; float4 color : COLOR; float3 world : TEXCOORD1; };
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                o.world = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }
            fixed4 frag (v2f i) : SV_Target
            {
                float d = distance(i.world, _GoyaTorch.xyz) / max(0.01, _GoyaTorch.w);
                float l = saturate(1 - d); l *= l;
                fixed3 light = _GoyaCaveAmbient.rgb + _GoyaTorchColor.rgb * l;
                fixed4 t = tex2D(_MainTex, i.uv);
                return fixed4(t.rgb * i.color.rgb * light, 1);
            }
            ENDCG
        }
    }
}
