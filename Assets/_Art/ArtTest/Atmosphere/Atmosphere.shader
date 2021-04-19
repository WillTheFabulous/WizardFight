Shader "Unlit/Atmosphere"
{
    Properties
    {
        [NoScaleOffset] _SkyGradient ("Sky Gradient", 2D) = "white" {}
        _Color0("顶部背光颜色", Color) = (1,1,1,1)
        _Color1("顶部受光", Color) = (1,1,1,1)
        _Color2("底部背光", Color) = (1,1,1,1)
        _Color3("底部受光", Color) = (1,1,1,1)
        _parameter0("受光背光范围", Range(0.0, 1.0)) = 0.5
        _parameter1("顶部底部范围", Range(0.0, 2)) = 1.0
        _Color4("地平线颜色(additive)", Color) = (1,1,1,1)
        _parameter2("地平线范围", Range(0.0, 5)) = 1.0
        _parameter3("地平线颜色强度", Range(0.0, 2)) = 1.0
        _Color5("太阳光晕颜色(additive)", Color) = (1,1,1,1)
        _parameter5("太阳光晕强度", Range(0.0, 2)) = 1.0
        _parameter4("太阳光晕范围", Range(0.0, 1000)) = 1.0
        _parameter6("颜色整体亮度", Range(0.0, 2)) = 1.0
        _StarMap ("star map", 2D) = "black" {}
        _Noise ("Noise map", 2D) = "white" {}
        _StarColor ("Star Color Tex", 2D) = "white" {}
        _NoiseFlowSpeed("Noise Flow Speed", Range(0.0, 1)) = 1.0
        _StarNum("Star Num Mask", Range(0.0, 0.2)) = 0.5
        _StarIntensity("Star Intensity", Range(0.0, 2)) = 0.5
        _WorldCenter("World Center Position", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="BackGround" "RenderType" = "Transparent" }
        LOD 100
        Cull Off
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "Skybox"
			HLSLPROGRAM
			#pragma target 2.0
			#pragma vertex Vertex
			#pragma fragment Fragment

			#include "UnityCG.cginc"

			struct Attributes
			{
				float4 vertex     : POSITION;
				float2 uv     : TEXCOORD0;
			};
		
			

			struct Varyings
			{
				float4 vertex 		: SV_POSITION;
				float4 uv           : TEXCOORD0;
				float4 uvFlow 		: TEXCOORD4;
				half3 color 		: TEXCOORD2;
				float4 view 		: TEXCOORD1;
			};

			sampler2D _StarMap;
			float4 _StarMap_ST;
			sampler2D _SkyGradient;
			sampler2D _Noise;
			float4 _Noise_ST;
			sampler2D _StarColor;
			float4 _StarColor_ST;
			float _parameter0;
			float _parameter1;
			float _parameter2;
			float _parameter3;
			float _parameter4;
			float _parameter5;
			float _parameter6;
			float _NoiseFlowSpeed;
			float _StarNum;
			float _StarIntensity;
			half4 _Color0;
			half4 _Color1;
			half4 _Color2;
			half4 _Color3;
			half4 _Color4;
			half4 _Color5;
			float4 _WorldCenter;

			Varyings Vertex(Attributes i)
			{
				Varyings o = (Varyings)0;
                o.vertex = UnityObjectToClipPos(i.vertex);
                o.uv.xy = i.uv * _StarMap_ST.xy + _StarMap_ST.zw;
                o.uv.zw = i.uv * 20.0;
                float2 noiseuv = i.uv * _Noise_ST.xy;
                float noiseSpeed = _Time.y * _NoiseFlowSpeed;
                o.uvFlow.xy = noiseuv + float2(0.4, 0.2) * noiseSpeed;
                o.uvFlow.zw = noiseuv + float2(0.1, 0.5) * noiseSpeed;

                float3 viewDir = normalize(_WorldCenter.xyz - mul(unity_ObjectToWorld, i.vertex).xyz);
                float upDir = asin(dot(float3(0, 1, 0), -viewDir)) * 0.6366;
                float3 lightDir = normalize(-_WorldSpaceLightPos0.xyz);
                float LdV = dot(viewDir, lightDir);
                float lerper = saturate(LdV * _parameter0 + 1 - _parameter0);
                lerper = pow(lerper, 3);
                LdV = saturate(LdV * 0.5 + 0.5);

                float lerper1 = tex2Dlod(_SkyGradient, float4(abs(upDir) / _parameter1, 0.5, 0.0, 0.0)).r;
                half3 color5 = lerp(_Color0, _Color1, lerper).rgb;
                half3 color6 = lerp(_Color2, _Color3, lerper).rgb;
                o.color = lerp(color5, color6, lerper1);
                float leaper2 = tex2Dlod(_SkyGradient, float4(abs(upDir) / _parameter2, 0.5, 0.0, 0.0)).g;
                o.color += _Color4 * leaper2 * _parameter3;

                o.view = float4(viewDir, upDir);
				return o;
			}

			half4 Fragment(Varyings i) : SV_TARGET
			{
				float4 col = float4(i.color, 1.0);

				half3 view = normalize(i.view.xyz);
				float LDV = saturate(dot(normalize(-_WorldSpaceLightPos0.xyz), view) * 0.5 + 0.5);
				half powPara = abs(dot(view, float3(0, -1, 0))) * 0.9 + 0.1;
				float sunIntensity = pow(LDV, powPara * _parameter4);
				sunIntensity += saturate(pow(LDV, powPara * _parameter4 * 0.1)) * 0.12;
				sunIntensity += saturate(pow(LDV, powPara * _parameter4 * 0.01)) * 0.03;
				col.rgb += sunIntensity * _Color5 * _parameter5 * smoothstep(0.5, 1.0, LDV);

				col.rgb *= _parameter6;

				float star = tex2D(_StarMap,i.uv.xy).a;
				float noise = tex2D(_Noise,i.uv.zw).a;
				float noise2 = tex2D(_Noise, i.uvFlow.xy).a * tex2D(_Noise, i.uvFlow.zw).a * 3.0;
				float noisePara0 = (star - _StarNum) / (1.0 - _StarNum);
				float starAlpha = noisePara0 * noise2 * star * saturate(i.view.w * 1.5);
				float3 starColor = tex2D(_StarColor, float2(noise * _StarColor_ST.x + _StarColor_ST.z, 0.5));

				col.rgb += starColor * starAlpha * _StarIntensity;

				return col;
			}
			ENDHLSL
		
        }
    }
}
