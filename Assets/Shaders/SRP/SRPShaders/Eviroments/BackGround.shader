Shader "W02/Environment/BackGround"
{
    Properties
    {
    	[Toggle(_ALPHATEST_ON)] _ALPHATEST_ON("Aplha Test", float) = 0
        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
        _BaseMap ("Cloud", 2D) = "black" {}
		//_BottomColor("底部颜色",Color)=(0.5,0.5,0.5,1)
		_UpColor("颜色",Color)=(1,1,1,1)
    	
		//_Power("渐变参数",Range(0.1,10))=1
    }
    SubShader
    {
        Tags { "RenderType"="BackGround" }
        LOD 100

        Pass
        {
            Name "Skybox"
			//Name "DepthOnly"
			//Tags{"LightMode" = "DepthOnly"}
			HLSLPROGRAM
			// Required to compile gles 2.0 with standard srp library
			//#pragma prefer_hlslcc gles
			//#pragma exclude_renderers d3d11_9x
			#pragma target 2.0
			#pragma shader_feature _ALPHATEST_ON
			#pragma vertex Vertex
			#pragma fragment Fragment

			// -------------------------------------
			// Material Keywords
			//#pragma shader_feature _ALPHATEST_ON
			//#pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			//#pragma multi_compile_instancing
			//#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
			//#include "GrassWind.hlsl"

			struct Attributes
			{
				float4 position     : POSITION;
				float2 texcoord     : TEXCOORD0;
				//UNITY_VERTEX_INPUT_INSTANCE_ID
			};
		
			

			struct Varyings
			{
				float2 uv           : TEXCOORD0;
				float4 positionCS   : SV_POSITION;
				float3 positionWS	: TEXCOORD1;
				//UNITY_VERTEX_INPUT_INSTANCE_ID
				//UNITY_VERTEX_OUTPUT_STEREO
			};

			sampler2D _BaseMap;
			float4 _BaseMap_ST;
            float _Cutoff;
			half4 _BottomColor;
			half4 _UpColor;
			float _GlobalSceneIntensity;
			float _Power;

			Varyings Vertex(Attributes input)
			{
				Varyings output = (Varyings)0;
				

				output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
				output.positionWS = TransformObjectToWorld(input.position.xyz);
				output.positionCS = TransformWorldToHClip(output.positionWS);
				return output;
			}

			half4 Fragment(Varyings input) : SV_TARGET
			{
				half4 cloudcol = tex2D(_BaseMap,input.uv.xy);
				_GlobalSceneIntensity = _GlobalSceneIntensity <= 0 ? 1 : _GlobalSceneIntensity;
				//float3 dir =normalize( GetCameraPositionWS() - input.positionWS);

				//float cosTerm=pow(max(0,dot(dir,half3(0,-1,0))),_Power);

				//half4 col=lerp(_BottomColor,_UpColor,cosTerm);

				//col.rgb = cloudcol.a*cloudcol.rgb+(1-cloudcol.a)*col.rgb;
				half4 col = half4(1, 1, 1, 1);

				col.rgb = cloudcol.rgb * _UpColor*_GlobalSceneIntensity;

				col.a=1;
				#ifdef _ALPHATEST_ON
				clip(cloudcol.a-_Cutoff);
				#endif

				//UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				//Alpha(SampleAlbedoAlpha(input.uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap)).a, _BaseColor, _Cutoff);
				return col;
			}
			//--------------------------------------
			// GPU Instancing
			
			
			
			//#include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
			ENDHLSL
		
        }
    }
}
