using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEditor;

[ExecuteAlways]
public class BloomKFrame : MonoBehaviour
{
    public Bloom target;

    public bool ifOpen = true;
    public ScalableSettingLevelParameter.Level quality = ScalableSettingLevelParameter.Level.High;
    public float threshold = 0.51f;
    public float intensity = 0.246f;
    public float scatter = 0.183f;
    public Color tint = Color.white;
    public Texture texture = null;
    public float lensintensity = 0;

    public void Update()
    {
        if (target == null)
        {
            Volume tempVolume = GetComponent<Volume>();
            foreach (VolumeComponent tempComponent in tempVolume.sharedProfile.components)
            {
                target = tempComponent as Bloom;
                if (target != null)
                {
                    break;
                }
            }
        }
        else
        {
            target.active = ifOpen;
            target.quality.value = (int)quality;
            target.threshold.value = threshold;
            target.intensity.value = intensity;
            target.dirtIntensity.value = lensintensity;
            target.scatter.value = scatter;
            target.tint.value = tint;
            target.dirtTexture.value = texture;
        }
    }
}