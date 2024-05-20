using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SelectionQuestionInterface))]
public class SelectionQuestionInterfaceEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SelectionQuestionInterface selectionQuestionInterface = (SelectionQuestionInterface)target;

        if (GUILayout.Button("Submit and release"))
        {
            selectionQuestionInterface.SubmitButton.gameObject
                .GetComponent<TXRButton>().TriggerButtonEvent(ButtonEvent.Pressed, ButtonColliderResponse.Both);
            selectionQuestionInterface.SubmitButton.gameObject
                .GetComponent<TXRButton>().TriggerButtonEvent(ButtonEvent.Released, ButtonColliderResponse.Both);
        }
    }
}