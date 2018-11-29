Shader "Custom/Fresnel2" 
{Properties{
	_MainTex("Texture", 2D) = "white" {}
_Cube("Cubemap", CUBE) = "" {}
}
SubShader{
	Tags{ "RenderType" = "Opaque" }
	CGPROGRAM
#pragma surface surf Lambert  
	struct Input {
	float2 uv_MainTex;
	float3 worldRefl;
	float3 viewDir;
};
sampler2D _MainTex;
samplerCUBE _Cube;
void surf(Input IN, inout SurfaceOutput o) {
	o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb * 0.5;
	// FRESNEL CALCS 
	//float fcbias = 0.20373;
	//float facing = saturate(1.0 - max(dot(normalize(IN.viewDir.xyz), normalize(o.Normal)), 0.0));
	//float refl2Refr = max(fcbias + (1.0 - fcbias) * pow(facing, _FresnelPower), 0);
	o.Emission = texCUBE(_Cube, IN.worldRefl).rgb * 1;
}
ENDCG
}
Fallback "Diffuse"
}