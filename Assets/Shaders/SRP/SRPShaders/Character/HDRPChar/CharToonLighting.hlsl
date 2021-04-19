#ifndef HDRP_TOON_LIGHTING
#define HDRP_TOON_LIGHTING
    
// From JTRP ToonLighting.hlsl


//================= Data =================
void GetUVs(inout LitToonContext context, FragInputs input)
{
    context.uv.xy = input.texCoord0.xy;
    context.uv.zw = input.texCoord0.zw;
//#ifdef VARYINGS_NEED_TEXCOORD1
//            context.uv1 = input.texCoord0.zw;
//#endif
//#ifdef VARYINGS_NEED_TEXCOORD2
//            context.uv2 = input.texCoord1.xy;
//#endif
//#ifdef VARYINGS_NEED_TEXCOORD3
//            context.uv3 = input.texCoord1.zw;
//#endif
}

void PreData(float dirLightInt, inout PackedVaryingsToPS packedInput, inout FragInputs input, inout PositionInputs posInput,
    inout BuiltinData builtinData, inout SurfaceData surfaceData, inout LitToonContext context)
{    
        // We need to readapt the SS position as our screen space positions are for a low res buffer, but we try to access a full res buffer.
    input.positionSS.xy = _OffScreenRendering > 0 ? (input.positionSS.xy * _OffScreenDownsampleFactor) : input.positionSS.xy;
    uint2 tileIndex = uint2(input.positionSS.xy) / GetTileSize();
        // input.positionSS is SV_Position
    posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS.xyz, tileIndex);
    context.V = GetWorldSpaceNormalizeViewDir(input.positionRWS);
    context.T = input.tangentToWorld[0];
    context.B = input.tangentToWorld[1];
    context.N = input.tangentToWorld[2];
        
    GetSurfaceAndBuiltinData(input, context.V, posInput, surfaceData, builtinData);
    
    DirectionalLightData dirLight = _DirectionalLightDatas[0];    
    context.L = -dirLight.forward;
    context.H = normalize(context.V + context.L);
    context.exposure = GetCurrentExposureMultiplier();
    context.dirLightColor = dirLight.color * context.exposure * dirLightInt;
    context.envColor = SampleBakedGI(posInput.positionWS, context.N, input.texCoord1.xy, input.texCoord2.xy) * context.exposure;
        
    context.halfLambert = 0.5 * dot(context.N, context.L) + 0.5;

    //GetUVs
    context.uv.xy = input.texCoord0.xy;
    context.uv.zw = input.texCoord1.xy;

#ifdef ENABLE_MATCAP
	float3 e = normalize(mul((float3x3)UNITY_MATRIX_I_V,  context.V));
	float3 n = mul((float3x3)UNITY_MATRIX_I_V, context.N);	// UNITY_MATRIX_IT_MV 
	float3 r = reflect(e, n);
	r = normalize(r);
	float m = 2. * sqrt(
   pow( r.x, 2. ) +
   pow( r.y, 2. ) +
   pow( r.z + 1., 2. ));
    context.matcapuv.xy = r.xy / m + 0.5;
#endif 
}


DirectLighting ShadeSurface_Punctual(LightLoopContext lightLoopContext,
    PositionInputs posInput, BuiltinData builtinData, //PreLightData preLightData,
    LightData light, LitToonContext context, float roughness)
{
    DirectLighting lighting;
    ZERO_INITIALIZE(DirectLighting, lighting);
        
    float3 L;
    float4 distances; // {d, d^2, 1/d, d_proj}
    GetPunctualLightVectors(posInput.positionWS, light, L, distances);
        
    if (light.lightDimmer > 0)
    {
        float4 lightColor = EvaluateLight_Punctual(lightLoopContext, posInput, light, L, distances);
        lightColor.rgb *= lightColor.a; // 衰减
            
			// if SHADEROPTIONS_COLORED_SHADOW then SHADOW_TYPE == real3
        float shadow = EvaluateShadow_Punctual(lightLoopContext, posInput, light, builtinData, context.N, L, distances).x;
        shadow = step(0.5, shadow);
        lightColor.rgb *= ComputeShadowColor(shadow, light.shadowTint, light.penumbraTint);
            
        float halfLambert = 0.5 * dot(context.N, L) + 0.5;
        float shadowStep = GetShadowStep(halfLambert, _PointLightStep, _PointLightFeather);
            
        lighting.diffuse = lightColor.rgb * light.diffuseDimmer * (1 - shadowStep);
        //lighting.specular = GetHighLight(context.N, context.V, L, lighting.diffuse, shadowStep, roughness, _HighColorPointInt1, _HighColorPointInt2);
    }
        
    return lighting;
}

