#ifndef NPC_OUTLINE_INCLUDED
#define NPC_OUTLINE_INCLUDED

CBUFFER_START(UnityPerMaterial)
float _Outline;
float4 _OutlineColor;
float _OutlineZFactor;

#ifdef FADE
float _fade;
#endif

float _OutlineMin;
float _OutlineMax;
float _GlobalOutlineMin;
float _GlobalOutlineMax;
float _GlobalOutline;
float _GlobalOutlineZFactor;
CBUFFER_END

struct a2v
{
    float4 positionOS : POSITION;
    float3 normalOS : NORMAL;
    float3 tangentOS : TANGENT;
    half4 color : COLOR;
};

struct v2f_outline
{
    float4 pos : SV_POSITION;
};

v2f_outline vert_outline(a2v v)
{
    _OutlineMin = _GlobalOutlineMin <= 0 ? _OutlineMin : _GlobalOutlineMin;
    _OutlineMax = _GlobalOutlineMax <= 0 ? _OutlineMax : _GlobalOutlineMax;
    _Outline = _GlobalOutline <= 0 ? _Outline : _GlobalOutline;
    _OutlineZFactor = _GlobalOutlineZFactor <= 0 ? _OutlineZFactor : _GlobalOutlineZFactor;
    v2f_outline o;
    float4 posVS = mul(UNITY_MATRIX_V, mul(UNITY_MATRIX_M, v.positionOS));
    float3 normalWS = mul(UNITY_MATRIX_M, v.normalOS);
    float3 normalVS = mul(normalWS, (float3x3) UNITY_MATRIX_I_V);
     
    float4 posClip = mul(UNITY_MATRIX_P, posVS);
    posClip.w = clamp(posClip.w, _OutlineMin, _OutlineMax);
    _Outline /= 1000.0;
    _OutlineZFactor /= 1000.0;
    float outlineWidth = max(0, _Outline + _OutlineZFactor * -posVS.z) * posClip.w;
    
    posVS.xyz = posVS.xyz + normalVS * outlineWidth * v.color.r;
    o.pos = mul(UNITY_MATRIX_P, posVS);
    return o;


}

v2f_outline vert_outlineTangent(a2v v)
{
    
    //float3 normal = mul(UNITY_MATRIX_V, mul((float3x3) UNITY_MATRIX_M, v.normalOS));
    _OutlineMin = _GlobalOutlineMin <= 0 ? _OutlineMin : _GlobalOutlineMin;
    _OutlineMax = _GlobalOutlineMax <= 0 ? _OutlineMax : _GlobalOutlineMax;
    _Outline = _GlobalOutline <= 0 ? _Outline : _GlobalOutline;
    _OutlineZFactor = _GlobalOutlineZFactor <= 0 ? _OutlineZFactor : _GlobalOutlineZFactor;
    v2f_outline o;
    float4 pos = mul(UNITY_MATRIX_V, mul(UNITY_MATRIX_M, v.positionOS));    // View Space Pos
    //float3 normal = mul((float3x3) UNITY_MATRIX_IT_MV, v.tangentOS);
    float3 normalWS = mul(UNITY_MATRIX_M, v.tangentOS);
    float3 normal = mul(normalWS, (float3x3) UNITY_MATRIX_I_V);
    
    _Outline /= 1000.0;
    _OutlineZFactor /= 1000.0;
    float outlineWidth = max(0, _Outline + _OutlineZFactor * -pos.z);
    
    float signVar = dot(normalize(pos.xyz), normalize(normal)) < 0 ? -1 : 1;
    
    pos.xyz = pos.xyz + signVar*normal * outlineWidth * v.color.r;
    o.pos = mul(UNITY_MATRIX_P, pos);
    return o;
}

v2f_outline vert(a2v v)
{
#if defined TANGENT_OUTLINE_ON
	return vert_outlineTangent(v);
#else
    return vert_outline(v);
#endif
	
}

float4 frag_outline(v2f_outline i) : COLOR
{
    float alpha = 1;

    #ifdef FADE
	    alpha = lerp(0,1,_fade);
    #endif


    return half4(_OutlineColor.xyz, alpha);
}


#endif