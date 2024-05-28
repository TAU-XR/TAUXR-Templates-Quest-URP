using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SelectionQuestionInterfaceManager))]
public class SelectionQuestionInterfaceManagerEditor : Editor
{
    private SerializedProperty _startingQuestionIndexSerializedProperty;
    private SerializedProperty _currentQuestionIndexSerializedProperty;

    private void OnEnable()
    {
        _startingQuestionIndexSerializedProperty = serializedObject.FindProperty("_startingQuestionIndex");
        _currentQuestionIndexSerializedProperty = serializedObject.FindProperty("_currentQuestionIndex");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        SelectionQuestionInterfaceManager selectionQuestionInterfaceManager = (SelectionQuestionInterfaceManager)target;

        _startingQuestionIndexSerializedProperty.intValue =
            EditorGUILayout.IntSlider("Starting Question Index", _startingQuestionIndexSerializedProperty.intValue, 0,
                selectionQuestionInterfaceManager.NumberOfSelectionQuestions - 1);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Previous Question"))
        {
            selectionQuestionInterfaceManager.PreviousQuestion();
        }

        if (GUILayout.Button("Next Question"))
        {
            selectionQuestionInterfaceManager.NextQuestion();
        }

        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}