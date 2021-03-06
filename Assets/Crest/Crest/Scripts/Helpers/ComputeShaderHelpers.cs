// Crest Ocean System

// Copyright 2020 Wave Harmonic Ltd

using UnityEngine;

namespace Crest
{
    public static class ComputeShaderHelpers
    {
        public static ComputeShader LoadShader(string path)
        {
            // We provide this helper function to ensure the user gets a friendly error message in this error case
            ComputeShader computeShader = Resources.Load<ComputeShader>(path);
            Debug.Assert(computeShader != null,
                $"The shader {path} failed to load, this is likely due to an import error. Try right clicking the Crest folder in the Project view and selecting Reimport, and checking for errors.");
            return computeShader;
        }
    }
}
