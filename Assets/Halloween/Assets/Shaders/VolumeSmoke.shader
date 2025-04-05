// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Halloween/VolumeSmoke"
{
	Properties
	{
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[ASEBegin]_MainText("MainText", 2D) = "white" {}
		_Offset("Offset", Range( 0 , 1)) = 1
		_Contrast("Contrast", Range( 0 , 1)) = 0
		_Brightness("Brightness", Float) = 1
		_LightInfluence("LightInfluence", Float) = 1
		[ASEEnd]_Depth("Depth", Float) = 0

		//_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		//_TessValue( "Tess Max Tessellation", Range( 1, 32 ) ) = 16
		//_TessMin( "Tess Min Distance", Float ) = 10
		//_TessMax( "Tess Max Distance", Float ) = 25
		//_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		//_TessMaxDisp( "Tess Max Displacement", Float ) = 25
	}

	SubShader
	{
		LOD 0

		
		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }
		
		Cull Back
		AlphaToMask Off
		HLSLINCLUDE
		#pragma target 2.0

		#pragma prefer_hlslcc gles
		#pragma exclude_renderers d3d11_9x 

		#ifndef ASE_TESS_FUNCS
		#define ASE_TESS_FUNCS
		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}
		
		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlane (float3 pos, float4 plane)
		{
			float d = dot (float4(pos,1.0f), plane);
			return d;
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlane(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlane(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlane(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlane(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		#endif //ASE_TESS_FUNCS
		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForward" }
			
			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZWrite Off
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM
			
			#pragma multi_compile_instancing
			#define ASE_SRP_VERSION 70301
			#define REQUIRE_DEPTH_TEXTURE 1

			
			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#if ASE_SRP_VERSION <= 70108
			#define REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
			#endif

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_COLOR


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_tangent : TANGENT;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				#ifdef ASE_FOG
				float fogFactor : TEXCOORD2;
				#endif
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord6 : TEXCOORD6;
				float4 ase_color : COLOR;
				float4 ase_texcoord7 : TEXCOORD7;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _MainText_ST;
			float _Contrast;
			float _Offset;
			float _Brightness;
			float _LightInfluence;
			float _Depth;
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _MainText;
			uniform float4 _CameraDepthTexture_TexelSize;


						
			VertexOutput VertexFunction ( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 ase_worldTangent = TransformObjectToWorldDir(v.ase_tangent.xyz);
				o.ase_texcoord4.xyz = ase_worldTangent;
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord5.xyz = ase_worldNormal;
				float ase_vertexTangentSign = v.ase_tangent.w * unity_WorldTransformParams.w;
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				o.ase_texcoord6.xyz = ase_worldBitangent;
				
				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord7 = screenPos;
				
				o.ase_texcoord3.xy = v.ase_texcoord.xy;
				o.ase_color = v.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord3.zw = 0;
				o.ase_texcoord4.w = 0;
				o.ase_texcoord5.w = 0;
				o.ase_texcoord6.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				VertexPositionInputs vertexInput = (VertexPositionInputs)0;
				vertexInput.positionWS = positionWS;
				vertexInput.positionCS = positionCS;
				o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				#ifdef ASE_FOG
				o.fogFactor = ComputeFogFactor( positionCS.z );
				#endif
				o.clipPos = positionCS;
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_tangent : TANGENT;
				float4 ase_color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_tangent = v.ase_tangent;
				o.ase_color = v.ase_color;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag ( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif
				float2 uv_MainText = IN.ase_texcoord3.xy * _MainText_ST.xy + _MainText_ST.zw;
				float2 UVs9 = uv_MainText;
				float MainText12 = tex2D( _MainText, UVs9 ).a;
				float3 ase_worldTangent = IN.ase_texcoord4.xyz;
				float3 ase_worldNormal = IN.ase_texcoord5.xyz;
				float3 ase_worldBitangent = IN.ase_texcoord6.xyz;
				float3x3 ase_worldToTangent = float3x3(ase_worldTangent,ase_worldBitangent,ase_worldNormal);
				float3 LightDir17 = mul( _MainLightPosition.xyz, ase_worldToTangent );
				float Offset37 = _Offset;
				float temp_output_106_0 = ( Offset37 * 0.5 );
				float lerpResult35 = lerp( MainText12 , ( 1.0 - ( ( MainText12 - tex2D( _MainText, ( float3( UVs9 ,  0.0 ) + ( LightDir17 * temp_output_106_0 ) ).xy ).a ) + 0.5 ) ) , 0.5);
				float Sample_0131 = saturate( lerpResult35 );
				float temp_output_55_0 = ( temp_output_106_0 * 0.5 );
				float lerpResult52 = lerp( MainText12 , ( 1.0 - ( ( MainText12 - tex2D( _MainText, ( float3( UVs9 ,  0.0 ) + ( LightDir17 * temp_output_55_0 ) ).xy ).a ) + 0.5 ) ) , 0.5);
				float Sample_0254 = saturate( lerpResult52 );
				float lerpResult112 = lerp( Sample_0131 , Sample_0254 , 0.5);
				float temp_output_61_0 = ( temp_output_55_0 * 0.5 );
				float lerpResult71 = lerp( MainText12 , ( 1.0 - ( ( MainText12 - tex2D( _MainText, ( float3( UVs9 ,  0.0 ) + ( LightDir17 * temp_output_61_0 ) ).xy ).a ) + 0.5 ) ) , 0.5);
				float Sample_0373 = saturate( lerpResult71 );
				float lerpResult113 = lerp( lerpResult112 , Sample_0373 , 0.5);
				float temp_output_77_0 = ( temp_output_61_0 * 0.5 );
				float lerpResult87 = lerp( MainText12 , ( 1.0 - ( ( MainText12 - tex2D( _MainText, ( float3( UVs9 ,  0.0 ) + ( LightDir17 * temp_output_77_0 ) ).xy ).a ) + 0.5 ) ) , 0.5);
				float Sample_0489 = saturate( lerpResult87 );
				float lerpResult114 = lerp( lerpResult113 , Sample_0489 , 0.5);
				float lerpResult103 = lerp( MainText12 , ( 1.0 - ( ( MainText12 - tex2D( _MainText, ( float3( UVs9 ,  0.0 ) + ( LightDir17 * ( temp_output_77_0 * 0.5 ) ) ).xy ).a ) + 0.5 ) ) , 0.5);
				float Sample_05105 = saturate( lerpResult103 );
				float lerpResult115 = lerp( lerpResult114 , Sample_05105 , 0.5);
				float AllSamples57 = lerpResult115;
				float4 Combined124 = saturate( ( float4( ( ( ( 1.0 - ( _Contrast + AllSamples57 ) ) * _Brightness ) + ( _MainLightColor.rgb * ( _MainLightColor.a * _LightInfluence ) ) ) , 0.0 ) * IN.ase_color ) );
				
				float Alpfa129 = ( IN.ase_color.a * MainText12 );
				float4 screenPos = IN.ase_texcoord7;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth147 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth147 = abs( ( screenDepth147 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _Depth ) );
				
				float3 BakedAlbedo = 0;
				float3 BakedEmission = 0;
				float3 Color = Combined124.rgb;
				float Alpha = ( Alpfa129 * saturate( distanceDepth147 ) );
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;

				#ifdef _ALPHATEST_ON
					clip( Alpha - AlphaClipThreshold );
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				#ifdef ASE_FOG
					Color = MixFog( Color, IN.fogFactor );
				#endif

				return half4( Color, Alpha );
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }

			ZWrite On
			ZTest LEqual
			AlphaToMask Off

			HLSLPROGRAM
			
			#pragma multi_compile_instancing
			#define ASE_SRP_VERSION 70301
			#define REQUIRE_DEPTH_TEXTURE 1

			
			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _MainText_ST;
			float _Contrast;
			float _Offset;
			float _Brightness;
			float _LightInfluence;
			float _Depth;
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _MainText;
			uniform float4 _CameraDepthTexture_TexelSize;


			
			float3 _LightDirection;

			VertexOutput VertexFunction( VertexInput v )
			{
				VertexOutput o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord3 = screenPos;
				
				o.ase_color = v.ase_color;
				o.ase_texcoord2.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				float3 normalWS = TransformObjectToWorldDir( v.ase_normal );

				float4 clipPos = TransformWorldToHClip( ApplyShadowBias( positionWS, normalWS, _LightDirection ) );

				#if UNITY_REVERSED_Z
					clipPos.z = min(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
				#else
					clipPos.z = max(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				o.clipPos = clipPos;

				return o;
			}
			
			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_color = v.ase_color;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 uv_MainText = IN.ase_texcoord2.xy * _MainText_ST.xy + _MainText_ST.zw;
				float2 UVs9 = uv_MainText;
				float MainText12 = tex2D( _MainText, UVs9 ).a;
				float Alpfa129 = ( IN.ase_color.a * MainText12 );
				float4 screenPos = IN.ase_texcoord3;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth147 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth147 = abs( ( screenDepth147 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _Depth ) );
				
				float Alpha = ( Alpfa129 * saturate( distanceDepth147 ) );
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;

				#ifdef _ALPHATEST_ON
					#ifdef _ALPHATEST_SHADOW_ON
						clip(Alpha - AlphaClipThresholdShadow);
					#else
						clip(Alpha - AlphaClipThreshold);
					#endif
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				return 0;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0
			AlphaToMask Off

			HLSLPROGRAM
			
			#pragma multi_compile_instancing
			#define ASE_SRP_VERSION 70301
			#define REQUIRE_DEPTH_TEXTURE 1

			
			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _MainText_ST;
			float _Contrast;
			float _Offset;
			float _Brightness;
			float _LightInfluence;
			float _Depth;
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _MainText;
			uniform float4 _CameraDepthTexture_TexelSize;


			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord3 = screenPos;
				
				o.ase_color = v.ase_color;
				o.ase_texcoord2.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				o.clipPos = TransformWorldToHClip( positionWS );
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = o.clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_color = v.ase_color;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 uv_MainText = IN.ase_texcoord2.xy * _MainText_ST.xy + _MainText_ST.zw;
				float2 UVs9 = uv_MainText;
				float MainText12 = tex2D( _MainText, UVs9 ).a;
				float Alpfa129 = ( IN.ase_color.a * MainText12 );
				float4 screenPos = IN.ase_texcoord3;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth147 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth147 = abs( ( screenDepth147 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _Depth ) );
				
				float Alpha = ( Alpfa129 * saturate( distanceDepth147 ) );
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				return 0;
			}
			ENDHLSL
		}

	
	}
	CustomEditor "UnityEditor.ShaderGraph.PBRMasterGUI"
	Fallback "Hidden/InternalErrorShader"
	
}
/*ASEBEGIN
Version=18912
-1796;105;1280;503;821.7183;576.2078;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;13;-2918.814,-1079.642;Inherit;False;1511.596;377.5697;Comment;5;9;8;7;10;12;MainTexture;1,1,1,1;0;0
Node;AmplifyShaderEditor.TexturePropertyNode;7;-2868.814,-1029.643;Inherit;True;Property;_MainText;MainText;0;0;Create;True;0;0;0;False;0;False;None;bf159fe3079feda479008d55d9581ebf;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TextureCoordinatesNode;8;-2577.344,-866.0727;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;9;-2303.951,-865.0124;Inherit;False;UVs;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;10;-2015.341,-1031.249;Inherit;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;12;-1627.331,-1007.323;Inherit;False;MainText;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;130;-1505.665,-405.813;Inherit;False;764.981;400.0229;Comment;4;126;127;128;129;Alpfa;1,1,1,1;0;0
Node;AmplifyShaderEditor.VertexColorNode;126;-1455.665,-355.813;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;128;-1448.684,-121.79;Inherit;False;12;MainText;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;150;-60.71832,-250.2078;Inherit;False;Property;_Depth;Depth;5;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;127;-1180.684,-208.79;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;129;-964.6846,-213.79;Inherit;False;Alpfa;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;147;163.713,-294.1085;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;18;-2756.462,-547.7881;Inherit;False;876.271;434;Comment;4;14;16;15;17;LightDirection;1,1,1,1;0;0
Node;AmplifyShaderEditor.SaturateNode;149;426.2845,-170.131;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;74;-3712.55,1921.743;Inherit;False;2825.503;455.1933;Comment;14;89;88;87;86;85;84;83;82;81;80;79;78;77;76;Smaple_04;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;40;-3714.145,840.3522;Inherit;False;2825.503;455.1933;Comment;14;42;55;44;43;45;54;46;47;48;49;50;51;52;53;Smaple_02;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;36;-3707.218,331.8673;Inherit;False;2812.104;441.0284;Comment;15;31;34;35;33;30;26;25;27;24;22;20;23;19;106;38;Smaple_01;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;58;-3711.318,1394.888;Inherit;False;2825.503;455.1933;Comment;14;73;72;71;70;69;68;67;66;65;64;63;62;61;60;Smaple_03;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;90;-3715.576,2497.631;Inherit;False;2825.503;455.1933;Comment;14;105;104;103;102;101;100;99;98;97;96;95;94;93;92;Smaple_05;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;125;215.3774,-391.1216;Inherit;False;129;Alpfa;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;138;-498.5635,1579.564;Inherit;False;2122.644;630.1572;Comment;15;133;134;136;135;137;131;123;124;132;117;119;118;120;121;122;FinalTweaks;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;116;-338.418,501.7299;Inherit;False;1697.679;701.0997;Comment;10;108;107;109;112;113;110;111;114;115;57;BlendAllSamples;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;39;-2641.834,32.70823;Inherit;False;645.0605;166.1752;Comment;2;21;37;Offset;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-2565.089,84.46619;Inherit;False;Property;_Offset;Offset;1;0;Create;True;0;0;0;False;0;False;1;0.517;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;131;886.7915,1858.376;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;69;-2001.188,1581.486;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;93;-3435.724,2832.106;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;17;-2101.788,-367.4758;Inherit;False;LightDir;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;-3434.294,1174.827;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;62;-3201.434,1589.042;Inherit;False;9;UVs;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;42;-3456.933,1064.039;Inherit;False;17;LightDir;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;-3432.698,2256.217;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;84;-2219.899,2108.038;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-2339.161,-363.7582;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3x3;0,0,0,1,1,1,1,0,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;-3182.409,1150.003;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-3194.419,640.0568;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldToTangentMatrix;16;-2694.445,-215.1748;Inherit;False;0;1;FLOAT3x3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;106;-3438.78,658.7379;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;22;-2905.41,615.4185;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TexturePropertyNode;24;-2996.391,385.4432;Inherit;True;Property;_MainText;MainText;1;0;Fetch;True;0;0;0;False;0;False;None;bf159fe3079feda479008d55d9581ebf;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.GetLocalVarNode;19;-3570.856,542.9167;Inherit;False;17;LightDir;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;38;-3689.833,654.3193;Inherit;False;37;Offset;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;23;-3216.271,524.5601;Inherit;False;9;UVs;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;-3431.467,1729.363;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;43;-3204.261,1034.506;Inherit;False;9;UVs;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;32;223.3233,-613.4239;Inherit;False;124;Combined;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;14;-2696.061,-476.1582;Inherit;True;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;80;-2891.804,2206.755;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;25;-2649.935,514.8603;Inherit;True;Property;_TextureSample1;Texture Sample 1;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;67;-2635.098,1579.343;Inherit;True;Property;_TextureSample3;Texture Sample 3;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;48;-2637.925,1024.807;Inherit;True;Property;_TextureSample2;Texture Sample 2;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;49;-2221.494,1026.648;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;46;-2984.381,895.3903;Inherit;True;Property;_MainText;MainText;1;0;Fetch;True;0;0;0;False;0;False;None;bf159fe3079feda479008d55d9581ebf;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.GetLocalVarNode;94;-3205.692,2691.785;Inherit;False;9;UVs;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;65;-2981.554,1449.926;Inherit;True;Property;_MainText;MainText;1;0;Fetch;True;0;0;0;False;0;False;None;bf159fe3079feda479008d55d9581ebf;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;-3183.84,2807.282;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;27;-2551.683,407.8559;Inherit;False;12;MainText;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;78;-3202.666,2115.896;Inherit;False;9;UVs;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;92;-3458.364,2721.318;Inherit;False;17;LightDir;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;98;-2541.104,2575.082;Inherit;False;12;MainText;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;81;-2982.786,1976.781;Inherit;True;Property;_MainText;MainText;1;0;Fetch;True;0;0;0;False;0;False;None;bf159fe3079feda479008d55d9581ebf;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;-3180.813,2231.394;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;26;-2233.504,516.702;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;37;-2220.774,82.70823;Inherit;False;Offset;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;57;1135.261,1042.313;Inherit;False;AllSamples;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;96;-2894.83,2782.644;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;66;-2536.846,1472.339;Inherit;False;12;MainText;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;76;-3455.338,2145.429;Inherit;False;17;LightDir;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;60;-3454.106,1618.575;Inherit;False;17;LightDir;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TexturePropertyNode;97;-2985.812,2552.669;Inherit;True;Property;_MainText;MainText;1;0;Fetch;True;0;0;0;False;0;False;None;bf159fe3079feda479008d55d9581ebf;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;146;477.713,-344.1085;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;82;-2538.078,1999.193;Inherit;False;12;MainText;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;47;-2539.673,917.8029;Inherit;False;12;MainText;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;83;-2636.33,2106.197;Inherit;True;Property;_TextureSample4;Texture Sample 4;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;64;-2890.573,1679.901;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;102;-1791.9,2682.017;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;68;-2218.667,1581.184;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;119;-388.0023,1772.56;Inherit;False;57;AllSamples;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;53;-1339.452,917.3009;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;109;-23.29703,790.5334;Inherit;False;73;Sample_03;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;71;-1573.779,1473.142;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;31;-1113.94,401.666;Inherit;True;Sample_01;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;72;-1336.625,1471.837;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;87;-1575.01,1999.996;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;54;-1101.929,911.613;Inherit;True;Sample_02;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;63;-3179.582,1704.539;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;86;-1788.873,2106.128;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;107;-284.799,551.7299;Inherit;False;31;Sample_01;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;101;-2005.445,2684.229;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;88;-1337.856,1998.692;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;52;-1576.606,918.606;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;89;-1100.334,1993.004;Inherit;True;Sample_04;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;104;-1340.883,2574.58;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;112;-14.27441,634.3952;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;108;-288.418,652.8561;Inherit;False;54;Sample_02;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;85;-2002.419,2108.34;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;113;267.8881,770.7495;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;45;-2893.4,1125.365;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;140;224.8325,-491.1124;Inherit;False;Constant;_Smoothness;Smoothness;10;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;73;-1099.102,1466.149;Inherit;True;Sample_03;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;30;-2016.027,517.0037;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;103;-1578.037,2575.885;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;70;-1787.642,1579.274;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;50;-2004.014,1026.95;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;122;110.9234,1833.089;Inherit;False;Property;_Brightness;Brightness;3;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;34;-1351.463,407.3539;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;33;-1802.48,514.792;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;35;-1588.617,408.6591;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;114;526.4867,906.4647;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;111;506.3118,1061.782;Inherit;False;105;Sample_05;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;51;-1790.469,1024.738;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;117;-448.5635,1629.564;Inherit;False;Property;_Contrast;Contrast;2;0;Create;True;0;0;0;False;0;False;0;0.264;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;110;250.1255,927.043;Inherit;False;89;Sample_04;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;137;633.8473,1856.339;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;118;-80.00246,1715.559;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;99;-2639.356,2682.086;Inherit;True;Property;_TextureSample5;Texture Sample 5;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;136;358.6017,1983.601;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;100;-2222.925,2683.927;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;123;1124.113,1860.315;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightColorNode;133;-66.80037,1961.823;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.LerpOp;115;827.983,1043.83;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;132;627.8937,2002.721;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;135;-86.39788,2090.601;Inherit;False;Property;_LightInfluence;LightInfluence;4;0;Create;True;0;0;0;False;0;False;1;57.61;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;121;380.3999,1715.4;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;134;161.6021,2069.601;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;124;1400.08,1851.58;Inherit;False;Combined;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;120;106.7911,1715.249;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;105;-1103.36,2568.892;Inherit;True;Sample_05;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;143;559.9017,-611.4662;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=ShadowCaster;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;144;559.9017,-611.4662;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;False;False;True;False;False;False;False;0;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;False;False;True;1;LightMode=DepthOnly;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;145;559.9017,-611.4662;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;142;559.9017,-611.4662;Float;False;True;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;3;Halloween/VolumeSmoke;2992e84f91cbeb14eab234972e07ea9d;True;Forward;0;1;Forward;8;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;5;False;-1;10;False;-1;1;1;False;-1;10;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;2;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=UniversalForward;False;False;0;Hidden/InternalErrorShader;0;0;Standard;22;Surface;1;  Blend;0;Two Sided;1;Cast Shadows;1;  Use Shadow Threshold;0;Receive Shadows;1;GPU Instancing;1;LOD CrossFade;0;Built-in Fog;0;DOTS Instancing;0;Meta Pass;0;Extra Pre Pass;0;Tessellation;0;  Phong;0;  Strength;0.5,False,-1;  Type;0;  Tess;16,False,-1;  Min;10,False,-1;  Max;25,False,-1;  Edge Length;16,False,-1;  Max Displacement;25,False,-1;Vertex Position,InvertActionOnDeselection;1;0;5;False;True;True;True;False;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;141;559.9017,-611.4662;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;0;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
WireConnection;8;2;7;0
WireConnection;9;0;8;0
WireConnection;10;0;7;0
WireConnection;10;1;9;0
WireConnection;12;0;10;4
WireConnection;127;0;126;4
WireConnection;127;1;128;0
WireConnection;129;0;127;0
WireConnection;147;0;150;0
WireConnection;149;0;147;0
WireConnection;131;0;137;0
WireConnection;131;1;132;0
WireConnection;69;0;68;0
WireConnection;93;0;77;0
WireConnection;17;0;15;0
WireConnection;55;0;106;0
WireConnection;77;0;61;0
WireConnection;84;0;82;0
WireConnection;84;1;83;4
WireConnection;15;0;14;0
WireConnection;15;1;16;0
WireConnection;44;0;42;0
WireConnection;44;1;55;0
WireConnection;20;0;19;0
WireConnection;20;1;106;0
WireConnection;106;0;38;0
WireConnection;22;0;23;0
WireConnection;22;1;20;0
WireConnection;61;0;55;0
WireConnection;80;0;78;0
WireConnection;80;1;79;0
WireConnection;25;0;24;0
WireConnection;25;1;22;0
WireConnection;67;0;65;0
WireConnection;67;1;64;0
WireConnection;48;0;46;0
WireConnection;48;1;45;0
WireConnection;49;0;47;0
WireConnection;49;1;48;4
WireConnection;95;0;92;0
WireConnection;95;1;93;0
WireConnection;79;0;76;0
WireConnection;79;1;77;0
WireConnection;26;0;27;0
WireConnection;26;1;25;4
WireConnection;37;0;21;0
WireConnection;57;0;115;0
WireConnection;96;0;94;0
WireConnection;96;1;95;0
WireConnection;146;0;125;0
WireConnection;146;1;149;0
WireConnection;83;0;81;0
WireConnection;83;1;80;0
WireConnection;64;0;62;0
WireConnection;64;1;63;0
WireConnection;102;0;101;0
WireConnection;68;0;66;0
WireConnection;68;1;67;4
WireConnection;53;0;52;0
WireConnection;71;0;66;0
WireConnection;71;1;70;0
WireConnection;31;0;34;0
WireConnection;72;0;71;0
WireConnection;87;0;82;0
WireConnection;87;1;86;0
WireConnection;54;0;53;0
WireConnection;63;0;60;0
WireConnection;63;1;61;0
WireConnection;86;0;85;0
WireConnection;101;0;100;0
WireConnection;88;0;87;0
WireConnection;52;0;47;0
WireConnection;52;1;51;0
WireConnection;89;0;88;0
WireConnection;104;0;103;0
WireConnection;112;0;107;0
WireConnection;112;1;108;0
WireConnection;85;0;84;0
WireConnection;113;0;112;0
WireConnection;113;1;109;0
WireConnection;45;0;43;0
WireConnection;45;1;44;0
WireConnection;73;0;72;0
WireConnection;30;0;26;0
WireConnection;103;0;98;0
WireConnection;103;1;102;0
WireConnection;70;0;69;0
WireConnection;50;0;49;0
WireConnection;34;0;35;0
WireConnection;33;0;30;0
WireConnection;35;0;27;0
WireConnection;35;1;33;0
WireConnection;114;0;113;0
WireConnection;114;1;110;0
WireConnection;51;0;50;0
WireConnection;137;0;121;0
WireConnection;137;1;136;0
WireConnection;118;0;117;0
WireConnection;118;1;119;0
WireConnection;99;0;97;0
WireConnection;99;1;96;0
WireConnection;136;0;133;1
WireConnection;136;1;134;0
WireConnection;100;0;98;0
WireConnection;100;1;99;4
WireConnection;123;0;131;0
WireConnection;115;0;114;0
WireConnection;115;1;111;0
WireConnection;121;0;120;0
WireConnection;121;1;122;0
WireConnection;134;0;133;2
WireConnection;134;1;135;0
WireConnection;124;0;123;0
WireConnection;120;0;118;0
WireConnection;105;0;104;0
WireConnection;142;2;32;0
WireConnection;142;3;146;0
ASEEND*/
//CHKSM=FD987630B9ED1B857308AA938C6E24A14F9EF21B