// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Blockout/Gate_Particle"
{
	Properties
	{
		[Header(Refraction)]
		_ChromaticAberration("Chromatic Aberration", Range( 0 , 0.3)) = 0.1
		_SpecColor("Specular Color",Color)=(1,1,1,1)
		_Color_2("Color_2", Color) = (0.2980392,0.2980392,0.2980392,1)
		_Color_1("Color_1", Color) = (0.909804,0.909804,0.909804,1)
		_Spinner_2("Spinner_2", Float) = 0.8
		_Texture0("Texture 0", 2D) = "white" {}
		_Spinner_1("Spinner_1", Float) = 1
		_Spinner_4("Spinner_4", Float) = 0.6
		_Spinner_3("Spinner_3", Float) = 1
		_Depth_Blend("Depth_Blend", Float) = 1
		_Alpha("Alpha", Float) = 0.6
		_Wibblyocity("Wibblyocity", Float) = 1
		_Pan_Up("Pan_Up", Float) = 0.5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		GrabPass{ }
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma multi_compile _ALPHAPREMULTIPLY_ON
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPos;
			float3 worldPos;
		};

		uniform float _Wibblyocity;
		uniform sampler2D _Texture0;
		uniform float _Spinner_1;
		uniform float _Spinner_2;
		uniform float _Spinner_3;
		uniform float _Spinner_4;
		uniform float4 _Color_1;
		uniform float4 _Color_2;
		uniform float _Pan_Up;
		uniform float _Alpha;
		uniform sampler2D _CameraDepthTexture;
		uniform float _Depth_Blend;
		uniform sampler2D _GrabTexture;
		uniform float _ChromaticAberration;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float cos34 = cos( ( _Spinner_1 * _Time.y ) );
			float sin34 = sin( ( _Spinner_1 * _Time.y ) );
			float2 rotator34 = mul( v.texcoord.xy - float2( 0.5,0.5 ) , float2x2( cos34 , -sin34 , sin34 , cos34 )) + float2( 0.5,0.5 );
			float cos35 = cos( ( _Spinner_2 * _Time.y ) );
			float sin35 = sin( ( _Spinner_2 * _Time.y ) );
			float2 rotator35 = mul( v.texcoord.xy - float2( 0.5,0.5 ) , float2x2( cos35 , -sin35 , sin35 , cos35 )) + float2( 0.5,0.5 );
			float blendOpSrc13 = tex2Dlod( _Texture0, float4( rotator34, 0, 0.0) ).r;
			float blendOpDest13 = tex2Dlod( _Texture0, float4( rotator35, 0, 0.0) ).g;
			float cos36 = cos( ( -_Spinner_3 * _Time.y ) );
			float sin36 = sin( ( -_Spinner_3 * _Time.y ) );
			float2 rotator36 = mul( v.texcoord.xy - float2( 0.5,0.5 ) , float2x2( cos36 , -sin36 , sin36 , cos36 )) + float2( 0.5,0.5 );
			float cos37 = cos( ( -_Spinner_4 * _Time.y ) );
			float sin37 = sin( ( -_Spinner_4 * _Time.y ) );
			float2 rotator37 = mul( v.texcoord.xy - float2( 0.5,0.5 ) , float2x2( cos37 , -sin37 , sin37 , cos37 )) + float2( 0.5,0.5 );
			float blendOpSrc14 = tex2Dlod( _Texture0, float4( rotator36, 0, 0.0) ).r;
			float blendOpDest14 = tex2Dlod( _Texture0, float4( rotator37, 0, 0.0) ).g;
			float blendOpSrc12 = ( saturate( (( blendOpDest13 > 0.5 ) ? ( 1.0 - ( 1.0 - 2.0 * ( blendOpDest13 - 0.5 ) ) * ( 1.0 - blendOpSrc13 ) ) : ( 2.0 * blendOpDest13 * blendOpSrc13 ) ) ));
			float blendOpDest12 = ( saturate( abs( blendOpSrc14 - blendOpDest14 ) ));
			float temp_output_12_0 = ( saturate( ( 0.5 - 2.0 * ( blendOpSrc12 - 0.5 ) * ( blendOpDest12 - 0.5 ) ) ));
			float3 ase_worldNormal = UnityObjectToWorldNormal( v.normal );
			v.vertex.xyz += ( _Wibblyocity * ( temp_output_12_0 * ase_worldNormal ) );
		}

		inline float4 Refraction( Input i, SurfaceOutput o, float indexOfRefraction, float chomaticAberration ) {
			float3 worldNormal = o.Normal;
			float4 screenPos = i.screenPos;
			#if UNITY_UV_STARTS_AT_TOP
				float scale = -1.0;
			#else
				float scale = 1.0;
			#endif
			float halfPosW = screenPos.w * 0.5;
			screenPos.y = ( screenPos.y - halfPosW ) * _ProjectionParams.x * scale + halfPosW;
			#if SHADER_API_D3D9 || SHADER_API_D3D11
				screenPos.w += 0.00000000001;
			#endif
			float2 projScreenPos = ( screenPos / screenPos.w ).xy;
			float3 worldViewDir = normalize( UnityWorldSpaceViewDir( i.worldPos ) );
			float3 refractionOffset = ( ( ( ( indexOfRefraction - 1.0 ) * mul( UNITY_MATRIX_V, float4( worldNormal, 0.0 ) ) ) * ( 1.0 / ( screenPos.z + 1.0 ) ) ) * ( 1.0 - dot( worldNormal, worldViewDir ) ) );
			float2 cameraRefraction = float2( refractionOffset.x, -( refractionOffset.y * _ProjectionParams.x ) );
			float4 redAlpha = tex2D( _GrabTexture, ( projScreenPos + cameraRefraction ) );
			float green = tex2D( _GrabTexture, ( projScreenPos + ( cameraRefraction * ( 1.0 - chomaticAberration ) ) ) ).g;
			float blue = tex2D( _GrabTexture, ( projScreenPos + ( cameraRefraction * ( 1.0 + chomaticAberration ) ) ) ).b;
			return float4( redAlpha.r, green, blue, redAlpha.a );
		}

		void RefractionF( Input i, SurfaceOutput o, inout half4 color )
		{
			#ifdef UNITY_PASS_FORWARDBASE
			float clampResult16 = clamp( ( ( 1.0 - length( (float2( -1,-1 ) + (i.uv_texcoord - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 ))) ) ) * 2.33 ) , 0.0 , 1.0 );
			float cos34 = cos( ( _Spinner_1 * _Time.y ) );
			float sin34 = sin( ( _Spinner_1 * _Time.y ) );
			float2 rotator34 = mul( i.uv_texcoord - float2( 0.5,0.5 ) , float2x2( cos34 , -sin34 , sin34 , cos34 )) + float2( 0.5,0.5 );
			float cos35 = cos( ( _Spinner_2 * _Time.y ) );
			float sin35 = sin( ( _Spinner_2 * _Time.y ) );
			float2 rotator35 = mul( i.uv_texcoord - float2( 0.5,0.5 ) , float2x2( cos35 , -sin35 , sin35 , cos35 )) + float2( 0.5,0.5 );
			float blendOpSrc13 = tex2D( _Texture0, rotator34 ).r;
			float blendOpDest13 = tex2D( _Texture0, rotator35 ).g;
			float cos36 = cos( ( -_Spinner_3 * _Time.y ) );
			float sin36 = sin( ( -_Spinner_3 * _Time.y ) );
			float2 rotator36 = mul( i.uv_texcoord - float2( 0.5,0.5 ) , float2x2( cos36 , -sin36 , sin36 , cos36 )) + float2( 0.5,0.5 );
			float cos37 = cos( ( -_Spinner_4 * _Time.y ) );
			float sin37 = sin( ( -_Spinner_4 * _Time.y ) );
			float2 rotator37 = mul( i.uv_texcoord - float2( 0.5,0.5 ) , float2x2( cos37 , -sin37 , sin37 , cos37 )) + float2( 0.5,0.5 );
			float blendOpSrc14 = tex2D( _Texture0, rotator36 ).r;
			float blendOpDest14 = tex2D( _Texture0, rotator37 ).g;
			float blendOpSrc12 = ( saturate( (( blendOpDest13 > 0.5 ) ? ( 1.0 - ( 1.0 - 2.0 * ( blendOpDest13 - 0.5 ) ) * ( 1.0 - blendOpSrc13 ) ) : ( 2.0 * blendOpDest13 * blendOpSrc13 ) ) ));
			float blendOpDest12 = ( saturate( abs( blendOpSrc14 - blendOpDest14 ) ));
			float temp_output_12_0 = ( saturate( ( 0.5 - 2.0 * ( blendOpSrc12 - 0.5 ) * ( blendOpDest12 - 0.5 ) ) ));
			float blendOpSrc15 = clampResult16;
			float blendOpDest15 = temp_output_12_0;
			float temp_output_15_0 = ( saturate( ( blendOpSrc15 * blendOpDest15 ) ));
			float2 appendResult53 = (float2(temp_output_15_0 , temp_output_15_0));
			float blendOpSrc51 = temp_output_12_0;
			float blendOpDest51 = ( 1.0 - ( 5.0 * ( i.uv_texcoord.y + -_Pan_Up ) ) );
			float clampResult4 = clamp( ( saturate( (( blendOpSrc51 > 0.5 )? ( blendOpDest51 + 2.0 * blendOpSrc51 - 1.0 ) : ( blendOpDest51 + 2.0 * ( blendOpSrc51 - 0.5 ) ) ) )) , 0.0 , 1.0 );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth63 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth63 = abs( ( screenDepth63 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Depth_Blend ) );
			color.rgb = color.rgb + Refraction( i, o, ( ( ( appendResult53 * 0.1 ) * clampResult4 ) * distanceDepth63 ).x, _ChromaticAberration ) * ( 1 - color.a );
			color.a = 1;
			#endif
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Normal = float3(0,0,1);
			float cos34 = cos( ( _Spinner_1 * _Time.y ) );
			float sin34 = sin( ( _Spinner_1 * _Time.y ) );
			float2 rotator34 = mul( i.uv_texcoord - float2( 0.5,0.5 ) , float2x2( cos34 , -sin34 , sin34 , cos34 )) + float2( 0.5,0.5 );
			float cos35 = cos( ( _Spinner_2 * _Time.y ) );
			float sin35 = sin( ( _Spinner_2 * _Time.y ) );
			float2 rotator35 = mul( i.uv_texcoord - float2( 0.5,0.5 ) , float2x2( cos35 , -sin35 , sin35 , cos35 )) + float2( 0.5,0.5 );
			float blendOpSrc13 = tex2D( _Texture0, rotator34 ).r;
			float blendOpDest13 = tex2D( _Texture0, rotator35 ).g;
			float cos36 = cos( ( -_Spinner_3 * _Time.y ) );
			float sin36 = sin( ( -_Spinner_3 * _Time.y ) );
			float2 rotator36 = mul( i.uv_texcoord - float2( 0.5,0.5 ) , float2x2( cos36 , -sin36 , sin36 , cos36 )) + float2( 0.5,0.5 );
			float cos37 = cos( ( -_Spinner_4 * _Time.y ) );
			float sin37 = sin( ( -_Spinner_4 * _Time.y ) );
			float2 rotator37 = mul( i.uv_texcoord - float2( 0.5,0.5 ) , float2x2( cos37 , -sin37 , sin37 , cos37 )) + float2( 0.5,0.5 );
			float blendOpSrc14 = tex2D( _Texture0, rotator36 ).r;
			float blendOpDest14 = tex2D( _Texture0, rotator37 ).g;
			float blendOpSrc12 = ( saturate( (( blendOpDest13 > 0.5 ) ? ( 1.0 - ( 1.0 - 2.0 * ( blendOpDest13 - 0.5 ) ) * ( 1.0 - blendOpSrc13 ) ) : ( 2.0 * blendOpDest13 * blendOpSrc13 ) ) ));
			float blendOpDest12 = ( saturate( abs( blendOpSrc14 - blendOpDest14 ) ));
			float temp_output_12_0 = ( saturate( ( 0.5 - 2.0 * ( blendOpSrc12 - 0.5 ) * ( blendOpDest12 - 0.5 ) ) ));
			float4 temp_cast_1 = (abs( ( 1.0 - temp_output_12_0 ) )).xxxx;
			float div8=256.0/float((int)5.0);
			float4 posterize8 = ( floor( temp_cast_1 * div8 ) / div8 );
			float lerpResult5 = lerp( _Color_1.r , _Color_2.r , posterize8.r);
			float blendOpSrc51 = temp_output_12_0;
			float blendOpDest51 = ( 1.0 - ( 5.0 * ( i.uv_texcoord.y + -_Pan_Up ) ) );
			float clampResult4 = clamp( ( saturate( (( blendOpSrc51 > 0.5 )? ( blendOpDest51 + 2.0 * blendOpSrc51 - 1.0 ) : ( blendOpDest51 + 2.0 * ( blendOpSrc51 - 0.5 ) ) ) )) , 0.0 , 1.0 );
			float3 temp_cast_3 = (( lerpResult5 * clampResult4 )).xxx;
			o.Emission = temp_cast_3;
			float clampResult16 = clamp( ( ( 1.0 - length( (float2( -1,-1 ) + (i.uv_texcoord - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 ))) ) ) * 2.33 ) , 0.0 , 1.0 );
			float blendOpSrc15 = clampResult16;
			float blendOpDest15 = temp_output_12_0;
			float temp_output_15_0 = ( saturate( ( blendOpSrc15 * blendOpDest15 ) ));
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth63 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth63 = abs( ( screenDepth63 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Depth_Blend ) );
			o.Alpha = ( ( ( _Alpha * temp_output_15_0 ) * clampResult4 ) * distanceDepth63 );
			o.Normal = o.Normal + 0.00001 * i.screenPos * i.worldPos;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf BlinnPhong keepalpha finalcolor:RefractionF fullforwardshadows exclude_path:deferred vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float4 screenPos : TEXCOORD3;
				float4 tSpace0 : TEXCOORD4;
				float4 tSpace1 : TEXCOORD5;
				float4 tSpace2 : TEXCOORD6;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.screenPos = IN.screenPos;
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15401
252;41;2107;991;6002.836;-31.17796;1.897318;True;True
Node;AmplifyShaderEditor.RangedFloatNode;46;-5520.553,1189.734;Float;False;Property;_Spinner_3;Spinner_3;9;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;48;-5499.752,1466.634;Float;False;Property;_Spinner_4;Spinner_4;8;0;Create;True;0;0;False;0;0.6;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;49;-5255.351,1477.034;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;50;-5310.433,1663.612;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;23;-4919.932,-245.7537;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;45;-5274.851,915.4333;Float;False;Property;_Spinner_2;Spinner_2;5;0;Create;True;0;0;False;0;0.8;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;43;-5272.252,638.5339;Float;False;Property;_Spinner_1;Spinner_1;7;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;47;-5291.753,1191.034;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-4984.854,644.1381;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;-4964.612,1470.038;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;-4976.758,1200.811;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-4972.709,915.3896;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;55;-3736.237,-295.4812;Float;True;5;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;1,1;False;3;FLOAT2;-1,-1;False;4;FLOAT2;1,1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RotatorNode;34;-4379.035,645.6139;Float;True;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RotatorNode;35;-4377.399,884.5768;Float;True;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;78;-3007.2,1533.282;Float;False;Property;_Pan_Up;Pan_Up;13;0;Create;True;0;0;False;0;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LengthOpNode;20;-3383.747,-295.063;Float;True;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;37;-4390.492,1400.146;Float;True;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RotatorNode;36;-4386.672,1122.902;Float;True;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;30;-4382.91,225.0116;Float;True;Property;_Texture0;Texture 0;6;0;Create;True;0;0;False;0;5ca8eaff460c0fc46ac11ec7ca5653ea;5ca8eaff460c0fc46ac11ec7ca5653ea;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;76;-2852.2,1254.282;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NegateNode;77;-2755.2,1538.282;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-2987.355,-56.32663;Float;False;Constant;_Float1;Float 1;5;0;Create;True;0;0;False;0;2.33;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;19;-3001.955,-286.3285;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;33;-3659.325,746.0717;Float;True;Property;_TextureSample3;Texture Sample 3;5;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;29;-3668.163,466.4211;Float;True;Property;_TextureSample0;Texture Sample 0;5;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;31;-3661.739,1325.202;Float;True;Property;_TextureSample1;Texture Sample 1;5;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;32;-3661.738,1030.811;Float;True;Property;_TextureSample2;Texture Sample 2;5;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BlendOpsNode;14;-3016.57,470.825;Float;False;Difference;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;13;-3013.914,221.1914;Float;False;Overlay;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;75;-2484.2,1471.282;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;74;-2509.2,1288.282;Float;False;Constant;_Float3;Float 3;13;0;Create;True;0;0;False;0;5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-2759.462,-274.2075;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;16;-2498.076,-204.0621;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;73;-2231.2,1389.282;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;12;-2650.086,332.7297;Float;False;Exclusion;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;15;-1961.882,429.9384;Float;True;Multiply;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;72;-1984.2,1389.874;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;11;-2344.684,85.75168;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;53;-1560.39,434.2904;Float;True;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BlendOpsNode;51;-1679.279,1237.339;Float;False;LinearLight;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;9;-2039.282,-29.77029;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-2033.972,283.5998;Float;False;Constant;_Float0;Float 0;5;0;Create;True;0;0;False;0;5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;61;-1287.039,588.5185;Float;False;Constant;_Float2;Float 2;10;0;Create;True;0;0;False;0;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;67;-1757.64,202.4184;Float;False;Property;_Alpha;Alpha;11;0;Create;True;0;0;False;0;0.6;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosterizeNode;8;-1736.536,-3.21352;Float;False;1;2;1;COLOR;0,0,0,0;False;0;INT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;64;-868.4393,849.8184;Float;False;Property;_Depth_Blend;Depth_Blend;10;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;7;-1789.649,-538.3323;Float;False;Property;_Color_1;Color_1;4;0;Create;True;0;0;False;0;0.909804,0.909804,0.909804,1;0.909804,0.909804,0.909804,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;6;-1795.628,-328.8812;Float;False;Property;_Color_2;Color_2;3;0;Create;True;0;0;False;0;0.2980392,0.2980392,0.2980392,1;0.2980392,0.2980392,0.2980392,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;-1110.237,433.8186;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;66;-1580.839,197.2184;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;4;-1273.77,1245.084;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;71;-1675.741,957.7182;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LerpOp;5;-1302.706,-256.3927;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;-652.6392,673.0184;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;63;-657.8393,853.7183;Float;False;True;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;-657.8383,431.2186;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;70;-1393.64,882.3185;Float;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;69;-1394.939,732.8185;Float;False;Property;_Wibblyocity;Wibblyocity;12;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2;-283.4,233;Float;True;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1;-534,-106;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-265.6,829.6999;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;-1193.44,747.1183;Float;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;BlinnPhong;Blockout/Gate_Particle;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Transparent;;Geometry;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;-1;False;-1;-1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;1;-1;0;-1;0;False;0;0;False;-1;2;0;False;-1;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;49;0;48;0
WireConnection;47;0;46;0
WireConnection;38;0;43;0
WireConnection;38;1;50;0
WireConnection;41;0;49;0
WireConnection;41;1;50;0
WireConnection;40;0;47;0
WireConnection;40;1;50;0
WireConnection;39;0;45;0
WireConnection;39;1;50;0
WireConnection;55;0;23;0
WireConnection;34;0;23;0
WireConnection;34;2;38;0
WireConnection;35;0;23;0
WireConnection;35;2;39;0
WireConnection;20;0;55;0
WireConnection;37;0;23;0
WireConnection;37;2;41;0
WireConnection;36;0;23;0
WireConnection;36;2;40;0
WireConnection;77;0;78;0
WireConnection;19;0;20;0
WireConnection;33;0;30;0
WireConnection;33;1;35;0
WireConnection;29;0;30;0
WireConnection;29;1;34;0
WireConnection;31;0;30;0
WireConnection;31;1;37;0
WireConnection;32;0;30;0
WireConnection;32;1;36;0
WireConnection;14;0;32;1
WireConnection;14;1;31;2
WireConnection;13;0;29;1
WireConnection;13;1;33;2
WireConnection;75;0;76;2
WireConnection;75;1;77;0
WireConnection;17;0;19;0
WireConnection;17;1;18;0
WireConnection;16;0;17;0
WireConnection;73;0;74;0
WireConnection;73;1;75;0
WireConnection;12;0;13;0
WireConnection;12;1;14;0
WireConnection;15;0;16;0
WireConnection;15;1;12;0
WireConnection;72;0;73;0
WireConnection;11;0;12;0
WireConnection;53;0;15;0
WireConnection;53;1;15;0
WireConnection;51;0;12;0
WireConnection;51;1;72;0
WireConnection;9;0;11;0
WireConnection;8;1;9;0
WireConnection;8;0;10;0
WireConnection;60;0;53;0
WireConnection;60;1;61;0
WireConnection;66;0;67;0
WireConnection;66;1;15;0
WireConnection;4;0;51;0
WireConnection;5;0;7;1
WireConnection;5;1;6;1
WireConnection;5;2;8;0
WireConnection;65;0;66;0
WireConnection;65;1;4;0
WireConnection;63;0;64;0
WireConnection;62;0;60;0
WireConnection;62;1;4;0
WireConnection;70;0;12;0
WireConnection;70;1;71;0
WireConnection;2;0;62;0
WireConnection;2;1;63;0
WireConnection;1;0;5;0
WireConnection;1;1;4;0
WireConnection;3;0;65;0
WireConnection;3;1;63;0
WireConnection;68;0;69;0
WireConnection;68;1;70;0
WireConnection;0;2;1;0
WireConnection;0;8;2;0
WireConnection;0;9;3;0
WireConnection;0;11;68;0
ASEEND*/
//CHKSM=AD435977A28F227B41F6F37292FDDC6F31FB8E58