// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Coustom/FillShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (0,0,0,1)
		_FillValue ("FillValue", Range(0.0,1.0)) = 0.0
		_Emission ("Emission", Float) = 1.0
		_FillAngle ("FillAngle", Range(0,360)) = 0
		_GradationRang("GradationRang", Range(0,5)) = 0
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			Lighting Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _FillValue;
			float _Emission;
			float4 _Color;
			float _FillAngle;
			float _GradationRang;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			float FillAngle(float2 uv){
				float value;
				float c = sin(radians(_FillAngle+225));
				value = ( uv.x*(sin(radians(_FillAngle))*2) + uv.y*(sin(radians(_FillAngle+90))*2)
				 		)*0.35 + clamp(((c*0.5)+0.5),0,1);
				return value;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) * _Color;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);

				//float a = clamp(1 - _FillValue*3,0,1);
				col.a *= clamp((FillAngle(i.uv)*(_GradationRang)-_FillValue*(_GradationRang)),0,1);//step(FillAngle(i.uv), _FillValue);
				return col * _Emission;
			}
			ENDCG
		}
	}
}
