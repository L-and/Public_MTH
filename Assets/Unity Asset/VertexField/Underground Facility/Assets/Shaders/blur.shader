// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:True,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:7253,x:34187,y:33550,varname:node_7253,prsc:2|custl-7758-OUT;n:type:ShaderForge.SFN_ScreenPos,id:6110,x:28736,y:32830,varname:node_6110,prsc:2,sctp:2;n:type:ShaderForge.SFN_SceneColor,id:5363,x:32116,y:32915,varname:node_5363,prsc:2|UVIN-4024-OUT;n:type:ShaderForge.SFN_Set,id:8966,x:29244,y:33049,varname:__screenPos,prsc:2|IN-6110-UVOUT;n:type:ShaderForge.SFN_Slider,id:3862,x:29051,y:32469,ptovrint:False,ptlb:Offset,ptin:_Offset,varname:node_3862,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.01445401,max:0.05;n:type:ShaderForge.SFN_Set,id:2498,x:29756,y:32696,varname:__offset,prsc:2|IN-2204-OUT;n:type:ShaderForge.SFN_Get,id:3971,x:30925,y:33282,varname:node_3971,prsc:2|IN-8966-OUT;n:type:ShaderForge.SFN_Get,id:5616,x:30919,y:33384,varname:node_5616,prsc:2|IN-2498-OUT;n:type:ShaderForge.SFN_Add,id:1507,x:31387,y:33308,varname:node_1507,prsc:2|A-3971-OUT,B-5616-OUT;n:type:ShaderForge.SFN_Set,id:5440,x:29740,y:32862,varname:__screenPosV,prsc:2|IN-6110-V;n:type:ShaderForge.SFN_Set,id:5921,x:29538,y:32808,varname:__screenPosU,prsc:2|IN-6110-U;n:type:ShaderForge.SFN_Get,id:9971,x:31335,y:33085,varname:node_9971,prsc:2|IN-5921-OUT;n:type:ShaderForge.SFN_Get,id:8933,x:31342,y:33186,varname:node_8933,prsc:2|IN-2498-OUT;n:type:ShaderForge.SFN_Add,id:6236,x:31577,y:33080,varname:node_6236,prsc:2|A-9971-OUT,B-8933-OUT;n:type:ShaderForge.SFN_Append,id:4024,x:31789,y:33077,varname:node_4024,prsc:2|A-6236-OUT,B-9347-OUT;n:type:ShaderForge.SFN_Get,id:9347,x:31570,y:33024,varname:node_9347,prsc:2|IN-5440-OUT;n:type:ShaderForge.SFN_Get,id:9045,x:31369,y:32118,varname:node_9045,prsc:2|IN-5440-OUT;n:type:ShaderForge.SFN_Get,id:6694,x:31365,y:32199,varname:node_6694,prsc:2|IN-2498-OUT;n:type:ShaderForge.SFN_Add,id:6042,x:31593,y:32124,varname:node_6042,prsc:2|A-9045-OUT,B-6694-OUT;n:type:ShaderForge.SFN_Append,id:3720,x:31795,y:32034,varname:node_3720,prsc:2|A-3061-OUT,B-6042-OUT;n:type:ShaderForge.SFN_Get,id:3061,x:31574,y:31984,varname:node_3061,prsc:2|IN-5921-OUT;n:type:ShaderForge.SFN_SceneColor,id:7691,x:32128,y:32037,varname:node_7691,prsc:2|UVIN-3720-OUT;n:type:ShaderForge.SFN_Add,id:107,x:32362,y:33145,varname:node_107,prsc:2|A-6705-RGB,B-5363-RGB,C-7691-RGB,D-9274-RGB,E-3666-RGB;n:type:ShaderForge.SFN_Divide,id:7758,x:32647,y:33166,varname:node_7758,prsc:2|A-4013-OUT,B-3592-OUT;n:type:ShaderForge.SFN_Vector1,id:3592,x:32725,y:33372,varname:node_3592,prsc:2,v1:9;n:type:ShaderForge.SFN_SceneColor,id:6705,x:32113,y:32749,varname:node_6705,prsc:2|UVIN-7436-OUT;n:type:ShaderForge.SFN_Get,id:7436,x:31767,y:32754,varname:node_7436,prsc:2|IN-8966-OUT;n:type:ShaderForge.SFN_Get,id:2581,x:31391,y:32342,varname:node_2581,prsc:2|IN-5921-OUT;n:type:ShaderForge.SFN_Get,id:2587,x:31394,y:32433,varname:node_2587,prsc:2|IN-2498-OUT;n:type:ShaderForge.SFN_Append,id:5197,x:31841,y:32324,varname:node_5197,prsc:2|A-6612-OUT,B-3972-OUT;n:type:ShaderForge.SFN_Get,id:3972,x:31583,y:32256,varname:node_3972,prsc:2|IN-5440-OUT;n:type:ShaderForge.SFN_Get,id:8912,x:31398,y:32529,varname:node_8912,prsc:2|IN-5440-OUT;n:type:ShaderForge.SFN_Get,id:3541,x:31390,y:32615,varname:node_3541,prsc:2|IN-2498-OUT;n:type:ShaderForge.SFN_Append,id:9566,x:31834,y:32532,varname:node_9566,prsc:2|A-1762-OUT,B-9477-OUT;n:type:ShaderForge.SFN_Get,id:1762,x:31608,y:32506,varname:node_1762,prsc:2|IN-5921-OUT;n:type:ShaderForge.SFN_SceneColor,id:3666,x:32167,y:32535,varname:node_3666,prsc:2|UVIN-9566-OUT;n:type:ShaderForge.SFN_SceneColor,id:9274,x:32169,y:32296,varname:node_9274,prsc:2|UVIN-5197-OUT;n:type:ShaderForge.SFN_Subtract,id:6612,x:31623,y:32341,varname:node_6612,prsc:2|A-2581-OUT,B-2587-OUT;n:type:ShaderForge.SFN_Subtract,id:9477,x:31608,y:32560,varname:node_9477,prsc:2|A-8912-OUT,B-3541-OUT;n:type:ShaderForge.SFN_SceneColor,id:6556,x:31824,y:33347,varname:node_6556,prsc:2|UVIN-1507-OUT;n:type:ShaderForge.SFN_Subtract,id:2519,x:31404,y:33458,varname:node_2519,prsc:2|A-3971-OUT,B-5616-OUT;n:type:ShaderForge.SFN_SceneColor,id:8776,x:31829,y:33483,varname:node_8776,prsc:2|UVIN-2519-OUT;n:type:ShaderForge.SFN_Add,id:4231,x:32211,y:33400,varname:node_4231,prsc:2|A-6556-RGB,B-8776-RGB,C-3723-RGB,D-2009-RGB;n:type:ShaderForge.SFN_Get,id:5348,x:31193,y:33677,varname:node_5348,prsc:2|IN-5921-OUT;n:type:ShaderForge.SFN_Get,id:6050,x:31208,y:33831,varname:node_6050,prsc:2|IN-5440-OUT;n:type:ShaderForge.SFN_Get,id:1473,x:31201,y:33758,varname:node_1473,prsc:2|IN-2498-OUT;n:type:ShaderForge.SFN_Add,id:776,x:31544,y:33669,varname:node_776,prsc:2|A-5348-OUT,B-1473-OUT;n:type:ShaderForge.SFN_Subtract,id:9580,x:31539,y:33814,varname:node_9580,prsc:2|A-6050-OUT,B-1473-OUT;n:type:ShaderForge.SFN_Append,id:4881,x:31865,y:33778,varname:node_4881,prsc:2|A-776-OUT,B-9580-OUT;n:type:ShaderForge.SFN_SceneColor,id:3723,x:32081,y:33778,varname:node_3723,prsc:2|UVIN-4881-OUT;n:type:ShaderForge.SFN_Get,id:617,x:31174,y:33967,varname:node_617,prsc:2|IN-5440-OUT;n:type:ShaderForge.SFN_Get,id:5366,x:31189,y:34121,varname:node_5366,prsc:2|IN-5921-OUT;n:type:ShaderForge.SFN_Get,id:9544,x:31182,y:34048,varname:node_9544,prsc:2|IN-2498-OUT;n:type:ShaderForge.SFN_Add,id:2556,x:31525,y:33959,varname:node_2556,prsc:2|A-617-OUT,B-9544-OUT;n:type:ShaderForge.SFN_Subtract,id:2894,x:31520,y:34104,varname:node_2894,prsc:2|A-5366-OUT,B-9544-OUT;n:type:ShaderForge.SFN_Append,id:4480,x:31846,y:34068,varname:node_4480,prsc:2|A-2894-OUT,B-2556-OUT;n:type:ShaderForge.SFN_SceneColor,id:2009,x:32062,y:34068,varname:node_2009,prsc:2|UVIN-4480-OUT;n:type:ShaderForge.SFN_Add,id:4013,x:32568,y:33431,varname:node_4013,prsc:2|A-107-OUT,B-4231-OUT;n:type:ShaderForge.SFN_ObjectPosition,id:86,x:28679,y:32538,varname:node_86,prsc:2;n:type:ShaderForge.SFN_ViewPosition,id:8180,x:28675,y:32682,varname:node_8180,prsc:2;n:type:ShaderForge.SFN_Divide,id:2204,x:29476,y:32636,varname:node_2204,prsc:2|A-3862-OUT,B-9028-OUT;n:type:ShaderForge.SFN_Log,id:3638,x:29065,y:32760,varname:node_3638,prsc:2,lt:0|IN-627-OUT;n:type:ShaderForge.SFN_Distance,id:627,x:28937,y:32589,varname:node_627,prsc:2|A-86-XYZ,B-8180-XYZ;n:type:ShaderForge.SFN_Divide,id:9028,x:29248,y:32628,varname:node_9028,prsc:2|A-627-OUT,B-3638-OUT;proporder:3862;pass:END;sub:END;*/

