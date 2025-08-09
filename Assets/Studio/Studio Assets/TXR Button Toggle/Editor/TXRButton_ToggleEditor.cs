// using UnityEngine;
// using UnityEditor;
//
// [CustomEditor(typeof(TXRButton_Toggle))]
// public class TXRButton_ToggleEditor : TXRButtonEditor
// {
//     SerializedProperty toggleOnEventProperty;
//     SerializedProperty toggleOffEventProperty;
//     SerializedProperty toggleStateProperty;
//     SerializedProperty startingStateResponseProperty;
//
//     protected override void OnEnable()
//     {
//         base.OnEnable();
//
//         toggleOnEventProperty = serializedObject.FindProperty("ToggleOn");
//         toggleOffEventProperty = serializedObject.FindProperty("ToggleOff");
//         toggleStateProperty = serializedObject.FindProperty("ToggleState");
//         startingStateResponseProperty = serializedObject.FindProperty("StartingStateResponse");
//     }
//
//     public override void OnInspectorGUI()
//     {
//         base.OnInspectorGUI();
//
//         // Draw additional properties for TXRButton_Toggle
//         EditorGUILayout.PropertyField(toggleStateProperty, new GUIContent("Toggle State"));
//         EditorGUILayout.PropertyField(toggleOnEventProperty, new GUIContent("Toggle On Event"));
//         EditorGUILayout.PropertyField(toggleOffEventProperty, new GUIContent("Toggle Off Event"));
//
//         // Apply changes to the serialized object
//         serializedObject.ApplyModifiedProperties();
//     }
// }

