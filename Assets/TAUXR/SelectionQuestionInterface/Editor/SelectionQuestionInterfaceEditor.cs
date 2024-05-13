using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SelectionQuestionInterface))]
public class SelectionQuestionInterfaceEditor : Editor
{
    [Range(0, 3)] private static int _buttonIndex = 0;
    private static bool _isEditorUtilitiesFoldoutGroupOpen = true;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SelectionQuestionInterface selectionQuestionInterface = (SelectionQuestionInterface)target;

        EditorGUILayout.Space(10);
        _isEditorUtilitiesFoldoutGroupOpen =
            EditorGUILayout.Foldout(_isEditorUtilitiesFoldoutGroupOpen, "Editor Utilities", EditorStyles.foldoutHeader);

        if (!_isEditorUtilitiesFoldoutGroupOpen) return;

        int numberOfButtons =
            selectionQuestionInterface.SelectionQuestionInterfaceReferences.SelectionAnswers.Count(selectionAnswer =>
                selectionAnswer.gameObject.activeSelf);
        _buttonIndex = EditorGUILayout.IntSlider(_buttonIndex, 0, numberOfButtons - 1);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Select Button"))
        {
            selectionQuestionInterface.SelectionQuestionInterfaceReferences.SelectionAnswers[_buttonIndex].gameObject
                .GetComponent<TXRButton_Toggle>().TriggerToggleEvent(TXRButtonToggleState.On, ButtonColliderResponse.Both);
        }

        if (GUILayout.Button("Deselect Button"))
        {
            selectionQuestionInterface.SelectionQuestionInterfaceReferences.SelectionAnswers[_buttonIndex].gameObject
                .GetComponent<TXRButton_Toggle>().TriggerToggleEvent(TXRButtonToggleState.Off, ButtonColliderResponse.Both);
        }

        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Submit and release"))
        {
            selectionQuestionInterface.SelectionQuestionInterfaceReferences.SubmitButton.gameObject
                .GetComponent<TXRButton>().TriggerButtonEvent(ButtonEvent.Pressed, ButtonColliderResponse.Both);
            selectionQuestionInterface.SelectionQuestionInterfaceReferences.SubmitButton.gameObject
                .GetComponent<TXRButton>().TriggerButtonEvent(ButtonEvent.Released, ButtonColliderResponse.Both);
        }
    }
}