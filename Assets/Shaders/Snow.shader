// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Lab42/Snow"
{
	Properties
	{
	    _BaseColor ("Basic Snow Color", Color) = (1,1,1,1)
		_NoiseTex ("Noise Texture", 2D) = "black" {}
        _SnowPilePeriod ( "Seconds To Max Snow", Float) = 1
        _SnowMeltPeriod ( "Seconds To Zero Snow", Float) = 1
        //下雪堆积出高度，相对于开始显现的延时
        _SnowVolumeDelay ("Snow Volume Delay From Clip", Float) = 0
		_ChangeStartTime("Snow Change Start Time", Float) = 0
		[Toggle]_IsSnowHigher("Is Snow Higher", Float) = 0
		[Toggle]_IsSnowLower("Is Snow Lower", Float) = 0
	}

    CGINCLUDE
    #include "UnityCG.cginc"
    #include "Lighting.cginc"
    #include "AutoLight.cginc"
    
    fixed4 _BaseColor;
    sampler2D _NoiseTex;
    half4 _NoiseTex_ST;
    fixed _IsSnowHigher;
    fixed _IsSnowLower;
    float _ChangeStartTime;
    float _SnowPilePeriod;
    float _SnowMeltPeriod;
    float _SnowVolumeDelay;
			
	struct v2f{
        float4 pos : SV_POSITION;
        float3 worldNormal : TEXCOORD0;
        float3 worldPos : TEXCOORD1;
        float3 uv : TEXCOORD2;
        unityShadowCoord4 _ShadowCoord : TEXCOORD3;
    };
	
	//计算下雪可见
    half CalcSnowClip(v2f i){
        fixed noise = tex2D(_NoiseTex, 
            half2(i.worldPos.x * _NoiseTex_ST.x, i.worldPos.z * _NoiseTex_ST.y)).r;
        
        //积雪时，先显现，后增高，化雪时，先减低，后消失
        float strengthHigher =  _IsSnowHigher * (_Time.y - _ChangeStartTime) / _SnowPilePeriod;
        float strengthLower =  _IsSnowLower * (1 -(_Time.y - _ChangeStartTime - _SnowVolumeDelay) / _SnowMeltPeriod);
        
        half strength = noise*2 + strengthHigher + strengthLower - 1.5;
        
        return strength;
    }
    
    //计算下雪厚度
    half CalcSnowStrength(v2f i){
        fixed noise = tex2Dlod(_NoiseTex, 
            half4(i.worldPos.x * _NoiseTex_ST.x, i.worldPos.z * _NoiseTex_ST.y, 0, 0)).r;
        
        float strengthHigher =  max(0, _IsSnowHigher * (_Time.y - _ChangeStartTime - _SnowVolumeDelay) / _SnowPilePeriod);
        float strengthLower =  max(0, _IsSnowLower * (1 -(_Time.y - _ChangeStartTime) / _SnowMeltPeriod));
        
        half strength = min(noise*10, (strengthHigher + strengthLower) * noise * 10);
        
        return strength;
    }
    
    //下雪厚度变形
    float4 CalcSnowShape(float4 vertex, v2f i){
        vertex.z *= 1 + CalcSnowStrength(i);
        return UnityObjectToClipPos(vertex);
    }
    ENDCG

	SubShader
	{
		Tags
		{ 
			"Queue"="AlphaTest" 
			"RenderType"="TransparentCutOut" 
			"DisableBatching"="True"
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

			v2f vert(appdata_base v)
			{
				v2f o;

				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;				
				o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
				o.pos = CalcSnowShape(v.vertex, o);
				o.uv = v.texcoord;
				
				TRANSFER_SHADOW(o);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				//透明度测试，低于阈值直接丢弃
				fixed4 c = _BaseColor;
				half snowClip = CalcSnowClip(i);
				clip(snowClip-0.5);

				//计算光照强度
				fixed3 worldNormal = normalize(i.worldNormal);
				fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
				fixed3 worldLightStrengh = max(0, dot(worldNormal, worldLightDir)*0.15+0.85);
				fixed3 diffuse = _LightColor0 * worldLightStrengh * c.xyz;
                
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * c.xyz;
				
				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

				return fixed4(ambient + diffuse*atten, 1);
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

			v2f vert(appdata_base v)
			{
				v2f o;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
				o.pos = CalcSnowShape(v.vertex, o);
				o.uv = v.texcoord;
				
				TRANSFER_SHADOW(o);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
			    //透明度测试，低于阈值直接丢弃
				fixed4 c = _BaseColor;
				half snowClip = CalcSnowClip(i);
				clip(snowClip-0.5);

				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

                //计算光照强度
				fixed3 worldNormal = normalize(i.worldNormal);
				fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos.xyz);
				fixed worldLightStrengh = max(0, dot(worldNormal, worldLightDir)*0.15+0.85);

				//衰减值阶梯化，做出三个不同亮度的梯度
				int attenInt = min(3,(int)((atten+0.016)*30));

				fixed4 diffuse = _LightColor0 * attenInt * c * worldLightStrengh * 0.75;

				return diffuse;
			}
		ENDCG
		}
	}
}