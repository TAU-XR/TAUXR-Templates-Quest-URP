using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TextDisplay))]
public class TextDisplayEditor : Editor
{
    private readonly int _spacingBetweenSections = 5;
    private TextDisplay _textDisplay;
    private static int _languageToggleValue;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        _textDisplay = (TextDisplay)target;

        EditorGUILayout.Space(_spacingBetweenSections);
        AddStateSection();
        EditorGUILayout.Space(_spacingBetweenSections);
        AddLanguageSection();
    }


    private void AddStateSection()
    {
        EditorGUILayout.LabelField("Set visibility state:", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Show"))
        {
            _textDisplay.SetActiveState(true);
        }

        if (GUILayout.Button("Hide"))
        {
            _textDisplay.SetActiveState(false);
        }

        GUILayout.EndHorizontal();
    }

    private void AddLanguageSection()
    {
        EditorGUILayout.LabelField("Set language:", EditorStyles.boldLabel);
        int previousLanguageToggleValue = _languageToggleValue;
        _languageToggleValue = GUILayout.SelectionGrid(_languageToggleValue, new[] { "English", "Hebrew" }, 2, EditorStyles.radioButton);

        if (_languageToggleValue == previousLanguageToggleValue)
            return;

        Action changeLanguageMethod = _languageToggleValue == 0 ? _textDisplay.SetLanguageToEnglish : _textDisplay.SetLanguageToHebrew;
        changeLanguageMethod.Invoke();
    }
}