void PointLightLoop(inout LitToonContext toonContext, PositionInputs posInput, // PreLightData preLightData, BSDFData bsdfData,
    BuiltinData builtinData, float pointIntensity, float specularInt)
{
#ifdef _SURFACE_TYPE_TRANSPARENT
            uint featureFlags = LIGHT_FEATURE_MASK_FLAGS_TRANSPARENT;
#else
    uint featureFlags = LIGHT_FEATURE_MASK_FLAGS_OPAQUE;
#endif
    LightLoopContext context;
        
    context.shadowContext = InitShadowContext();
    context.shadowValue = 1;
    context.sampleReflection = 0;
    ApplyCameraRelativeXR(posInput.positionWS);
    InitContactShadow(posInput, context);
        
    AggregateLighting aggregateLighting;
    ZERO_INITIALIZE(AggregateLighting, aggregateLighting); // LightLoop is in charge of initializing the struct
        
    if (featureFlags & LIGHTFEATUREFLAGS_PUNCTUAL)
    {
        uint lightCount, lightStart;
            
#ifndef LIGHTLOOP_DISABLE_TILE_AND_CLUSTER
        GetCountAndStart(posInput, LIGHTCATEGORY_PUNCTUAL, lightStart, lightCount);
#else   // LIGHTLOOP_DISABLE_TILE_AND_CLUSTER
                lightCount = _PunctualLightCount;
                lightStart = 0;
#endif
            
        bool fastPath = false;
#if SCALARIZE_LIGHT_LOOP
                uint lightStartLane0;
                fastPath = IsFastPath(lightStart, lightStartLane0);
                
                if (fastPath)
                {
                    lightStart = lightStartLane0;
                }
#endif
            
        uint v_lightListOffset = 0;
        uint v_lightIdx = lightStart;
            
        while (v_lightListOffset < lightCount)
        {
            v_lightIdx = FetchIndex(lightStart, v_lightListOffset);
            uint s_lightIdx = ScalarizeElementIndex(v_lightIdx, fastPath);
            if (s_lightIdx == -1)
                break;
                
            LightData s_lightData = FetchLight(s_lightIdx);
                
            if (s_lightIdx >= v_lightIdx)
            {
                v_lightListOffset++;
                if (IsMatchingLightLayer(s_lightData.lightLayers, builtinData.renderingLayers))
                {
                    DirectLighting lighting = ShadeSurface_Punctual(context, posInput, builtinData, //preLightData,
                        s_lightData, toonContext, toonContext.roughness);
                        
                    AccumulateDirectLighting(lighting, aggregateLighting);
                }
            }
        }
    }
        
    toonContext.pointLightColor = aggregateLighting.direct.diffuse * toonContext.exposure * pointIntensity;
    //toonContext.highLightColor += (aggregateLighting.direct.specular + aggregateLighting.indirect.specularReflected) * saturate(specularInt);
}    

float3 ToonLightColorAddMode(float3 baseColor, float3 addLightColor)
{
    return lerp(1, baseColor, _LightColorBlend) * addLightColor;
}
  
void ToonDiffuseLighting(inout LitToonContext context, float3 mainTex, float3 shadowColor)
{
    // 亮部颜色随平行光的亮度变化    
    context.baseColor = mainTex * lerp(1, context.dirLightColor, _DirLightLerp)
        * lerp(1, context.envColor, _EnvLightLerp);
    
    shadowColor = shadowColor * lerp(1, context.dirLightColor, _ShadowDirLightLerp)
        * lerp(1, context.envColor, _ShadowEnvLightLerp);
    
    float shadowArea = context.shadowStep;        
    float3 diffuse = lerp(shadowColor, context.baseColor, shadowArea);
            
    context.diffuse = diffuse + ToonLightColorAddMode(mainTex, context.pointLightColor);
}

float GetSelfShadow(LitToonContext context, PositionInputs posInput)
{
#ifndef _ENABLE_SELFSHADOW
    return 1;
#else
            DirectionalLightData dirLight = _DirectionalLightDatas[0];
            HDShadowContext sc = InitShadowContext();
            float attenuation = GetDirectionalShadowAttenuation(sc, posInput.positionSS, posInput.positionWS,
            context.N, dirLight.shadowIndex, context.L);
            //attenuation = StepAntiAliasing(attenuation, 0.5);
            
            // 0:shadow
            float selfShadow = (attenuation * 0.5) + 0.5;
            //return clamp(selfShadow, 0.001, 1);
            return attenuation;
#endif
}

