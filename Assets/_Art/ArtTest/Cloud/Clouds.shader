Shader "Unlit/Clouds"
{
    Properties
    {
        [NoScaleOffset] _BaseMap ("Cloud", 2D) = "black" {}
        _Color0("背光暗部", Color) = (1,1,1,1)
        _Color1("朝光暗部", Color) = (1,1,1,1)
        _Color2("背光亮部", Color) = (1,1,1,1)
        _Color3("朝光亮部", Color) = (1,1,1,1)
        _parameter0("朝光背光切分", Range(0.0, 1.0)) = 0.5
        _parameter7("朝光部分提亮", Range(0.0, 1.0)) = 0.5
        _parameter6("贴图亮部提亮", Range(0.0, 0.5)) = 0.25
        _Color4("边缘光", Color) = (1,1,1,1)
        _parameter1("边缘光范围", Range(0.0, 1.0)) = 0.5
        _parameter2("边缘光亮度", Range(0.0, 2.0)) = 0.01
        _Noise ("noise", 2D) = "black" {}
        _parameter8("噪音扭曲程度", Range(0.0, 1.0)) = 0.5
        _WorldCenter("World Center Position", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="BackGround" "RenderType" = "Transparent" "Queue" = "Transparent+1"}
        LOD 100
        Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		Zwrite Off

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
				float4 position     : POSITION;
				float4 texcoord0 	: TEXCOORD0;
				float4 texcoord1 	: TEXCOORD1;
				float4 color        : COLOR;
				// float2 texcoord     : TEXCOORD0;
				// float2 uv2 		    : TEXCOORD1;
				// float2 uv3		    : TEXCOORD2;
				// float2 uv4		    : TEXCOORD3;
				// float4 color        : COLOR;
			};
		
			

			struct Varyings
			{
				float4 vertex : SV_POSITION;
				float4 uv           : TEXCOORD0;
				half4 color0 		: TEXCOORD1;
				half4 color1 		: TEXCOORD2;
				half4 rimColor 		: TEXCOORD3;
				half4 param 		: TEXCOORD4;
				float3 test         : TEXCOORD6;
				half4 particleColor : TEXCOORD7;
			};


			float _parameter0;
			float _parameter1;
			float _parameter2;
			float _parameter6;
			float _parameter7;
			float _parameter8;
			half4 _Color0;
			half4 _Color1;
			half4 _Color2;
			half4 _Color3;
			half4 _Color4;
			sampler2D _BaseMap;
			sampler2D _Noise;
			float4 _Noise_ST;
			float4 _WorldCenter;

			Varyings Vertex(Attributes input)
			{
				Varyings output = (Varyings)0;
                output.vertex = UnityObjectToClipPos(input.position);
				int index = floor(input.texcoord0.w * 7 + 0.5);
				output.uv.xy = (input.texcoord0.xy + float2(index % 2, floor(index / 2))) / float2(2, 4);
				output.uv.zw = output.uv.xy * _Noise_ST.xy + _Noise_ST.zw;
				float3 viewDir = normalize(_WorldCenter.xyz - mul(unity_ObjectToWorld, input.position).xyz);
				float3 lightDir = normalize(-_WorldSpaceLightPos0.xyz);
				output.test = viewDir;
				float LdV = dot(viewDir, lightDir);
				float lerper = max(LdV * _parameter0 + (1.0 - _parameter0), 0.0);
				output.color0 = lerp(_Color0, _Color1, lerper);
				output.color1 = lerp(_Color2, _Color3, lerper);
				output.particleColor = input.color;

				LdV = (LdV * 0.5 + 0.5);
				output.rimColor = _Color4 * pow(smoothstep(_parameter1, 1, LdV) * _parameter2, 2);

				output.param.x = LdV * _parameter7;
				output.param.y = input.texcoord1.x;
				output.param.z = input.texcoord1.y;
				float particleAge = input.texcoord0.z;
				output.param.w = 1.0 - smoothstep(0.0, input.texcoord1.z, particleAge) * (1.0 - smoothstep(input.texcoord1.w, 1.0, particleAge));
				return output;
			}

			half4 Fragment(Varyings input) : SV_TARGET
			{
				half3 noise = tex2D(_Noise, input.uv.zw);
				float2 uvOffset = (noise.xy - 0.5) * noise.z * _parameter8;
				half4 cloudcol = tex2D(_BaseMap,input.uv.xy+uvOffset);

				half4 col = lerp(input.color0, input.color1, cloudcol.r);
				float alpha1 = saturate(input.param.w - input.param.y);
				float alpha2 = saturate(input.param.w + input.param.y);
				col.a = cloudcol.a * smoothstep(alpha1, alpha2, cloudcol.b);
				clip(col.a - 0.01);

				col.rgb += (input.rimColor * lerp(cloudcol.g, (1.0 - smoothstep(max(alpha1, 0.0), min(input.param.w + input.param.z, 1.0), cloudcol.b)), input.param.w)).rgb;
				col.rgb += (input.color1 * _parameter6).rgb;
				//这里应该还有个计算是叠加月亮周围，我们目前没有月亮就先不加了
				col.rgb *= (input.param.x + 1);
				col.rgba *= input.particleColor;
				return col;
			}
			ENDHLSL
		
        }
    }
}
