using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SelectionQuestionInterface))]
public class SelectionQuestionInterfaceEditor : Editor
{
    [Range(0, 3)] private static int _buttonIndex = 0;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SelectionQuestionInterface selectionQuestionInterface = (SelectionQuestionInterface)target;

        EditorGUILayout.Space(10);
        _buttonIndex = EditorGUILayout.IntSlider(_buttonIndex, 0, 3);

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