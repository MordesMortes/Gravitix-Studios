// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Blockout/Blockout_Water"
{
	Properties
	{
		_SpecColor("Specular Color",Color)=(1,1,1,1)
		_Gloss("Gloss", Range( 0 , 1)) = 0.5
		_Spec("Spec", Range( 0 , 1)) = 0.5
		_Depth_Fade("Depth_Fade", Float) = 1
		_Water_Colour_Power("Water_Colour_Power", Float) = 2
		_Speed("Speed", Float) = 1
		_Tex_World_Scale("Tex_World_Scale", Float) = 0.5
		_Alpha("Alpha", Float) = 0.5
		_Tex("Tex", 2D) = "white" {}
		_Color_2("Color_2", Color) = (0.9333333,0.9333333,0.9333333,0.003921569)
		_Depth_Color("Depth_Color", Color) = (0.627451,0.654902,0.6784314,0.003921569)
		_Color_1("Color_1", Color) = (0.7215686,0.5411765,0.3764706,0.003921569)
		_Fresnel_Colour_Bias("Fresnel_Colour_Bias", Float) = 0
		_Fresnel_Colour_Scale("Fresnel_Colour_Scale", Float) = 1
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		CGPROGRAM
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf BlinnPhong alpha:fade keepalpha noshadow exclude_path:deferred 
		struct Input
		{
			float3 worldPos;
			float3 viewDir;
			float3 worldNormal;
			float4 screenPos;
		};

		uniform float4 _Depth_Color;
		uniform float4 _Color_2;
		uniform float _Fresnel_Colour_Bias;
		uniform float _Fresnel_Colour_Scale;
		uniform float _Water_Colour_Power;
		uniform float4 _Color_1;
		uniform sampler2D _CameraDepthTexture;
		uniform float _Depth_Fade;
		uniform sampler2D _Tex;
		uniform float _Speed;
		uniform float _Tex_World_Scale;
		uniform float _Spec;
		uniform float _Gloss;
		uniform float _Alpha;

		void surf( Input i , inout SurfaceOutput o )
		{
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV14 = dot( ase_worldNormal, i.viewDir );
			float fresnelNode14 = ( _Fresnel_Colour_Bias + _Fresnel_Colour_Scale * pow( 1.0 - fresnelNdotV14, _Water_Colour_Power ) );
			float4 lerpResult8 = lerp( _Depth_Color , _Color_2 , fresnelNode14);
			float4 lerpResult9 = lerp( _Color_1 , _Color_2 , fresnelNode14);
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth6 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth6 = abs( ( screenDepth6 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Depth_Fade ) );
			float4 lerpResult5 = lerp( lerpResult8 , lerpResult9 , distanceDepth6);
			float screenDepth22 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth22 = abs( ( screenDepth22 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( 1.0 ) );
			float temp_output_83_0 = ( _Speed * _Time.y );
			float3 ase_worldPos = i.worldPos;
			float2 appendResult44 = (float2(ase_worldPos.x , ase_worldPos.z));
			float2 temp_output_45_0 = ( appendResult44 * _Tex_World_Scale );
			float2 panner36 = ( temp_output_83_0 * float2( 0,0.55 ) + temp_output_45_0);
			float2 panner35 = ( temp_output_83_0 * float2( 0.4,0 ) + ( temp_output_45_0 * 1.2 ));
			float blendOpSrc26 = tex2D( _Tex, panner36 ).r;
			float blendOpDest26 = tex2D( _Tex, panner35 ).r;
			float2 panner34 = ( temp_output_83_0 * float2( 0,-0.45 ) + ( temp_output_45_0 * 1.4 ));
			float2 panner33 = ( temp_output_83_0 * float2( 0.35,0 ) + ( temp_output_45_0 * 0.8 ));
			float blendOpSrc27 = tex2D( _Tex, panner34 ).r;
			float blendOpDest27 = tex2D( _Tex, panner33 ).r;
			float clampResult23 = clamp( ( ( saturate( min( blendOpSrc26 , blendOpDest26 ) )) + ( saturate( min( blendOpSrc27 , blendOpDest27 ) )) ) , 0.4980392 , 1.0 );
			o.Albedo = ( lerpResult5 + ( ( 1.0 - distanceDepth22 ) * step( ( distanceDepth22 * clampResult23 ) , 0.5 ) ) ).rgb;
			o.Specular = _Spec;
			o.Gloss = _Gloss;
			float screenDepth51 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth51 = abs( ( screenDepth51 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( 1.0 ) );
			float clampResult55 = clamp( pow( ( distanceDepth51 * 3.0 ) , 3.0 ) , 0.0 , 1.0 );
			o.Alpha = ( clampResult55 * _Alpha );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15401
252;41;2107;991;7163.438;754.1594;4.140761;True;True
Node;AmplifyShaderEditor.WorldPosInputsNode;43;-5264.831,62.89426;Float;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;47;-5001.706,292.6325;Float;False;Property;_Tex_World_Scale;Tex_World_Scale;6;0;Create;True;0;0;False;0;0.5;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;44;-4867.909,258.8461;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;48;-4686.112,334.8026;Float;False;Constant;_Float2;Float 2;10;0;Create;True;0;0;False;0;1.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;50;-4688.052,854.4339;Float;False;Constant;_Float4;Float 4;10;0;Create;True;0;0;False;0;0.8;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-4677.952,84.50328;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;41;-4689.455,1228.814;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;42;-4689.25,1092.88;Float;False;Property;_Speed;Speed;5;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;49;-4688.832,573.6889;Float;False;Constant;_Float3;Float 3;10;0;Create;True;0;0;False;0;1.4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-4337.78,836.7537;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-4334.058,322.4195;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;83;-4471.013,1127.734;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-4329.978,555.0343;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;32;-4070.876,1283.441;Float;True;Property;_Tex;Tex;8;0;Create;True;0;0;False;0;6630628f1b8a442d69da16b374c1eb0e;6630628f1b8a442d69da16b374c1eb0e;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.PannerNode;36;-3891.199,45.05392;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0.55;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;35;-3890.32,317.1183;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0.4,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;34;-3891.68,582.3806;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,-0.45;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;33;-3897.123,870.7699;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0.35,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;31;-3478.862,883.0572;Float;True;Property;_TextureSample3;Texture Sample 3;7;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;28;-3481.459,12.05655;Float;True;Property;_TextureSample0;Texture Sample 0;7;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;30;-3477.563,578.8563;Float;True;Property;_TextureSample2;Texture Sample 2;7;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;29;-3482.763,282.4566;Float;True;Property;_TextureSample1;Texture Sample 1;7;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BlendOpsNode;27;-2960.155,578.8561;Float;False;Darken;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;26;-2958.856,236.9565;Float;False;Darken;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-2644.249,-115.3431;Float;False;Constant;_Blend;Blend;7;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;24;-2653.347,347.4567;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;84;-3355.526,-809.1999;Float;False;Property;_Fresnel_Colour_Scale;Fresnel_Colour_Scale;14;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-3355.622,-731.1651;Float;False;Property;_Water_Colour_Power;Water_Colour_Power;4;0;Create;True;0;0;False;0;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;22;-2336.144,-114.0426;Float;False;True;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;85;-3355.527,-881.9999;Float;False;Property;_Fresnel_Colour_Bias;Fresnel_Colour_Bias;13;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;53;-1819.052,790.6007;Float;False;Constant;_Float5;Float 5;10;0;Create;True;0;0;False;0;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;51;-1808.834,638.0206;Float;False;True;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;86;-3354.225,-636.2999;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ClampOpNode;23;-2274.416,415.2956;Float;False;3;0;FLOAT;0;False;1;FLOAT;0.4980392;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-1904.541,101.7586;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;66;-3503.404,-1328.424;Float;False;Property;_Color_1;Color_1;12;0;Create;True;0;0;False;0;0.7215686,0.5411765,0.3764706,0.003921569;0.345098,0.9311413,1,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;-1500.571,724.5879;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-3496.087,-1763.593;Float;False;Property;_Depth_Fade;Depth_Fade;3;0;Create;True;0;0;False;0;1;3.17;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-1903.241,273.3587;Float;False;Constant;_Float1;Float 1;7;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;67;-3503.593,-1075.439;Float;False;Property;_Color_2;Color_2;10;0;Create;True;0;0;False;0;0.9333333,0.9333333,0.9333333,0.003921569;0.9333334,0.9333334,0.9333334,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;68;-3500.117,-1610.731;Float;False;Property;_Depth_Color;Depth_Color;11;0;Create;True;0;0;False;0;0.627451,0.654902,0.6784314,0.003921569;0.627451,0.654902,0.6784314,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FresnelNode;14;-2992.12,-858.3987;Float;False;Standard;TangentNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;54;-1224.814,721.1542;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;18;-1613.341,48.45856;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;6;-2692.5,-1753.648;Float;False;True;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;9;-2571.736,-1207.746;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;69;-1815.506,-175.0633;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;8;-2561.319,-1576.25;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;57;-977.8145,879.1542;Float;False;Property;_Alpha;Alpha;7;0;Create;True;0;0;False;0;0.5;0.72;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-1239.175,-110.843;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;5;-2244.343,-1561.052;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;55;-968.8145,721.1542;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-716.8145,721.1542;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1;-733.7202,46.15399;Float;False;Property;_Spec;Spec;2;0;Create;True;0;0;False;0;0.5;0.433;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;60;-1250.975,1165.012;Float;False;Constant;_Float6;Float 6;13;0;Create;True;0;0;False;0;0.003;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2;-726.7202,171.154;Float;False;Property;_Gloss;Gloss;1;0;Create;True;0;0;False;0;0.5;0.72;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;62;-956.2753,1513.012;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;1,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;-705.7603,1358.874;Float;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;59;-955.9753,1211.012;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;61;-1245.975,1287.012;Float;False;Property;_Refraction;Refraction;9;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;65;-1173.375,1624.712;Float;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;3;-761.3881,-258.994;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector2Node;63;-1394.975,1500.012;Float;False;Constant;_Vector0;Vector 0;14;0;Create;True;0;0;False;0;1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;BlinnPhong;Blockout/Blockout_Water;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;5;False;-1;10;False;-1;-1;False;-1;-1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;0;0;False;-1;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;44;0;43;1
WireConnection;44;1;43;3
WireConnection;45;0;44;0
WireConnection;45;1;47;0
WireConnection;39;0;45;0
WireConnection;39;1;50;0
WireConnection;37;0;45;0
WireConnection;37;1;48;0
WireConnection;83;0;42;0
WireConnection;83;1;41;0
WireConnection;38;0;45;0
WireConnection;38;1;49;0
WireConnection;36;0;45;0
WireConnection;36;1;83;0
WireConnection;35;0;37;0
WireConnection;35;1;83;0
WireConnection;34;0;38;0
WireConnection;34;1;83;0
WireConnection;33;0;39;0
WireConnection;33;1;83;0
WireConnection;31;0;32;0
WireConnection;31;1;33;0
WireConnection;28;0;32;0
WireConnection;28;1;36;0
WireConnection;30;0;32;0
WireConnection;30;1;34;0
WireConnection;29;0;32;0
WireConnection;29;1;35;0
WireConnection;27;0;30;1
WireConnection;27;1;31;1
WireConnection;26;0;28;1
WireConnection;26;1;29;1
WireConnection;24;0;26;0
WireConnection;24;1;27;0
WireConnection;22;0;25;0
WireConnection;51;0;25;0
WireConnection;23;0;24;0
WireConnection;17;0;22;0
WireConnection;17;1;23;0
WireConnection;52;0;51;0
WireConnection;52;1;53;0
WireConnection;14;4;86;0
WireConnection;14;1;85;0
WireConnection;14;2;84;0
WireConnection;14;3;16;0
WireConnection;54;0;52;0
WireConnection;54;1;53;0
WireConnection;18;0;17;0
WireConnection;18;1;20;0
WireConnection;6;0;7;0
WireConnection;9;0;66;0
WireConnection;9;1;67;0
WireConnection;9;2;14;0
WireConnection;69;0;22;0
WireConnection;8;0;68;0
WireConnection;8;1;67;0
WireConnection;8;2;14;0
WireConnection;4;0;69;0
WireConnection;4;1;18;0
WireConnection;5;0;8;0
WireConnection;5;1;9;0
WireConnection;5;2;6;0
WireConnection;55;0;54;0
WireConnection;56;0;55;0
WireConnection;56;1;57;0
WireConnection;62;0;63;0
WireConnection;62;1;65;0
WireConnection;62;2;23;0
WireConnection;58;0;59;0
WireConnection;58;1;62;0
WireConnection;59;0;60;0
WireConnection;59;1;61;0
WireConnection;65;0;63;0
WireConnection;3;0;5;0
WireConnection;3;1;4;0
WireConnection;0;0;3;0
WireConnection;0;3;1;0
WireConnection;0;4;2;0
WireConnection;0;9;56;0
ASEEND*/
//CHKSM=6813A8E925AFC678EB5F97C828DBABAAD3F4B3E0