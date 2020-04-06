// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Blockout/Blockout_Shader_Tri"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_Locked_Drop("Locked_Drop", Float) = 0.7
		_Drop_Value("Drop_Value", Float) = 0.5
		_Texture("Texture", 2D) = "white" {}
		_Gloss("Gloss", Range( 0 , 1)) = 0.227
		_Metallic("Metallic", Range( 0 , 1)) = 0.087
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float3 worldNormal;
			float3 worldPos;
			float4 vertexColor : COLOR;
		};

		uniform float4 _Color;
		uniform sampler2D _Texture;
		uniform float _Drop_Value;
		uniform float _Locked_Drop;
		uniform float _Metallic;
		uniform float _Gloss;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldNormal = i.worldNormal;
			float dotResult31 = dot( ase_worldNormal , float3(1,0,0) );
			float temp_output_37_0 = pow( dotResult31 , 3.0 );
			float3 ase_worldPos = i.worldPos;
			float2 appendResult22 = (float2(ase_worldPos.y , ase_worldPos.z));
			float4 tex2DNode18 = tex2D( _Texture, appendResult22 );
			float dotResult35 = dot( ase_worldNormal , float3(0,1,0) );
			float temp_output_39_0 = pow( dotResult35 , 3.0 );
			float2 appendResult23 = (float2(ase_worldPos.x , ase_worldPos.z));
			float4 tex2DNode19 = tex2D( _Texture, appendResult23 );
			float dotResult36 = dot( ase_worldNormal , float3(0,0,1) );
			float temp_output_40_0 = pow( dotResult36 , 3.0 );
			float2 appendResult24 = (float2(ase_worldPos.x , ase_worldPos.y));
			float4 tex2DNode20 = tex2D( _Texture, appendResult24 );
			float4 clampResult3 = clamp( ( ( _Color * ( ( temp_output_37_0 * tex2DNode18 ) + ( temp_output_39_0 * tex2DNode19 ) + ( temp_output_40_0 * tex2DNode20 ) ) ) + ( ( ( temp_output_37_0 * tex2DNode18.a ) + ( temp_output_39_0 * tex2DNode19.a ) + ( temp_output_40_0 * tex2DNode20.a ) ) * _Drop_Value ) ) , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
			float4 lerpResult1 = lerp( ( clampResult3 * _Locked_Drop ) , clampResult3 , i.vertexColor.r);
			o.Albedo = lerpResult1.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Gloss;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

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
				float3 worldNormal : TEXCOORD2;
				half4 color : COLOR0;
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
				o.color = v.color;
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
				surfIN.vertexColor = IN.color;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
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
252;41;2107;991;6424.864;1736.198;4.425892;True;True
Node;AmplifyShaderEditor.Vector3Node;34;-3892.377,-855.7488;Float;False;Constant;_Vector2;Vector 2;3;0;Create;True;0;0;False;0;0,0,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;30;-3853.531,-1085.551;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;32;-3894.377,-1234.749;Float;False;Constant;_Vector0;Vector 0;3;0;Create;True;0;0;False;0;1,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;25;-4551.577,204.5671;Float;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;33;-3892.377,-1043.749;Float;False;Constant;_Vector1;Vector 1;3;0;Create;True;0;0;False;0;0,1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;22;-3999.229,15.64079;Float;True;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;24;-3995.293,522.0683;Float;True;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;23;-3993.98,258.3586;Float;True;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DotProductOpNode;35;-3602.377,-1139.749;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;31;-3603.913,-1355.286;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;36;-3604.377,-907.7488;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;38;-3604.778,-663.4488;Float;False;Constant;_Float0;Float 0;3;0;Create;True;0;0;False;0;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;21;-4544.881,-353.4438;Float;True;Property;_Texture;Texture;3;0;Create;True;0;0;False;0;403ab31ff043eff4ea4124a30eb5cdcc;1129170a07ee4ad47983f67d5d938fcf;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.PowerNode;40;-3262.377,-806.7487;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;39;-3264.377,-988.749;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;37;-3267.377,-1138.749;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;20;-3627.318,607.7366;Float;True;Property;_TextureSample2;Texture Sample 2;3;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;19;-3627.318,274.5158;Float;True;Property;_TextureSample1;Texture Sample 1;3;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;18;-3632.635,-64.02202;Float;True;Property;_TextureSample0;Texture Sample 0;3;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-2804.43,-518.4937;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-2810.547,-287.6381;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-2809.017,-401.5331;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-2822.372,12.64342;Float;True;2;2;0;FLOAT;1;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-2805.473,601.745;Float;True;2;2;0;FLOAT;1;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-2821.073,251.3526;Float;True;2;2;0;FLOAT;1;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;29;-2559.04,-390.067;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-2141.062,124.3274;Float;False;Property;_Drop_Value;Drop_Value;2;0;Create;True;0;0;False;0;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;14;-2498.937,197.4955;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;8;-2162.036,-284.8002;Float;False;Property;_Color;Color;0;0;Create;True;0;0;False;0;1,1,1,1;0.5566038,0.5566038,0.5566038,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-1762.386,-94.37982;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-1763.546,102.2357;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;5;-1236.423,17.80121;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-1011.314,-247.4608;Float;False;Property;_Locked_Drop;Locked_Drop;1;0;Create;True;0;0;False;0;0.7;0.7;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;3;-1025.236,15.90935;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2;-670.0364,-276.5735;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;12;-683.7457,132.1424;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;13;-382.8079,279.7501;Float;False;Property;_Gloss;Gloss;4;0;Create;True;0;0;False;0;0.227;0.227;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;1;-379.1365,-3.973541;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-381.7238,157.2113;Float;False;Property;_Metallic;Metallic;5;0;Create;True;0;0;False;0;0.087;0.087;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;181.6155,-3.584516;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Blockout/Blockout_Shader_Tri;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;-1;False;-1;-1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;22;0;25;2
WireConnection;22;1;25;3
WireConnection;24;0;25;1
WireConnection;24;1;25;2
WireConnection;23;0;25;1
WireConnection;23;1;25;3
WireConnection;35;0;30;0
WireConnection;35;1;33;0
WireConnection;31;0;30;0
WireConnection;31;1;32;0
WireConnection;36;0;30;0
WireConnection;36;1;34;0
WireConnection;40;0;36;0
WireConnection;40;1;38;0
WireConnection;39;0;35;0
WireConnection;39;1;38;0
WireConnection;37;0;31;0
WireConnection;37;1;38;0
WireConnection;20;0;21;0
WireConnection;20;1;24;0
WireConnection;19;0;21;0
WireConnection;19;1;23;0
WireConnection;18;0;21;0
WireConnection;18;1;22;0
WireConnection;28;0;37;0
WireConnection;28;1;18;4
WireConnection;26;0;40;0
WireConnection;26;1;20;4
WireConnection;27;0;39;0
WireConnection;27;1;19;4
WireConnection;15;0;37;0
WireConnection;15;1;18;0
WireConnection;17;0;40;0
WireConnection;17;1;20;0
WireConnection;16;0;39;0
WireConnection;16;1;19;0
WireConnection;29;0;28;0
WireConnection;29;1;27;0
WireConnection;29;2;26;0
WireConnection;14;0;15;0
WireConnection;14;1;16;0
WireConnection;14;2;17;0
WireConnection;6;0;8;0
WireConnection;6;1;14;0
WireConnection;7;0;29;0
WireConnection;7;1;10;0
WireConnection;5;0;6;0
WireConnection;5;1;7;0
WireConnection;3;0;5;0
WireConnection;2;0;3;0
WireConnection;2;1;4;0
WireConnection;1;0;2;0
WireConnection;1;1;3;0
WireConnection;1;2;12;1
WireConnection;0;0;1;0
WireConnection;0;3;11;0
WireConnection;0;4;13;0
ASEEND*/
//CHKSM=A603FCBEEA6BCB46D77052A245BF31FEF5B53B6D