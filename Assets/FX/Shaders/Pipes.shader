Shader "Unlit/Pipes"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_uvMove ("uvMove", Range(0.0,1.0)) = 0.25
		_CutoutThresh("CutoutThreshold", Range(0.0,2.0)) = 0.5
		_Color ("Color", color) = (0,0,0,0)
		_Force ("Force", Range(0.0,50.0)) = 2.5
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
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
				//float3 worldPos;
				//float3 worldNorm;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
			};

			sampler2D _MainTex;
			uniform float4 _MainTex_ST,_Color;
			uniform float _uvMove, _CutoutThresh, _Force;
			
			
			v2f vert (appdata v)
			{
				v2f o;
				//UNITY_INITIALIZE_OUTPUT(VertexOutput, o);
				v.vertex.xyz += v.normal *_Force;
				half3 normal = UnityObjectToWorldNormal(v.normal);

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv.y += (_Time.y * _uvMove);
				//float3 worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)).xyz;
				//float3 worldNorm = UnityObjectToWorldNormal(v.normal);
				//worldPos += worldNormal * _Force;
				//v.vertex.xyz = mul(unity_WorldToObject, float4(worldPos, 1)).xyz;
				

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv );
				clip(col.r - _CutoutThresh);
				col = _Color;
				
				return col;
			}
			ENDCG
		}
	}
}
