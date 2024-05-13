using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TXRButton))]
public class TXRButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TXRButton button = (TXRButton)target;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Press"))
        {
            button.TriggerButtonEvent(ButtonEvent.Pressed, ButtonColliderResponse.Both);
        }

        if (GUILayout.Button("Release"))
        {
            button.TriggerButtonEvent(ButtonEvent.Released, ButtonColliderResponse.Both);
        }

        EditorGUILayout.EndHorizontal();
    }
}