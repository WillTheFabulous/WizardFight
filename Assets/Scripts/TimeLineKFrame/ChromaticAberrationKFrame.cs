using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEditor;

[ExecuteAlways]
public class ChromaticAberrationKFrame : MonoBehaviour
{
    public ChromaticAberration target;

    public bool ifOpen = true;
    public Texture spectralLut = null;
    public float intensity = 0;
    public ScalableSettingLevelParameter.Level quality = ScalableSettingLevelParameter.Level.High;
    public int maxSamples = 3;
    public void Update()
    {
        if (target == null)
        {
            Volume tempVolume = GetComponent<Volume>();
            foreach (VolumeComponent tempComponent in tempVolume.sharedProfile.components)
            {
                target = tempComponent as ChromaticAberration;
                if (target != null)
                {
                    break;
                }
            }
        }
        else
        {
            target.active = ifOpen;

            target.spectralLut.value = spectralLut;
            target.intensity.value = intensity;
            target.quality.value = (int)quality;
            target.maxSamples = maxSamples;
        }
    }
}
