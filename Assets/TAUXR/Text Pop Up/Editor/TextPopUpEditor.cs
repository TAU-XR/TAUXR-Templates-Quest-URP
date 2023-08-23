using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TextPopUp))]
public class TextPopUpEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TextPopUp textPopUp = target as TextPopUp;
        GUILayout.Label("write your text and then press set new scale.");
        if (GUILayout.Button("Set new scale"))
        {
            textPopUp.SetNewScale();
        }

        if (GUILayout.Button("Reset scale"))
        {
            textPopUp.ResetScale();
        }

        if (GUILayout.Button("Debug number of words"))
        {
            textPopUp.DebugNumberOfWords();
        }
    }
}