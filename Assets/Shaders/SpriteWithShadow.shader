// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Lab42/SpriteWithShadow"
{
	Properties
	{
		_MainTex ("Sprite Texture", 2D) = "yello" {}
		_MaskTex ("Mask Texture", 2D) = "black" {}
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
			#pragma shader_feature EMISSION
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

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
				o.pos = UnityWorldToClipPos(o.worldPos);
				o.uv = v.texcoord;

				TRANSFER_SHADOW(o);

				return o;
			}

			sampler2D _MainTex;
			sampler2D _MaskTex;

			fixed4 frag(v2f i) : SV_Target
			{
				//alpha test
				fixed4 c = tex2D (_MainTex, i.uv);
				clip(c.a-0.5);

				//self illuminate, from green channel of mask texture
				fixed3 emitStrength = c*tex2D(_MaskTex, i.uv).g;

				//use the unified vector up to keep the brightness of every faces the same.
				fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
				fixed3 worldLightStrengh = max(0, dot(fixed3(0.0, 1.0, 0.0), worldLightDir));
				fixed3 diffuse = _LightColor0 * worldLightStrengh * c.xyz;
                
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * c.xyz;
				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

				return fixed4(ambient + diffuse*atten + emitStrength, 1);
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
			#pragma shader_feature EMISSION
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

			v2f vert(appdata_base v)
			{
				v2f o;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
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

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert(appdata_base v)
			{
				v2f o;
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