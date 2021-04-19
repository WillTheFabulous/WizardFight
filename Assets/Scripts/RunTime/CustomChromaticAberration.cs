using System;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace UnityEngine.Rendering.Universal {
    [Serializable, VolumeComponentMenu ("CustomPP/Custom Chromatic Aberration")]
    public sealed class CustomChromaticAberration : VolumeComponent, IPostProcessComponent {
        [Tooltip ("Amount of tangential distortion.")]
        public ClampedFloatParameter _Custom_Chroma_Params = new ClampedFloatParameter (0f, 0f, 1f);
        public ClampedFloatParameter _Custom_Chroma_Lerp = new ClampedFloatParameter (0f, 0f, 1f);
        // public FloatParameter _Custom_Chroma_DistScale = new FloatParameter (0f);
        // public FloatParameter _Custom_Chroma_DistTheta = new FloatParameter (0f);
        // public FloatParameter _Custom_Chroma_DistSigma = new FloatParameter (0f);
        public Vector2Parameter _Custom_Chroma_DistCenter = new Vector2Parameter (new Vector2(0.5f, 0.5f));
        // public Vector2Parameter _Custom_Chroma_DistAxis = new Vector2Parameter (new Vector2(0f, 0f));
        public TextureParameter _ChromaticAberration_SpectralLut = new TextureParameter(null);
        public BoolParameter highQualityFiltering = new BoolParameter(false);
        public bool IsActive () => _Custom_Chroma_Params.value > 0f;

        public bool IsTileCompatible () => false;
    }
}