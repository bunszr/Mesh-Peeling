Shader "Custom/TwoPassCutoutShader"
{
    Properties
    {
        [PerRendererData]_FrontColor ("FrontColor", Color) = (1, 1, 1, 1)
        [PerRendererData]_FrontTex ("FrontTex", 2D) = "white" {}
        [PerRendererData]_BackColor ("BackColor", Color) = (1, 1, 1, 1)
        [PerRendererData]_BackTex ("BackTex", 2D) = "white" {}
        
        _Cutoff ("Cutoff", float) = 0.05
    }
 
    SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" }

Cull Back

        CGPROGRAM
        #include "UnityStandardUtils.cginc"
        #pragma target 3.0
        #pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 

        struct Input
        {
            float2 uv_FrontTex;
            float2 clipUv;
        };

        uniform sampler2D _FrontTex;
        uniform float4 _FrontColor;
        uniform float _Cutoff;

        void vertexDataFunc( inout appdata_full v, out Input o )
        {
        	UNITY_INITIALIZE_OUTPUT( Input, o );
            o.clipUv = v.texcoord1;
            // v.normal *= 1;
        }

        void surf( Input i , inout SurfaceOutputStandard o )
        {
            o.Albedo = tex2D( _FrontTex, i.uv_FrontTex.xy ) * _FrontColor;
            o.Alpha = 1;
            clip( i.clipUv.y - _Cutoff );
        }
        ENDCG

Cull Front

        CGPROGRAM
        #include "UnityStandardUtils.cginc"
        #pragma target 3.0
        #pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
        
        struct Input
        {
            float2 uv_BackTex;
            float2 clipUv;
        };

        uniform sampler2D _BackTex;
        uniform float4 _BackColor;
        uniform float _Cutoff;

        void vertexDataFunc( inout appdata_full v, out Input o )
        {
        	UNITY_INITIALIZE_OUTPUT( Input, o );
            o.clipUv = v.texcoord1;
            v.normal *= -1;
        }

        void surf( Input i , inout SurfaceOutputStandard o )
        {
            o.Albedo = tex2D( _BackTex, i.uv_BackTex.xy ) * _BackColor;
            o.Alpha = 1;
            clip( i.clipUv.y - _Cutoff );
        }
        ENDCG
	}
	Fallback "Diffuse"
}