// Toony Colors Pro+Mobile 2
// (c) 2014-2016 Jean Moreno



Shader "Toony Colors Pro 2/User/My TCP2 Shader"
{
	Properties
	{
	[TCP2HeaderHelp(BASE, Base Properties)]
		//TOONY COLORS
		_Color ("Color", Color) = (0.5,0.5,0.5,1.0)
		_HColor ("Highlight Color", Color) = (0.6,0.6,0.6,1.0)
		_SColor ("Shadow Color", Color) = (0.3,0.3,0.3,1.0)
		
		//DIFFUSE
		_MainTex ("Main Texture (RGB)", 2D) = "white" {}
	[TCP2Separator]
		
		//TOONY COLORS RAMP
		_RampThreshold ("Ramp Threshold", Range(0,1)) = 0.5
		_RampSmooth ("Ramp Smoothing", Range(0.001,1)) = 0.1
	[TCP2Separator]
		
		//Avoid compile error if the properties are ending with a drawer
		[HideInInspector] __dummy__ ("unused", Float) = 0
	}
	
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		
		CGPROGRAM
		
		#include "../../Shaders 2.0/Include/TCP2_Include.cginc"
		#pragma surface surf ToonyColors noforwardadd
		#pragma target 3.0
		
		
		//================================================================
		// VARIABLES
		
		fixed4 _Color;
		sampler2D _MainTex;
		
		
		struct Input
		{
			half2 uv_MainTex;
		};
		




		//================================================================
		// SURFACE FUNCTION
		
		void surf (Input IN, inout SurfaceOutput o)
		{
			fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
			


			o.Albedo = mainTex.rgb * _Color.rgb;
			o.Alpha = mainTex.a * _Color.a;
			
			
		}
		
		ENDCG

	}
	
	Fallback "Diffuse"
	CustomEditor "TCP2_MaterialInspector_SG"
}
