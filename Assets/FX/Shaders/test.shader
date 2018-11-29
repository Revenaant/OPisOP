// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Shield"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	_Color("Color", Color) = (1, 1, 1, 1)
		//outline
		_OutlineColor("Outline Color", Color) = (1,1,1,1)
		_Outline("Outline width", float) = 1
		_EdgeColor("Edge Color", Color) = (1, 1, 1, 1)
		_DepthFactor("Depth Factor", float) = 1.0
		_Transparency("Transparency", Range(0.0,1.0)) = 0.5
		_Speed("Speed", Range(0.0,5.0)) = 2.5
		_Distance("Distance", float) = 1
		_CutoutThresh("CutoutThreshold", Range(0.0,2.0)) = 0.5
		_TintColor("TintColor", Color) = (1,1,1,1)
		_Amplitude("Amplitude",float) = 1
		_uvMove("uvMove",float) = 1
	}
		SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 100
		Cull Off
		ZWrite Off
		Blend OneMinusDstColor One
		//Blend SrcAlpha OneMinusSrcAlpha

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag


		// make fog work
#pragma multi_compile_fog

#include "UnityCG.cginc"
		sampler2D _CameraDepthTexture;

	struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		UNITY_FOG_COORDS(1)
			float4 vertex : SV_POSITION;
		float4 screenPos : TEXCOORD1;
		float4 color : COLOR;
	};

	sampler2D _MainTex;
	uniform float4 _MainTex_ST;
	uniform float _DepthFactor;
	uniform float4 _Color;
	uniform float4 _EdgeColor;
	uniform float _Transparency;
	uniform float _Speed;
	uniform float _Distance;
	uniform float _CutoutThresh;
	uniform float4 _TintColor;
	uniform float _Amplitude;
	uniform float _uvMove;
	uniform float _Outline;
	uniform float4 _OutlineColor;

	v2f vert(appdata v)
	{

		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		float3 uv = mul((float3x3)UNITY_MATRIX_IT_MV, v.uv);
		float2 offset = TransformViewToProjection(uv.xy);
		v.vertex.y += tan(_Time.y * _Speed * v.vertex.y * _Amplitude) * _Distance * 1;
		// convert obj-space position to camera clip space
		o.vertex = UnityObjectToClipPos(v.vertex);
		// compute depth (screenPos is a float4)
		o.screenPos = ComputeScreenPos(o.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		o.uv.x += sin(_Time.x * _uvMove);
		o.uv.y += sin(_Time.x * _uvMove);
		o.vertex.xy += offset * o.vertex.z * _Outline;
		o.color = _OutlineColor;
		//UNITY_TRANSFER_FOG(o,o.vertex);
		return o;

	}

	fixed4 frag(v2f i) : SV_Target
	{

		float4 depthSample = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, i.screenPos);
		float depth = LinearEyeDepth(depthSample).r;
		float foamLine = 1 - saturate(_DepthFactor * (depth - i.screenPos.w));
		//float4 color = _Color + foamLine * _EdgeColor + col;
		// sample the texture
		fixed4 col = tex2D(_MainTex, i.uv) + (_Color + foamLine * _EdgeColor) + _TintColor;
		col.a = _Transparency;
		clip(col.g - _CutoutThresh);
		// apply fog
		//UNITY_APPLY_FOG(i.fogCoord, col);
		//float4 col = float4(depth, depth, depth, 1);

		return col;
	}
		ENDCG
	}
	}
}
