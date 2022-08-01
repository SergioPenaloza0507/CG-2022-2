using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PROPS = ParticlesUberPropertyHolder;

public class ParticlesUberCustomEditor : ShaderGUI
{
    enum TextureAnimationMode
    {
        Time,
        CustomVertexStreams,
        TimePeriodic
    }

    enum AlphaMaskingMode
    {
        None,
        UV,
        Texture,
        Position
    }

    enum EmissionMode
    {
        Material,
        CustomVertexStreams
    }

	enum GeneralBlendMode
	{
		AlphaBlend,
		AlphaPremultiply,
		Additive,
		SoftAdditive,
		Multiplicative,
		Multiplicative2x
	}

    #region Block View Controls
    bool showBlendingOptions;
    bool showColorTintOptions;
    bool showMainTextureOptions;
    bool showMainTextureDistortionOptions;
    bool showAlphaMaskingOptions;
    bool showDistortionOptions;
    #endregion

    private GeneralBlendMode generalBlendMode;
    private bool blendsAreDirtyFromEditor;

    void GuiLine(int i_height = 1)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, i_height);

        rect.height = i_height;

        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));

    }

    string GetCommonBlendModeFromSurceAndDestinationValues(float src, float dest)
    {
        if (src == 5 && dest == 10)
        {
            return "Alpha blend";
        }

        if (src == 1 && dest == 10)
        {
            return "Alpha Premultiply";
        }

        if (src == 1 && dest == 1)
        {
            return "Additive";
        }

        if (src == 4 && dest == 1)
        {
            return "Sfot Additive";
        }

        if (src == 2 && dest == 0)
        {
            return "Multiply";
        }

        if (src == 2 && dest == 3)
        {
            return "Multiply 2X";
        }
        return "Custom";
    }

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        GuiLine();
        showBlendingOptions = EditorGUILayout.Foldout(showBlendingOptions, "Blend");
        if (showBlendingOptions)
        {
            generalBlendMode = (GeneralBlendMode)EditorGUILayout.EnumPopup(generalBlendMode);
            float sourceBlendMode = 0;
            float blendOperation = 0;
            float destinationBlendMode = 0;
            if (GUILayout.Button("Setup Common Blend Mode"))
            {
                switch (generalBlendMode)
                {
                    case GeneralBlendMode.AlphaBlend:
                        sourceBlendMode = 5;
                        destinationBlendMode = 10;
                        break;
                    case GeneralBlendMode.AlphaPremultiply:
                        sourceBlendMode = 1;
                        destinationBlendMode = 10;
                        break;
                    case GeneralBlendMode.Additive:
                        sourceBlendMode = 1;
                        destinationBlendMode = 1;
                        break;
                    case GeneralBlendMode.SoftAdditive:
                        sourceBlendMode = 4;
                        destinationBlendMode = 1;
                        break;
                    case GeneralBlendMode.Multiplicative:
                        sourceBlendMode = 2;
                        destinationBlendMode = 0;
                        break;
                    case GeneralBlendMode.Multiplicative2x:
                        sourceBlendMode = 2;
                        destinationBlendMode = 3;
                        break;
                    default:
                        break;
                }

                blendsAreDirtyFromEditor = true;
            }
            
            
            //Draw soft particles and blending Block
            var _SrcBlend = FindProperty(PROPS.SRC_BLEND, properties);
            var _BlendOperation = FindProperty(PROPS.BLEND_OPERATION, properties);
            var _DstBlend = FindProperty(PROPS.DST_BLEND, properties);
            var _SoftParticles = FindProperty(PROPS.SOFT_PARTICLES, properties);
            var _SoftParticlesThreshold = FindProperty(PROPS.SOFT_PARTICLES_THRESHOLD, properties);
            var _SoftParticlesFalloff = FindProperty(PROPS.SOFT_PARTICLES_FALLOFF, properties);

            if (blendsAreDirtyFromEditor)
            {
                _SrcBlend.floatValue = sourceBlendMode;
                _DstBlend.floatValue = destinationBlendMode;
                blendsAreDirtyFromEditor = false;
            }
            EditorGUILayout.HelpBox($"Current Blend Mode: {GetCommonBlendModeFromSurceAndDestinationValues(_SrcBlend.floatValue, _DstBlend.floatValue)}", MessageType.Info);
            materialEditor.ShaderProperty(_SrcBlend, new GUIContent(_SrcBlend.displayName, "Source factor in equation:\n\nfinalValue = sourceFactor * sourceValue operation destinationFactor * destinationValue"));
            materialEditor.ShaderProperty(_BlendOperation, new GUIContent(_BlendOperation.displayName, "Blend Operation\n\nIf the blending operation is Add, Sub, RevSub, Min, or Max, the GPU multiplies the value of the output of the fragment shader by the source factor.\n\n" +
            "If the blending operation is Add, Sub, RevSub, Min, or Max, the GPU multiplies the value that is already in the render target by the destination factor."));
            materialEditor.ShaderProperty(_DstBlend, new GUIContent(_DstBlend.displayName, "Destination factor in equation:\n\nfinalValue = sourceFactor * sourceValue operation destinationFactor * destinationValue"));

            materialEditor.ShaderProperty(_SoftParticles, new GUIContent(_SoftParticles.displayName, "Enables soft particles"));
            if (_SoftParticles.floatValue == 1)
            {
                materialEditor.ShaderProperty(_SoftParticlesThreshold, new GUIContent(_SoftParticlesThreshold.displayName, "Eye space value where the depth fade starts"));
                materialEditor.ShaderProperty(_SoftParticlesFalloff, new GUIContent(_SoftParticlesFalloff.displayName, "Eye space offset value where the depth fade stops relative to the threshold value"));
            }
        }
        GuiLine();
        showColorTintOptions = EditorGUILayout.Foldout(showColorTintOptions, "Color Tint");
        if (showColorTintOptions)
        {
            //Draw color tint Block
            var _Color = FindProperty(PROPS.COLOR, properties);
            var _EmissionMode = FindProperty(PROPS.EMISSION_MODE, properties);
            var _EmissionColor = FindProperty(PROPS.EMISSION_COLOR, properties);
            var _AnimatedEmissionMap = FindProperty(PROPS.ANIMATED_EMISSION_MAP, properties);
            var _EmissionMap = FindProperty(PROPS.EMISSION_MAP, properties);

            materialEditor.ShaderProperty(_Color, new GUIContent(_Color.displayName, "Main color tint applied to overall color result"));
            materialEditor.ShaderProperty(_EmissionMode, new GUIContent(_EmissionMode.displayName, "Chooses emission color mode:\nMaterial - uses the property from material inspector\nCustom vertex streams - uses UV channel 2 coordinates from mesh as emission color"));
            switch ((EmissionMode)_EmissionMode.floatValue)
            {
                case EmissionMode.Material:
                    materialEditor.ShaderProperty(_EmissionColor, new GUIContent(_EmissionColor.displayName, "HDR color value added to the final color result"));
                    break;
                default:
                    break;
            }
            materialEditor.ShaderProperty(_AnimatedEmissionMap, new GUIContent(_AnimatedEmissionMap.displayName, "Choose whether you want the emission map to be animated by the custom vertex streams (TEXCOORD0.zw)"));
            materialEditor.ShaderProperty(_EmissionMap, _EmissionMap.displayName);
            //materialEditor.TextureScaleOffsetProperty(_EmissionMap);
        }
        GuiLine();
        showMainTextureOptions = EditorGUILayout.Foldout(showMainTextureOptions, "Main Texture Parameters");
        if (showMainTextureOptions)
        {

            //Draw Main texture block
            var _MainTex = FindProperty(PROPS.MAIN_TEX, properties);
            var _ClampMainTex = FindProperty(PROPS.CLAMP_MAIN_TEX, properties);
            var _MainTexAnimMode = FindProperty(PROPS.MAIN_TEX_ANIMATION_MODE, properties);
            var _MainTexAnimParams = FindProperty(PROPS.MAIN_TEX_ANIMATION_PARAMETERS, properties);

            materialEditor.ShaderProperty(_MainTex, _MainTex.displayName);
            materialEditor.TextureScaleOffsetProperty(_MainTex);
            materialEditor.ShaderProperty(_ClampMainTex, new GUIContent(_ClampMainTex.displayName, "Choose whether the texture should not be repeated when animated"));
            materialEditor.ShaderProperty(_MainTexAnimMode, new GUIContent(_MainTexAnimMode.displayName, "Choose which animation mode to use when animating the main texture:\n\nTime - Uses a Vector2 velocity value scaled by time\n\nCustomVertexStreams - Uses Mesh UV first channels Z and W components scaled by time\n\nTimePeriodic - Uses a Vector2 value where x=frequency and y=amplitude scaled by sin(time)"));

            switch ((TextureAnimationMode)_MainTexAnimMode.floatValue)
            {
                case TextureAnimationMode.Time:
                case TextureAnimationMode.TimePeriodic:
                    materialEditor.ShaderProperty(_MainTexAnimParams, _MainTexAnimParams.displayName);
                    break;
                default:
                    break;
            }

            EditorGUI.indentLevel+=2;
            showMainTextureDistortionOptions = EditorGUILayout.Foldout(showMainTextureDistortionOptions, "Main Texture Distortion");

            if (showMainTextureDistortionOptions)
            {
                //Draw Main texture distortion block
                var _MainTexTwirlParams = FindProperty(PROPS.MAIN_TEX_TWIRL_PARAMETERS, properties);
                var _MainTexTwirlStrength = FindProperty(PROPS.MAIN_TEX_TWIRL_STRENGTH, properties);
                var _MainTexDistortNoise = FindProperty(PROPS.MAIN_TEX_DISTORT_NOISE, properties);
                var _MainTexDistortNoiseAnimMode = FindProperty(PROPS.MAIN_TEX_DISTORTION_NOISE_ANIMATION_MODE, properties);
                var _MainTexDistortAnimParams = FindProperty(PROPS.MAIN_TEX_DISTORTION_ANIMATION_PARAMETERS, properties);
                var _MainTexDistortStrength = FindProperty(PROPS.MAIN_TEX_DISTORTION_STRENGTH, properties);

                materialEditor.ShaderProperty(_MainTexTwirlParams, _MainTexTwirlParams.displayName);
                materialEditor.ShaderProperty(_MainTexTwirlStrength, _MainTexTwirlStrength.displayName);
                materialEditor.ShaderProperty(_MainTexDistortNoise, _MainTexDistortNoise.displayName);
                materialEditor.TextureScaleOffsetProperty(_MainTexDistortNoise);
                materialEditor.ShaderProperty(_MainTexDistortNoiseAnimMode, _MainTexDistortNoiseAnimMode.displayName);
                switch ((TextureAnimationMode)_MainTexDistortNoiseAnimMode.floatValue)
                {
                    case TextureAnimationMode.Time:
                    case TextureAnimationMode.TimePeriodic:
                        materialEditor.ShaderProperty(_MainTexDistortAnimParams, _MainTexDistortAnimParams.displayName);
                        break;
                    default:
                        break;
                }
                materialEditor.ShaderProperty(_MainTexDistortStrength, _MainTexDistortStrength.displayName);
            }
            EditorGUI.indentLevel-=2;
        }
        GuiLine();
        showAlphaMaskingOptions = EditorGUILayout.Foldout(showAlphaMaskingOptions, "Alpha Masking");
        if (showAlphaMaskingOptions)
        {
            //Draw alpha masking block
            var _AlphaMaskingMode = FindProperty(PROPS.ALPHA_MASKING_MODE, properties);
            var _AlphaMaskDirection = FindProperty(PROPS.ALPHA_MASK_DIRECTION, properties);
            var _AlphaMask = FindProperty(PROPS.ALPHA_MASK, properties);
            var _AlphaMaskAnimMode = FindProperty(PROPS.ALPHA_MASK_ANIMATION_MODE, properties);
            var _AlphaMaskAnimParams = FindProperty(PROPS.ALPHA_MASK_ANIMATION_PARAMETERS, properties);

            materialEditor.ShaderProperty(_AlphaMaskingMode, _AlphaMaskingMode.displayName);
            switch ((AlphaMaskingMode)_AlphaMaskingMode.floatValue)
            {
                case AlphaMaskingMode.UV:
                case AlphaMaskingMode.Position:
                    materialEditor.ShaderProperty(_AlphaMaskDirection, _AlphaMaskDirection.displayName);
                    break;
                case AlphaMaskingMode.Texture:
                    materialEditor.ShaderProperty(_AlphaMask, _AlphaMask.displayName);
                    materialEditor.TextureScaleOffsetProperty(_AlphaMask);
                    materialEditor.ShaderProperty(_AlphaMaskAnimMode, _AlphaMaskAnimMode.displayName);
                    materialEditor.ShaderProperty(_AlphaMaskAnimParams, new GUIContent(_AlphaMaskAnimParams.displayName, "Alpha mask animation parameters where:\nX = UV speed X\nY = UV speed Y\nZ = Periodic frequency\nW = Periodic amplitude"));
                    break;
                default:
                    break;
            }
        }
        GuiLine();
        showDistortionOptions = EditorGUILayout.Foldout(showDistortionOptions, "Distortion");
        if (showDistortionOptions)
        {
            //Draw background distortion block
            var _DistortionToggle = FindProperty(PROPS.DISTORTION_TOGGLE, properties);
            var _DistortMap = FindProperty(PROPS.DISTORT_MAP, properties);
            var _DistortionBlend = FindProperty(PROPS.DISTORTION_BLEND, properties);
            var _DistortionStrength = FindProperty(PROPS.DISTORTION_STRENGTH, properties);

            materialEditor.ShaderProperty(_DistortionToggle, _DistortionToggle.displayName);
            if (_DistortionToggle.floatValue >= 1)
            {
                materialEditor.ShaderProperty(_DistortMap, _DistortMap.displayName);
                materialEditor.TextureScaleOffsetProperty(_DistortMap);
                materialEditor.ShaderProperty(_DistortionBlend, _DistortionBlend.displayName);
                materialEditor.ShaderProperty(_DistortionStrength, _DistortionStrength.displayName);
            }
        }
        GuiLine();
    }
}
