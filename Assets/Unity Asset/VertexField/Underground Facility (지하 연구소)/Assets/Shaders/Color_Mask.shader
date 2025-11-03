// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:3,spmd:0,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:True,rprd:True,enco:False,rmgx:True,imps:True,rpth:1,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:1847,x:32719,y:32712,varname:node_1847,prsc:2|diff-7197-OUT,spec-4772-OUT,gloss-3449-OUT,normal-752-OUT,difocc-4371-OUT;n:type:ShaderForge.SFN_Tex2d,id:2716,x:31378,y:32533,ptovrint:False,ptlb:Diffuse,ptin:_Diffuse,varname:node_2716,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-64-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:5447,x:31519,y:33240,ptovrint:False,ptlb:SpecularGloss,ptin:_SpecularGloss,varname:node_5447,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-64-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:6080,x:30934,y:32749,ptovrint:False,ptlb:Normal,ptin:_Normal,varname:node_6080,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:True,ntxv:3,isnm:True|UVIN-64-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:9710,x:31359,y:33471,ptovrint:False,ptlb:AO,ptin:_AO,varname:node_9710,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-64-UVOUT;n:type:ShaderForge.SFN_Slider,id:9733,x:31780,y:33662,ptovrint:False,ptlb:AO Power,ptin:_AOPower,varname:node_9733,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Lerp,id:7197,x:32414,y:32538,varname:node_7197,prsc:2|A-3605-OUT,B-2409-OUT,T-3895-R;n:type:ShaderForge.SFN_Tex2d,id:3895,x:31496,y:31887,ptovrint:False,ptlb:TexMask,ptin:_TexMask,varname:node_3895,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-64-UVOUT;n:type:ShaderForge.SFN_Color,id:4152,x:31759,y:32247,ptovrint:False,ptlb:Color A,ptin:_ColorA,varname:node_4152,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Color,id:4467,x:31760,y:32415,ptovrint:False,ptlb:Color B,ptin:_ColorB,varname:node_4467,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:3605,x:32077,y:32473,varname:node_3605,prsc:2|A-4152-RGB,B-2716-RGB;n:type:ShaderForge.SFN_Multiply,id:2409,x:32074,y:32613,varname:node_2409,prsc:2|A-4467-RGB,B-2716-RGB;n:type:ShaderForge.SFN_Lerp,id:4772,x:32367,y:32748,varname:node_4772,prsc:2|A-525-OUT,B-6916-OUT,T-3895-R;n:type:ShaderForge.SFN_Color,id:8922,x:31727,y:32555,ptovrint:False,ptlb:SpecularGloss A,ptin:_SpecularGlossA,varname:node_8922,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Color,id:4010,x:31730,y:32711,ptovrint:False,ptlb:SpecularGloss B,ptin:_SpecularGlossB,varname:node_4010,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Lerp,id:3449,x:32360,y:33011,varname:node_3449,prsc:2|A-719-OUT,B-842-OUT,T-3895-R;n:type:ShaderForge.SFN_Add,id:525,x:32026,y:32704,varname:node_525,prsc:2|A-8922-RGB,B-5447-RGB;n:type:ShaderForge.SFN_Add,id:6916,x:32031,y:32841,varname:node_6916,prsc:2|A-4010-RGB,B-5447-RGB;n:type:ShaderForge.SFN_Add,id:719,x:32076,y:33013,varname:node_719,prsc:2|A-8922-A,B-5447-A;n:type:ShaderForge.SFN_Add,id:842,x:32067,y:33147,varname:node_842,prsc:2|A-4010-A,B-5447-A;n:type:ShaderForge.SFN_Parallax,id:64,x:30349,y:32449,varname:node_64,prsc:2|HEI-3061-R,DEP-7035-OUT;n:type:ShaderForge.SFN_Tex2d,id:3061,x:29941,y:32556,ptovrint:False,ptlb:heightmapTexture,ptin:_heightmapTexture,varname:node_3061,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:9396,x:30834,y:33243,ptovrint:False,ptlb:Normal_B_power,ptin:_Normal_B_power,varname:node_9396,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Lerp,id:752,x:31799,y:32980,varname:node_752,prsc:2|A-8284-OUT,B-7664-OUT,T-3895-R;n:type:ShaderForge.SFN_Slider,id:9238,x:30771,y:33065,ptovrint:False,ptlb:Normal_A_power,ptin:_Normal_A_power,varname:node_9238,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Lerp,id:8284,x:31441,y:32878,varname:node_8284,prsc:2|A-6080-RGB,B-385-RGB,T-9238-OUT;n:type:ShaderForge.SFN_Lerp,id:7664,x:31434,y:33059,varname:node_7664,prsc:2|A-6080-RGB,B-385-RGB,T-9396-OUT;n:type:ShaderForge.SFN_Tex2d,id:385,x:30570,y:32850,ptovrint:False,ptlb:Normal_simple,ptin:_Normal_simple,varname:node_385,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Blend,id:4371,x:32169,y:33359,varname:node_4371,prsc:2,blmd:1,clmp:True|SRC-9710-B,DST-9733-OUT;n:type:ShaderForge.SFN_Color,id:4756,x:31902,y:32298,ptovrint:False,ptlb:Color A_copy,ptin:_ColorA_copy,varname:_ColorA_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Slider,id:7035,x:30095,y:32807,ptovrint:False,ptlb:heightmap_Depth,ptin:_heightmap_Depth,varname:node_7035,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:0.1;proporder:2716-9710-5447-9733-7035-6080-3061-3895-4152-4467-8922-4010-385-9396-9238;pass:END;sub:END;*/