Shader "vertexfield/blur" {
    Properties {
        _Offset ("Offset", Range(0, 0.05)) = 0.01445401
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 200
        GrabPass{ }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform float _Offset;
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 projPos : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                float2 sceneUVs = (i.projPos.xy / i.projPos.w);
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
////// Lighting:
                float2 __screenPos = sceneUVs.rg;
                float __screenPosU = sceneUVs.r;
                float node_627 = distance(objPos.rgb,_WorldSpaceCameraPos);
                float __offset = (_Offset/(node_627/log(node_627)));
                float __screenPosV = sceneUVs.g;
                float2 node_3971 = __screenPos;
                float node_5616 = __offset;
                float node_1473 = __offset;
                float node_9544 = __offset;
                float3 finalColor = (((tex2D( _GrabTexture, __screenPos).rgb+tex2D( _GrabTexture, float2((__screenPosU+__offset),__screenPosV)).rgb+tex2D( _GrabTexture, float2(__screenPosU,(__screenPosV+__offset))).rgb+tex2D( _GrabTexture, float2((__screenPosU-__offset),__screenPosV)).rgb+tex2D( _GrabTexture, float2(__screenPosU,(__screenPosV-__offset))).rgb)+(tex2D( _GrabTexture, (node_3971+node_5616)).rgb+tex2D( _GrabTexture, (node_3971-node_5616)).rgb+tex2D( _GrabTexture, float2((__screenPosU+node_1473),(__screenPosV-node_1473))).rgb+tex2D( _GrabTexture, float2((__screenPosU-node_9544),(__screenPosV+node_9544))).rgb))/9.0);
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
 //   CustomEditor "ShaderForgeMaterialInspector"
}
