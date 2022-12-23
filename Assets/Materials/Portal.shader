Shader "Chameleon/Portal"{
	Properties{
		[MainTexture] matTexture("Texture",2D) = "white"{}
		[MainColor] matTint("Tint",Color) = (1,1,1,1)
		[MatVector2] matSpeed("Speed",Vector) = (1,1,1,1)
		matEmission("Emission",float) = 1
		[Toggle] matZWrite("ZWrite",float) = 1
		[MatInt] matPerlinOctave("Perlin Octave",float) = 1
		matDeltaColor("Delta Color",float) = 3
	}
	SubShader{
		LOD 100
		Tags{"Queue"="Transparent"}
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite [matZWrite] //Credit: ian_unity431, UF

		Pass{
			Name "Main"

			HLSLPROGRAM
			#pragma vertex vertexShader
			#pragma fragment fragmentShader
			#define BUILTIN_SHADER
			#include "UnityCG.cginc"
			//for Universal Render Pipelines
			//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.chameleonplayground.usefulscripts/UsefulScripts/chm_ShaderHelper.hlsl"

			struct VertToFrag{
				float4 clipPos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			sampler2D matTexture;
			float4 matTexture_ST;
			half4 matTint;
			float2 matSpeed;
			float matEmission;
			int matPerlinOctave;
			float matDeltaColor;

			VertToFrag vertexShader(
				float4 objectPos : POSITION,
				float2 uv : TEXCOORD0
			){
				VertToFrag v2f;
				v2f.clipPos = UnityObjectToClipPos(objectPos);
				//For Universal Render Pipelines
				//float3 worldPos = TransformObjectToWorld(objectPos.xyz);
				//v2f.clipPos = TransformWorldToHClip(worldPos);
				v2f.uv = uv*matTexture_ST.xy + matTexture_ST.zw + matSpeed*_Time.y;
				return v2f;
			}
			half4 fragmentShader(
				VertToFrag v2f
			) : SV_TARGET
			{
				float noise = perlinNoise(v2f.uv,matPerlinOctave); //[-1,1]
				return tex2D(matTexture,v2f.uv) * matTint * (0.5+noise/matDeltaColor) * matEmission;
			}
			ENDHLSL
		}
	}
}
