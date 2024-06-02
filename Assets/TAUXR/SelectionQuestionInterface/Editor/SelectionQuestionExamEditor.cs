using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SelectionQuestionExam))]
public class SelectionQuestionExamEditor : Editor
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

        SelectionQuestionExam selectionQuestionExam = (SelectionQuestionExam)target;

        _startingQuestionIndexSerializedProperty.intValue =
            EditorGUILayout.IntSlider("Starting Question Index", _startingQuestionIndexSerializedProperty.intValue, 0,
                selectionQuestionExam.NumberOfSelectionQuestions - 1);

        EditorGUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Previous Question"))
        {
            selectionQuestionExam.PreviousQuestion();
        }

        if (GUILayout.Button("Next Question"))
        {
            selectionQuestionExam.NextQuestion();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        _questionToStartExamFromIndex = EditorGUILayout.IntSlider("Question to start exam from", _questionToStartExamFromIndex, 0,
            selectionQuestionExam.NumberOfSelectionQuestions - 1);
        if (GUILayout.Button("Restart Exam from new question"))
        {
            selectionQuestionExam.RunExamFromQuestionIndex(_questionToStartExamFromIndex);
        }

        serializedObject.ApplyModifiedProperties();
    }
}