using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SelectionQuestionInterfaceManager))]
public class SelectionQuestionInterfaceManagerEditor : Editor
{
    private SerializedProperty _startingQuestionIndexSerializedProperty;
    private int _questionToStartExamFromIndex = 0;

    private void OnEnable()
    {
        _startingQuestionIndexSerializedProperty = serializedObject.FindProperty("_startingQuestionIndex");
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

        EditorGUILayout.Space(10);

        _questionToStartExamFromIndex = EditorGUILayout.IntSlider("Question to start exam from", _questionToStartExamFromIndex, 0,
            selectionQuestionInterfaceManager.NumberOfSelectionQuestions - 1);
        if (GUILayout.Button("Restart Exam from new question"))
        {
            selectionQuestionInterfaceManager.RunExamFromQuestionIndex(_questionToStartExamFromIndex);
        }

        serializedObject.ApplyModifiedProperties();
    }
}