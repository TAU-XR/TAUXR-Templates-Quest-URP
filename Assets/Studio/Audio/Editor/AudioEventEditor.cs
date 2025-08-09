using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioEvent))]
public class AudioEventEditor : Editor
{
    // private void OnEnable()
    // {
    //     // Initialize the embedded editor for the ScriptableObject
    //     AudioPlayer audioPlayer = (AudioPlayer)target;
    //     if (audioPlayer.exampleObject != null)
    //         CreateCachedEditor(audioPlayer.exampleObject, null, ref scriptableObjectEditor);
    // }
    private Editor _currentAudioPlayerEditor;
    private bool _isAudioPlayerSettingOpen = true;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SerializedProperty playOnProperty = serializedObject.FindProperty("_playOn");
        EditorGUILayout.PropertyField(playOnProperty);
        if (playOnProperty.enumValueIndex == 1)
        {
            DrawPropertyField("_detector");
        }

        AudioPlayer audioPlayer = (AudioPlayer)serializedObject.FindProperty("_audioPlayer").objectReferenceValue;
        DrawPropertyField("_audioPlayer");

        // HandleAudioPlayerInspector(audioPlayer);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawPropertyField(string propertyName)
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty(propertyName));
    }

    //Extract drawing scriptable object to class
    //Then use it to draw note scale
    private void HandleAudioPlayerInspector(AudioPlayer audioPlayer)
    {
        if (audioPlayer == null)
        {
            if (_currentAudioPlayerEditor != null)
            {
                DestroyImmediate(_currentAudioPlayerEditor);
            }
        }
        else
        {
            if (_currentAudioPlayerEditor == null)
            {
                _currentAudioPlayerEditor = CreateEditor(audioPlayer);
            }

            DrawAudioPlayerInspector();
        }
    }

    private void DrawAudioPlayerInspector()
    {
        EditorGUILayout.Separator();
        _isAudioPlayerSettingOpen =
            EditorGUILayout.Foldout(_isAudioPlayerSettingOpen, "Audio player settings", true, EditorStyles.foldoutHeader);

        if (!_isAudioPlayerSettingOpen) return;

        EditorGUI.indentLevel += 1;
        _currentAudioPlayerEditor.OnInspectorGUI();
        EditorGUI.indentLevel -= 0;
    }
}