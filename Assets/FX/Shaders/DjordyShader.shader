Shader "UI/DjordyShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}

		_redChannel("Highlight", Color) = (1, 0, 0, 1)
		_blueChannel("Shadow", Color) = (0, 0, 1, 1)
		_greenChannel("Cast Shadow", Color) = (0, 1, 0, 1)

		_Transparency("Transparency", Range(0.0,1.0)) = 1

		// required for UI.Mask
		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255
		_ColorMask("Color Mask", Float) = 15
	}
	SubShader
	{
		Tags { "Queue" = "Transparent" /*"IgnoreProjector" = "True"*/ "RenderType" = "Transparent" }
		LOD 100


		 //required for UI.Mask
		Stencil
		{
			Ref[_Stencil]
			Comp[_StencilComp]
			Pass[_StencilOp]
			ReadMask[_StencilReadMask]
			WriteMask[_StencilWriteMask]
		}
		ColorMask[_ColorMask]

		Cull Off
		Lighting Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		//Offset -1, -1

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			//struct appdata
			//{
			//	float4 vertex : POSITION;
			//	float2 uv : TEXCOORD0;
			//};

			//struct v2f
			//{
			//	float2 uv : TEXCOORD0;
			//	UNITY_FOG_COORDS(1)
			//	float4 vertex : SV_POSITION;
			//};

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				half2 texcoord : TEXCOORD0;
			};

			float _Transparency;

			sampler2D _MainTex;
			float4 _MainTex_ST;

			float4 _redChannel;
			float4 _greenChannel;
			float4 _blueChannel;

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// sample the texture
				fixed4 color = tex2D(_MainTex, i.texcoord);
				float alpha = color.a;

				if (color.r > 0.1f) color = _redChannel;
				else if (color.b > 0.1f) color = _blueChannel;
				else if (color.g > 0.1f) color = _greenChannel;

				color.a *= alpha * _Transparency;
				//color = clamp(color, 0, 1);
				return color;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
