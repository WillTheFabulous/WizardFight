using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[ExecuteAlways]
public class IndirectLitghtingControllerKFrame : MonoBehaviour
{
    public IndirectLightingController target = null;
    public bool ifOpen = true;
    public float diffuseLight = 1.0f;
    public float reflectionLight = 1.0f;

    public void Update()
    {
        if (target == null)
        {
            Volume tempVolume = GetComponent<Volume>();
            foreach (VolumeComponent tempComponent in tempVolume.sharedProfile.components)
            {
                target = tempComponent as IndirectLightingController;
                if (target != null)
                {
                    break;
                }
            }
        }
        else
        {
            target.active = ifOpen;

            target.indirectDiffuseLightingMultiplier.value = diffuseLight;
            target.reflectionLightingMultiplier.value = reflectionLight;

        }
    }
}