float GetSelfShadowFromPunctualLight(LitToonContext context, PositionInputs posInput, float L_dist, int shadowIndex)
{
#ifndef _RECEIVE_OBJECT_SHADOW
    return 1;
#else
            HDShadowContext sc = InitShadowContext();
            float attenuation = GetPunctualShadowAttenuation(sc, posInput.positionSS, posInput.positionWS,
            context.N, shadowIndex, context.L, L_dist, false, false);
            //attenuation = StepAntiAliasing(attenuation, 0.5);
            
            // 0:shadow
            float selfShadow = (attenuation * 0.5) + 0.5;
            //return clamp(selfShadow, 0.001, 1);
            return attenuation;
#endif
}

// From CharCommonLessTex.hlsl
half3 GetMainColor(half4 color1, half3 color2, half ramp12, half darkEnvLerp, float3 normalWS)
{
    half3 ambient = lerp(half3(1, 1, 1), EvaluateAmbientProbe(normalWS) * GetCurrentExposureMultiplier(), darkEnvLerp);
    half linearA = pow(color1.w, 2.2);
    half3 finalCol = lerp(color2 * ambient, color1, ramp12) * linearA;
    return finalCol;
}

half GetNeckLine(half neckLine, half ramp12, float neckLineAppear, half neckLineFadeFactor)
{
    float3 modelSpaceCamPos = mul(UNITY_MATRIX_I_M, float4(_WorldSpaceCameraPos, 1)).xyz;
    modelSpaceCamPos = normalize(modelSpaceCamPos);
	
#if defined(BIND_BONE_ON)
	half  frontBias = abs(modelSpaceCamPos.z);
	half signedFrontBias = modelSpaceCamPos.z;
#else
    half frontBias = abs(modelSpaceCamPos.x);
    half signedFrontBias = modelSpaceCamPos.x;
#endif
	
    half shadowLineShow = smoothstep(neckLineAppear, neckLineAppear + neckLineFadeFactor, frontBias);
    half shadowLine = lerp(1, neckLine, shadowLineShow * step(ramp12, 0.001));
	
    return shadowLine;
}

struct RimParam
{
	//
    float3 viewDirWS;
    float3 normalWS;
	// 环境光方向因子
    half sceneLitFactor;
	// rim1
    half3 rimLitDir;
    half4 rimTexCol;
    half fScale;
    half fPower;
    half fBorder;
    half fFeather;
};

half3 GetRimLit(RimParam rimParam, half3 inColor)
{
	// 边缘光方向和场景方向挂钩
    rimParam.rimLitDir.x *= rimParam.sceneLitFactor;

    float3 worldRimDir = normalize(rimParam.rimLitDir.x * UNITY_MATRIX_V[0].xyz +
	rimParam.rimLitDir.y * UNITY_MATRIX_V[1].xyz + rimParam.rimLitDir.z * UNITY_MATRIX_V[2].xyz);

    float3 rimHDir = normalize(worldRimDir + rimParam.viewDirWS);

    float fresnelPart1 = pow(1 - max(0, dot(rimHDir, rimParam.normalWS)), rimParam.fPower);
	// float fresnelPart2 =  pow(1 - dot(rimParam.viewDirWS, rimParam.normalWS), rimParam.fPower);
    float fresnel = rimParam.fScale * saturate(fresnelPart1);

    half3 finalRimColor = lerp(inColor, rimParam.rimTexCol.rgb, rimParam.rimTexCol.a) * fresnel;
    half3 finalCol = inColor + finalRimColor;

    return finalCol;
}

//From CharCommonMetal.hlsl

float Pow5(float3 x)
{
    return x * x * x * x * x;
}

float3 Fresnel_schlickstep(float VoN, float3 rF0, half power, half3 color)
{
    return rF0 * 0.25 + (1 - rF0) * smoothstep(0.15, 0.65, Pow5(1 - VoN)) * 3 * power * color;
}

float3 Fresnel_schlick(float VoN, float3 rF0, half power, half3 color)
{
    return rF0 + (1 - rF0) * Pow5(1 - VoN) * power * color;
}

float MYGGX_ID(float3 N, float3 H, float3 NdotH, float Roughness)
{
    float3 NxH = cross(N, H);
    float OneMinusNoHSqr = dot(NxH, NxH);
    half a = Roughness * Roughness;
    float n = NdotH * a;
    float p = a / (OneMinusNoHSqr + n * n);
    float d = p * p;
    return d;
}

float D_GGXaniso(float ax, float ay, float NoH, float3 H, float3 X, float3 Y)
{
    float XoH = dot(X, H);
    float YoH = dot(Y, H);
    float d = XoH * XoH / (ax * ax) + YoH * YoH / (ay * ay) + NoH * NoH;
    return 1 / (3.14159 * ax * ay * d * d);
}
#endif