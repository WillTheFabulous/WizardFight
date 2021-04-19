using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[CustomEditor(typeof(ChromaticAberrationKFrame))]
public class ChromaticAberrationKFrameInspector : Editor
{
    ChromaticAberrationKFrame cak;

    SerializedProperty ifOpen;
    SerializedProperty spectralLut;
    SerializedProperty intensity;
    SerializedProperty quality;
    SerializedProperty maxSamples;

    private void OnEnable()
    {
        cak = target as ChromaticAberrationKFrame;
        ifOpen = serializedObject.FindProperty("ifOpen");
        spectralLut = serializedObject.FindProperty("spectralLut");
        intensity = serializedObject.FindProperty("intensity");
        quality = serializedObject.FindProperty("quality");
        maxSamples = serializedObject.FindProperty("maxSamples");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        ifOpen.boolValue = EditorGUILayout.Toggle("If Open CA", ifOpen.boolValue);
        if (cak.ifOpen == true)
        {
            EditorGUILayout.PropertyField(spectralLut, new GUIContent("Focus Mode"));
            intensity.floatValue = EditorGUILayout.Slider("Intensity", intensity.floatValue, 0, 1);
            EditorGUILayout.PropertyField(quality, new GUIContent("Quality"));
            maxSamples.intValue = EditorGUILayout.IntSlider("Spectral Lut", maxSamples.intValue,3, 24);
        }
        serializedObject.ApplyModifiedProperties();
        cak.Update();
    }
}
