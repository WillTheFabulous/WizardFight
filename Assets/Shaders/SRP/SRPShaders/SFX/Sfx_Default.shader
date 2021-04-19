Shader "W02/Sfx/Default" {

	Properties {
		[Enum(UnityEngine.Rendering.BlendMode)] 		_SrcFactor ("SrcFactor()", float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] 		_DstFactor ("DstFactor()", float) = 10
		[Enum(UnityEngine.Rendering.CullMode)] 	    _CullMode ("消隐模式(CullMode)", int) = 0
		[Enum(LessEqual,4,Always,8)]				        _ZTestMode ("深度测试(ZTest)", int) = 4
		// [Toggle(ENABLE_TEX_ALPHAASRGB)]   _EnableAlphaAsRgb("Use For Blend:Screen", float) = 1
		//	[Toggle(ENABLE_ROTATEUV)]       _EnableRotateUV("启用RotateUV", float) = 1
		[Toggle(ENABLE_CUSTOMDATA)] _EnableCustomData("启用自定义数据(CustomData)", float) = 0


		[Header(Main Settings)]
		[HDR]_TintColor ("Tint Color", Color ) = (1,1,1,1)
		// _Multiplier	("Multiplier",float) = 1
		_MainTex ("Main Tex", 2D) = "white" {}
		_MainTexRot ("Tex Rotation", float ) = 0
		_MainTexX ("TexUscroll", float ) = 0
		_MainTexY ("TexVscroll", float ) = 0
		[Toggle(ENABLE_MAIN_UVLOOP)] _EnableMainUVloop("启用主贴图(UVLOOP)", float) = 0


		[Header(Mask Settings)]
		[Toggle(ENABLE_TEX_MASK)] _EnableMaskMode("启用遮罩(MaskMode)", float) = 0
		_MaskTex ("Mask Tex", 2D) = "white" {}
		_MaskTexRot ("Mask Rotation", float ) = 0
		_MaskTexX ("MaskUscroll", float ) = 0
		_MaskTexY ("MaskVscroll", float ) = 0
		[Toggle(ENABLE_MASK_UVLOOP)] _EnableMaskUVloop("启用遮罩(UVLOOP)", float) = 0


		[Header(Flow Settings)]
		[Toggle(ENABLE_TEX_FLOW)] _EnableFlowMode("启用扰动(FlowMode)", float) = 0
		_FlowTex ("Flow Tex", 2D) = "black" {}
		_FlowTexMask ("Flow MaskTex", 2D) = "white" {}
		_FlowTexRot ("Flow Rotation", float ) = 0
		_FlowTexMaskRot("Flow Mask Rotation", float ) = 0
		_FlowScaleX ("Flow Value_X", Range(-1, 1)) = 0
		_FlowScaleY ("Flow Value_Y", Range(-1, 1)) = 0
		_FlowOffsetX ("Flow Offset_X", Range(-1, 1)) = 0
		_FlowOffsetY ("Flow Offset_Y", Range(-1, 1)) = 0
		// _FlowRemapMin("FlowRemapMin", Range(-1, 1)) = 0
		// _FlowRemapMax("FlowRemapMax", Range(-1, 1)) = 1

		_FlowTexX ("FlowVscroll", float ) = 0
		_FlowTexY ("FlowUscroll", float ) = 0
		[Toggle(ENABLE_FLOW_UVLOOP)] _EnableFlowUVloop("启用扰动(UVLOOP)", float) = 0
		[Toggle(ENABLE_FLOWMASK_UVLOOP)] _EnableFlowMaskUVloop("启用扰动遮罩(UVLOOP)", float) = 0



		[Header(Dissolve Settings)]
		[Toggle(ENABLE_TEX_DISSOLVE)] _EnableDissolveMode("启用溶解(DissolveMode)", float) = 0
		_DissolveTex("Dissolve Tex", 2D) = "white" {}
		_DissolveAmount ("Dissolve Amount", Range(0,2) ) = 0.0
		_DissolvePow("Dissolve DissolvePow", float ) = 1
		[HDR]_DissolveRimColor("DissolveRimColor", Color ) = (1,1,1,1)
		// [Toggle(ENABLE_TEX_DISSOLVE_SOFT)] _EnableDissolveSoftMode("启用溶解软过度(DissolveSoftMode)", float) = 0
		_DissolveSoft("Dissolve Soft",Range(0,1)) = 0.0
		_DissolveRimSize("Dissolve Rim Size",Range(0,1)) = 0.0
		_DissolveRimSoft("DissolveRimSoft", Range(0,1)) = 0
		_DissolveTexRot ("Dissolve Rotation", float ) = 0
		_DissolveTexX ("DissolveUscroll", float ) = 0
		_DissolveTexY ("DissolveVscroll", float ) = 0
		[Toggle(ENABLE_DISSOLVE_UVLOOP)] _EnableDissolveUVloop("启用溶解(UVLOOP)", float) = 0

		[Toggle(ENABLE_SOFT_PARTICLE)] _EnableSoftParticle("启用软粒子", float) = 0
		_SoftParticleFadeParams("_SoftParticleFadeParams", Vector) = (0,0,0,0)
		




	}
	SubShader {
		Tags { "RenderPipeline"="HDRenderPipeline" "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Pass {
			Name "SFX"

			Tags { "LightMode"="ForwardOnly" }
			Blend [_SrcFactor] [_DstFactor]
			Cull [_CullMode]
			ZWrite off
			ZTest [_ZTestMode]
			
 
			// CGPROGRAM
			HLSLPROGRAM
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
			//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Particles.hlsl"


			#pragma target 3.0



			//#pragma multi_compile_fwdbase
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_particles
			#pragma shader_feature ENABLE_TEX_DISSOLVE
			#pragma shader_feature ENABLE_DISSOLVE_UVLOOP
			#pragma shader_feature ENABLE_CUSTOMDATA
			#pragma shader_feature ENABLE_MAIN_UVLOOP
			#pragma shader_feature ENABLE_TEX_FLOW
			#pragma shader_feature ENABLE_FLOW_UVLOOP
			#pragma shader_feature ENABLE_FLOWMASK_UVLOOP
			#pragma shader_feature ENABLE_TEX_MASK
			#pragma shader_feature ENABLE_MASK_UVLOOP
			#pragma shader_feature ENABLE_TEX_ALPHAASRGB
			//#pragma multi_compile __ ENABLE_ROTATEUV
			#pragma shader_feature ENABLE_SOFT_PARTICLE





			float2 RotateUV(float2 uv,float uvRotate)
			{
				//#ifdef ENABLE_ROTATEUV
				float2 outUV;
				float s;
				float c;
				s = sin(uvRotate/57.2958);
				c = cos(uvRotate/57.2958);
				outUV = uv - float2(0.5f, 0.5f);
				outUV = float2(outUV.x * c - outUV.y * s, outUV.x * s + outUV.y * c);
				outUV = outUV + float2(0.5f, 0.5f);
				return outUV;
				//#else
				//return uv;
				//#endif
			}

			half remap(half x, half t1, half t2, half s1, half s2)
			{
				return (x - t1) / (t2 - t1) * (s2 - s1) + s1;
			}

			

			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);
			float4 _MainTex_ST;

			float4 _TintColor;
			// float _Multiplier;
			float _MainTexRot;
			float _MainTexX;
			float _MainTexY;

			// sampler2D _MaskTex;
			TEXTURE2D(_MaskTex);
			SAMPLER(sampler_MaskTex);
			float4 _MaskTex_ST;
			float _MaskTexRot;
			float _MaskTexX;
			float _MaskTexY;

			// sampler2D _FlowTex ;
			// sampler2D _FlowTexMask;
			TEXTURE2D(_FlowTex);
			SAMPLER(sampler_FlowTex);
			TEXTURE2D(_FlowTexMask);
			SAMPLER(sampler_FlowTexMask);
			float4 _FlowTex_ST;
			float4 _FlowTexMask_ST;
			float _FlowTexMaskRot;
			float _FlowTexRot;
			float _FlowScaleX;
			float _FlowScaleY;
			float _FlowTexX;
			float _FlowTexY;
			float _FlowRemapMin;
			float _FlowRemapMax;
            float _FlowOffsetX;
			float _FlowOffsetY;
            float _DissolvePow;

			float _DissolveAmount;
			float _DissolveSoft;
			float _DissolveRimSize;
			// sampler2D _DissolveTex;
			TEXTURE2D(_DissolveTex);
			SAMPLER(sampler_DissolveTex);
			float4 _DissolveTex_ST;
			float _DissolveTexRot;
			float _DissolveTexX;
			float _DissolveTexY;
			float _DissolveRimSoft;
			float4 _SoftParticleFadeParams;
			float4 _CameraFadeParams;
			float4 _DissolveRimColor;
			float4 _ClipRect;
			float _UseClipRect;
			#define SOFT_PARTICLE_NEAR_FADE _SoftParticleFadeParams.x
			#define SOFT_PARTICLE_INV_FADE_DISTANCE _SoftParticleFadeParams.y


			struct a2v {
				float4 vertex : POSITION;
				float4 color : COLOR;
				float4 texcoord : TEXCOORD0;
				#ifdef ENABLE_CUSTOMDATA
					float4 customData : TEXCOORD1;
				#endif


			};
			struct v2f {
				float4 pos : SV_POSITION;
				float4 color : COLOR;
				float2 uvMainTex : TEXCOORD0;
				float2 uvMaskTex : TEXCOORD5;
				float2 uvDissolveTex : TEXCOORD2;
				float2 uvFlowTex : TEXCOORD3;
				float2 uvFlowTexMask : TEXCOORD4;
				#ifdef ENABLE_CUSTOMDATA
					float4 customData : TEXCOORD1;
				#endif
				float4 projectedPosition : TEXCOORD6;
				float3 worldPos:TEXCOORD7;



			};
			v2f vert(a2v v)
			{
				v2f o;

				float t = _Time.z;
				t = fmod(t,255);

				o.color = v.color;
				// o.pos = UnityObjectToClipPos(v.vertex);
				o.pos = TransformObjectToHClip(v.vertex);
				o.worldPos = TransformObjectToWorld(v.vertex);
				float4 ndc = o.pos * 0.5f;
				o.projectedPosition.xy = float2(ndc.x, ndc.y * _ProjectionParams.x) + ndc.w;
				o.projectedPosition.zw = o.pos.zw;

				#ifdef ENABLE_CUSTOMDATA
					_MainTex_ST.z += v.customData.x;
					_MainTex_ST.w += v.customData.y;
					// _MainTex_ST.z = _MainTexX;
					// _MainTex_ST.w = _MainTexY;
					// _MainTex_ST.xy -= 1;


				#endif

				o.uvMainTex = TRANSFORM_TEX(v.texcoord, _MainTex);

				o.uvMainTex.xy = RotateUV(o.uvMainTex.xy,_MainTexRot);
				o.uvMainTex.xy += t * float2(_MainTexX,_MainTexY);


				#ifdef ENABLE_TEX_MASK
					o.uvMaskTex.xy = TRANSFORM_TEX(v.texcoord ,_MaskTex);
					o.uvMaskTex.xy = RotateUV(o.uvMaskTex.xy,_MaskTexRot);
					o.uvMaskTex.xy += t * float2(_MaskTexX,_MaskTexY);
				#endif

				#ifdef ENABLE_TEX_FLOW
					o.uvFlowTex.xy = TRANSFORM_TEX(v.texcoord ,_FlowTex);
					o.uvFlowTexMask.xy = TRANSFORM_TEX(v.texcoord ,_FlowTexMask);
					o.uvFlowTexMask.xy = RotateUV(o.uvFlowTexMask.xy,_FlowTexMaskRot);
					o.uvFlowTex.xy += t * float2(_FlowTexX,_FlowTexY);
					o.uvFlowTex.xy = RotateUV(o.uvFlowTex.xy,_FlowTexRot);
				#endif

				#ifdef ENABLE_TEX_DISSOLVE
					o.uvDissolveTex.xy = TRANSFORM_TEX(v.texcoord , _DissolveTex);
					o.uvDissolveTex.xy += t * float2(_DissolveTexX,_DissolveTexY);
					o.uvDissolveTex.xy = RotateUV(o.uvDissolveTex.xy,_DissolveTexRot);
				#endif
				#ifdef ENABLE_CUSTOMDATA
					o.customData = v.customData;
				#endif



				return o;
			}
			float4 frag(v2f i , float facing : VFACE) : SV_Target {

				#ifdef ENABLE_CUSTOMDATA
					_DissolveAmount += i.customData.z;
					_FlowScaleX *= i.customData.w;
					_FlowScaleY *= i.customData.w;

				#endif

				#ifdef ENABLE_TEX_DISSOLVE
					float3 DissolveColor = SAMPLE_TEXTURE2D(_DissolveTex,sampler_DissolveTex,i.uvDissolveTex.xy).rgb;
					#ifdef ENABLE_DISSOLVE_UVLOOP
						DissolveColor = SAMPLE_TEXTURE2D(_DissolveTex,sampler_DissolveTex,frac(i.uvDissolveTex.xy)).rgb;
					#endif


					//_TintColor.a = 1 - i.color.a;
					// _TintColor.a = step(0.0, DissolveColor.r - (_DissolveAmount +(_TintColor.a+(1-i.color.a))));
				#endif

				float2 flowUV = i.uvMainTex.xy;
				#ifdef ENABLE_TEX_FLOW
					float4 Disturb = SAMPLE_TEXTURE2D(_FlowTex,sampler_FlowTex,i.uvFlowTex.xy);
					#ifdef ENABLE_FLOW_UVLOOP
						Disturb = SAMPLE_TEXTURE2D(_FlowTex,sampler_FlowTex,frac(i.uvFlowTex.xy));
					#endif
					float4 FlowMask = SAMPLE_TEXTURE2D(_FlowTexMask,sampler_FlowTexMask,i.uvFlowTexMask.xy);
					#ifdef ENABLE_FLOWMASK_UVLOOP
						FlowMask = SAMPLE_TEXTURE2D(_FlowTexMask,sampler_FlowTexMask,frac(i.uvFlowTexMask.xy));
					#endif
					// Disturb = remap(Disturb,0,1,_FlowRemapMin,_FlowRemapMax);
				    //Disturb=Disturb*2-1;
				
					Disturb *= FlowMask.r;
					Disturb.xy-=float2(_FlowOffsetX,_FlowOffsetY);
					flowUV += (Disturb) * half2(_FlowScaleX,_FlowScaleY);

					// flowUV.x= lerp(i.uvMainTex.x,lerp(i.uvMainTex.x,Disturb,_FlowScaleX) ,_FlowScaleX);
					// flowUV.y= lerp(i.uvMainTex.y,lerp(i.uvMainTex.y,Disturb,_FlowScaleY) ,_FlowScaleY);

				#endif

				float4 MainTexColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,flowUV);
				#ifdef ENABLE_MAIN_UVLOOP
					MainTexColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,frac(flowUV));
				#endif

				float3 finalColor = MainTexColor.rgb;
				float result = MainTexColor.a;

				// #ifdef ENABLE_TEX_ALPHAASRGB
				// finalColor *= lerp(1,MainTexColor.a,0.45);
				// #endif
				float alpha = 1;

				#ifdef ENABLE_TEX_DISSOLVE
					// float t = 1 - (DissolveColor.r - _DissolveAmount);
					// alpha = t * step(0.001, _DissolveAmount);
				alpha = smoothstep(saturate(_DissolveAmount - _DissolveSoft),saturate(_DissolveAmount),saturate(pow(DissolveColor.r,_DissolvePow)));
				float alpha1 = smoothstep(saturate(_DissolveAmount - _DissolveRimSize),saturate(_DissolveAmount),saturate(pow(DissolveColor.r,_DissolvePow)));
				
				finalColor = lerp(MainTexColor.rgb, MainTexColor.rgb,alpha);
				if(alpha1>0&&alpha1<1)
				{
					finalColor.xyz=_DissolveRimColor;
					
					alpha=lerp(1,alpha1,_DissolveRimSoft);
					
				}
				#endif

				#ifdef ENABLE_TEX_MASK
					float4 MaskColor = SAMPLE_TEXTURE2D(_MaskTex,sampler_MaskTex,i.uvMaskTex.xy);
					#ifdef ENABLE_MASK_UVLOOP
						MaskColor = SAMPLE_TEXTURE2D(_MaskTex,sampler_MaskTex,frac(i.uvMaskTex.xy));
					#endif
					// finalColor *= MaskColor.r;
					result = MaskColor.r * MainTexColor.a;
				#endif
				// result = smoothstep(0.05,0.85,result);
				#ifdef ENABLE_SOFT_PARTICLE
					alpha *= SoftParticles(SOFT_PARTICLE_NEAR_FADE, 1/(SOFT_PARTICLE_INV_FADE_DISTANCE-SOFT_PARTICLE_NEAR_FADE), i.projectedPosition);
				#endif

				finalColor *= _TintColor;
				finalColor *= i.color.rgb;
				float4 resultcol=float4(finalColor, result *_TintColor.a  *alpha* i.color.a);
				float2 inside = step(_ClipRect.xy, i.worldPos.xy) * step(i.worldPos.xy, _ClipRect.zw);

			
				
				resultcol.a = lerp(resultcol.a,  inside.x * inside.y * resultcol.a, _UseClipRect); 
				return resultcol;

			}
			ENDHLSL
		}
	}
	
}
