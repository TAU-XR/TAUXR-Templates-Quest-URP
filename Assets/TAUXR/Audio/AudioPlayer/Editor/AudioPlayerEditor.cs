using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioPlayer))]
[CanEditMultipleObjects]
public class AudioPlayerEditor : Editor
{
    [SerializeField] private AudioSource _previewer;
    private int _selectedTab = 0;
    private Color _guiBackgroundColor;

    public void OnEnable()
    {
        _previewer = EditorUtility.CreateGameObjectWithHideFlags("Audio preview", HideFlags.HideAndDontSave, typeof(AudioSource))
            .GetComponent<AudioSource>();
        AudioPlayer audioPlayer = target as AudioPlayer;
        if (audioPlayer != null)
        {
            audioPlayer.Reset();
        }
    }

    public void OnDisable()
    {
        DestroyImmediate(_previewer.gameObject);
    }

    public override void OnInspectorGUI()
    {
        // DrawDefaultInspector();
        serializedObject.Update();

        _selectedTab = GUILayout.Toolbar(_selectedTab, new string[] { "Basic", "Advanced" });
        EditorGUILayout.Separator();

        if (_selectedTab == 0)
        {
            DrawBasicSection();
        }
        else
        {
            DrawAdvancedSection();
        }


        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        ShowPreviewButtons();

        bool hasModifiedProperties = serializedObject.hasModifiedProperties;
        serializedObject.ApplyModifiedProperties();

        if (hasModifiedProperties) ((AudioPlayer)target).Reset();
    }

    private void DrawBasicSection()
    {
        DrawPropertyField("_audioClips");
        if (serializedObject.FindProperty("_audioClips").arraySize > 1)
        {
            DrawPropertyField("_playOrder");
        }

        EditorGUILayout.Separator();
        DrawPropertyField("_loop");
        EditorGUILayout.Separator();
        DrawVolumeFields();
    }

    private void DrawPropertyField(string propertyName)
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty(propertyName));
    }

    private void DrawVolumeFields()
    {
        EditorGUILayout.BeginVertical("box");
        SerializedProperty dynamicVolumeRangeProperty = serializedObject.FindProperty("_dynamicVolumeRange");
        if (dynamicVolumeRangeProperty.boolValue)
        {
            ShowMinMaxSlider(serializedObject.FindProperty("_volumeRange"), "Volume", 0, 1);
        }
        else
        {
            serializedObject.FindProperty("_volume").floatValue =
                EditorGUILayout.Slider("Volume", serializedObject.FindProperty("_volume").floatValue, 0, 1);
        }

        EditorGUILayout.PropertyField(dynamicVolumeRangeProperty);
        EditorGUILayout.EndVertical();
    }

    //TODO: refactor and improve
    private void ShowMinMaxSlider(SerializedProperty vectorProperty, string label, float min, float max)
    {
        Vector2 vectorValue = vectorProperty.vector2Value;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label, GUILayout.MaxWidth(120));
        GUILayout.FlexibleSpace();

        vectorValue.x = EditorGUILayout.FloatField((float)System.Math.Round(vectorValue.x, 3), GUILayout.Width(40));
        vectorValue.x = Mathf.Max(vectorValue.x, min);

        EditorGUILayout.MinMaxSlider(ref vectorValue.x, ref vectorValue.y, min, max);

        vectorValue.y = EditorGUILayout.FloatField((float)System.Math.Round(vectorValue.y, 3), GUILayout.Width(40));
        vectorValue.y = Mathf.Min(vectorValue.y, max);

        EditorGUILayout.EndHorizontal();

        vectorProperty.vector2Value = vectorValue;
    }

    private void DrawAdvancedSection()
    {
        DrawPitchFields();
        EditorGUILayout.Separator();
        DrawFadeFields();
        EditorGUILayout.Separator();
        DrawArpeggiatorFields();
        EditorGUILayout.Separator();
        MultiplePlays();
    }

    private void DrawPitchFields()
    {
        EditorGUILayout.BeginVertical("box");
        SerializedProperty dynamicPitchRangeProperty = serializedObject.FindProperty("_dynamicPitchRange");
        if (dynamicPitchRangeProperty.boolValue)
        {
            ShowMinMaxSlider(serializedObject.FindProperty("_pitchRange"), "Pitch", -3, 3);
        }
        else
        {
            serializedObject.FindProperty("_pitch").floatValue =
                EditorGUILayout.Slider("Pitch", serializedObject.FindProperty("_pitch").floatValue, -3, 3);
        }

        EditorGUILayout.PropertyField(dynamicPitchRangeProperty);
        EditorGUILayout.EndVertical();
    }

    private void DrawFadeFields()
    {
        SerializedProperty fadeTypeField = serializedObject.FindProperty("_fadeType");
        EditorGUILayout.PropertyField(fadeTypeField);
        if (fadeTypeField.enumValueIndex != 0)
        {
            EditorGUI.indentLevel++;
            DrawPropertyField("_fadeDuration");
            EditorGUI.indentLevel--;
        }
    }

    //Consider hiding dynamic pitch if arpeggiator is on
    private void DrawArpeggiatorFields()
    {
        SerializedProperty useArpeggiatorProperty = serializedObject.FindProperty("_useArpeggiator");
        EditorGUILayout.PropertyField(useArpeggiatorProperty);
        if (useArpeggiatorProperty.boolValue)
        {
            DrawPropertyField("_arpeggiator");
        }
    }

    private void MultiplePlays()
    {
        SerializedProperty playMultipleTimesProperty = serializedObject.FindProperty("_playMultipleTimes");
        EditorGUILayout.PropertyField(playMultipleTimesProperty);
        if (playMultipleTimesProperty.boolValue)
        {
            EditorGUI.indentLevel++;
            DrawPropertyField("_numberOfPlays");
            DrawPropertyField("_delayBetweenPlays");
            EditorGUI.indentLevel--;
        }
    }


    private void ShowPreviewButtons()
    {
        EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);

        if (GUILayout.Button("Play Preview"))
        {
            ((AudioPlayer)target).PlayPreview(_previewer);
        }

        GUI.enabled = _previewer.isPlaying;
        if (GUILayout.Button("Stop Preview"))
        {
            ((AudioPlayer)target).StopLastPlayedSound();
        }

        GUI.enabled = true;

        EditorGUI.EndDisabledGroup();
    }
}