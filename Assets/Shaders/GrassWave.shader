// Upgrade NOTE: replaced '_LightMatrix0' with 'unity_WorldToLight'

Shader "Lab42/GrassWave"
{
	Properties
	{
		_MainTex ("Sprite Texture", 2D) = "white" {}
		_MaskTex ("Mask Texture", 2D) = "black" {}
		_Amplitude("Amplitude", Float) = 1
		_Frequency("Frequency", Float) = 1
		_Phase("Phase", Float) = 1
        _GrassWaveAmplitude("GrassWave Amplitude", Float) = 1
		_GrassWaveHighLight("Grass Wave High Light", Float) = 0
		_GrassWaveFrequency("GrassWave Frequency", Float) = 0
		_GrassWavePhase("GrassWave Phase", Float) = 0
        _GrassWaveExtraPhase("GrassWave Extra Phase", Float) = 0
        _ColorN("Color North", Color) = (1,1,1,1)
        _ColorS("Color South", Color) = (1,1,1,1)
        _ColorAddRange("Color Add Range", Float) = 10
        _ColorAddDistance("Color Distance", Float) = 24
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="AlphaTest" 
			"RenderType"="TransparentCutOut"
		}

		//Direct Light
		Pass
		{
			Tags { "LightMode"="ForwardBase" }
			
			CGPROGRAM
			#pragma multi_compile_fwdbase	
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			
			float _Amplitude;
			float _Frequency;
			float _Phase;
            float _GrassWaveAmplitude;
			float _GrassWaveHighLight;
			float _GrassWaveFrequency;
			float _GrassWavePhase;
            float _GrassWaveExtraPhase;

			struct v2f{
				float4 pos : SV_POSITION;
    			half2 uv : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				SHADOW_COORDS(2)
			};

			v2f vert(appdata_img v)
			{
				v2f o;

				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.worldPos.x += sin(-_Time.y * _Frequency + _Phase + o.worldPos.y*10) * _Amplitude * o.worldPos.y;
				
				o.pos = UnityWorldToClipPos(o.worldPos);
				o.uv = v.texcoord;

				TRANSFER_SHADOW(o);

				return o;
			}

			sampler2D _MainTex;
			sampler2D _MaskTex;
			float4 _ColorN;
			float4 _ColorS;
			float _ColorAddRange;
			float _ColorAddDistance;

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 c = tex2D (_MainTex, i.uv);
				clip(c.a-0.5);

				//从蒙版贴图的r分量计算光照度加成
				fixed lightMask = tex2D(_MaskTex, i.uv).r;
				lightMask = (lightMask*2-1)/60;

				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * c.xyz;

				//附加草浪光照度加成
                fixed grassWaveStrength = _GrassWaveAmplitude * sin(_Time.y * _GrassWaveFrequency+i.worldPos.x*_GrassWavePhase + _GrassWaveExtraPhase + sin(i.worldPos.z*0.06))+0.5;
				
				int grassWaveStrengthInt = (int)(grassWaveStrength+lightMask+_GrassWaveHighLight);

				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
				
				//色彩叠加
				float distanceN = distance(float2(i.worldPos.x, _ColorAddDistance), float2(i.worldPos.x, i.worldPos.z));
				float colorNPercent = max(0, 1-distanceN/_ColorAddRange);
				fixed4 colorN = _ColorN + (1-_ColorN)*(1-colorNPercent);
				
				float distanceS = distance(float2(i.worldPos.x, -_ColorAddDistance), float2(i.worldPos.x, i.worldPos.z));
				float colorSPercent = max(0, 1-distanceS/_ColorAddRange);
				fixed4 colorS = _ColorS + (1-_ColorS)*(1-colorSPercent);
				
				c = colorN * colorS * c;

				return fixed4(ambient + _LightColor0 * (atten + lightMask + grassWaveStrengthInt*0.7) * c * 0.5 , 1);
			}
			ENDCG
		}

		//Other Light
		Pass
		{
			Tags { "LightMode"="ForwardAdd" }
			BlendOp Max
			Blend One One
			CGPROGRAM
			#pragma multi_compile_fwdadd_fullshadows
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			

			struct v2f
			{
				float4 pos   : SV_POSITION;
				float2 uv  : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				SHADOW_COORDS(2)
			};

			float _Amplitude;
			float _Frequency;
			float _Phase;
			float _WindAmplitude;
			float _WindFrequency;
			float _WindPhase;

			v2f vert(appdata_base v)
			{
				v2f o;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.worldPos.x += sin(-_Time.y * _Frequency + _Phase + o.worldPos.y*10) * _Amplitude * o.worldPos.y;
				
				o.pos = UnityWorldToClipPos(o.worldPos);
				o.uv = v.texcoord;

				TRANSFER_SHADOW(o);

				return o;
			}

			sampler2D _MainTex;
			sampler2D _MaskTex;

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 c = tex2D (_MainTex, i.uv);
				clip(c.a-0.5);

				//从蒙版贴图的r分量计算光照度加成
				fixed lightMask = tex2D(_MaskTex, i.uv).r;
				lightMask = (lightMask*2-1)/60;

				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

				// //衰减值阶梯化，做出三个不同亮度的梯度
				int attenInt = min(3,(int)((atten+lightMask+0.016)*30));

				//出于像素画风的考虑，不进行光源入射夹角计算，但这样亮度会过高，因此统一乘以一个倍率减弱亮度
				fixed4 diffuse = _LightColor0 * attenInt * c * 0.4;

				return diffuse;
			}
		ENDCG
		}

		Pass
		{
			Tags{ "LightMode" = "ShadowCaster" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#include "UnityCG.cginc"

			struct v2f {
				V2F_SHADOW_CASTER;
				float2 uv : TEXCOORD1;
			};

			float _Amplitude;
			float _Frequency;
			float _Phase;
			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert(appdata_base v)
			{
				v2f o;

				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				worldPos.x += sin(-_Time.y * _Frequency + _Phase + worldPos.y*10) * _Amplitude * worldPos.y;
				v.vertex = mul(unity_WorldToObject, worldPos);

				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

                UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				fixed4 texColor = tex2D(_MainTex, i.uv);
				// Alpha test
				clip (texColor.a - 0.5);

				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
	}
}