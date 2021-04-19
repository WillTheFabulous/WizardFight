using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[CustomEditor(typeof(IndirectLitghtingControllerKFrame))]
public class IndirectLitghtingControllerKFrameInspector : Editor
{
    IndirectLitghtingControllerKFrame ilc;

    SerializedProperty ifOpen;
    SerializedProperty diffuseLight;
    SerializedProperty reflectionLight;

    private void OnEnable()
    {
        ilc = target as IndirectLitghtingControllerKFrame;
        ifOpen = serializedObject.FindProperty("ifOpen");
        diffuseLight = serializedObject.FindProperty("diffuseLight");
        reflectionLight = serializedObject.FindProperty("reflectionLight");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        ifOpen.boolValue = EditorGUILayout.Toggle("If Open DoF", ifOpen.boolValue);

        if (ilc.ifOpen == true)
        {
            diffuseLight.floatValue = EditorGUILayout.FloatField("Diffuse Light", diffuseLight.floatValue);
            if (diffuseLight.floatValue < 0) diffuseLight.floatValue = 0;
            reflectionLight.floatValue = EditorGUILayout.FloatField("Reflection Light", reflectionLight.floatValue);
            if (reflectionLight.floatValue < 0) reflectionLight.floatValue = 0;
        }

        serializedObject.ApplyModifiedProperties();
        ilc.Update();
    }
}
