using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEditor;

[ExecuteAlways]
public class ColorAdjustMentKFrame : MonoBehaviour
{
    public ColorAdjustments target = null;
    public bool ifOpen = true;
    public float postExposure = 0;
    public float contrast = 0;
    public Color fliter = Color.white;
    public float hueShift = 0;
    public float saturation = 0;

    public void Update()
    {
        if(target == null)
        {
            Volume tempVolume = GetComponent<Volume>();
            foreach (VolumeComponent tempComponent in tempVolume.sharedProfile.components)
            {
                target = tempComponent as ColorAdjustments;
                if (target != null)
                {
                    break;
                }
            }
        }
        else
        {
            target.active = ifOpen;
            target.postExposure.value = postExposure;
            target.contrast.value = contrast;
            target.colorFilter.value = fliter;
            target.hueShift.value = hueShift;
            target.saturation.value = saturation;
        }
    }
}
