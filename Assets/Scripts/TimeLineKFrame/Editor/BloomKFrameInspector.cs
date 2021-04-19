using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[CustomEditor(typeof(BloomKFrame))]
public class BloomKFrameInspector : Editor
{
    BloomKFrame bkf;

    SerializedProperty ifOpen;
    SerializedProperty quality;
    SerializedProperty threshold;
    SerializedProperty intensity;
    SerializedProperty scatter;
    SerializedProperty tint;
    SerializedProperty texture;
    SerializedProperty lensintensity;

    private void OnEnable()
    {
        bkf = target as BloomKFrame;

        ifOpen = serializedObject.FindProperty("ifOpen");
        quality = serializedObject.FindProperty("quality");
        threshold = serializedObject.FindProperty("threshold");
        intensity = serializedObject.FindProperty("intensity");
        scatter = serializedObject.FindProperty("scatter");
        tint = serializedObject.FindProperty("tint");
        texture = serializedObject.FindProperty("texture");
        lensintensity = serializedObject.FindProperty("lensintensity");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        ifOpen.boolValue = EditorGUILayout.Toggle("If Open Bloom", ifOpen.boolValue);
        if (bkf.ifOpen == true)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.PropertyField(quality, new GUIContent("Quality"));

            GUILayout.Label("Bloom");
            threshold.floatValue = EditorGUILayout.FloatField("Threshold", threshold.floatValue);
            intensity.floatValue = EditorGUILayout.Slider("Intensity", intensity.floatValue, 0, 1);
            scatter.floatValue = EditorGUILayout.Slider("Scatter", scatter.floatValue, 0, 1);
            tint.colorValue = EditorGUILayout.ColorField("Tint", tint.colorValue);

            GUILayout.Label("Lens Dirt");
            EditorGUILayout.PropertyField(texture, new GUIContent("Texture"));

            lensintensity.floatValue = EditorGUILayout.FloatField("Intensity", lensintensity.floatValue);

            EditorGUILayout.EndVertical();
        }
        bkf.Update();
        serializedObject.ApplyModifiedProperties();
    }
}
