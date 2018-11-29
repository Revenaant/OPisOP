// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/DissolveShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_ColorTint("Tint", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

		_DissolveTex("Dissolve Tex", 2D) = "white" {}
		_DissolveScale("Dissolve Progression", Range(0.0, 1.0)) = 0.5
		_DissolveBand("Band Size", Float) = 0.5
		_DissolveStart("Dissolve Start", Vector) = (0, 0, 0, 0)
		_DissolveEnd("Dissolve End", Vector) = (1, 1, 1, 1)

		_Glow("Glow Color", Color) = (1, 1, 1, 1)
		_GlowEnd("Glow End Color", Color) = (1, 1, 1, 1)
		_GlowColFac("Glow Colorshift", Range(0.01, 2.0)) = 0.75
		_GlowIntensity("Glow Intensity", Range(0.0, 5.0)) = 0.05
		_GlowScale("Glow Size", Range(0.0, 5.0)) = 1.0

	}
	SubShader {
		Tags 
		{ 
			"Queue" = "Transparent"
			"RenderType" = "Fade" 
		}

		Pass{
			ColorMask 0
		}

		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha:fade vertex:vert finalcolor:mycolor

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float dGeometry;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		sampler2D _DissolveTex;
		half _DissolveScale;
		float3 _DissolveStart;
		float3 _DissolveEnd;
		float _DissolveBand;

		fixed4 _Glow;
		fixed4 _GlowEnd;
		half _GlowColFac;
		half _GlowIntensity;
		half _GlowScale;

		fixed4 _ColorTint;

		// Precompute dissolve direction 
		static float3 dDir = normalize(_DissolveEnd - _DissolveStart);

		// Precompute gradient start position
		static float3 dissolveStartConverted = _DissolveStart - _DissolveBand * dDir;

		// Precompute reciprocal of band size 
		static float dBandFactor = 1.0f / _DissolveBand;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void mycolor(Input IN, SurfaceOutputStandard o, inout fixed4 color) {
			color *= _ColorTint;
		}

		void vert(inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);

			// Calculate geometry-based dissolve coefficient 
			// Compute top of dissolution gradient according to dissolve progression
			float3 dPoint = lerp(dissolveStartConverted, _DissolveEnd, _DissolveScale);

			// Project vector between current vertex and top of gradient onto dissolve direction
			// Scale coefficient by band(gradient) size
			o.dGeometry = dot(v.vertex - dPoint, dDir) * dBandFactor;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;

			// Convert dissolve progression to -1 to 1 scale
			half dBase = -2.0f * _DissolveScale + 1.0f;


			// Read from noise texture
			fixed4 dTex = tex2D(_DissolveTex, IN.uv_MainTex);
			// Convert dissolve texture sample based on dissolve progression
			half dTexRead = dTex.r + dBase;
			// Combine texture factor with geometry coefficient from vertex routine
			half dFinal = dTexRead + IN.dGeometry;

			// Shift the computed raw alpha value based on the scale factor of the glow
			// Scale the shifted value based on effect intensity
			half dPredict = (_GlowScale - dFinal) * _GlowIntensity;
			// Change colour interpolation by adding in another factor controlling the gradient
			half dPredictCol = (_GlowScale * _GlowColFac - dFinal) * _GlowIntensity;

			// Calculate and Clamp glow colour
			fixed4 glowCol = dPredict * lerp(_Glow, _GlowEnd, clamp(dPredictCol, 0.0f, 1.0f));
			glowCol = clamp(glowCol, 0.0f, 1.0f);
			o.Emission = glowCol;

			// Set output alpha value
			//half alpha = clamp(IN.dGeometry, 0.0f, 1.0f); // Linear Gradient fade
			half alpha = clamp(dFinal, 0.0f, 1.0f);
			o.Alpha = alpha;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
