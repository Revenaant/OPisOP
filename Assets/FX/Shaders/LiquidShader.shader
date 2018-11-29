Shader "Unlit/SpecialFX/Liquid"
{
	Properties
	{
		_Tint("Tint", Color) = (1,1,1,1)
		_MainTex("Texture", 2D) = "white" {}
	_FillAmount("Fill Amount", Range(-10,10)) = 0.0
		[HideInInspector] _WobbleX("WobbleX", Range(-1,1)) = 0.0
		[HideInInspector] _WobbleZ("WobbleZ", Range(-1,1)) = 0.0
		_TopColor("Top Color", Color) = (1,1,1,1)
		_FoamColor("Foam Line Color", Color) = (1,1,1,1)
		_Rim("Foam Line Width", Range(0,0.1)) = 0.0
		_RimColor("Rim Color", Color) = (1,1,1,1)
		_RimPower("Rim Power", Range(0,10)) = 0.0
		_Transparency("Transparency", Range(0.0,1.0)) = 0.5
	}

		SubShader
	{
		Tags{ "Queue" = "Geometry" }

		Pass
	{
		Zwrite On
		Cull Off // we want the front and back faces
		AlphaToMask On // transparency

		CGPROGRAM


#pragma vertex vert
#pragma fragment frag
		// make fog work
#pragma multi_compile_fog

#include "UnityCG.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
		float3 normal : NORMAL;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		UNITY_FOG_COORDS(1)
			float4 vertex : SV_POSITION;
		float3 viewDir : COLOR;
		float3 normal : COLOR2;
		float fillEdge : TEXCOORD2;
	};

	sampler2D _MainTex;
	float4 _MainTex_ST;
	float _FillAmount;
	float4 _TopColor;
	float4 _RimColor;
	float4 _FoamColor;
	float4 _Tint;
	float _Rim;
	float _RimPower;


	v2f vert(appdata v)
	{
		v2f o;

		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		UNITY_TRANSFER_FOG(o,o.vertex);
		// get world position of the vertex
		float3 worldPos = mul(unity_ObjectToWorld, v.vertex.xyz);
		// how high up the liquid is
		o.fillEdge = worldPos.y + _FillAmount;

		return o;
	}

	fixed4 frag(v2f i, fixed facing : VFACE) : SV_Target
	{
		// sample the texture
		fixed4 col = tex2D(_MainTex, i.uv) * _Tint;
	// apply fog
	UNITY_APPLY_FOG(i.fogCoord, col);

	// rim light
	float dotProduct = 1 - pow(dot(i.normal, i.viewDir), _RimPower);
	float4 RimResult = smoothstep(0.5, 1.0, dotProduct);
	RimResult *= _RimColor;

	// foam edge
	float4 foam = (step(i.fillEdge, 0.5) - step(i.fillEdge, (0.5 - _Rim)));
	float4 foamColored = foam * (_FoamColor * 0.9);
	// rest of the liquid
	float4 result = step(i.fillEdge, 0.5) - foam;
	float4 resultColored = result * col;
	// both together, with the texture
	float4 finalResult = resultColored + foamColored;
	finalResult.rgb += RimResult;

	// color of backfaces/ top
	float4 topColor = _TopColor * (foam + result);
	//VFACE returns positive for front facing, negative for backfacing
	return facing > 0 ? finalResult : topColor;

	}
		ENDCG
	}

	}
}