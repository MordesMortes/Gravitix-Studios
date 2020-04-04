// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Blockout/Blockout_Shader_Comment"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_Drop_Value("Drop_Value", Float) = 0.5
		_Gloss("Gloss", Range( 0 , 1)) = 0.5
		_Metallic("Metallic", Range( 0 , 1)) = 0.5
		_Texture("Texture", 2D) = "white" {}
		_Depth_Blend("Depth_Blend", Float) = 0.1
		_TilingFactor("TilingFactor", Float) = 2
		_Float3("Float 3", Range( 0 , 1)) = 0
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "ForceNoShadowCasting" = "True" }
		Cull Off
		CGPROGRAM
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf Standard alpha:fade keepalpha noshadow exclude_path:deferred nolightmap  nodynlightmap nodirlightmap 
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			float4 screenPos;
		};

		uniform float4 _Color;
		uniform sampler2D _Texture;
		uniform float _TilingFactor;
		uniform float _Drop_Value;
		uniform float _Metallic;
		uniform float _Gloss;
		uniform sampler2D _CameraDepthTexture;
		uniform float _Depth_Blend;
		uniform float _Float3;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV45 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode45 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV45, 5.0 ) );
			float lerpResult42 = lerp( 1.0 , 0.5 , fresnelNode45);
			float dotResult31 = dot( ase_worldNormal , float3(1,0,0) );
			float temp_output_37_0 = pow( dotResult31 , 3.0 );
			float2 appendResult22 = (float2(ase_worldPos.z , ase_worldPos.y));
			float4 tex2DNode18 = tex2D( _Texture, ( appendResult22 * _TilingFactor ) );
			float dotResult35 = dot( ase_worldNormal , float3(0,1,0) );
			float temp_output_39_0 = pow( dotResult35 , 3.0 );
			float2 appendResult23 = (float2(ase_worldPos.x , ase_worldPos.z));
			float4 tex2DNode19 = tex2D( _Texture, ( appendResult23 * _TilingFactor ) );
			float dotResult36 = dot( ase_worldNormal , float3(0,0,1) );
			float temp_output_40_0 = pow( dotResult36 , 3.0 );
			float2 appendResult24 = (float2(ase_worldPos.x , ase_worldPos.y));
			float4 tex2DNode20 = tex2D( _Texture, ( appendResult24 * _TilingFactor ) );
			float4 temp_output_6_0 = ( _Color * ( ( temp_output_37_0 * tex2DNode18 ) + ( temp_output_39_0 * tex2DNode19 ) + ( temp_output_40_0 * tex2DNode20 ) ) );
			float4 clampResult3 = clamp( ( temp_output_6_0 + ( ( ( temp_output_37_0 * tex2DNode18.a ) + ( temp_output_39_0 * tex2DNode19.a ) + ( temp_output_40_0 * tex2DNode20.a ) ) * _Drop_Value ) ) , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
			o.Albedo = ( lerpResult42 * clampResult3 ).rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Gloss;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth47 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth47 = abs( ( screenDepth47 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Depth_Blend ) );
			o.Alpha = ( ( _Color.a * distanceDepth47 ) * _Float3 );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15401
252;41;2107;991;4767.867;156.241;1;True;True
Node;AmplifyShaderEditor.WorldPosInputsNode;25;-4898.908,847.1019;Float;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;23;-4341.311,900.8933;Float;True;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;24;-4333.224,1180.004;Float;True;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-4622.087,1350.08;Float;False;Property;_TilingFactor;TilingFactor;6;0;Create;True;0;0;False;0;2;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;22;-4346.56,658.1758;Float;True;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector3Node;32;-4155.727,-171.7807;Float;False;Constant;_Vector0;Vector 0;3;0;Create;True;0;0;False;0;1,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;34;-4153.727,207.2194;Float;False;Constant;_Vector2;Vector 2;3;0;Create;True;0;0;False;0;0,0,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;33;-4153.727,19.21925;Float;False;Constant;_Vector1;Vector 1;3;0;Create;True;0;0;False;0;0,1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;30;-4153.841,-384.68;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-4135.087,652.0801;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-4109.087,933.0801;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;38;-3866.127,399.5195;Float;False;Constant;_Opacity;Opacity;3;0;Create;True;0;0;False;0;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;-4134.087,1204.08;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DotProductOpNode;31;-3865.262,-292.3184;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;35;-3863.726,-76.78072;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;21;-4893.211,289.0915;Float;True;Property;_Texture;Texture;4;0;Create;True;0;0;False;0;134986b68f9a3374fb9b7ca764d1d3c8;134986b68f9a3374fb9b7ca764d1d3c8;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.DotProductOpNode;36;-3865.726,155.2195;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;18;-3979.964,578.5131;Float;True;Property;_TextureSample0;Texture Sample 0;3;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;20;-3974.647,1250.271;Float;True;Property;_TextureSample2;Texture Sample 2;3;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;40;-3523.726,256.2194;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;39;-3525.726,74.21954;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;37;-3528.726,-75.78072;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;19;-3974.647,917.0505;Float;True;Property;_TextureSample1;Texture Sample 1;3;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-2976.32,569.8295;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-2995.469,903.6613;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-2995.469,1013.553;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-2970.203,338.972;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-2995.468,788.4526;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-2974.79,455.9331;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;8;-2246.537,-205.5001;Float;False;Property;_Color;Color;0;0;Create;True;0;0;False;0;1,1,1,1;1,1,1,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;14;-2685.033,817.3044;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-2173.125,483.4359;Float;False;Property;_Drop_Value;Drop_Value;1;0;Create;True;0;0;False;0;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;29;-2724.813,467.3995;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;48;-1283.429,593.3068;Float;False;Property;_Depth_Blend;Depth_Blend;5;0;Create;True;0;0;False;0;0.1;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-1795.609,461.3442;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-1762.386,-94.37982;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FresnelNode;45;-1030.037,-235.0933;Float;False;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;47;-1006.824,598.217;Float;False;True;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;43;-944.5126,-513.9661;Float;False;Constant;_Float1;Float 1;3;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;44;-944.5128,-404.7564;Float;False;Constant;_Float2;Float 2;3;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;5;-1236.423,17.80121;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;3;-1025.236,18.50935;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;46;-715.4888,557.2991;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;42;-719.593,-508.7654;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;57;-499.0954,901.6631;Float;True;Property;_Float3;Float 3;7;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-218.1921,809.51;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;-373.1256,0.1890643;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-382.9355,244.2666;Float;False;Property;_Gloss;Gloss;2;0;Create;True;0;0;False;0;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;53;-1538.276,718.2568;Float;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;11;-381.7238,157.2113;Float;False;Property;_Metallic;Metallic;3;0;Create;True;0;0;False;0;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Blockout/Blockout_Shader_Comment;False;False;False;False;False;False;True;True;True;False;False;False;False;False;True;True;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;-1;False;-1;-1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;23;0;25;1
WireConnection;23;1;25;3
WireConnection;24;0;25;1
WireConnection;24;1;25;2
WireConnection;22;0;25;3
WireConnection;22;1;25;2
WireConnection;49;0;22;0
WireConnection;49;1;52;0
WireConnection;50;0;23;0
WireConnection;50;1;52;0
WireConnection;51;0;24;0
WireConnection;51;1;52;0
WireConnection;31;0;30;0
WireConnection;31;1;32;0
WireConnection;35;0;30;0
WireConnection;35;1;33;0
WireConnection;36;0;30;0
WireConnection;36;1;34;0
WireConnection;18;0;21;0
WireConnection;18;1;49;0
WireConnection;20;0;21;0
WireConnection;20;1;51;0
WireConnection;40;0;36;0
WireConnection;40;1;38;0
WireConnection;39;0;35;0
WireConnection;39;1;38;0
WireConnection;37;0;31;0
WireConnection;37;1;38;0
WireConnection;19;0;21;0
WireConnection;19;1;50;0
WireConnection;26;0;40;0
WireConnection;26;1;20;4
WireConnection;16;0;39;0
WireConnection;16;1;19;0
WireConnection;17;0;40;0
WireConnection;17;1;20;0
WireConnection;28;0;37;0
WireConnection;28;1;18;4
WireConnection;15;0;37;0
WireConnection;15;1;18;0
WireConnection;27;0;39;0
WireConnection;27;1;19;4
WireConnection;14;0;15;0
WireConnection;14;1;16;0
WireConnection;14;2;17;0
WireConnection;29;0;28;0
WireConnection;29;1;27;0
WireConnection;29;2;26;0
WireConnection;7;0;29;0
WireConnection;7;1;10;0
WireConnection;6;0;8;0
WireConnection;6;1;14;0
WireConnection;47;0;48;0
WireConnection;5;0;6;0
WireConnection;5;1;7;0
WireConnection;3;0;5;0
WireConnection;46;0;8;4
WireConnection;46;1;47;0
WireConnection;42;0;43;0
WireConnection;42;1;44;0
WireConnection;42;2;45;0
WireConnection;56;0;46;0
WireConnection;56;1;57;0
WireConnection;41;0;42;0
WireConnection;41;1;3;0
WireConnection;53;0;6;0
WireConnection;0;0;41;0
WireConnection;0;3;11;0
WireConnection;0;4;13;0
WireConnection;0;9;56;0
ASEEND*/
//CHKSM=F63EC011F1CABBE578F04910A1BBDDD5DE7E9ABC