using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEditor;

[ExecuteAlways]
public class DepthofFieldKFrame : MonoBehaviour
{
    public DepthOfField target;

    public bool ifOpen = true;
    public DepthOfFieldMode focusMode = DepthOfFieldMode.Manual;
    public float nearRangeStart = 0;
    public float focusDistance = 100;
    public float nearRangeEnd = 0;
    public float farRangeStart = 0;
    public float farRangeEnd = 0;
    public ScalableSettingLevelParameter.Level quality = ScalableSettingLevelParameter.Level.High;
    public int nearBlurCount = 0;
    public float nearRadius = 0;
    public int farBlurCount = 0;
    public float farRadius = 0;

    public void Update()
    {
        if (target == null)
        {
            Volume tempVolume = GetComponent<Volume>();
            foreach (VolumeComponent tempComponent in tempVolume.sharedProfile.components)
            {
                target = tempComponent as DepthOfField;
                if (target != null)
                {
                    break;
                }
            }
        }
        else
        {
            target.active = ifOpen;

            target.focusMode.value = focusMode;
            target.nearFocusStart.value = nearRangeStart;
            target.nearFocusEnd.value = nearRangeEnd;
            target.farFocusStart.value = farRangeStart;
            target.farFocusEnd.value = farRangeEnd;
            target.focusDistance.value = focusDistance;
            target.quality.value = (int)quality;
            target.nearSampleCount = nearBlurCount;
            target.nearMaxBlur = nearRadius;
            target.farSampleCount = farBlurCount;
            target.farMaxBlur = farRadius;

        }
    }

}
