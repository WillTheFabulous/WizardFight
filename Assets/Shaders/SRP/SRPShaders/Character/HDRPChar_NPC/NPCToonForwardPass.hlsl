#ifndef NPC_FORWARD_PASS
#define NPC_FORWARD_PASS

	#include "../HDRPChar/CharToonForwardFunctions.hlsl"
#ifdef HAS_LIGHTLOOP
	#include "../HDRPChar/CharToonLighting.hlsl"
#endif

PackedVaryingsType vert(AttributesMesh inputMesh)
{
    VaryingsType varyingsType;
    varyingsType.vmesh = VertMesh(inputMesh);
    
    return PackVaryingsType(varyingsType);
}

void frag(PackedVaryingsToPS packedInput,
out float4 outColor : SV_Target0)
{
    FragInputs input;
    PositionInputs posInput;
    BuiltinData builtinData;
    SurfaceData surfaceData;
    LitToonContext context = (LitToonContext) 0;
    
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(packedInput);
    input = UnpackVaryingsMeshToFragInputs(packedInput.vmesh);
    
    PreData(1, packedInput, input, posInput, builtinData, surfaceData, context);

    
    _GlobalIntensity = _GlobalIntensity <= 0 ? 1 : _GlobalIntensity;
    _GlobalCharacterShadowScale = _GlobalCharacterShadowScale <= 0 ? 1 : _GlobalCharacterShadowScale;    
    
    half4 texCol1 = tex2D(_MainTex, context.uv.xy);
    half4 texCol2 = tex2D(_LowLightTex, context.uv.xy);
    
    #ifdef MAT_BLEND
	half4 texCol3 = tex2D(_Diffuse02, context.uv.xy);
	half4 texCol4 = tex2D(_Diffuse03, context.uv.xy);
	half mask = tex2D(_Mask, context.uv.xy).r;
	_Lerp=1-_Lerp;
	texCol1=lerp(texCol1,texCol3,saturate(smoothstep(0,1,(mask-_Lerp*2+1))));
	texCol2=lerp(texCol2,texCol4,saturate(smoothstep(0,1,(mask-_Lerp*2+1))));
    #endif
    
    half4 rimTexCol1 = tex2D(RimColorTex1, context.uv.xy);

    
    DirectionalLightData mainLight = _DirectionalLightDatas[0];
    
    float3 mainLightDirVS = TransformWorldToViewDir(-mainLight.forward.xyz);
    float uvPackw = smoothstep(0, 0.1, abs(mainLightDirVS.x)) * sign(mainLightDirVS.x);

    _MainLightDir.x *= uvPackw;
    float3 mainLightDir = normalize(_MainLightDir.x * UNITY_MATRIX_V[0].xyz +
        _MainLightDir.y * UNITY_MATRIX_V[1].xyz + _MainLightDir.z * UNITY_MATRIX_V[2].xyz);
    
    float nl = dot(normalize(context.N), mainLightDir);
     
    
    // surface
    float3 worldNormal = normalize(context.N);
    float3 worldSpaceViewDir = normalize(context.V);    
    float3 worldLightDir = _MainLightDir;
    
    
    
    float3 hDir = normalize(worldLightDir + worldSpaceViewDir);
    half nh = dot(worldNormal, hDir);
    half ramp12 = smoothstep(rampVal, rampVal + rampFeather, nl);
    
    //half3 attenuatedLightColor = min(0.65, mainLight.color * mainLight.distanceAttenuation);
    half3 attenuatedLightColor = min(0.65, mainLight.color * GetCurrentExposureMultiplier());
    half3 bump = half3(0, 0, 1);
    half3 worldNormalmetal = normalize(bump.x * context.T + bump.y * context.B + bump.z * context.N);
    half3 tangent = normalize(context.T);
    half3 binormal = normalize(context.B);
    half3 worldLightDirmetal = normalize(- mainLight.forward.xyz);
    
    half3 viewDir = normalize(context.V);
    float3 H = normalize(viewDir + worldLightDirmetal);
    float NdotV = dot(worldNormalmetal, viewDir);
    
    float NdotL = nl;
    float VdotL = dot(viewDir, worldLightDirmetal);
    float InvLenH = 1 / sqrt(2 + 2 * VdotL);
    float NdotH = clamp((NdotL + NdotV) * InvLenH, 0.0, 1.0);
    float RoL = 2 * NdotL * NdotV - VdotL;
    float roughness = 1 - 0.98 * _Smoothness;
    float mata = 0.98 * _Metallic;
    
    // 色阶颜色

    half4 finalCol;
    half3 mainCol = GetMainColor(texCol1, texCol2, ramp12, _DarkEnvLerp, context.N);
    finalCol.xyz =mainCol;
    finalCol.w = 1;

    //metal start diffuse
    half4 col = texCol1 * half4(_baseColor, 1);
    half4 sss = texCol2 * half4(_sssColor, 1);
    float shadowThreshold = 1 - _shadowThreshold;
    
    float CelContrast1 = smoothstep(shadowThreshold, shadowThreshold + _shadowThresholdFade, NdotL);
    float CelContrast2 = lerp(0.4, 1, smoothstep(shadowThreshold + _shadowThreshold2,
                                                 shadowThreshold + _shadowThreshold2 + _shadowThresholdFade, NdotL));
    
    float CelContrast = CelContrast1 * CelContrast2;

    float MetalThreshold = 1 - _metalThreshold;
    float MetalModel = lerp(1 - pow(NdotL, 1), 1 - NdotV, _MetalModel);
    float NdotLMix = smoothstep(MetalThreshold, MetalThreshold - _metalThresholdFade, MetalModel);
    float MetalMix = CelContrast;
    float3 Basecolor = lerp(col.rgb, lerp(col.rgb, lerp(col.rgb * 0.5 * _metalColor, col.rgb, NdotLMix), _Metallic),
                            _MetalMod);
    float3 ssscolor = sss.rgb;
    float3 NMcolor = lerp(ssscolor, Basecolor, MetalMix);
    float3 diffuseColor = (NMcolor - 0.15 * NMcolor * _Metallic) * texCol1.a;
    
    // 脖颈线
    half neckLineMask = tex2D(_sideHatchMask, context.uv.xy).r;

    half neckLine = GetNeckLine(neckLineMask, 0, _NeckLineAppear, _NeckLineFadeFactor);


    finalCol.xyz *= neckLine;
    
    //边缘光
    RimParam rimParam;
    rimParam.viewDirWS = worldSpaceViewDir;
    rimParam.normalWS = worldNormal;
    rimParam.sceneLitFactor = uvPackw;

    rimParam.rimLitDir = RimLitDir1;
    rimParam.rimTexCol = rimTexCol1 * RimColor1;
    rimParam.fScale = FScale1;
    rimParam.fPower = FPower1;

    float3 rimLit = GetRimLit(rimParam, finalCol.xyz);
    finalCol.xyz = rimLit;
    
    
    //TODO: how to sample reflection probe in hdrp? Temp solution: assign reflection probe cubemap manually
    //metal start ambient
    float3 reflectDir = reflect(-viewDir, worldNormalmetal);
    float4 envSample = SAMPLE_TEXTURECUBE_LOD(_ReflectionProbe, sampler_ReflectionProbe, reflectDir,
                                              PerceptualRoughnessToMipmapLevel(roughness));
    //float4 envSample = SAMPLE_TEXTURECUBE_LOD(unity_SpecCube0, samplerunity_SpecCube0, reflectDir,
    //                                          PerceptualRoughnessToMipmapLevel(roughness));
    //envSample = float4(GlossyEnvironmentReflection(reflectDir, roughness, 1), 1);
    float3 Ambientmetal = envSample;
    float3 MatAmbient = Ambientmetal * col.rgb;
    //metal end ambient
    
    //metal start fresnel
    float3 Fresnelschlick = Fresnel_schlick(NdotV, Basecolor * (1 - roughness), _Fresnel_B_power, half3(1, 1, 1));
    float3 Fresnelschlickstep = Fresnel_schlickstep(NdotV, Basecolor * (1 - roughness), _Fresnel_F_power,
                                                    rimTexCol1.rgb * 2);
    float3 fresnelmetal = pow((Fresnelschlick * (1 - roughness) * 2), 0.85);
    float3 fresnelstep = (Fresnelschlickstep * (1 - roughness) * 15);
    fresnelmetal = lerp(fresnelmetal, fresnelstep, MetalMix);
    fresnelmetal *= Ambientmetal * texCol1.a;
    //metal end fresnel
    
    #ifdef ENABLE_MATCAP
    
	fresnelmetal=0;
	half4 matcapCol = tex2D(_MatCapMap, context.matcapuv);
	half4 matcapColMask = tex2D(_MatCapMapMask, context.uv);
	fresnelmetal+=matcapCol.rgb*matcapColMask.rgb;

	#endif
    
    //metal start specular
    float DielectricSpecular = 2.0;
    float3 SpecColor3 = (DielectricSpecular - DielectricSpecular * (1 - mata)) + NMcolor * (1 - mata);
    float2 roughness2 = float2(1 - _SmoothnessX, 1 - _SmoothnessY);
    roughness2 *= (1.7 - 0.7 * roughness2) * 0.98 * (1 - roughness);
    roughness2 += 0.02;
    half NDF0 = D_GGXaniso(roughness2.x, roughness2.y, 1, worldNormalmetal, tangent, binormal);
    half NDF = D_GGXaniso(roughness2.x, roughness2.y, NdotH, H, tangent, binormal) / NDF0;
    half ndfs = (NDF - _specBoardLine);
    half4 specularStep = half4(ndfs, roughness2.x, 1, 1);
    specularStep += half4(ndfs, roughness2.y, 1, 1);
    specularStep *= 0.5;
    half specularWin = specularStep.x;
    half3 specular = specularWin * _specBoardLine * NDF0;
    specular = smoothstep(_SpecStep, _SpecStep + _SpecStepFade, specular);
    specular *= Basecolor * (1 - roughness) * (SpecColor3) * 3 * MetalMix * attenuatedLightColor
        /** step(0.1,texCol1.r)*/;
    //metal end specular
    
    //metal Combine start

    diffuseColor = diffuseColor * attenuatedLightColor;
    float4 finalcolormetal = float4(diffuseColor + specular + fresnelmetal, 1);

    //metal Combine end
    
    if (_MetalMask)
    {
        finalCol = lerp(finalCol, finalcolormetal, texCol2.a);
    }
    else
    {
        finalCol = lerp(finalCol, finalcolormetal, 0);
    }
    //custom Specular
    #if defined(CUSTOMSPEC_ON)
					half NdotH2 = max(0,dot(worldNormal,normalize(mainLightDir + viewDir)));
					half Distribution = pow(NdotH2,max(1,_customspecrange * 40))*_customspecpower;
					// Distribution *= (2+_customspecpower) / (2*3.1415926535);
					Distribution = smoothstep(_customspecpower*0.15,_customspecpower*0.2,Distribution);
					half4 specular2 = Distribution*_customspecColor*_customspecpower;
					finalCol += specular2;
    #endif
    
    half3 lit = lerp(half3(1, 1, 1), mainLight.color.xyz * GetCurrentExposureMultiplier(), _MainLitLerp);
    lit *= _MainLightIntensity;
    finalCol.xyz *= lit;
    
     // 点光
    //float3 pointLightColor = PointLitPhase(finalCol.xyz, texCol1, lit, i.worldPos, worldNormal);
    
    PointLightLoop(context, posInput, builtinData, _PointLightColorIntensity * 1, 1);
    //finalCol.xyz = lerp(finalCol.xyz, context.pointLightColor, _ExtraPointLight);   // TODO:待验证pointLightColor是否使用正确
    finalCol.xyz += ToonLightColorAddMode(finalCol.xyz, context.pointLightColor);
    // 额发投影
    #ifdef _RECEIVE_HAIR_SHADOW
        //计算该像素的Screen Position
        float4 positionCS = ComputeClipSpacePosition(posInput.positionNDC, posInput.deviceDepth);
        float4 positionSS = positionCS * 0.5f;
        positionSS.xy = float2(positionSS.x, positionSS.y * _ProjectionParams.x) + positionSS.w;
        positionSS.zw = positionCS.zw;
        
        float NDCw = positionCS.w;      
        float2 scrPos = positionSS.xy/positionSS.w;

    
        //计算View Space的光照方向
        float linearEyeDepth = LinearEyeDepth(posInput.deviceDepth, _ZBufferParams);
        float3 viewLightDir = TransformWorldToViewDir(-mainLight.forward.xyz, true) * (1 / min(NDCw, 1)) * min(1, 5 / linearEyeDepth);

        //计算采样点，其中_HairShadowDistace用于控制采样距离
        float2 samplingPoint = scrPos + _HairShadowDistance * 
            float2(clamp(viewLightDir.x, -_HairShadowDistanceClampX, _HairShadowDistanceClampX),
            clamp(viewLightDir.y, -_HairShadowDistanceClampY, _HairShadowDistanceClampY));
     
        float hairDepth = LOAD_TEXTURE2D_X_LOD(_SelfShadowTexture, samplingPoint * _ScreenSize.xy, 0).r;
        //若采样点在阴影区内,则取得的value为1,作为阴影的话还得用1 - value;
        float faceDepth = positionCS.z / positionCS.w;
        float hairShadow = (hairDepth < faceDepth)?hairDepth:0;
  
        finalCol = lerp(texCol2 * _CharShadowColorAtten, finalCol, 1 - hairShadow );
    
	    //finalCol.xyz*=hairShadow;

    #endif
    
    //场景阴影

    LightData punctualLight = _LightDatas[_CharShadowLightID];
    float dist = distance(punctualLight.positionRWS, posInput.positionWS);
   
#ifdef _RECEIVE_OBJECT_SHADOW
        float objectShadow = GetSelfShadowFromPunctualLight(context, posInput, dist, punctualLight.shadowIndex);
        finalCol = lerp(texCol2 * _CharShadowColorAtten, finalCol, objectShadow);   
#endif      
    
    finalCol.xyz = finalCol.xyz * _Color;
    
    // 人物闪白
    finalCol.xyz = finalCol.xyz * (1 - _FlickColor.a) + _FlickColor.xyz * _FlickColor.a;
    
    
    #ifdef RIMLITEXT
				    //Rim Light Extra
				    float rim=1-max(0, dot(worldSpaceViewDir,worldNormal));
				    float3 rimColor=pow(rim,RimLightGloss)*RimLitColor*RimLitIntensity;
				    finalCol.xyz*=BaseLitScale;
				    finalCol.xyz +=rimColor;
				    finalCol.xyz = finalCol.xyz*_GlobalIntensity;
    #else
        finalCol.xyz = (finalCol.xyz) * _GlobalIntensity;
    #endif


        //后处理Mask
    #if defined(UI_RENDER_ON)
					    finalCol.a = 1;
    #else
        finalCol.a = 0.5;
    #endif
    
    if(_DebugID == 0)
        outColor = finalCol;
    else if(_DebugID == 1)
        outColor = float4(mainCol, 1);
        //outColor = mainCol;
    else if(_DebugID == 2)
        outColor = finalcolormetal;
    else if (_DebugID == 3)
        outColor = float4(lit, 1);
}

#endif