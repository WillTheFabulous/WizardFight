using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

[ExecuteAlways]
public class RenderParams : MonoBehaviour
{
    private bool runFlag = false;
    private Texture2D _ScatteringTex;
    [Header("Character")] [Range(0, 1)] public float GlobalCharactorIntensity; //角色全局亮度

    // Start is called before the first frame update
    [Range(1, 5)] public float GlobalCharacterShadowScale = 1; //角色全局阴影亮度
    public float GlobalOutlineMin;
    public float GlobalOutlineMax;
    public float GlobalOutline;
    public float GlobalOutlineZFactor;
    public bool GlobalCharactorReceiveShadow = true;
    public bool GlobalCharactorReceivePointLight = true;
    [Header("Scene")] [Range(0, 1)] public float GlobalSceneIntensity; //场景全局亮度
    [Header("Shadow")] public float GlobalVBias = 0.1f;
    public float GlobalShadowStrength = 0.5f;
    public Gradient GlobalShadowRamp;
    public Color ShadowSkyColor = Color.white;
    public Color ShadowMidColor = Color.white;
    public Color ShadowBottomColor = Color.white;

    private void OnEnable()
    {
        init();
    }

    public void Update()
    {
        Shader.SetGlobalFloat("_GlobalIntensity", GlobalCharactorIntensity);
        Shader.SetGlobalFloat("_GlobalCharacterShadowScale", GlobalCharacterShadowScale);
        Shader.SetGlobalFloat("_GlobalSceneIntensity", GlobalSceneIntensity <= 0f ? 0.00001f : GlobalSceneIntensity);
        Shader.SetGlobalFloat("_GlobalOutlineMin", GlobalOutlineMin);
        Shader.SetGlobalFloat("_GlobalOutlineMax", GlobalOutlineMax);
        Shader.SetGlobalFloat("_GlobalOutline", GlobalOutline);
        Shader.SetGlobalFloat("_GlobalOutlineZFactor", GlobalOutlineZFactor);
        Shader.SetGlobalFloat("_vBias", GlobalVBias);
        Shader.SetGlobalFloat("_ShadowStrength", GlobalShadowStrength);
        Shader.SetGlobalColor("_ShadowColor", ShadowSkyColor);
        Shader.SetGlobalColor("_ShadowColorLeft", ShadowMidColor);
        Shader.SetGlobalColor("_ShadowColorBottom", ShadowBottomColor);


        if (GlobalCharactorReceivePointLight)
            Shader.SetGlobalFloat("_ExtraPointLight", 1);
        else
        {
            Shader.SetGlobalFloat("_ExtraPointLight", 0);
        }

        if (GlobalCharactorReceiveShadow)
            Shader.SetGlobalFloat("_CharReceiveShadow", 1);
        else
        {
            Shader.SetGlobalFloat("_CharReceiveShadow", 0);
        }
    }

    private void init()
    {
#if UNITY_EDITOR
        Application.targetFrameRate = -1;
#endif
        Shader.SetGlobalFloat("_GlobalIntensity", GlobalCharactorIntensity);
        Shader.SetGlobalFloat("_GlobalCharacterShadowScale", GlobalCharacterShadowScale);
        Shader.SetGlobalFloat("_GlobalSceneIntensity", GlobalSceneIntensity <= 0f ? 0.00001f : GlobalSceneIntensity);
        Shader.SetGlobalFloat("_GlobalOutlineMin", GlobalOutlineMin);
        Shader.SetGlobalFloat("_GlobalOutlineMax", GlobalOutlineMax);
        Shader.SetGlobalFloat("_GlobalOutline", GlobalOutline);
        Shader.SetGlobalFloat("_GlobalOutlineZFactor", GlobalOutlineZFactor);
        Shader.SetGlobalFloat("_vBias", GlobalVBias);
        Shader.SetGlobalFloat("_ShadowStrength", GlobalShadowStrength);
        Shader.SetGlobalColor("_ShadowColor", ShadowSkyColor);
        Shader.SetGlobalColor("_ShadowColorLeft", ShadowMidColor);
        Shader.SetGlobalColor("_ShadowColorBottom", ShadowBottomColor);


        if (GlobalCharactorReceivePointLight)
            Shader.SetGlobalFloat("_ExtraPointLight", 1);
        else
        {
            Shader.SetGlobalFloat("_ExtraPointLight", 0);
        }

        if (GlobalCharactorReceiveShadow)
            Shader.SetGlobalFloat("_CharReceiveShadow", 1);
        else
        {
            Shader.SetGlobalFloat("_CharReceiveShadow", 0);
        }

 
        generateRampTex();
    }

    // Start is called before the first frame update
    private void OnDestroy()
    {
        _ScatteringTex = null;
    }


    // Update is called once per frame


    private void generateRampTex()
    {
        _ScatteringTex = new Texture2D(256, 1, GraphicsFormat.R8G8B8A8_SRGB, TextureCreationFlags.None);
        _ScatteringTex.wrapMode = TextureWrapMode.Clamp;
        var cols = new Color[256];
        for (var i = 0; i < 256; i++)
        {
            cols[i] = GlobalShadowRamp.Evaluate(i / 256f);
        }

        _ScatteringTex.SetPixels(cols);
        _ScatteringTex.Apply();


        Shader.SetGlobalTexture("_ShadowRamp", _ScatteringTex);
    }
}