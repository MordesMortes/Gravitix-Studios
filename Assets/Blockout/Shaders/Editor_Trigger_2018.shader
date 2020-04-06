// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Blockout/Editor_Trigger"
{
	Properties
	{
		_SpecColor("Specular Color",Color)=(1,1,1,1)
		_Depth_Blend("Depth_Blend", Float) = 1
		_Emissive_Brightness("Emissive_Brightness", Float) = 1
		_Color_1("Color_1", Color) = (0.345098,0.3686275,0.627451,1)
		_Extra_Lines("Extra_Lines", Float) = 0.5
		_Texture0("Texture 0", 2D) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		Blend One One
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			float4 screenPos;
		};

		uniform float _Emissive_Brightness;
		uniform float4 _Color_1;
		uniform sampler2D _Texture0;
		uniform float _Extra_Lines;
		uniform sampler2D _CameraDepthTexture;
		uniform float _Depth_Blend;

		void surf( Input i , inout SurfaceOutput o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV12 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode12 = ( 0.0 + 0.9 * pow( 1.0 - fresnelNdotV12, 2.0 ) );
			float lerpResult9 = lerp( 0.1 , 0.4 , fresnelNode12);
			float dotResult22 = dot( ase_worldNormal , float3(1,0,0) );
			float4 appendResult36 = (float4(ase_worldPos.y , ase_worldPos.z , 0.0 , 0.0));
			float4 tex2DNode34 = tex2D( _Texture0, appendResult36.xy );
			float temp_output_39_0 = ( _Time.y * 0.8 );
			float2 panner50 = ( temp_output_39_0 * float2( -0.1,0 ) + ( appendResult36 * 0.5 ).xy);
			float2 panner51 = ( temp_output_39_0 * float2( 0.1,0 ) + appendResult36.xy);
			float blendOpSrc59 = tex2D( _Texture0, panner50 ).g;
			float blendOpDest59 = tex2D( _Texture0, panner51 ).g;
			float2 panner52 = ( temp_output_39_0 * float2( 0,-0.1 ) + ( appendResult36 * 0.25 ).xy);
			float2 panner53 = ( temp_output_39_0 * float2( 0,0.1 ) + ( appendResult36 * 0.75 ).xy);
			float blendOpSrc60 = tex2D( _Texture0, panner52 ).g;
			float blendOpDest60 = tex2D( _Texture0, panner53 ).g;
			float blendOpSrc61 = ( saturate(  round( 0.5 * ( blendOpSrc59 + blendOpDest59 ) ) ));
			float blendOpDest61 = ( saturate(  round( 0.5 * ( blendOpSrc60 + blendOpDest60 ) ) ));
			float clampResult30 = clamp( ( ( tex2DNode34.r + ( tex2DNode34.b * _Extra_Lines ) ) + ( saturate( ( 1.0 - ( 1.0 - blendOpSrc61 ) * ( 1.0 - blendOpDest61 ) ) )) ) , 0.0 , 1.0 );
			float dotResult23 = dot( ase_worldNormal , float3(0,1,0) );
			float4 appendResult92 = (float4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
			float4 tex2DNode88 = tex2D( _Texture0, ( float4( float2( 0.5,0 ), 0.0 , 0.0 ) + appendResult92 ).xy );
			float2 panner75 = ( temp_output_39_0 * float2( -0.1,0 ) + ( appendResult92 * 0.5 ).xy);
			float2 panner69 = ( temp_output_39_0 * float2( 0.1,0 ) + appendResult92.xy);
			float blendOpSrc81 = tex2D( _Texture0, panner75 ).g;
			float blendOpDest81 = tex2D( _Texture0, panner69 ).g;
			float2 panner70 = ( temp_output_39_0 * float2( 0,-0.1 ) + ( appendResult92 * 0.25 ).xy);
			float2 panner71 = ( temp_output_39_0 * float2( 0,0.1 ) + ( appendResult92 * 0.75 ).xy);
			float blendOpSrc80 = tex2D( _Texture0, panner70 ).g;
			float blendOpDest80 = tex2D( _Texture0, panner71 ).g;
			float blendOpSrc82 = ( saturate(  round( 0.5 * ( blendOpSrc81 + blendOpDest81 ) ) ));
			float blendOpDest82 = ( saturate(  round( 0.5 * ( blendOpSrc80 + blendOpDest80 ) ) ));
			float clampResult63 = clamp( ( ( tex2DNode88.r + ( tex2DNode88.b * _Extra_Lines ) ) + ( 0.1 * ( saturate( ( 1.0 - ( 1.0 - blendOpSrc82 ) * ( 1.0 - blendOpDest82 ) ) )) ) ) , 0.0 , 1.0 );
			float dotResult24 = dot( ase_worldNormal , float3(0,0,1) );
			float4 appendResult117 = (float4(ase_worldPos.x , ase_worldPos.y , 0.0 , 0.0));
			float4 tex2DNode121 = tex2D( _Texture0, ( 0.5 + appendResult117 ).xy );
			float2 panner118 = ( temp_output_39_0 * float2( -0.1,0 ) + ( appendResult117 * 0.5 ).xy);
			float2 panner119 = ( temp_output_39_0 * float2( 0.1,0 ) + appendResult117.xy);
			float blendOpSrc106 = tex2D( _Texture0, panner118 ).g;
			float blendOpDest106 = tex2D( _Texture0, panner119 ).g;
			float2 panner96 = ( temp_output_39_0 * float2( 0,-0.1 ) + ( appendResult117 * 0.25 ).xy);
			float2 panner97 = ( temp_output_39_0 * float2( 0,0.1 ) + ( appendResult117 * 0.75 ).xy);
			float blendOpSrc105 = tex2D( _Texture0, panner96 ).g;
			float blendOpDest105 = tex2D( _Texture0, panner97 ).g;
			float blendOpSrc107 = ( saturate(  round( 0.5 * ( blendOpSrc106 + blendOpDest106 ) ) ));
			float blendOpDest107 = ( saturate(  round( 0.5 * ( blendOpSrc105 + blendOpDest105 ) ) ));
			float clampResult65 = clamp( ( ( tex2DNode121.r + ( tex2DNode121.b * _Extra_Lines ) ) + ( 0.1 * ( saturate( ( 1.0 - ( 1.0 - blendOpSrc107 ) * ( 1.0 - blendOpDest107 ) ) )) ) ) , 0.0 , 1.0 );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth2 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth2 = abs( ( screenDepth2 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Depth_Blend ) );
			o.Emission = ( ( _Emissive_Brightness * ( _Color_1 * ( lerpResult9 + ( ( pow( dotResult22 , 5.0 ) * clampResult30 ) + ( pow( dotResult23 , 5.0 ) * clampResult63 ) + ( pow( dotResult24 , 5.0 ) * clampResult65 ) ) ) ) ) * distanceDepth2 ).rgb;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf BlinnPhong keepalpha fullforwardshadows 

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
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float3 worldPos : TEXCOORD1;
				float4 screenPos : TEXCOORD2;
				float3 worldNormal : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
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
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				surfIN.screenPos = IN.screenPos;
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
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
252;41;2107;991;1612.201;190.0889;1;True;True
Node;AmplifyShaderEditor.SimpleTimeNode;40;-7517.765,2266.702;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-7492.261,2386.719;Float;False;Constant;_Float4;Float 4;5;0;Create;True;0;0;False;0;0.8;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;37;-7504.02,2078.921;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;98;-5972.343,5851.032;Float;False;Constant;_Float16;Float 16;6;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;99;-5972.344,6042.824;Float;False;Constant;_Float17;Float 17;6;0;Create;True;0;0;False;0;0.25;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;100;-5975.419,6281.6;Float;False;Constant;_Float18;Float 18;6;0;Create;True;0;0;False;0;0.75;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;92;-5937.771,3225.271;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-7293.582,2335.321;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;73;-5879.882,4076.899;Float;False;Constant;_Float9;Float 9;6;0;Create;True;0;0;False;0;0.25;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;72;-5879.881,3885.107;Float;False;Constant;_Float8;Float 8;6;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;74;-5882.957,4315.674;Float;False;Constant;_Float10;Float 10;6;0;Create;True;0;0;False;0;0.75;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;117;-6030.233,5191.196;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;94;-5470.686,5955.601;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RelayNode;84;-6125.135,3407.314;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;-5378.223,3989.675;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;45;-5816.698,1512.217;Float;False;Constant;_Float5;Float 5;6;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;46;-5816.699,1704.01;Float;False;Constant;_Float6;Float 6;6;0;Create;True;0;0;False;0;0.25;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-5819.774,1942.785;Float;False;Constant;_Float7;Float 7;6;0;Create;True;0;0;False;0;0.75;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RelayNode;109;-6217.597,5373.239;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;36;-6208.511,461.8702;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;93;-5478.822,5746.581;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;-5468.686,6208.601;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;-5376.223,4242.675;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;66;-5386.36,3780.655;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;120;-5433.866,5112.065;Float;False;Constant;_Float20;Float 20;11;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;71;-5076.73,4562.403;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RelayNode;38;-6061.953,1034.425;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-5313.041,1869.785;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;-5315.041,1616.785;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;-5323.177,1407.766;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.PannerNode;97;-5169.192,6528.328;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;96;-5167.591,6221.129;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,-0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;69;-5081.529,3941.603;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0.1,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;75;-5090.224,3668.675;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.1,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;35;-7620.161,2748.707;Float;True;Property;_Texture0;Texture 0;5;0;Create;True;0;0;False;0;cfd3ef0affa5a984d948919178dc441a;cfd3ef0affa5a984d948919178dc441a;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.Vector2Node;91;-5471.84,3119.786;Float;False;Constant;_Vector3;Vector 3;10;0;Create;True;0;0;False;0;0.5,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;118;-5182.687,5634.601;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.1,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;119;-5173.991,5907.528;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0.1,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;70;-5075.129,4255.203;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,-0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;51;-5018.347,1568.713;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0.1,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;53;-5013.548,2189.513;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;50;-5027.042,1295.785;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.1,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;52;-5011.947,1882.313;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,-0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;122;-4795.042,5605.103;Float;True;Property;_TextureSample17;Texture Sample 17;6;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;123;-4786.697,5877.021;Float;True;Property;_TextureSample18;Texture Sample 18;6;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;125;-4792.904,6496.632;Float;True;Property;_TextureSample15;Texture Sample 15;6;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;124;-4790.629,6173.331;Float;True;Property;_TextureSample16;Texture Sample 16;6;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;76;-4692.231,4561.006;Float;True;Property;_TextureSample3;Texture Sample 3;6;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;79;-4703.561,3911.335;Float;True;Property;_TextureSample8;Texture Sample 8;6;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;77;-4693.174,4232.996;Float;True;Property;_TextureSample6;Texture Sample 6;6;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;115;-5215.833,5162.724;Float;False;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;43;-7527.826,2596.708;Float;False;Property;_Extra_Lines;Extra_Lines;4;0;Create;True;0;0;False;0;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;78;-4700.838,3609.223;Float;True;Property;_TextureSample7;Texture Sample 7;6;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;90;-5123.371,3196.798;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;34;-5122.454,157.7675;Float;True;Property;_TextureSample0;Texture Sample 0;4;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;54;-4637.655,1236.333;Float;True;Property;_TextureSample1;Texture Sample 1;6;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RelayNode;83;-6132.808,3638.243;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;106;-4324.88,5738.772;Float;True;HardMix;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;80;-4239.286,4289.615;Float;True;HardMix;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RelayNode;108;-6225.271,5604.168;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;105;-4331.749,6255.541;Float;True;HardMix;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;81;-4232.418,3772.847;Float;True;HardMix;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;88;-4679.477,3061.283;Float;True;Property;_TextureSample9;Texture Sample 9;10;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;121;-4791.113,5230.627;Float;True;Property;_TextureSample20;Texture Sample 20;6;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RelayNode;44;-6069.626,1265.353;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;58;-4629.049,2188.116;Float;True;Property;_TextureSample5;Texture Sample 5;6;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;55;-4640.378,1538.445;Float;True;Property;_TextureSample2;Texture Sample 2;6;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;57;-4629.992,1860.106;Float;True;Property;_TextureSample4;Texture Sample 4;6;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;113;-4195.094,5449.815;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;86;-3707.381,3744.519;Float;False;Constant;_Float11;Float 11;10;0;Create;True;0;0;False;0;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;107;-3885.374,5951.659;Float;True;Screen;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-4745.456,406.7674;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;111;-3799.845,5710.444;Float;False;Constant;_Float19;Float 19;10;0;Create;True;0;0;False;0;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;60;-4176.104,1916.725;Float;True;HardMix;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;89;-4102.632,3483.89;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;59;-4169.235,1399.958;Float;True;HardMix;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;82;-3792.91,3985.734;Float;True;Screen;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;61;-3729.727,1612.845;Float;True;Screen;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;85;-3446.584,3790.298;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;32;-3325.542,355.581;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;26;-2977.508,-68.45514;Float;False;Constant;_Vector0;Vector 0;4;0;Create;True;0;0;False;0;0,0,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;29;-3307.508,-559.4551;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;87;-3670.671,3450.663;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;27;-2974.508,-236.4551;Float;False;Constant;_Vector1;Vector 1;4;0;Create;True;0;0;False;0;0,1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;110;-3539.048,5756.224;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;28;-2974.508,-419.4551;Float;False;Constant;_Vector2;Vector 2;4;0;Create;True;0;0;False;0;1,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;112;-3763.135,5416.589;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;24;-2695.508,-66.45514;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;23;-2700.508,-245.4551;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-2699.508,168.5449;Float;False;Constant;_Float3;Float 3;4;0;Create;True;0;0;False;0;5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;22;-2669.508,-390.4551;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;62;-3172.235,3475.22;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;64;-3213.809,5899.508;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;31;-3070.542,455.581;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;19;-2390.295,198.9507;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-1979.3,107.4;Float;False;Constant;_Float2;Float 2;4;0;Create;True;0;0;False;0;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;63;-2955.235,3477.22;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;30;-2853.542,457.581;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;65;-2996.809,5901.508;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;20;-2386.075,30.18257;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;18;-2381.855,-151.2432;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;12;-1785.3,21.7;Float;True;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0.9;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-1669,-289;Float;False;Constant;_Float0;Float 0;4;0;Create;True;0;0;False;0;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-2061.198,762.2147;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-2071.745,439.4454;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-2063.307,595.5561;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-1672,-132.7;Float;False;Constant;_Float1;Float 1;4;0;Create;True;0;0;False;0;0.4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;9;-1468,-229;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;15;-1812.264,570.2407;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;8;-1116,-335;Float;False;Property;_Color_1;Color_1;3;0;Create;True;0;0;False;0;0.345098,0.3686275,0.627451,1;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;7;-1055,-6;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-840,221;Float;False;Property;_Depth_Blend;Depth_Blend;1;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-792,-31;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-789,-171;Float;False;Property;_Emissive_Brightness;Emissive_Brightness;2;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;2;-623,225;Float;False;True;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-565,-40;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1;-313.7002,70;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;BlinnPhong;Blockout/Editor_Trigger;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;4;1;False;-1;1;False;-1;0;0;False;-1;0;False;-1;-1;False;-1;-1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;0;0;False;-1;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;92;0;37;1
WireConnection;92;1;37;3
WireConnection;39;0;40;0
WireConnection;39;1;41;0
WireConnection;117;0;37;1
WireConnection;117;1;37;2
WireConnection;94;0;117;0
WireConnection;94;1;99;0
WireConnection;84;0;39;0
WireConnection;67;0;92;0
WireConnection;67;1;73;0
WireConnection;109;0;39;0
WireConnection;36;0;37;2
WireConnection;36;1;37;3
WireConnection;93;0;117;0
WireConnection;93;1;98;0
WireConnection;95;0;117;0
WireConnection;95;1;100;0
WireConnection;68;0;92;0
WireConnection;68;1;74;0
WireConnection;66;0;92;0
WireConnection;66;1;72;0
WireConnection;71;0;68;0
WireConnection;71;1;84;0
WireConnection;38;0;39;0
WireConnection;49;0;36;0
WireConnection;49;1;47;0
WireConnection;48;0;36;0
WireConnection;48;1;46;0
WireConnection;42;0;36;0
WireConnection;42;1;45;0
WireConnection;97;0;95;0
WireConnection;97;1;109;0
WireConnection;96;0;94;0
WireConnection;96;1;109;0
WireConnection;69;0;92;0
WireConnection;69;1;84;0
WireConnection;75;0;66;0
WireConnection;75;1;84;0
WireConnection;118;0;93;0
WireConnection;118;1;109;0
WireConnection;119;0;117;0
WireConnection;119;1;109;0
WireConnection;70;0;67;0
WireConnection;70;1;84;0
WireConnection;51;0;36;0
WireConnection;51;1;38;0
WireConnection;53;0;49;0
WireConnection;53;1;38;0
WireConnection;50;0;42;0
WireConnection;50;1;38;0
WireConnection;52;0;48;0
WireConnection;52;1;38;0
WireConnection;122;0;35;0
WireConnection;122;1;118;0
WireConnection;123;0;35;0
WireConnection;123;1;119;0
WireConnection;125;0;35;0
WireConnection;125;1;97;0
WireConnection;124;0;35;0
WireConnection;124;1;96;0
WireConnection;76;0;35;0
WireConnection;76;1;71;0
WireConnection;79;0;35;0
WireConnection;79;1;69;0
WireConnection;77;0;35;0
WireConnection;77;1;70;0
WireConnection;115;0;120;0
WireConnection;115;1;117;0
WireConnection;78;0;35;0
WireConnection;78;1;75;0
WireConnection;90;0;91;0
WireConnection;90;1;92;0
WireConnection;34;0;35;0
WireConnection;34;1;36;0
WireConnection;54;0;35;0
WireConnection;54;1;50;0
WireConnection;83;0;43;0
WireConnection;106;0;122;2
WireConnection;106;1;123;2
WireConnection;80;0;77;2
WireConnection;80;1;76;2
WireConnection;108;0;43;0
WireConnection;105;0;124;2
WireConnection;105;1;125;2
WireConnection;81;0;78;2
WireConnection;81;1;79;2
WireConnection;88;0;35;0
WireConnection;88;1;90;0
WireConnection;121;0;35;0
WireConnection;121;1;115;0
WireConnection;44;0;43;0
WireConnection;58;0;35;0
WireConnection;58;1;53;0
WireConnection;55;0;35;0
WireConnection;55;1;51;0
WireConnection;57;0;35;0
WireConnection;57;1;52;0
WireConnection;113;0;121;3
WireConnection;113;1;108;0
WireConnection;107;0;106;0
WireConnection;107;1;105;0
WireConnection;33;0;34;3
WireConnection;33;1;44;0
WireConnection;60;0;57;2
WireConnection;60;1;58;2
WireConnection;89;0;88;3
WireConnection;89;1;83;0
WireConnection;59;0;54;2
WireConnection;59;1;55;2
WireConnection;82;0;81;0
WireConnection;82;1;80;0
WireConnection;61;0;59;0
WireConnection;61;1;60;0
WireConnection;85;0;86;0
WireConnection;85;1;82;0
WireConnection;32;0;34;1
WireConnection;32;1;33;0
WireConnection;87;0;88;1
WireConnection;87;1;89;0
WireConnection;110;0;111;0
WireConnection;110;1;107;0
WireConnection;112;0;121;1
WireConnection;112;1;113;0
WireConnection;24;0;29;0
WireConnection;24;1;26;0
WireConnection;23;0;29;0
WireConnection;23;1;27;0
WireConnection;22;0;29;0
WireConnection;22;1;28;0
WireConnection;62;0;87;0
WireConnection;62;1;85;0
WireConnection;64;0;112;0
WireConnection;64;1;110;0
WireConnection;31;0;32;0
WireConnection;31;1;61;0
WireConnection;19;0;24;0
WireConnection;19;1;25;0
WireConnection;63;0;62;0
WireConnection;30;0;31;0
WireConnection;65;0;64;0
WireConnection;20;0;23;0
WireConnection;20;1;25;0
WireConnection;18;0;22;0
WireConnection;18;1;25;0
WireConnection;12;3;13;0
WireConnection;17;0;19;0
WireConnection;17;1;65;0
WireConnection;14;0;18;0
WireConnection;14;1;30;0
WireConnection;16;0;20;0
WireConnection;16;1;63;0
WireConnection;9;0;10;0
WireConnection;9;1;11;0
WireConnection;9;2;12;0
WireConnection;15;0;14;0
WireConnection;15;1;16;0
WireConnection;15;2;17;0
WireConnection;7;0;9;0
WireConnection;7;1;15;0
WireConnection;5;0;8;0
WireConnection;5;1;7;0
WireConnection;2;0;3;0
WireConnection;4;0;6;0
WireConnection;4;1;5;0
WireConnection;1;0;4;0
WireConnection;1;1;2;0
WireConnection;0;2;1;0
ASEEND*/
//CHKSM=A18C4CB98C2355C54176D8E5D6E3D8F8963A0AC1