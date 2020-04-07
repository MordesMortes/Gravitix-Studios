// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Blockout/Blockout_Shader_Base"
{
	Properties
	{
		_Color("Color", Color) = (0.4470588,0.4470588,0.4470588,1)
		_Locked_Drop("Locked_Drop", Float) = 0.7
		_Texture("Texture", 2D) = "black" {}
		_Drop_Value("Drop_Value", Float) = 0.5
		_Gloss("Gloss", Range( 0 , 1)) = 0.227
		_Metallic("Metallic", Range( 0 , 1)) = 0.087
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows exclude_path:deferred 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform float4 _Color;
		uniform sampler2D _Texture;
		uniform float4 _Texture_ST;
		uniform float _Drop_Value;
		uniform float _Locked_Drop;
		uniform float _Metallic;
		uniform float _Gloss;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Texture = i.uv_texcoord * _Texture_ST.xy + _Texture_ST.zw;
			float4 tex2DNode9 = tex2D( _Texture, uv_Texture );
			float4 clampResult3 = clamp( ( ( _Color * tex2DNode9 ) + ( tex2DNode9.a * _Drop_Value ) ) , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
			float4 lerpResult1 = lerp( ( clampResult3 * _Locked_Drop ) , clampResult3 , i.vertexColor.r);
			o.Albedo = lerpResult1.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Gloss;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15401
252;41;2107;991;3275.525;874.3894;2.050645;True;True
Node;AmplifyShaderEditor.RangedFloatNode;10;-2106.095,319.1316;Float;False;Property;_Drop_Value;Drop_Value;3;0;Create;True;0;0;False;0;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;8;-2162.036,-284.8002;Float;False;Property;_Color;Color;0;0;Create;True;0;0;False;0;0.4470588,0.4470588,0.4470588,1;0.827451,0.3058824,0.1411765,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;9;-2248.095,46.13159;Float;True;Property;_Texture;Texture;2;0;Create;True;0;0;False;0;403ab31ff043eff4ea4124a30eb5cdcc;1129170a07ee4ad47983f67d5d938fcf;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-1763.546,102.2357;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-1762.386,-94.37982;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;5;-1236.423,17.80121;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-833.1365,-214.9735;Float;False;Property;_Locked_Drop;Locked_Drop;1;0;Create;True;0;0;False;0;0.7;0.7;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;3;-1025.236,18.50935;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2;-627.1365,-286.9735;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;12;-649.3438,127.7501;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;11;-381.7238,157.2113;Float;False;Property;_Metallic;Metallic;5;0;Create;True;0;0;False;0;0.087;0.087;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;1;-379.1365,-3.973541;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-380.3438,344.7501;Float;False;Property;_Gloss;Gloss;4;0;Create;True;0;0;False;0;0.227;0.227;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Blockout/Blockout_Shader_Base;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;-1;False;-1;-1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;7;0;9;4
WireConnection;7;1;10;0
WireConnection;6;0;8;0
WireConnection;6;1;9;0
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
//CHKSM=4D4CE48540B17734CC4280553FD0F261098DADCF