Shader "vertexfield/Colormask defered" {
    Properties {
        _Diffuse ("Diffuse", 2D) = "white" {}
        _AO ("AO", 2D) = "white" {}
        _SpecularGloss ("SpecularGloss", 2D) = "white" {}
        _AOPower ("AO Power", Range(0, 1)) = 0
        _heightmap_Depth ("heightmap_Depth", Range(0, 0.1)) = 0
        [Normal]_Normal ("Normal", 2D) = "bump" {}
        _heightmapTexture ("heightmapTexture", 2D) = "white" {}
        _TexMask ("TexMask", 2D) = "white" {}
        _ColorA ("Color A", Color) = (0.5,0.5,0.5,1)
        _ColorB ("Color B", Color) = (0.5,0.5,0.5,1)
        _SpecularGlossA ("SpecularGloss A", Color) = (0.5,0.5,0.5,1)
        _SpecularGlossB ("SpecularGloss B", Color) = (0.5,0.5,0.5,1)
        _Normal_simple ("Normal_simple", 2D) = "bump" {}
        _Normal_B_power ("Normal_B_power", Range(0, 1)) = 0
        _Normal_A_power ("Normal_A_power", Range(0, 1)) = 0
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        LOD 200
        Pass {
            Name "DEFERRED"
            Tags {
                "LightMode"="Deferred"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_DEFERRED
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile ___ UNITY_HDR_ON
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform sampler2D _SpecularGloss; uniform float4 _SpecularGloss_ST;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform sampler2D _AO; uniform float4 _AO_ST;
            uniform float _AOPower;
            uniform sampler2D _TexMask; uniform float4 _TexMask_ST;
            uniform float4 _ColorA;
            uniform float4 _ColorB;
            uniform float4 _SpecularGlossA;
            uniform float4 _SpecularGlossB;
            uniform sampler2D _heightmapTexture; uniform float4 _heightmapTexture_ST;
            uniform float _Normal_B_power;
            uniform float _Normal_A_power;
            uniform sampler2D _Normal_simple; uniform float4 _Normal_simple_ST;
            uniform float _heightmap_Depth;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD7;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            void frag(
                VertexOutput i,
                out half4 outDiffuse : SV_Target0,
                out half4 outSpecSmoothness : SV_Target1,
                out half4 outNormal : SV_Target2,
                out half4 outEmission : SV_Target3 )
            {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 _heightmapTexture_var = tex2D(_heightmapTexture,TRANSFORM_TEX(i.uv0, _heightmapTexture));
                float2 node_64 = (_heightmap_Depth*(_heightmapTexture_var.r - 0.5)*mul(tangentTransform, viewDirection).xy + i.uv0);
                float3 _Normal_var = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_64.rg, _Normal)));
                float3 _Normal_simple_var = UnpackNormal(tex2D(_Normal_simple,TRANSFORM_TEX(i.uv0, _Normal_simple)));
                float4 _TexMask_var = tex2D(_TexMask,TRANSFORM_TEX(node_64.rg, _TexMask));
                float3 normalLocal = lerp(lerp(_Normal_var.rgb,_Normal_simple_var.rgb,_Normal_A_power),lerp(_Normal_var.rgb,_Normal_simple_var.rgb,_Normal_B_power),_TexMask_var.r);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
////// Lighting:
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float4 _SpecularGloss_var = tex2D(_SpecularGloss,TRANSFORM_TEX(node_64.rg, _SpecularGloss));
                float gloss = lerp((_SpecularGlossA.a+_SpecularGloss_var.a),(_SpecularGlossB.a+_SpecularGloss_var.a),_TexMask_var.r);
                float perceptualRoughness = 1.0 - lerp((_SpecularGlossA.a+_SpecularGloss_var.a),(_SpecularGlossB.a+_SpecularGloss_var.a),_TexMask_var.r);
                float roughness = perceptualRoughness * perceptualRoughness;
/////// GI Data:
                UnityLight light; // Dummy light
                light.color = 0;
                light.dir = half3(0,1,0);
                light.ndotl = max(0,dot(normalDirection,light.dir));
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = 1;
                #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                    d.ambient = 0;
                    d.lightmapUV = i.ambientOrLightmapUV;
                #else
                    d.ambient = i.ambientOrLightmapUV;
                #endif
                #if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMin[0] = unity_SpecCube0_BoxMin;
                    d.boxMin[1] = unity_SpecCube1_BoxMin;
                #endif
                #if UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMax[0] = unity_SpecCube0_BoxMax;
                    d.boxMax[1] = unity_SpecCube1_BoxMax;
                    d.probePosition[0] = unity_SpecCube0_ProbePosition;
                    d.probePosition[1] = unity_SpecCube1_ProbePosition;
                #endif
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
////// Specular:
                float3 specularColor = lerp((_SpecularGlossA.rgb+_SpecularGloss_var.rgb),(_SpecularGlossB.rgb+_SpecularGloss_var.rgb),_TexMask_var.r);
                float specularMonochrome;
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(node_64.rg, _Diffuse));
                float3 diffuseColor = lerp((_ColorA.rgb*_Diffuse_var.rgb),(_ColorB.rgb*_Diffuse_var.rgb),_TexMask_var.r); // Need this for specular when using metallic
                diffuseColor = EnergyConservationBetweenDiffuseAndSpecular(diffuseColor, specularColor, specularMonochrome);
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = max(0.0,dot( normalDirection, viewDirection ));
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (gi.indirect.specular);
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
/////// Diffuse:
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += gi.indirect.diffuse;
                float4 _AO_var = tex2D(_AO,TRANSFORM_TEX(node_64.rg, _AO));
                indirectDiffuse *= saturate((_AO_var.b*_AOPower)); // Diffuse AO
                diffuseColor *= 1-specularMonochrome;
/// Final Color:
                outDiffuse = half4( diffuseColor, saturate((_AO_var.b*_AOPower)) );
                outSpecSmoothness = half4( specularColor, gloss );
                outNormal = half4( normalDirection * 0.5 + 0.5, 1 );
                outEmission = half4(0,0,0,1);
                outEmission.rgb += indirectSpecular * 1;
                outEmission.rgb += indirectDiffuse * diffuseColor;
                #ifndef UNITY_HDR_ON
                    outEmission.rgb = exp2(-outEmission.rgb);
                #endif
            }
            ENDCG
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform sampler2D _SpecularGloss; uniform float4 _SpecularGloss_ST;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform sampler2D _AO; uniform float4 _AO_ST;
            uniform float _AOPower;
            uniform sampler2D _TexMask; uniform float4 _TexMask_ST;
            uniform float4 _ColorA;
            uniform float4 _ColorB;
            uniform float4 _SpecularGlossA;
            uniform float4 _SpecularGlossB;
            uniform sampler2D _heightmapTexture; uniform float4 _heightmapTexture_ST;
            uniform float _Normal_B_power;
            uniform float _Normal_A_power;
            uniform sampler2D _Normal_simple; uniform float4 _Normal_simple_ST;
            uniform float _heightmap_Depth;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD10;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 _heightmapTexture_var = tex2D(_heightmapTexture,TRANSFORM_TEX(i.uv0, _heightmapTexture));
                float2 node_64 = (_heightmap_Depth*(_heightmapTexture_var.r - 0.5)*mul(tangentTransform, viewDirection).xy + i.uv0);
                float3 _Normal_var = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_64.rg, _Normal)));
                float3 _Normal_simple_var = UnpackNormal(tex2D(_Normal_simple,TRANSFORM_TEX(i.uv0, _Normal_simple)));
                float4 _TexMask_var = tex2D(_TexMask,TRANSFORM_TEX(node_64.rg, _TexMask));
                float3 normalLocal = lerp(lerp(_Normal_var.rgb,_Normal_simple_var.rgb,_Normal_A_power),lerp(_Normal_var.rgb,_Normal_simple_var.rgb,_Normal_B_power),_TexMask_var.r);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float4 _SpecularGloss_var = tex2D(_SpecularGloss,TRANSFORM_TEX(node_64.rg, _SpecularGloss));
                float gloss = lerp((_SpecularGlossA.a+_SpecularGloss_var.a),(_SpecularGlossB.a+_SpecularGloss_var.a),_TexMask_var.r);
                float perceptualRoughness = 1.0 - lerp((_SpecularGlossA.a+_SpecularGloss_var.a),(_SpecularGlossB.a+_SpecularGloss_var.a),_TexMask_var.r);
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                    d.ambient = 0;
                    d.lightmapUV = i.ambientOrLightmapUV;
                #else
                    d.ambient = i.ambientOrLightmapUV;
                #endif
                #if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMin[0] = unity_SpecCube0_BoxMin;
                    d.boxMin[1] = unity_SpecCube1_BoxMin;
                #endif
                #if UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMax[0] = unity_SpecCube0_BoxMax;
                    d.boxMax[1] = unity_SpecCube1_BoxMax;
                    d.probePosition[0] = unity_SpecCube0_ProbePosition;
                    d.probePosition[1] = unity_SpecCube1_ProbePosition;
                #endif
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = lerp((_SpecularGlossA.rgb+_SpecularGloss_var.rgb),(_SpecularGlossB.rgb+_SpecularGloss_var.rgb),_TexMask_var.r);
                float specularMonochrome;
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(node_64.rg, _Diffuse));
                float3 diffuseColor = lerp((_ColorA.rgb*_Diffuse_var.rgb),(_ColorB.rgb*_Diffuse_var.rgb),_TexMask_var.r); // Need this for specular when using metallic
                diffuseColor = EnergyConservationBetweenDiffuseAndSpecular(diffuseColor, specularColor, specularMonochrome);
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                half surfaceReduction;
                #ifdef UNITY_COLORSPACE_GAMMA
                    surfaceReduction = 1.0-0.28*roughness*perceptualRoughness;
                #else
                    surfaceReduction = 1.0/(roughness*roughness + 1.0);
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (gi.indirect.specular);
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                indirectSpecular *= surfaceReduction;
                float3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += gi.indirect.diffuse;
                float4 _AO_var = tex2D(_AO,TRANSFORM_TEX(node_64.rg, _AO));
                indirectDiffuse *= saturate((_AO_var.b*_AOPower)); // Diffuse AO
                diffuseColor *= 1-specularMonochrome;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform sampler2D _SpecularGloss; uniform float4 _SpecularGloss_ST;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform sampler2D _TexMask; uniform float4 _TexMask_ST;
            uniform float4 _ColorA;
            uniform float4 _ColorB;
            uniform float4 _SpecularGlossA;
            uniform float4 _SpecularGlossB;
            uniform sampler2D _heightmapTexture; uniform float4 _heightmapTexture_ST;
            uniform float _Normal_B_power;
            uniform float _Normal_A_power;
            uniform sampler2D _Normal_simple; uniform float4 _Normal_simple_ST;
            uniform float _heightmap_Depth;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 _heightmapTexture_var = tex2D(_heightmapTexture,TRANSFORM_TEX(i.uv0, _heightmapTexture));
                float2 node_64 = (_heightmap_Depth*(_heightmapTexture_var.r - 0.5)*mul(tangentTransform, viewDirection).xy + i.uv0);
                float3 _Normal_var = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_64.rg, _Normal)));
                float3 _Normal_simple_var = UnpackNormal(tex2D(_Normal_simple,TRANSFORM_TEX(i.uv0, _Normal_simple)));
                float4 _TexMask_var = tex2D(_TexMask,TRANSFORM_TEX(node_64.rg, _TexMask));
                float3 normalLocal = lerp(lerp(_Normal_var.rgb,_Normal_simple_var.rgb,_Normal_A_power),lerp(_Normal_var.rgb,_Normal_simple_var.rgb,_Normal_B_power),_TexMask_var.r);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float4 _SpecularGloss_var = tex2D(_SpecularGloss,TRANSFORM_TEX(node_64.rg, _SpecularGloss));
                float gloss = lerp((_SpecularGlossA.a+_SpecularGloss_var.a),(_SpecularGlossB.a+_SpecularGloss_var.a),_TexMask_var.r);
                float perceptualRoughness = 1.0 - lerp((_SpecularGlossA.a+_SpecularGloss_var.a),(_SpecularGlossB.a+_SpecularGloss_var.a),_TexMask_var.r);
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = lerp((_SpecularGlossA.rgb+_SpecularGloss_var.rgb),(_SpecularGlossB.rgb+_SpecularGloss_var.rgb),_TexMask_var.r);
                float specularMonochrome;
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(node_64.rg, _Diffuse));
                float3 diffuseColor = lerp((_ColorA.rgb*_Diffuse_var.rgb),(_ColorB.rgb*_Diffuse_var.rgb),_TexMask_var.r); // Need this for specular when using metallic
                diffuseColor = EnergyConservationBetweenDiffuseAndSpecular(diffuseColor, specularColor, specularMonochrome);
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                diffuseColor *= 1-specularMonochrome;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "Meta"
            Tags {
                "LightMode"="Meta"
            }
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_META 1
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform sampler2D _SpecularGloss; uniform float4 _SpecularGloss_ST;
            uniform sampler2D _TexMask; uniform float4 _TexMask_ST;
            uniform float4 _ColorA;
            uniform float4 _ColorB;
            uniform float4 _SpecularGlossA;
            uniform float4 _SpecularGlossB;
            uniform sampler2D _heightmapTexture; uniform float4 _heightmapTexture_ST;
            uniform float _heightmap_Depth;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                o.Emission = 0;
                
                float4 _heightmapTexture_var = tex2D(_heightmapTexture,TRANSFORM_TEX(i.uv0, _heightmapTexture));
                float2 node_64 = (_heightmap_Depth*(_heightmapTexture_var.r - 0.5)*mul(tangentTransform, viewDirection).xy + i.uv0);
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(node_64.rg, _Diffuse));
                float4 _TexMask_var = tex2D(_TexMask,TRANSFORM_TEX(node_64.rg, _TexMask));
                float3 diffColor = lerp((_ColorA.rgb*_Diffuse_var.rgb),(_ColorB.rgb*_Diffuse_var.rgb),_TexMask_var.r);
                float4 _SpecularGloss_var = tex2D(_SpecularGloss,TRANSFORM_TEX(node_64.rg, _SpecularGloss));
                float3 specColor = lerp((_SpecularGlossA.rgb+_SpecularGloss_var.rgb),(_SpecularGlossB.rgb+_SpecularGloss_var.rgb),_TexMask_var.r);
                float specularMonochrome = max(max(specColor.r, specColor.g),specColor.b);
                diffColor *= (1.0-specularMonochrome);
                float roughness = 1.0 - lerp((_SpecularGlossA.a+_SpecularGloss_var.a),(_SpecularGlossB.a+_SpecularGloss_var.a),_TexMask_var.r);
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
  //  CustomEditor "ShaderForgeMaterialInspector"
}
