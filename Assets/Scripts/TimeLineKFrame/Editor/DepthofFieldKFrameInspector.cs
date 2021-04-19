using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[CustomEditor(typeof(DepthofFieldKFrame))]
public class DepthofFieldKFrameInspector : Editor
{
    DepthofFieldKFrame dof;

    SerializedProperty ifOpen;
    SerializedProperty focusMode;
    SerializedProperty nearRangeStart;
    SerializedProperty focusDistance;
    SerializedProperty nearRangeEnd;
    SerializedProperty farRangeStart;
    SerializedProperty farRangeEnd;
    SerializedProperty quality;
    SerializedProperty nearBlurCount;
    SerializedProperty nearRadius;
    SerializedProperty farBlurCount;
    SerializedProperty farRadius;


    private void OnEnable()
    {
        dof = target as DepthofFieldKFrame;
        ifOpen = serializedObject.FindProperty("ifOpen");
        focusMode = serializedObject.FindProperty("focusMode");
        nearRangeStart = serializedObject.FindProperty("nearRangeStart");
        focusDistance = serializedObject.FindProperty("focusDistance");
        nearRangeEnd = serializedObject.FindProperty("nearRangeEnd");
        farRangeStart = serializedObject.FindProperty("farRangeStart");
        farRangeEnd = serializedObject.FindProperty("farRangeEnd");
        quality = serializedObject.FindProperty("quality");
        nearBlurCount = serializedObject.FindProperty("nearBlurCount");
        nearRadius = serializedObject.FindProperty("nearRadius");
        farBlurCount = serializedObject.FindProperty("farBlurCount");
        farRadius = serializedObject.FindProperty("farRadius");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        ifOpen.boolValue = EditorGUILayout.Toggle("If Open DoF", ifOpen.boolValue);

        if (dof.ifOpen == true)
        {
            EditorGUILayout.PropertyField(focusMode,new GUIContent("Focus Mode"));

            if (dof.focusMode != DepthOfFieldMode.Off)
            {
                if (dof.focusMode == DepthOfFieldMode.Manual)
                {
                    GUILayout.Label("Near Range");
                    nearRangeStart.floatValue = EditorGUILayout.FloatField("Start", nearRangeStart.floatValue);
                    nearRangeEnd.floatValue = EditorGUILayout.FloatField("End", nearRangeEnd.floatValue);

                    GUILayout.Label("Far Range");
                    farRangeStart.floatValue = EditorGUILayout.FloatField("Start", farRangeStart.floatValue);
                    farRangeEnd.floatValue = EditorGUILayout.FloatField("End", farRangeEnd.floatValue);
                }
                else if (dof.focusMode == DepthOfFieldMode.UsePhysicalCamera)
                {
                    focusDistance.floatValue = EditorGUILayout.FloatField("Focus Distance", focusDistance.floatValue);
                }
                EditorGUILayout.PropertyField(quality, new GUIContent("Quality"));

                GUILayout.Label("Near Blur");
                nearBlurCount.intValue = EditorGUILayout.IntSlider("Sample Count", nearBlurCount.intValue, 3, 8);
                nearRadius.floatValue = EditorGUILayout.Slider("MAx Radius", nearRadius.floatValue, 0, 8);

                GUILayout.Label("Far Blur");
                farBlurCount.intValue = EditorGUILayout.IntSlider("Sample Count", farBlurCount.intValue, 3, 16);
                farRadius.floatValue = EditorGUILayout.Slider("MAx Radius", farRadius.floatValue, 0, 16);
            }
        }
        serializedObject.ApplyModifiedProperties();
        dof.Update();
    }
}