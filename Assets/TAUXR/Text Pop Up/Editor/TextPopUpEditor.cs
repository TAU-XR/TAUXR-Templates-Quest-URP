using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TextPopUp))]
public class TextPopUpEditor : Editor
{
    private Vector2 _textSize;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TextPopUp textPopUp = (TextPopUp)target;

        if (GUILayout.Button("Get text from component"))
        {
            textPopUp.GetTextFromComponent();
        }

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Scaling");
        if (GUILayout.Button("Set text and auto scale"))
        {
            textPopUp.SetTextAndAutoScale();
        }

        EditorGUILayout.Space(5);

        _textSize = EditorGUILayout.Vector2Field("Text size", _textSize);
        if (GUILayout.Button("Set text and scale"))
        {
            textPopUp.SetTextAndScale(_textSize);
        }
    }
}