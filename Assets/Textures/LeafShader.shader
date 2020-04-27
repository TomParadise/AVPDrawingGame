Shader "Custom/LeafShader"
{
    // https://docs.unity3d.com/Manual/SL-VertexFragmentShaderExamples.html
    // https://forum.unity.com/threads/beginner-in-graphics-on-a-quest-to-do-the-foliage-from-the-witness.518842/#post-4255579
    // https://pastebin.com/u0JjSjAC
    // https://catlikecoding.com/unity/tutorials/rendering/part-18/Rendering-18.pdf
    // https://catlikecoding.com/unity/tutorials/rendering/part-12/
    // https://docs.unity3d.com/Manual/SL-ShaderSemantics.html

    // moved most possible calculation from fragment to vertex shader for perfomance

    // DOUBLESIDES rendering and lightning
    // LIGHTNING diffuse Lambert shading including lightning/reflection probes and lightmaps
    // ALPHA uses cutout for transparency, full screen anti-aliasing effect recommended
    // FRESNEL hides faces almost perdendicular to camera using simple fresnel rim
    // FLUFFYNORMALS calculates normal vectors pointing from mesh origin away for fluffy effect
    // FOG disabled for performance
    // SHADOWS casting disabled for performance
    // INNERDARKEN allows darker pixels closer to origin of the mesh
    // DITHER order independent transparency (OIT) alpha two pass or dithering

    // TODO
    // SOFT_PARTICLES soft particles, but needs a costly extra grab pass
    // LOD_FADE_CROSSFADE support LOD cross fading

    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    // ALPHA
    _Cutoff("Alpha cutoff", Range(0.0, 1.0)) = 0.4
        // FRESNEL
        _AlphaOffset("Alpha offset", Range(-1.0, 1.0)) = 0
        // DITHER
        _AlphaDither("Alpha dither", Range(0.0, 1.0)) = 0.2
        // FLUFFYNORMALS
        _NormalsOffset("Normals offset", Vector) = (0,0,0)
        // INNERDARKEN
        _DarkenRadius("Inner Darken Radius", Range(0.0, 10.0)) = 1.0
        _DarkenHeight("Inner Darken Height", Range(-10.0, 10.0)) = 0.0
        _DarkenPower("Inner Darken Power", Range(0.0, 1.0)) = 0.0
    }
        SubShader
    {
        Tags
        {
            //"RenderType"="Transparent"
            "RenderType" = "TransparentCutout"
            "Queue" = "AlphaTest"
            "IgnoreProjector" = "True"
        }
        LOD 100
        Cull Off

        Pass
        {
            // try use Alpha To Coverage if multisample anti-aliasing is set to 4 (MSAA)
            //AlphaToMask On

            Tags
            {
                "LightMode" = "ForwardBase"     // for ShadeSH9
                //"LightMode" = "ShadowCaster"
            }

            CGPROGRAM

        // Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members vertex)
        //#pragma exclude_renderers d3d11

        #pragma vertex vert
        #pragma fragment frag
        #pragma target 3.0  // for VPOS

        // FOG make fog work
        //#pragma multi_compile_fog

        // LOD
        #pragma multi_compile _ LOD_FADE_CROSSFADE
        //#pragma multi_compile _ LOD_FADE_CROSSFADE DITHER_ALPA
        //#pragma shader_feature _ _RENDERING_CUTOUT _RENDERING_FADE _RENDERING_TRANSPARENT


        #include "UnityCG.cginc"     // for UnityObjectToWorldNormal
        #include "Lighting.cginc"    // for _LightColor0

        struct appdata
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
            half3 normal : NORMAL;
        };

    // note: no SV_POSITION in this struct
    struct v2f
    {
        float2 uv : TEXCOORD0;
        fixed4 diff : COLOR0;

        // FOG
        //UNITY_FOG_COORDS(1)

        // FRESNEL LOD
//#if SHADOWS_SEMITRANSPARENT || defined(LOD_FADE_CROSSFADE)
//                UNITY_VPOS_TYPE vpos : VPOS;
//#else
//                float4 vertex : POSITION;    // deprecated? SV_POSITION to ensure compatibility with xbox
//#endif
                half3 worldNormal : NORMAL;
                float fresnel : TEXCOORD1;
            };

    // LIGHTING
    sampler2D _MainTex;
    float4 _MainTex_ST;

    // FRESNEL
    float _Cutoff;
    float _AlphaOffset;
    float _AlphaDither;
    float3 _NormalsOffset;

    // INNER DARKEN
    float _DarkenRadius;
    float _DarkenHeight;
    float _DarkenPower;

    // FRESNEL LOD automatically assigned by unity
    sampler3D _DitherMaskLOD;

    v2f vert(
        appdata v,
        out float4 outpos : SV_POSITION // clip space position output
        )
    {
        v2f o;

        // transform from local to world space
        //o.vertex = UnityObjectToClipPos(v.vertex);
        float4 worldpos = mul(unity_ObjectToWorld, v.vertex);
        o.uv = TRANSFORM_TEX(v.uv, _MainTex);
        //o.worldNormal = UnityObjectToWorldNormal(v.normal);
        o.worldNormal = normalize(mul(v.normal, (float3x3)unity_WorldToObject));

        // FRESNEL precalc camera-perpedicular alpha
        //float3 view = normalize(ObjSpaceViewDir(v.vertex));
        //float vn = dot(view, v.normal);
        float vn = dot(o.worldNormal, normalize(_WorldSpaceCameraPos.xyz - worldpos.xyz));

        // DOUBLESIDED flip normal if necessary
        //o.worldNormal *= sign(o.worldNormal);
        vn *= sign(vn);
        //o.fresnel = _FresnelBias + _FresnelScale * pow(1 + vn, _FresnelPower);
        o.fresnel = saturate(_AlphaOffset + vn);

        // FLUFFY NORMALS overwrite normal with the normals on a sphere
        // attention: vector 0,0,0 can result to NAN
        o.worldNormal = UnityObjectToWorldNormal(normalize(_NormalsOffset + v.vertex.xyz));
        //o.worldnormal = normalize(mul(_NormalsOffset + v.vertex.xyz, (float3x3)unity_WorldToObject));

        // LIGHTING dot product between normal and light direction for standard diffuse (Lambert) lighting
        half nl = max(0, dot(o.worldNormal, _WorldSpaceLightPos0.xyz));
        // factor in the light color
        o.diff = nl * _LightColor0;
        // in addition to the diffuse lighting from the main light, add illumination from ambient or light probes
        // ShadeSH9 function from UnityCG.cginc evaluates it, using world space normal
        o.diff.rgb += ShadeSH9(half4(o.worldNormal, 1));
        o.diff.a = 1;

        // INNER DARKER make darker in the middle, attention does not calculate scale
        float dist = length(v.vertex);
        //float grad = saturate(dist * (1.0 / _DarkenRadius)) * (_DarkenPower - 1.0);
        float grad = saturate(((dist - _DarkenRadius) * _DarkenPower) + 1.0);
        o.diff.rgb *= grad;

        // HEIGHT DARKER make darker in the bottom, attention does not calculate scale
        grad = saturate(((v.vertex.y - _DarkenHeight) * _DarkenPower) + 1.0);
        o.diff.rgb *= grad;

        // this would be more performant, but darkens the result a bit. why?
        //o.diff = saturate(o.diff);

        // DITHER pass coordinates to calculate screen pixel position
        //outpos = UnityObjectToClipPos(v.vertex);
        outpos = mul(UNITY_MATRIX_VP, worldpos);    // performance optimization

        // FOG
        //UNITY_TRANSFER_FOG(o, outpos);

        return o;
    }

    fixed4 frag(
        v2f i,
        /*fixed facing : VFACE,*/
        UNITY_VPOS_TYPE screenPos : VPOS
        ) : SV_Target
    {
        // LIGHTING sample the texture, diffuse lighting
        fixed4 col = tex2D(_MainTex, i.uv);
    //col *= i.diff;
    col *= saturate(i.diff);

    // FRESNEL calc alpha
    col.a *= i.fresnel;
    // test show normals
    //col.rgb = i.worldNormal * 0.5 + 0.5;

    // APLHA apply alpha offset
    col.a -= _Cutoff;

    // ALPHA LOD
//#if defined(LOD_FADE_CROSSFADE)
//                UnityApplyDitherCrossFade(i.vpos);
//#else
                 // note: clip HLSL instruction stops rendering a pixel if value is negative
                // FRESNEL DITHER if pixel is clipped by fresnel effect replace it by dither
                if (i.fresnel < _AlphaDither)
                {
                    // test with big checkerboard pattern
                    //screenPos.xy = floor(screenPos.xy * 0.25) * 0.5;
                    // test with pixel checkerboard pattern
                    //screenPos.xy = floor(screenPos.xy) * 0.5;

                    //float checker = -frac(screenPos.r + screenPos.g);
                    //clip(checker);

                    // query a texture with 16 dither pattern from full transparent (0) to full opaque (0.9375) in steps of 0.625
                    // the border to the non-dithered area is mapped to reach full opaque
                    float dither = tex3D(_DitherMaskLOD, float3(screenPos.xy * 0.25, i.fresnel / _AlphaDither * 0.9375)).a;
                    clip(dither - 0.01);
                }

                // ALPHA cutout discard pixel when low alpha
                //if defined(_RENDERING_CUTOUT)
                clip(col.a);
                //#endif

//#endif
                // FOG apply
                //UNITY_APPLY_FOG(i.fogCoord, col);

                return col;
            }
            ENDCG
        }

        // pull in shadow caster from VertexLit built-in shader
        // SHADOWS enable casting here, might look wrong, depends on tree
        //UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"

        /*
        // using macros from UnityCG.cginc
        Pass
        {
            Tags {"LightMode" = "ShadowCaster"}

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"

            struct v2f {
                V2F_SHADOW_CASTER;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
        */
    }

        //Fallback "Standard"
}