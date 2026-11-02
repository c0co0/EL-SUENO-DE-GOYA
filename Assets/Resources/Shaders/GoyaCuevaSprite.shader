// Las obras colgadas en la cueva: como un sprite comun, pero iluminadas solo por la antorcha.
Shader "Goya/CuevaSprite"
{
    Properties { [PerRendererData] _MainTex ("Sprite", 2D) = "white" {} }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" "PreviewType" = "Plane" "CanUseSpriteAtlas" = "True" }
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
            float4 _GoyaTorch, _GoyaTorchColor, _GoyaCaveAmbient;
            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; float4 color : COLOR; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; float4 color : COLOR; float3 world : TEXCOORD1; };
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                o.world = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }
            fixed4 frag (v2f i) : SV_Target
            {
                float d = distance(i.world, _GoyaTorch.xyz) / max(0.01, _GoyaTorch.w);
                float l = saturate(1 - d); l *= l;
                fixed3 light = _GoyaCaveAmbient.rgb + _GoyaTorchColor.rgb * l;
                fixed4 t = tex2D(_MainTex, i.uv) * i.color;
                return fixed4(t.rgb * light, t.a);
            }
            ENDCG
        }
    }
}
