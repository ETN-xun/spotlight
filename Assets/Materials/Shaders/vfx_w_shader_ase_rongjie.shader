// Made with Amplify Shader Editor v1.9.1.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "VFX/vfx_w_shader_ase_rongjie_URP"
{
	Properties
	{
		_All_alpha("Alpha", Range( 0 , 1)) = 1
		[Enum(UnityEngine.Rendering.BlendMode)]_Src("Src", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)]_Dst("Dst", Float) = 10
		[Toggle][Enum()]_xrsd("写入深度", Float) = 0
		[Enum(UnityEngine.Rendering.CompareFunction)]_Zdethp("Z深度模式", Float) = 4
		[Enum(UnityEngine.Rendering.CompareFunction)]stenci_lbuffer("渲染顺序（遮罩剔除）", Float) = 0
		[HDR]_Main_Color1("主颜色(中心颜色)", Color) = (0.5849056,0.5849056,0.5849056,1)
		_Main_Tex("主贴图", 2D) = "white" {}
		[Enum(UnityEngine.Rendering.CullMode)]_cull_("背面剔除", Float) = 2
		_MainTexRotator("主贴图旋转", Float) = 0
		[Toggle]_maintexRA("主贴图通道R/A", Float) = 0
		[Toggle(_MAINTEXUV_ON)] _MainTexUV("custom主贴图UV流动", Float) = 0
		_mainUV("主贴图UV流动", Vector) = (0,0,0,0)
		[Main(3color,_KEYWORD,on,off)]_m_3color("三色", Float) = 0
		[Toggle(_RGBCOLOR_ONOFF_ON)] _RGBcolor_onoff("RGB三色 off/on", Float) = 0
		[Toggle(_3COLORALPHA_ON)] _3colorAlpha("3色Alpha_off/on", Float) = 0
		[HDR]_R_color("R_color", Color) = (1,0,0,1)
		[HDR]_G_color("G_color", Color) = (0,1,0,1)
		[HDR]_B_color("B_color", Color) = (0,0,1,1)
		[Main(rj,_KEYWORD,on,off)]_m_rongjie("溶解", Float) = 0
		[Toggle(_RJ_ON)] _rj("——————溶解————————————————", Float) = 0
		_RongJieTex("溶解贴图", 2D) = "white" {}
		_xz("旋转", Float) = 0
		[Toggle]_polar("极坐标", Float) = 0
		_polar_tilling("极坐标重复度", Vector) = (1,1,0,0)
		_RongJieRY("溶解软硬", Range( 0.001 , 1.5)) = 0.001
		[Toggle]_customRJ1("custom溶解", Float) = 0
		_RongJie("溶解值", Float) = 0.92
		_rongjieB("双色溶解边缘宽度", Range( 0 , 1)) = 0.1108583
		[HDR]_RJBColor("溶解边颜色", Color) = (1,0.4103774,0.4103774,1)
		[Toggle(_CUSTUM_RJ_UV_ON)] _custum_rj_uv("custum溶解UV流动", Float) = 0
		_RJUVliudong("溶解UV流动", Vector) = (0,0,0,0)
		[Toggle]_rjTex2onoff("开启溶解图2", Float) = 0
		[Toggle]_rj2_ma("溶解2模式-乘/加", Float) = 0
		_rjTex2("溶解图2", 2D) = "white" {}
		_rotator2("旋转2", Float) = 0
		_rj2("溶解2", Float) = 0
		[Main(nq,_KEYWORD,on,off)]_m_niuqu("扭曲", Float) = 0
		[Toggle(_NQ_ON)] _nq("——————扭曲————————————————", Float) = 0
		_NiuQuTex("扭曲贴图", 2D) = "white" {}
		[Toggle]_nqMain_Tex1("扭曲主贴图", Float) = 1
		[Toggle]_nqMask("扭曲遮罩", Float) = 1
		[Toggle]_NQRJTex("扭曲溶解图", Float) = 1
		_NiuQuPow("扭曲强度", Float) = 0
		[Toggle]_customNQQD("custom扭曲强度", Float) = 0
		_NiuQuUV("扭曲UV流动", Vector) = (0,0,0,0)
		[Toggle(_NQBY_ON)] _nqby("固定扭曲边缘", Float) = 0
		[Main(mask,_KEYWORD,on,off)]_m_mask("遮罩", Float) = 0
		[Toggle(_MASK_ON)] _mask("——————遮罩————————————————", Float) = 0
		_maskTex("遮罩图", 2D) = "white" {}
		[Toggle]_mask_RA("遮罩图R/A", Float) = 0
		_maskUVliuDong("遮罩UV流动", Vector) = (0,0,0,0)
		[Main(ddpy,_KEYWORD,on,off)]_m_ddpianyi("定点偏移", Float) = 0
		[Toggle(_DDPY_ON)] _DDpy("——————顶点偏移——————————————", Float) = 0
		_ddpyTex("顶点偏移图", 2D) = "white" {}
		[Toggle]_cd_ddpy_pow("custom顶点偏移Pow", Float) = 0
		_DDpianyi("顶点偏移强度", Float) = 0
		_DDpianyiUV("顶点偏移UV流动", Vector) = (0,0,0,0)
		[Toggle]_pymsqh("偏移模式切换", Float) = 0
		_ddpyxyz("定点偏移xyz", Vector) = (1,1,1,0)
		_ddpymask("顶点偏移遮罩", 2D) = "white" {}
		[Main(fne,_KEYWORD,on,off)]_m_ddpianyi1("菲尼尔", Float) = 0
		[Toggle(_FRESNEL_ON)] _fresnel("——————菲尼尔————————————————", Float) = 0
		[HDR]_feinierColor("菲尼尔颜色", Color) = (0.5188679,0.5188679,0.5188679,1)
		_Float6("Float 6", Float) = 1
		_fresnelPow("菲尼尔强度", Float) = 1
		_fresnelSize("菲尼尔范围", Float) = 0
		[Toggle]_FanXiangFrenal("反向菲尼尔", Float) = 0
		[Toggle]_RJlK("溶解轮廓", Float) = 1
		[Toggle]_frenalRJ("菲尼尔遮罩影响溶解", Float) = 0
		_soft_lz("软粒子", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Opaque" "Queue"="Transparent" }
	LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend [_Src] [_Dst]
		AlphaToMask Off
		Cull [_cull_]
		ColorMask RGBA
		ZWrite [_xrsd]
		ZTest [_Zdethp]
		Offset 0 , 0
		Stencil
		{
			Ref 1
			Comp [stenci_lbuffer]
			Pass Keep
			Fail Keep
			ZFail Keep
		}
		
		
		Pass
		{
			Name "Unlit"

			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define ASE_NEEDS_FRAG_COLOR
			#pragma shader_feature_local _DDPY_ON
			#pragma shader_feature_local _RGBCOLOR_ONOFF_ON
			#pragma shader_feature_local _MAINTEXUV_ON
			#pragma shader_feature_local _NQ_ON
			#pragma shader_feature_local _NQBY_ON
			#pragma shader_feature_local _RJ_ON
			#pragma shader_feature_local _CUSTUM_RJ_UV_ON
			#pragma shader_feature_local _FRESNEL_ON
			#pragma shader_feature_local _MASK_ON
			#pragma shader_feature_local _3COLORALPHA_ON


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord2 : TEXCOORD2;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_color : COLOR;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform float _m_mask;
			uniform float _m_3color;
			uniform float _m_ddpianyi1;
			uniform float _m_niuqu;
			uniform float _m_rongjie;
			uniform float _m_ddpianyi;
			uniform float _Dst;
			uniform float _Src;
			uniform float _cull_;
			uniform float _Zdethp;
			uniform float _xrsd;
			uniform float stenci_lbuffer;
			uniform sampler2D _ddpyTex;
			uniform float4 _ddpyTex_ST;
			uniform float2 _DDpianyiUV;
			uniform float _DDpianyi;
			uniform float _cd_ddpy_pow;
			uniform float3 _ddpyxyz;
			uniform float _pymsqh;
			uniform sampler2D _ddpymask;
			uniform float4 _ddpymask_ST;
			uniform sampler2D _Main_Tex;
			uniform float4 _Main_Tex_ST;
			uniform float2 _mainUV;
			uniform float _MainTexRotator;
			uniform sampler2D _NiuQuTex;
			uniform float4 _NiuQuTex_ST;
			uniform float2 _NiuQuUV;
			uniform float _NiuQuPow;
			uniform float _customNQQD;
			uniform float _nqMain_Tex1;
			uniform float4 _R_color;
			uniform float4 _G_color;
			uniform float4 _B_color;
			uniform float4 _Main_Color1;
			uniform float4 _RJBColor;
			uniform float _RongJie;
			uniform float _customRJ1;
			uniform float _RongJieRY;
			uniform sampler2D _RongJieTex;
			uniform float4 _RongJieTex_ST;
			uniform float2 _polar_tilling;
			uniform float _polar;
			uniform float _xz;
			uniform float2 _RJUVliudong;
			uniform float _NQRJTex;
			uniform sampler2D _rjTex2;
			uniform float4 _rjTex2_ST;
			uniform float _rotator2;
			uniform float _rj2;
			uniform float _rj2_ma;
			uniform float _rjTex2onoff;
			uniform float _rongjieB;
			uniform float4 _feinierColor;
			uniform float _fresnelSize;
			uniform float _Float6;
			uniform float _fresnelPow;
			uniform float _FanXiangFrenal;
			uniform float _RJlK;
			uniform float _maintexRA;
			uniform sampler2D _maskTex;
			uniform float4 _maskTex_ST;
			uniform float2 _maskUVliuDong;
			uniform float _nqMask;
			uniform float _mask_RA;
			uniform float _frenalRJ;
			uniform float _All_alpha;
			UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
			uniform float4 _CameraDepthTexture_TexelSize;
			uniform float _soft_lz;

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float3 temp_cast_0 = (0.0).xxx;
				float2 uv_ddpyTex = v.ase_texcoord.xy * _ddpyTex_ST.xy + _ddpyTex_ST.zw;
				float time54 = _Time.y;
				float3 texCoord168 = v.ase_texcoord2.xyz;
				texCoord168.xy = v.ase_texcoord2.xyz.xy * float2( 1,1 ) + float2( 0,0 );
				float cd_ddpy_pow415 = texCoord168.z;
				float lerpResult417 = lerp( _DDpianyi , cd_ddpy_pow415 , _cd_ddpy_pow);
				float3 lerpResult411 = lerp( v.ase_normal , _ddpyxyz , _pymsqh);
				float2 uv_ddpymask = v.ase_texcoord.xy * _ddpymask_ST.xy + _ddpymask_ST.zw;
				#ifdef _DDPY_ON
				float3 staticSwitch115 = ( ( tex2Dlod( _ddpyTex, float4( ( uv_ddpyTex + frac( ( _DDpianyiUV * time54 ) ) ), 0, 0.0) ).r - 0.5 ) * lerpResult417 * lerpResult411 * tex2Dlod( _ddpymask, float4( uv_ddpymask, 0, 0.0) ).r );
				#else
				float3 staticSwitch115 = temp_cast_0;
				#endif
				float3 DDpianyi68 = staticSwitch115;
				
				float3 ase_worldNormal = UnityObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord4.xyz = ase_worldNormal;
				float4 ase_clipPos = UnityObjectToClipPos(v.vertex);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord5 = screenPos;
				
				o.ase_color = v.color;
				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				o.ase_texcoord2.xyz = v.ase_texcoord2.xyz;
				o.ase_texcoord3 = v.ase_texcoord1;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.zw = 0;
				o.ase_texcoord2.w = 0;
				o.ase_texcoord4.w = 0;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = DDpianyi68;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
				#endif
				float2 uv_Main_Tex = i.ase_texcoord1.xy * _Main_Tex_ST.xy + _Main_Tex_ST.zw;
				float time54 = _Time.y;
				float3 texCoord168 = i.ase_texcoord2.xyz;
				texCoord168.xy = i.ase_texcoord2.xyz.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult166 = (float2(texCoord168.x , texCoord168.y));
				float2 CD_Main_Tex_UV95 = appendResult166;
				#ifdef _MAINTEXUV_ON
				float2 staticSwitch250 = CD_Main_Tex_UV95;
				#else
				float2 staticSwitch250 = frac( ( _mainUV * time54 ) );
				#endif
				float cos379 = cos( _MainTexRotator );
				float sin379 = sin( _MainTexRotator );
				float2 rotator379 = mul( ( uv_Main_Tex + staticSwitch250 ) - float2( 0,0 ) , float2x2( cos379 , -sin379 , sin379 , cos379 )) + float2( 0,0 );
				float2 uv_NiuQuTex = i.ase_texcoord1.xy * _NiuQuTex_ST.xy + _NiuQuTex_ST.zw;
				float4 texCoord171 = i.ase_texcoord3;
				texCoord171.xy = i.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float CD_NQ_Pow94 = texCoord171.w;
				float lerpResult253 = lerp( _NiuQuPow , CD_NQ_Pow94 , _customNQQD);
				float rd_mask303 = saturate( (-0.16 + (( i.ase_texcoord1.xy.x * i.ase_texcoord1.xy.y * ( ( 1.0 - i.ase_texcoord1.xy.x ) * ( 1.0 - i.ase_texcoord1.xy.y ) ) * 6.01 ) - 0.01) * (0.92 - -0.16) / (0.15 - 0.01)) );
				#ifdef _NQBY_ON
				float staticSwitch335 = rd_mask303;
				#else
				float staticSwitch335 = 1.0;
				#endif
				#ifdef _NQ_ON
				float staticSwitch112 = ( ( tex2D( _NiuQuTex, ( uv_NiuQuTex + frac( ( _NiuQuUV * time54 ) ) ) ).r - 0.5 ) * lerpResult253 * staticSwitch335 );
				#else
				float staticSwitch112 = 0.0;
				#endif
				float NiuQu47 = staticSwitch112;
				float lerpResult273 = lerp( 0.0 , NiuQu47 , _nqMain_Tex1);
				float2 temp_output_51_0 = ( rotator379 + lerpResult273 );
				float4 tex2DNode2 = tex2D( _Main_Tex, temp_output_51_0 );
				float4 tex2DNode327 = tex2D( _Main_Tex, temp_output_51_0 );
				float4 _color3325 = ( ( _R_color * tex2DNode327.r ) + ( _G_color * tex2DNode327.g ) + ( _B_color * tex2DNode327.b ) );
				#ifdef _RGBCOLOR_ONOFF_ON
				float4 staticSwitch326 = _color3325;
				#else
				float4 staticSwitch326 = tex2DNode2;
				#endif
				float CD_RJ91 = ( 1.0 - texCoord171.x );
				float lerpResult251 = lerp( ( 1.0 - _RongJie ) , CD_RJ91 , _customRJ1);
				float2 uv_RongJieTex = i.ase_texcoord1.xy * _RongJieTex_ST.xy + _RongJieTex_ST.zw;
				float2 CenteredUV15_g2 = ( uv_RongJieTex - float2( 0.5,0.5 ) );
				float2 break17_g2 = CenteredUV15_g2;
				float2 appendResult23_g2 = (float2(( length( CenteredUV15_g2 ) * _polar_tilling.x * 2.0 ) , ( atan2( break17_g2.x , break17_g2.y ) * ( 1.0 / 6.28318548202515 ) * _polar_tilling.y )));
				float2 lerpResult377 = lerp( uv_RongJieTex , appendResult23_g2 , _polar);
				float cos353 = cos( _xz );
				float sin353 = sin( _xz );
				float2 rotator353 = mul( lerpResult377 - float2( 0.5,0.5 ) , float2x2( cos353 , -sin353 , sin353 , cos353 )) + float2( 0.5,0.5 );
				float2 appendResult101 = (float2(texCoord171.y , texCoord171.z));
				float2 CD_RJ_UV92 = appendResult101;
				#ifdef _CUSTUM_RJ_UV_ON
				float2 staticSwitch104 = CD_RJ_UV92;
				#else
				float2 staticSwitch104 = frac( ( _RJUVliudong * time54 ) );
				#endif
				float lerpResult270 = lerp( 0.0 , NiuQu47 , _NQRJTex);
				float4 tex2DNode6 = tex2D( _RongJieTex, ( rotator353 + staticSwitch104 + lerpResult270 ) );
				float2 uv_rjTex2 = i.ase_texcoord1.xy * _rjTex2_ST.xy + _rjTex2_ST.zw;
				float cos356 = cos( _rotator2 );
				float sin356 = sin( _rotator2 );
				float2 rotator356 = mul( uv_rjTex2 - float2( 0.5,0.5 ) , float2x2( cos356 , -sin356 , sin356 , cos356 )) + float2( 0.5,0.5 );
				float temp_output_284_0 = ( tex2D( _rjTex2, ( rotator356 + lerpResult270 ) ).r + _rj2 );
				float lerpResult345 = lerp( ( tex2DNode6.r * temp_output_284_0 ) , ( tex2DNode6.r + temp_output_284_0 ) , _rj2_ma);
				float lerpResult282 = lerp( tex2DNode6.r , lerpResult345 , _rjTex2onoff);
				float temp_output_16_0 = ( 1.0 - lerpResult282 );
				float smoothstepResult7 = smoothstep( lerpResult251 , ( lerpResult251 - _RongJieRY ) , temp_output_16_0);
				float temp_output_17_0 = ( lerpResult251 - _rongjieB );
				float smoothstepResult20 = smoothstep( temp_output_17_0 , ( temp_output_17_0 - _RongJieRY ) , temp_output_16_0);
				#ifdef _RJ_ON
				float4 staticSwitch242 = ( ( _RJBColor * ( smoothstepResult7 - smoothstepResult20 ) ) + ( smoothstepResult20 * _Main_Color1 ) );
				#else
				float4 staticSwitch242 = _Main_Color1;
				#endif
				float4 RongJie30 = staticSwitch242;
				float4 temp_cast_2 = (0.0).xxxx;
				float3 ase_worldViewDir = UnityWorldSpaceViewDir(WorldPosition);
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 ase_worldNormal = i.ase_texcoord4.xyz;
				float fresnelNdotV122 = dot( ase_worldNormal, ase_worldViewDir );
				float fresnelNode122 = ( _fresnelSize + _Float6 * pow( 1.0 - fresnelNdotV122, _fresnelPow ) );
				float lerpResult255 = lerp( fresnelNode122 , ( 1.0 - fresnelNode122 ) , _FanXiangFrenal);
				#ifdef _FRESNEL_ON
				float4 staticSwitch187 = ( _feinierColor * saturate( lerpResult255 ) );
				#else
				float4 staticSwitch187 = temp_cast_2;
				#endif
				float4 fresnel130 = staticSwitch187;
				float4 lerpResult262 = lerp( fresnel130 , ( fresnel130 * RongJie30 ) , _RJlK);
				float temp_output_175_0 = (lerpResult262).a;
				float lerpResult236 = lerp( tex2DNode2.r , tex2DNode2.a , _maintexRA);
				float Main_Tex_a183 = lerpResult236;
				float2 uv_maskTex = i.ase_texcoord1.xy * _maskTex_ST.xy + _maskTex_ST.zw;
				float lerpResult279 = lerp( 0.0 , NiuQu47 , _nqMask);
				float4 tex2DNode80 = tex2D( _maskTex, ( uv_maskTex + frac( ( _maskUVliuDong * time54 ) ) + lerpResult279 ) );
				float lerpResult223 = lerp( tex2DNode80.r , tex2DNode80.a , _mask_RA);
				#ifdef _MASK_ON
				float staticSwitch118 = lerpResult223;
				#else
				float staticSwitch118 = 1.0;
				#endif
				float mask82 = saturate( staticSwitch118 );
				float V_color_a194 = i.ase_color.a;
				float temp_output_169_0 = ( Main_Tex_a183 * mask82 * V_color_a194 * (RongJie30).a );
				float lerpResult257 = lerp( ( temp_output_175_0 + temp_output_169_0 ) , ( temp_output_175_0 * temp_output_169_0 ) , _frenalRJ);
				float4 screenPos = i.ase_texcoord5;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth341 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
				float distanceDepth341 = saturate( abs( ( screenDepth341 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _soft_lz ) ) );
				float RGB_alpha394 = ( ( _R_color.a * tex2DNode327.r ) + ( _G_color.a * tex2DNode327.g ) + ( _B_color.a * tex2DNode327.b ) );
				#ifdef _3COLORALPHA_ON
				float staticSwitch398 = RGB_alpha394;
				#else
				float staticSwitch398 = 1.0;
				#endif
				float4 appendResult402 = (float4((( ( float4( (i.ase_color).rgb , 0.0 ) * staticSwitch326 * float4( (RongJie30).rgb , 0.0 ) ) + fresnel130 )).rgb , saturate( ( lerpResult257 * _All_alpha * distanceDepth341 * staticSwitch398 ) )));
				
				
				finalColor = appendResult402;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	Fallback Off
}
/*ASEBEGIN
Version=19103
Node;AmplifyShaderEditor.CommentaryNode;336;-6177.283,-763.3359;Inherit;False;1647.487;528.1527;Comment;9;294;297;289;300;301;303;293;295;332;扭曲边缘遮罩;1,1,1,1;0;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;332;-6127.283,-713.3359;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;293;-5880.133,-596.1251;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;294;-5880.647,-456.209;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;34;-5620.875,791.616;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;79;-4267.257,-786.3394;Inherit;False;1860.424;944.6576;Comment;20;44;254;47;112;113;253;42;109;41;45;43;61;53;58;57;60;304;307;335;362;扭曲;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;54;-5311.622,781.2335;Inherit;False;time;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;295;-5699.592,-489.1832;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;297;-5635.152,-602.6164;Inherit;False;Constant;_Float5;Float 5;46;0;Create;True;0;0;0;False;0;False;6.01;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;289;-5462.144,-695.3623;Inherit;True;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;60;-4206.701,-563.1299;Inherit;False;Property;_NiuQuUV;扭曲UV流动;45;0;Create;False;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;57;-4197.835,-377.6837;Inherit;False;54;time;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;300;-5215.289,-524.999;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0.01;False;2;FLOAT;0.15;False;3;FLOAT;-0.16;False;4;FLOAT;0.92;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;-4034.375,-543.5253;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;301;-4921.334,-516.4113;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;362;-3894.47,-471.5134;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;53;-4217.257,-736.3394;Inherit;False;0;41;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;61;-3867.285,-685.3732;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;94;-5464.474,2121.781;Inherit;False;CD_NQ_Pow;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;303;-4753.796,-502.3519;Inherit;False;rd_mask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;304;-3464.875,-86.5163;Inherit;False;303;rd_mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;109;-3685.035,-232.1709;Inherit;False;94;CD_NQ_Pow;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;254;-3674.27,-147.0955;Inherit;False;Property;_customNQQD;custom扭曲强度;44;1;[Toggle];Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;45;-3730.102,-318.8631;Inherit;False;Property;_NiuQuPow;扭曲强度;43;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;43;-3591.102,-458.863;Inherit;False;Constant;_Float0;Float 0;9;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;41;-3695.694,-666.6322;Inherit;True;Property;_NiuQuTex;扭曲贴图;39;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;307;-3416.051,-192.027;Inherit;False;Constant;_Float7;Float 7;46;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;335;-3184.088,-251.7853;Inherit;False;Property;_nqby;固定扭曲边缘;46;0;Create;False;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;253;-3425.929,-336.1696;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;42;-3381.103,-501.863;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;113;-3175.225,-651.4954;Inherit;False;Constant;_1;1;21;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;-3216.103,-427.863;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;78;-4925.121,559.4558;Inherit;False;3459.432;1239.095;Comment;56;37;361;30;242;28;25;27;24;26;21;20;7;19;16;10;17;282;9;251;345;18;283;343;105;252;281;206;346;284;8;6;285;39;280;353;338;104;33;103;354;270;355;38;356;272;358;337;357;55;48;271;359;369;376;377;378;溶解;1,1,1,1;0;0
Node;AmplifyShaderEditor.StaticSwitch;112;-3002.347,-470.8774;Inherit;False;Property;_nq;——————扭曲————————————————;38;0;Create;False;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;47;-2699.591,-446.0722;Inherit;False;NiuQu;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;55;-4880.031,1190.62;Inherit;False;54;time;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;101;-5443.397,2019.319;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;376;-4885.241,758.0931;Inherit;False;Property;_polar_tilling;极坐标重复度;24;0;Create;False;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;33;-4912.185,605.5065;Inherit;False;0;6;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;37;-4885.5,1006.566;Inherit;False;Property;_RJUVliudong;溶解UV流动;31;0;Create;False;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;271;-4346.326,1074.017;Inherit;False;Constant;_nq_rjt;x;21;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;358;-4607.37,1634.907;Inherit;False;Property;_rotator2;旋转2;35;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;369;-4705.241,667.0931;Inherit;True;Polar Coordinates;-1;;2;7dab8e02884cf104ebefaa2e788e4162;0;4;1;FLOAT2;0,0;False;2;FLOAT2;0.5,0.5;False;3;FLOAT;1;False;4;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;378;-4425.124,740.3097;Inherit;False;Property;_polar;极坐标;23;1;[Toggle];Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-4715.32,1169.814;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;48;-4370.417,1168.432;Inherit;False;47;NiuQu;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;337;-4677.605,1369.472;Inherit;False;0;280;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;92;-5287.993,2026.88;Inherit;False;CD_RJ_UV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;357;-4653.37,1502.907;Inherit;False;Constant;_Vector1;Vector 1;56;0;Create;True;0;0;0;False;0;False;0.5,0.5;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;272;-4361.478,1254.705;Inherit;False;Property;_NQRJTex;扭曲溶解图;42;1;[Toggle];Create;False;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;359;-4577.024,1189.924;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;270;-4055.179,932.9104;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;103;-4602.482,1268.549;Inherit;False;92;CD_RJ_UV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;377;-4378.124,573.3097;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;355;-4525.347,878.9807;Inherit;False;Constant;_Vector0;Vector 0;56;0;Create;True;0;0;0;False;0;False;0.5,0.5;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;354;-4520.636,1005.903;Inherit;False;Property;_xz;旋转;22;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;356;-4416.37,1378.907;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RotatorNode;353;-4223.436,643.1035;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;338;-3980.605,1092.472;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;104;-4319.77,908.2883;Inherit;False;Property;_custum_rj_uv;custum溶解UV流动;30;0;Create;False;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;39;-3988.089,659.3055;Inherit;False;3;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;285;-3751.923,1068.737;Inherit;False;Property;_rj2;溶解2;36;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;280;-3860.745,864.3563;Inherit;True;Property;_rjTex2;溶解图2;34;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;205;-5546.846,1946.433;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;91;-5389.993,1934.498;Inherit;False;CD_RJ;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;284;-3609.026,1061.64;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;6;-3853.378,650.9774;Inherit;True;Property;_RongJieTex;溶解贴图;21;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;8;-4168.609,1189.256;Inherit;False;Property;_RongJie;溶解值;27;0;Create;False;0;0;0;False;0;False;0.92;0.92;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;252;-4197.426,1372.277;Inherit;False;Property;_customRJ1;custom溶解;26;1;[Toggle];Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;281;-3481.049,722.5206;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;343;-3476.924,924.9537;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;346;-3497.924,836.9537;Inherit;False;Property;_rj2_ma;溶解2模式-乘/加;33;1;[Toggle];Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;206;-4002.668,1207.467;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;105;-4167.143,1285.449;Inherit;False;91;CD_RJ;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-3973.105,1554.612;Inherit;False;Property;_rongjieB;双色溶解边缘宽度;28;0;Create;False;0;0;0;False;0;False;0.1108583;0.1108583;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;186;-3053.921,4267.969;Inherit;False;2089.441;877.4119;Comment;13;130;126;127;128;122;123;125;124;187;188;255;256;264;菲尼尔;1,1,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;345;-3313.924,742.9537;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;251;-3877.741,1261.718;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;283;-3444.655,1039.993;Inherit;False;Property;_rjTex2onoff;开启溶解图2;32;1;[Toggle];Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;123;-3003.921,4619.967;Inherit;False;Property;_fresnelSize;菲尼尔范围;66;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;237;-2289.497,-544.7785;Inherit;False;54;time;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;166;-5511.204,2338.637;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;238;-2292.608,-677.4506;Inherit;False;Property;_mainUV;主贴图UV流动;12;0;Create;False;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleSubtractOpNode;17;-3626.522,1526.682;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;117;-638.8363,2712.929;Inherit;False;1854.32;774.0223;Comment;17;276;277;278;279;267;80;207;223;269;266;268;82;229;118;119;265;364;遮罩图;1,1,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;282;-3124.182,656.7344;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;125;-3009.159,4827.463;Inherit;False;Property;_fresnelPow;菲尼尔强度;65;0;Create;False;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-3679.728,1322.639;Inherit;False;Property;_RongJieRY;溶解软硬;25;0;Create;False;0;0;0;False;0;False;0.001;0.001;0.001;1.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;122;-2817.613,4580.334;Inherit;True;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;19;-3384.777,1404.183;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;240;-2106.964,-608.2498;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;266;-603.457,3057.985;Inherit;False;54;time;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;268;-599.5034,2903.2;Inherit;False;Property;_maskUVliuDong;遮罩UV流动;51;0;Create;False;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.OneMinusNode;16;-2932.767,760.3784;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;10;-3380.664,1243.941;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;128;-2499.537,4642.254;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;276;-594.4437,3325.711;Inherit;False;Property;_nqMask;扭曲遮罩;41;1;[Toggle];Create;False;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;7;-2935.476,967.682;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;277;-594.8718,3245.1;Inherit;False;47;NiuQu;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;278;-599.8687,3164.896;Inherit;False;Constant;_nqMain_Tex3;flot12;40;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;269;-431.4277,2939.462;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;256;-2472.079,4897.544;Inherit;False;Property;_FanXiangFrenal;反向菲尼尔;67;1;[Toggle];Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;167;-2166.982,-426.2528;Inherit;False;95;CD_Main_Tex_UV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FractNode;365;-1980.288,-549.3535;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SmoothstepOpNode;20;-2957.338,1347.825;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;255;-2313.6,4578.449;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;50;-1903.995,-796.2391;Inherit;False;0;2;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;250;-1862.121,-544.8092;Inherit;False;Property;_MainTexUV;custom主贴图UV流动;11;0;Create;False;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FractNode;364;-368.2743,2852.145;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;279;-422.3508,3185.264;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;26;-2541.326,1148.923;Inherit;False;Property;_Main_Color1;主颜色(中心颜色);6;1;[HDR];Create;False;0;0;0;False;0;False;0.5849056,0.5849056,0.5849056,1;2.243137,0.7372549,3.968627,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;265;-591.7799,2761.992;Inherit;False;0;80;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;21;-2553.743,910.2158;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;275;-1649.476,-264.3641;Inherit;False;Property;_nqMain_Tex1;扭曲主贴图;40;1;[Toggle];Create;False;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;99;-1583.426,-611.7763;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;49;-1669.904,-365.976;Inherit;False;47;NiuQu;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;380;-1506.55,-527.2588;Inherit;False;Property;_MainTexRotator;主贴图旋转;9;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;274;-1603.901,-451.1801;Inherit;False;Constant;_nqMain_Tex;flot11;40;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-2253.782,797.1003;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;264;-2114.366,4601.066;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;127;-2200.92,4317.969;Inherit;False;Property;_feinierColor;菲尼尔颜色;63;1;[HDR];Create;False;0;0;0;False;0;False;0.5188679,0.5188679,0.5188679,1;0.5188679,0.5188679,0.5188679,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;267;-228.7242,2825.978;Inherit;False;3;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;207;90.56107,3021.582;Inherit;False;Property;_mask_RA;遮罩图R/A;50;1;[Toggle];Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;126;-1933.616,4560.783;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;188;-1777.982,4379.362;Inherit;False;Constant;_Float9;Float 9;31;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;273;-1433.383,-435.8121;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;80;-70.31247,2797.796;Inherit;True;Property;_maskTex;遮罩图;49;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;77;-3379.62,2984.284;Inherit;False;2351.031;1047.509;;22;413;412;410;411;287;66;71;64;68;115;116;65;70;76;72;363;75;73;74;416;417;418;顶点偏移;1,1,1,1;0;0
Node;AmplifyShaderEditor.RotatorNode;379;-1418.55,-701.2588;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;119;254.3549,2756.471;Inherit;False;Constant;_Float3;Float 3;22;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;74;-3318.62,3210.285;Inherit;False;Property;_DDpianyiUV;顶点偏移UV流动;57;0;Create;False;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.LerpOp;223;235.3506,2867.659;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;187;-1587.305,4524.441;Inherit;False;Property;_fresnel;——————菲尼尔————————————————;62;0;Create;False;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;73;-3293.62,3358.285;Inherit;False;54;time;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;329;-2028.135,-1792.702;Inherit;False;916.7438;807.6296;Comment;13;327;319;320;318;322;321;323;324;325;384;385;386;387;3色调整;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;51;-1294.915,-553.7899;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;319;-1958.177,-1565.702;Inherit;False;Property;_G_color;G_color;17;1;[HDR];Create;True;0;0;0;False;0;False;0,1,0,1;0,1,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-1117.006,-542.3409;Inherit;True;Property;_Main_Tex;主贴图;7;0;Create;False;0;0;0;False;0;False;-1;None;c46637ff78a164e418bf534c701c123c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;320;-1956.177,-1384.702;Inherit;False;Property;_B_color;B_color;18;1;[HDR];Create;True;0;0;0;False;0;False;0,0,1,1;0,0,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;327;-1981.135,-1202.073;Inherit;True;Property;_TextureSample0;Texture Sample 0;7;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;2;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;318;-1963.177,-1742.702;Inherit;False;Property;_R_color;R_color;16;1;[HDR];Create;True;0;0;0;False;0;False;1,0,0,1;1,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;75;-3102.62,3242.285;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;235;-1016.424,-289.2698;Inherit;False;Property;_maintexRA;主贴图通道R/A;10;1;[Toggle];Create;False;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;118;440.1664,2805.851;Inherit;False;Property;_mask;——————遮罩————————————————;48;0;Create;False;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;130;-1272.098,4612.796;Inherit;False;fresnel;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;323;-1635.432,-1338.763;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FractNode;363;-2961.837,3253.133;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;182;-1302.594,-63.48344;Inherit;False;130;fresnel;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;181;-1289.055,383.1097;Inherit;False;30;RongJie;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;236;-769.4241,-325.2698;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;321;-1606.351,-1707.457;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;4;-531.9063,-756.4598;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;322;-1641.072,-1512.47;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;229;769.3889,2798.022;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;72;-3329.62,3034.284;Inherit;False;0;64;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;183;-640.3953,-161.0146;Inherit;False;Main_Tex_a;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;386;-1486.512,-1153.932;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;385;-1481.512,-1262.932;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;76;-2937.619,3043.284;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;384;-1471.512,-1381.932;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;194;-300.4779,-666.2883;Inherit;False;V_color_a;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;263;-1043.18,-2.094498;Inherit;False;Property;_RJlK;溶解轮廓;68;1;[Toggle];Create;False;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;261;-1055.211,123.5532;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;324;-1442.492,-1590.67;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;82;929.8298,2869.776;Inherit;False;mask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;184;-905.3218,207.3758;Inherit;False;183;Main_Tex_a;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;262;-876.2109,-59.44684;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;325;-1284.391,-1553.414;Inherit;False;_color3;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;174;-1008.19,465.9046;Inherit;False;False;False;False;True;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;387;-1303.512,-1361.932;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;169;-696.2157,313.4575;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;328;-779.9963,-514.06;Inherit;False;325;_color3;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;394;-1122.106,-1357.952;Inherit;False;RGB_alpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;175;-719.2029,50.46649;Inherit;False;False;False;False;True;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;70;-2167.713,3128.17;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;258;-374.5286,307.7601;Inherit;False;Property;_frenalRJ;菲尼尔遮罩影响溶解;69;1;[Toggle];Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;399;86.51782,470.3504;Inherit;False;Constant;_Float4;Float 4;61;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;342;-545.9948,672.8927;Inherit;False;Property;_soft_lz;软粒子;71;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;388;-4.627777,631.7558;Inherit;False;394;RGB_alpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;-1889.691,3247.333;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;176;-449.6563,-0.2366982;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;116;-1919.632,3103.39;Inherit;False;Constant;_Float2;Float 2;21;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;326;-601.8394,-584.087;Inherit;False;Property;_RGBcolor_onoff;RGB三色 off/on;14;0;Create;False;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;185;-356.494,152.1602;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;200;-378.7057,409.9619;Inherit;False;Property;_All_alpha;Alpha;0;0;Create;False;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;341;-330.9828,592.1208;Inherit;False;True;True;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;257;-190.3493,44.64703;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;115;-1721.893,3122.526;Inherit;False;Property;_DDpy;——————顶点偏移——————————————;53;0;Create;False;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;131;-391.8301,-156.3898;Inherit;False;130;fresnel;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-300.3302,-396.6386;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;398;236.5178,533.3504;Inherit;False;Property;_3colorAlpha;3色Alpha_off/on;15;0;Create;False;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;68;-1351.216,3282.927;Inherit;False;DDpianyi;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;259;-29.21094,133.5532;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;172;-133.0114,-353.3738;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;204;-5647.962,648.3708;Half;False;Property;_EffectTimeScale;_EffectTimeScale;70;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;349;750.7202,-303.4568;Inherit;False;Property;_m_mask;遮罩;47;0;Create;False;0;0;0;True;1;Main(mask,_KEYWORD,on,off);False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;352;934.7815,-535.9681;Inherit;False;Property;_m_3color;三色;13;0;Create;False;0;0;0;True;1;Main(3color,_KEYWORD,on,off);False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;351;796.0829,-103.5939;Inherit;False;Property;_m_ddpianyi1;菲尼尔;61;0;Create;False;0;0;0;True;1;Main(fne,_KEYWORD,on,off);False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;69;337.4653,379.5786;Inherit;False;68;DDpianyi;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;213;166.68,2.317089;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;226;40.85071,-239.5532;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FmodOpNode;360;-5457.974,227.9649;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;361;-4595.273,1072.365;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;348;740.7422,-417.948;Inherit;False;Property;_m_niuqu;扭曲;37;0;Create;False;0;0;0;True;1;Main(nq,_KEYWORD,on,off);False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;202;-5306.962,599.3708;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;347;759.095,-534.0172;Inherit;False;Property;_m_rongjie;溶解;19;0;Create;False;0;0;0;True;1;Main(rj,_KEYWORD,on,off);False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;350;757.9875,-195.4357;Inherit;False;Property;_m_ddpianyi;定点偏移;52;0;Create;False;0;0;0;True;1;Main(ddpy,_KEYWORD,on,off);False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;402;350.1144,-195.783;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;403;1410.349,43.01209;Inherit;False;Property;_Dst;Dst;2;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.BlendMode;True;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;404;1412.349,-50.98787;Inherit;False;Property;_Src;Src;1;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.BlendMode;True;0;False;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;405;1413.802,-231.4023;Inherit;False;Property;_cull_;背面剔除;8;1;[Enum];Create;False;0;0;1;UnityEngine.Rendering.CullMode;True;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;64;-2570.114,3041.951;Inherit;True;Property;_ddpyTex;顶点偏移图;54;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;71;-2427.715,3231.171;Inherit;False;Constant;_Float1;Float 1;14;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;192;-169.3649,-785.4578;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;340;-533.4808,-315.7078;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;157;-679.038,-420.5663;Inherit;False;30;RongJie;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-2258.668,1286.364;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;30;-1684.54,1019.623;Inherit;False;RongJie;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;83;-942.4446,287.2107;Inherit;False;82;mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;195;-946.2463,380.1214;Inherit;False;194;V_color_a;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;24;-2552.061,653.8524;Inherit;False;Property;_RJBColor;溶解边颜色;29;1;[HDR];Create;False;0;0;0;False;0;False;1,0.4103774,0.4103774,1;1,0.4103774,0.4103774,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;242;-1963.86,842.6774;Inherit;False;Property;_rj;——————溶解————————————————;20;0;Create;False;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;28;-2138.703,1114.611;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;171;-5778.222,1966.396;Inherit;False;1;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;168;-5750.204,2322.637;Inherit;False;2;-1;3;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;95;-5289.03,2338.604;Inherit;False;CD_Main_Tex_UV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;415;-5361.332,2481.694;Inherit;False;cd_ddpy_pow;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;287;-2607.31,3508.278;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;411;-2100.178,3577.841;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector3Node;410;-2568.178,3657.841;Inherit;False;Property;_ddpyxyz;定点偏移xyz;59;0;Create;False;0;0;0;False;0;False;1,1,1;1,1,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;412;-2343.178,3731.841;Inherit;False;Property;_pymsqh;偏移模式切换;58;1;[Toggle];Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;413;-2141.258,3762.314;Inherit;True;Property;_ddpymask;顶点偏移遮罩;60;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;66;-2295.792,3295.744;Inherit;False;Property;_DDpianyi;顶点偏移强度;56;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;416;-2350.804,3389.561;Inherit;False;415;cd_ddpy_pow;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;418;-2271.804,3485.561;Inherit;False;Property;_cd_ddpy_pow;custom顶点偏移Pow;55;1;[Toggle];Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;417;-2060.804,3306.561;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;124;-2998.921,4691.967;Inherit;False;Property;_Float6;Float 6;64;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;406;1413.349,-135.9879;Inherit;False;Property;_Zdethp;Z深度模式;4;1;[Enum];Create;False;0;0;1;UnityEngine.Rendering.CompareFunction;True;0;False;4;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;407;1411.805,153.256;Inherit;False;Property;_xrsd;写入深度;3;2;[Toggle];[Enum];Create;False;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;419;1033.69,119.5717;Inherit;False;Property;stenci_lbuffer;渲染顺序（遮罩剔除）;5;1;[Enum];Create;False;0;0;1;UnityEngine.Rendering.CompareFunction;True;0;False;0;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;401;512.171,-73.76648;Float;False;True;-1;2;ASEMaterialInspector;100;5;VFX/vfx_w_shader_ase_rongjie_URP;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;True;2;5;True;_Src;10;True;_Dst;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;True;True;0;True;_cull_;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;True;True;True;1;False;;255;False;;255;False;;7;True;stenci_lbuffer;1;False;;1;False;;1;False;;0;False;;0;False;;0;False;;0;False;;True;True;2;True;_xrsd;True;3;True;_Zdethp;True;True;0;False;;0;False;;True;2;RenderType=Opaque=RenderType;Queue=Transparent=Queue=0;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;1;True;False;;False;0
WireConnection;293;0;332;1
WireConnection;294;0;332;2
WireConnection;54;0;34;0
WireConnection;295;0;293;0
WireConnection;295;1;294;0
WireConnection;289;0;332;1
WireConnection;289;1;332;2
WireConnection;289;2;295;0
WireConnection;289;3;297;0
WireConnection;300;0;289;0
WireConnection;58;0;60;0
WireConnection;58;1;57;0
WireConnection;301;0;300;0
WireConnection;362;0;58;0
WireConnection;61;0;53;0
WireConnection;61;1;362;0
WireConnection;94;0;171;4
WireConnection;303;0;301;0
WireConnection;41;1;61;0
WireConnection;335;1;307;0
WireConnection;335;0;304;0
WireConnection;253;0;45;0
WireConnection;253;1;109;0
WireConnection;253;2;254;0
WireConnection;42;0;41;1
WireConnection;42;1;43;0
WireConnection;44;0;42;0
WireConnection;44;1;253;0
WireConnection;44;2;335;0
WireConnection;112;1;113;0
WireConnection;112;0;44;0
WireConnection;47;0;112;0
WireConnection;101;0;171;2
WireConnection;101;1;171;3
WireConnection;369;1;33;0
WireConnection;369;3;376;1
WireConnection;369;4;376;2
WireConnection;38;0;37;0
WireConnection;38;1;55;0
WireConnection;92;0;101;0
WireConnection;359;0;38;0
WireConnection;270;0;271;0
WireConnection;270;1;48;0
WireConnection;270;2;272;0
WireConnection;377;0;33;0
WireConnection;377;1;369;0
WireConnection;377;2;378;0
WireConnection;356;0;337;0
WireConnection;356;1;357;0
WireConnection;356;2;358;0
WireConnection;353;0;377;0
WireConnection;353;1;355;0
WireConnection;353;2;354;0
WireConnection;338;0;356;0
WireConnection;338;1;270;0
WireConnection;104;1;359;0
WireConnection;104;0;103;0
WireConnection;39;0;353;0
WireConnection;39;1;104;0
WireConnection;39;2;270;0
WireConnection;280;1;338;0
WireConnection;205;0;171;1
WireConnection;91;0;205;0
WireConnection;284;0;280;1
WireConnection;284;1;285;0
WireConnection;6;1;39;0
WireConnection;281;0;6;1
WireConnection;281;1;284;0
WireConnection;343;0;6;1
WireConnection;343;1;284;0
WireConnection;206;0;8;0
WireConnection;345;0;281;0
WireConnection;345;1;343;0
WireConnection;345;2;346;0
WireConnection;251;0;206;0
WireConnection;251;1;105;0
WireConnection;251;2;252;0
WireConnection;166;0;168;1
WireConnection;166;1;168;2
WireConnection;17;0;251;0
WireConnection;17;1;18;0
WireConnection;282;0;6;1
WireConnection;282;1;345;0
WireConnection;282;2;283;0
WireConnection;122;1;123;0
WireConnection;122;2;124;0
WireConnection;122;3;125;0
WireConnection;19;0;17;0
WireConnection;19;1;9;0
WireConnection;240;0;238;0
WireConnection;240;1;237;0
WireConnection;16;0;282;0
WireConnection;10;0;251;0
WireConnection;10;1;9;0
WireConnection;128;0;122;0
WireConnection;7;0;16;0
WireConnection;7;1;251;0
WireConnection;7;2;10;0
WireConnection;269;0;268;0
WireConnection;269;1;266;0
WireConnection;365;0;240;0
WireConnection;20;0;16;0
WireConnection;20;1;17;0
WireConnection;20;2;19;0
WireConnection;255;0;122;0
WireConnection;255;1;128;0
WireConnection;255;2;256;0
WireConnection;250;1;365;0
WireConnection;250;0;167;0
WireConnection;364;0;269;0
WireConnection;279;0;278;0
WireConnection;279;1;277;0
WireConnection;279;2;276;0
WireConnection;21;0;7;0
WireConnection;21;1;20;0
WireConnection;99;0;50;0
WireConnection;99;1;250;0
WireConnection;25;0;24;0
WireConnection;25;1;21;0
WireConnection;264;0;255;0
WireConnection;267;0;265;0
WireConnection;267;1;364;0
WireConnection;267;2;279;0
WireConnection;126;0;127;0
WireConnection;126;1;264;0
WireConnection;273;0;274;0
WireConnection;273;1;49;0
WireConnection;273;2;275;0
WireConnection;80;1;267;0
WireConnection;379;0;99;0
WireConnection;379;2;380;0
WireConnection;223;0;80;1
WireConnection;223;1;80;4
WireConnection;223;2;207;0
WireConnection;187;1;188;0
WireConnection;187;0;126;0
WireConnection;51;0;379;0
WireConnection;51;1;273;0
WireConnection;2;1;51;0
WireConnection;327;1;51;0
WireConnection;75;0;74;0
WireConnection;75;1;73;0
WireConnection;118;1;119;0
WireConnection;118;0;223;0
WireConnection;130;0;187;0
WireConnection;323;0;320;0
WireConnection;323;1;327;3
WireConnection;363;0;75;0
WireConnection;236;0;2;1
WireConnection;236;1;2;4
WireConnection;236;2;235;0
WireConnection;321;0;318;0
WireConnection;321;1;327;1
WireConnection;322;0;319;0
WireConnection;322;1;327;2
WireConnection;229;0;118;0
WireConnection;183;0;236;0
WireConnection;386;0;320;4
WireConnection;386;1;327;3
WireConnection;385;0;319;4
WireConnection;385;1;327;2
WireConnection;76;0;72;0
WireConnection;76;1;363;0
WireConnection;384;0;318;4
WireConnection;384;1;327;1
WireConnection;194;0;4;4
WireConnection;261;0;182;0
WireConnection;261;1;181;0
WireConnection;324;0;321;0
WireConnection;324;1;322;0
WireConnection;324;2;323;0
WireConnection;82;0;229;0
WireConnection;262;0;182;0
WireConnection;262;1;261;0
WireConnection;262;2;263;0
WireConnection;325;0;324;0
WireConnection;174;0;181;0
WireConnection;387;0;384;0
WireConnection;387;1;385;0
WireConnection;387;2;386;0
WireConnection;169;0;184;0
WireConnection;169;1;83;0
WireConnection;169;2;195;0
WireConnection;169;3;174;0
WireConnection;394;0;387;0
WireConnection;175;0;262;0
WireConnection;70;0;64;1
WireConnection;70;1;71;0
WireConnection;65;0;70;0
WireConnection;65;1;417;0
WireConnection;65;2;411;0
WireConnection;65;3;413;1
WireConnection;176;0;175;0
WireConnection;176;1;169;0
WireConnection;326;1;2;0
WireConnection;326;0;328;0
WireConnection;185;0;175;0
WireConnection;185;1;169;0
WireConnection;341;0;342;0
WireConnection;257;0;176;0
WireConnection;257;1;185;0
WireConnection;257;2;258;0
WireConnection;115;1;116;0
WireConnection;115;0;65;0
WireConnection;5;0;192;0
WireConnection;5;1;326;0
WireConnection;5;2;340;0
WireConnection;398;1;399;0
WireConnection;398;0;388;0
WireConnection;68;0;115;0
WireConnection;259;0;257;0
WireConnection;259;1;200;0
WireConnection;259;2;341;0
WireConnection;259;3;398;0
WireConnection;172;0;5;0
WireConnection;172;1;131;0
WireConnection;213;0;259;0
WireConnection;226;0;172;0
WireConnection;361;2;37;0
WireConnection;202;0;204;0
WireConnection;202;1;34;0
WireConnection;402;0;226;0
WireConnection;402;3;213;0
WireConnection;64;1;76;0
WireConnection;192;0;4;0
WireConnection;340;0;157;0
WireConnection;27;0;20;0
WireConnection;27;1;26;0
WireConnection;30;0;242;0
WireConnection;242;1;26;0
WireConnection;242;0;28;0
WireConnection;28;0;25;0
WireConnection;28;1;27;0
WireConnection;95;0;166;0
WireConnection;415;0;168;3
WireConnection;411;0;287;0
WireConnection;411;1;410;0
WireConnection;411;2;412;0
WireConnection;417;0;66;0
WireConnection;417;1;416;0
WireConnection;417;2;418;0
WireConnection;401;0;402;0
WireConnection;401;1;69;0
ASEEND*/
//CHKSM=548B076591475BF485E8D850358FB80E3113C8C3