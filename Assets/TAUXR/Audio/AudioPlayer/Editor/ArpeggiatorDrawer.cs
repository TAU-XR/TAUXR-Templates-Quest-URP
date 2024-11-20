using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Arpeggiator))]
public class ArpeggiatorDrawer : PropertyDrawer
{
    [SerializeField] private MusicScale _musicScale;

    [SerializeField] private bool _isSingleOctave;

    [SerializeField] private float _timeBetweenResets = 0.5f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUILayout.Space(-22);
        EditorGUI.indentLevel += 1;
        EditorGUILayout.PropertyField(property.FindPropertyRelative("_musicScale"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative("_isSingleOctave"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative("_timeBetweenResets"));
        EditorGUI.indentLevel -= 1;
    }
}