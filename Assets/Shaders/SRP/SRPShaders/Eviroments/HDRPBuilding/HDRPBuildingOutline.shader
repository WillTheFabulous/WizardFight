Shader "W02/HDRP_W02_BuildingOutline"
{
    Properties
    {

        [Header(OUTLINE)]
        _OutlineMin("Outline Min", float) = 1
       // _OutlineMax("Outline Max", float) = 3
        _OutlineColor("Outline Color",color) = (0,0,0,1)
        _Outline("Outline Width",range(0, 0.01)) = 0.01              // 挤出描边的粗细
    	//_OutlineOffsetZ("OutlineOffsetZ",float) = 0
        //_OutlineZFactor("Outline Z Factor", range(0, 10)) = 1.23  // 远处加强
		_DistanceRate("Distance Rate", range(0,1)) = 0.01
    }

    HLSLINCLUDE
    #pragma target 4.5

    //-------------------------------------------------------------------------------------
    // Include
    //-------------------------------------------------------------------------------------

	#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/RenderPass/CustomPass/CustomPassRenderers.hlsl"
	//#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/Lighting.hlsl"



    ENDHLSL
    SubShader
    {
         // This tags allow to use the shader replacement features
		Tags{ "RenderPipeline" = "HDRenderPipeline"}// "RenderType" = "HDLitShader" }

        /*
         Pass
        {
            Name "DepthOnly"
            Tags{ "LightMode" = "DepthOnly" }

			Cull[_CullMode]
			ZWrite [_ZWrite]
			ZTest [_ZTest]
			Offset [_ZOffset], 0
			
			Stencil
			{
				Ref [_StencilRef]
				Comp [_StencilCompare]
				Pass [_StencilOP]
			}

            HLSLPROGRAM

            // In deferred, depth only pass don't output anything.
            // In forward it output the normal buffer
            #pragma multi_compile _ WRITE_NORMAL_BUFFER
            #pragma multi_compile _ WRITE_MSAA_DEPTH

            #define SHADERPASS SHADERPASS_DEPTH_ONLY
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/Lit.hlsl"

            #ifdef WRITE_NORMAL_BUFFER // If enabled we need all regular interpolator
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/ShaderPass/LitSharePass.hlsl"
            #else
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/ShaderPass/LitDepthPass.hlsl"
            #endif

            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/LitData.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPassDepthOnly.hlsl"

            #pragma vertex Vert
            #pragma fragment Frag

            ENDHLSL
        }
		*/


        Pass
		{
			Name "OUTLINE"
			Tags
			{
				"RenderType" = "Opaque" /* "lightMode" = "Outline" */
			}
			Cull Front
			ZTest On
			ZWrite On
			HLSLPROGRAM
			#pragma shader_feature _ALPHATEST_ON
			//#pragma prefer_hlslcc gles
			//#pragma exclude_renderers d3d11_9x
            #pragma enable_d3d11_debug_symbols

			float4 _OutlineColor;
			half _OutlineMin;
			float _Outline;
			float _DistanceRate;
			//float4 _WorldSpaceCameraPos;
			// half _OutlineMax;
			// half _NoiseRate;
			// half _NoiseIntensity;
			// half _NoisePower;
			// half _NoiseMin;
			// half _OutlineFade;

			//CBUFFER_END

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				//float4 tangentOS    : TANGENT;
				float2 texcoord : TEXCOORD0;
				//float2 lightmapUV   : TEXCOORD1;
			};

			struct Varyings
			{
				float4 positionCS :SV_POSITION;
				float4 uv :TEXCOORD0;
				float3 normalWS :TEXCOORD1;
				float3 positionVS :TEXCOORD2;
				float3 positionWS :TEXCOORD3;
				//float fogFactor		:TEXCOOORD3;
				//float4 shadowCoord  : TEXCOORD3;
			};

			float2 GetUVByCameraPosition(float3 ViewPos)
			{
				float4 ClipPos = mul(UNITY_MATRIX_P, float4(ViewPos, 1));
				float4 NDC = float4(ClipPos.xyz / ClipPos.w, 1);
				float2 uv = (NDC.xy + 1) * 0.5;
				//#if UNITY_UV_STARTS_AT_TOP
				//uv.y=1-uv.y;
				//#endif
				return uv;
			}


			float3 GetViewPosByUV(float2 uv)
			{
				float deviceDepth = SAMPLE_TEXTURE2D_X_LOD(_CameraDepthTexture, s_linear_clamp_sampler, uv, 0);
				#if UNITY_REVERSED_Z
				deviceDepth = 1 - deviceDepth;
				#endif
				deviceDepth = 2 * deviceDepth - 1; //NOTE: Currently must massage depth before computing CS position.

				#if UNITY_UV_STARTS_AT_TOP
				uv.y = 1 - uv.y;

				#endif
				uv.x = 1 - uv.x;
				float3 vpos = ComputeViewSpacePosition(uv, deviceDepth, _InvProjMatrix);
				float3 wpos = mul(_InvViewMatrix, float4(vpos, 1)).xyz;
				return vpos;
			}

			Varyings Vert(Attributes input)
			{
				Varyings output = (Varyings)0;
				//VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
				float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
				//output.uv.xy = TRANSFORM_TEX(input.texcoord, _BaseMap);
				float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
				float3 positionVS = TransformWorldToView(positionWS);

				float4 positionCS = TransformWViewToHClip(positionVS);
				
				float fade = positionCS.w * _DistanceRate;

				fade = 1 - saturate(fade / 40);

				float distanceLength = _Outline * lerp(1, 5, fade);

				/*
				float CamDistance = distance(_WorldSpaceCameraPos.xyz, positionWS);
				float distanceRate = CamDistance * _DistanceRate;
				//distanceRate=pow(distanceRate,2);
				//float noiseTerm=1-_NoiseIntensity+_NoiseIntensity*pow(saturate(perlin_noise(positionVS.xy*_NoiseRate)+_NoiseMin),_NoisePower);
				float fade = distance(_WorldSpaceCameraPos.xyz, positionWS);
				fade = 1 - saturate((fade - (20)) / ((60) - (20)));
				

				float distanceLength = _Outline * distanceRate * lerp(3, 2, fade);
				//(1-distanceRate)+distanceRate*_OutlineMax+_NoiseRate*_NoiseIntensity;
				*/

				float fade2 = distance(_WorldSpaceCameraPos.xyz, positionWS);
				fade2 = 1 - saturate((fade2 - (20)) / ((40) - (20)));

				float3 worldpos = lerp(positionWS, positionWS + distanceLength * normalWS, fade);

				//float3 worldpos = positionWS + distanceLength * normalWS;

				// float3 worldpos = vertexInput.positionWS+distanceLength*normalWS;
				output.positionWS = worldpos;
				output.positionVS = TransformWorldToView(worldpos);
				output.positionCS = TransformWorldToHClip(worldpos);
				output.uv.z = fade2;
				output.uv.w = fade;
				// output.uv = input.texcoord;
				//output.fogFactor=ComputeFogFactor(vertexInput.positionCS.z);
				//output.shadowCoord = GetShadowCoord(vertexInput);
				return output;
			}

			float _GlobalSceneIntensity;

			half4 Frag(Varyings input) :SV_Target
			{
				_GlobalSceneIntensity = _GlobalSceneIntensity <= 0 ? 1 : _GlobalSceneIntensity;

				
				float2 uv = GetUVByCameraPosition(input.positionVS);
				float3 mapVpos = GetViewPosByUV(uv);
				

				//float mapdis=distance(GetCameraPositionWS(),mapWorldpos);
				//float realdis=distance(GetCameraPositionWS(),input.positionWS);

				
				float mapdis = length(mapVpos);
				float realdis = length(input.positionVS);
				float outline = step(realdis - 0.01, mapdis);
				
				// float faderealdis = distance(GetCameraPositionWS(),input.positionWS);
				// faderealdis  =   1-saturate((faderealdis - (30))/((32)-(30)));
				
				outline = outline * (1 - min(realdis, 1000) / 1000);
				
				//float outline=step(mapViewpos.z,input.positionVS.z);
				//half4 col=_OutlineColor;
				// return half4(outline,0,0,0);

				/*
				#ifdef _ALPHATEST_ON
					clip(outlinecolor.a - _Cutoff);
				#endif
				*/
				//return lerp(_OutlineColor, outlinecolor * 0.35, 1 - input.uv.z) * _GlobalSceneIntensity;
				return _OutlineColor;
				// return input.uv.z;
			}

			#pragma vertex Vert
			#pragma fragment Frag
			ENDHLSL
			
		}
    }
}
