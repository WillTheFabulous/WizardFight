// 色阶
sampler2D _MainTex;
sampler2D _MatCapMap;
sampler2D _MatCapMapMask;
sampler2D _Mask;
sampler2D _Diffuse02;
sampler2D _Diffuse03;
sampler2D _LowLightTex;
sampler2D _HatchMask;
sampler2D _sideHatchMask;
sampler2D RimColorTex1;
TEXTURE2D_X(_SelfShadowTexture);
SAMPLER(sampler_SelfShadowTexture);

TEXTURECUBE(_ReflectionProbe);
SAMPLER(sampler_ReflectionProbe);
TEXTURE2D(_BaseColorMap);
SAMPLER(sampler_BaseColorMap);
TEXTURE2D(_HeightMap);
SAMPLER(sampler_HeightMap);
TEXTURE2D(_DistortionVectorMap);
SAMPLER(sampler_DistortionVectorMap);

sampler2D _AddLitMap;

CBUFFER_START(UnityPerMaterial)
// 色阶
float4 _MainTex_ST;
float4 _MatCapMap_ST;
float4 _HighLightTex_ST;
float4 _HatchMask_ST;
half4 _Color;
half4 _FlickColor;
float rampVal;
float rampFeather;
float _DarkEnvLerp;
float _Lerp;
// 光照
// 主光
half _MainLitLerp;
half4 _MainLightDir;

// rim1
float3 RimLitDir1;
half FScale1;
half FPower1;
half4 RimColor1;

// 脖颈线
float _NeckLineAppear;
half _NeckLineFadeFactor;

float _AdditionalLightFactor;
// receive shadow bias
half _ReceiveShadowBias;
half _ReceiveShadowAtten;
float _HairShadowDistance;

half4 _CharShadowColorAtten;

//metalstart property
bool _MetalMask;
half3 _baseColor;
half3 _sssColor;
float _Smoothness;
float _Metallic;
half _shadowThreshold;
half _shadowThresholdFade;
half _metalThreshold;
half _metalThresholdFade;
half _MetalModel;
half3 _metalColor;
half _MetalMod;
half _SmoothnessX;
half _SmoothnessY;
half _specBoardLine;
half _SpecStep;
half _SpecStepFade;
half _Metal;
half _Fresnel_F_power;
half _Fresnel_B_power;
half _shadowThreshold2;
//metalend property
//custom spec
half _customspecpower;
half4 _customspecColor;
half _customspecrange;
float _GlobalIntensity;
//custom spec
float4 RimLitColor;
float RimLitIntensity;
float RimLightGloss;
float BaseLitScale;
float _GlobalCharacterShadowScale;
float _HairShadowDistaceX;
float _HairShadowDistaceY;
float _HairShadowStrength;


// JTRP
float _PointLightStep;
float _PointLightFeather;
//float _DirLightIntensity;
float _PointLightColorIntensity;
uniform float _LightColorBlend;
uniform float _EnvLightLerp;


// Set of users variables, required by unity
float4 _BaseColor;
float4 _BaseColorMap_ST;

float _DiffusionProfileHash;
float _SubsurfaceMask;
float _Thickness;
float _Anisotropy;

float4 _SpecularColor;
float4 _UVMappingMask;
float4 _UVDetailsMappingMask;
float4 _DetailMap_ST;
float _LinkDetailsWithBase;

float _TexWorldScale;

float _EmissiveExposureWeight;
float3 _EmissiveColor;
float _AlbedoAffectEmissive;
float _DistortionScale;
float _DistortionVectorScale;
float _DistortionVectorBias;
float _DistortionBlurScale;
float _DistortionBlurRemapMin;
float _DistortionBlurRemapMax;

float4 _BaseColorMap_TexelSize;
float4 _BaseColorMap_MipInfo;


int _CharShadowLightID;

float _ShadowDirLightLerp;
float _ShadowEnvLightLerp;
float _CharReceiveShadow;
//float _ExtraPointLight;
float _MainLightIntensity;

float _DirLightLerp;

float _HairShadowDistanceClampX;
float _HairShadowDistanceClampY;
CBUFFER_END

int _DebugID;

struct LitToonContext
{
    float3 V; // view dir world space
    float3 L; // light
    float3 H; // half
    float3 T; // tangent
    float3 B; // binormal
    float3 N; // normal
    float3 charDirectionalLight;
        
    float4 uv;
    float2 matcapuv;

    float exposure;
    float halfLambert; // 0: dark 1: bright
    float shadowStep; // 0: bright 1: dark
    float roughness;
        
    float3 diffuse;
    float3 dirLightColor;
    float3 pointLightColor; // point / spot  light
    float3 baseColor;
    float3 envColor;
        
    float3 specular;
    float3 highLightColor; // dir + point / spot
    float3 matCapColor;
        
    float3 emissive;
};