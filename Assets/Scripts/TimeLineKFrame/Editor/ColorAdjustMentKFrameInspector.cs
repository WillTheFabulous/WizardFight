using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[CustomEditor(typeof(ColorAdjustMentKFrame))]
public class ColorAdjustMentKFrameInspector : Editor
{
    ColorAdjustMentKFrame cak;

    SerializedProperty ifOpen;
    SerializedProperty postExposure;
    SerializedProperty contrast;
    SerializedProperty fliter;
    SerializedProperty hueShift;
    SerializedProperty saturation;

    private void OnEnable()
    {
        cak = target as ColorAdjustMentKFrame;

        ifOpen = serializedObject.FindProperty("ifOpen");
        postExposure = serializedObject.FindProperty("postExposure");
        contrast = serializedObject.FindProperty("contrast");
        fliter = serializedObject.FindProperty("fliter");
        hueShift = serializedObject.FindProperty("hueShift");
        saturation = serializedObject.FindProperty("saturation");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        ifOpen.boolValue = EditorGUILayout.Toggle("If Open ColorAdjustment", ifOpen.boolValue);

        if (cak.ifOpen == true)
        {
            EditorGUILayout.BeginVertical();
            postExposure.floatValue = EditorGUILayout.FloatField("Post Exposure", postExposure.floatValue);
            contrast.floatValue = EditorGUILayout.Slider("Contrast", contrast.floatValue, -100, 100);
            fliter.colorValue = EditorGUILayout.ColorField("Color Filter", fliter.colorValue);
            hueShift.floatValue = EditorGUILayout.Slider("Hue Shift", hueShift.floatValue, -180, 180);
            saturation.floatValue = EditorGUILayout.Slider("Saturation", saturation.floatValue, -100, 100);
            EditorGUILayout.EndVertical();
        }
        serializedObject.ApplyModifiedProperties();
        cak.Update();
    }
}
