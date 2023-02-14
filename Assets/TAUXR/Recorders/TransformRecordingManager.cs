using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum TransformRecordingState
{
    Initialized,    // before choosing whether to load data or record new
    Recording,      // recording new data
    RecordingEnded,  // after ending a recording
    DataLoaded,     // recorded data is loaded
}

public class TransformRecordingManager : MonoBehaviour
{
    public TransformRecordingState State = TransformRecordingState.Initialized;

    public bool bRecord;
    public bool bLoadRecordingFile;
    public bool IsRecordingDataLoaded = false;

    public bool bPlay;
    public bool bIsPlaying;

    public TransformRecorder[] allrecordings;
    public int framesRecorded = 0;
    public float timeRecorded = 0;
    public int loadedFrames = 0;

    void Start()
    {
        allrecordings = GameObject.FindObjectsOfType<TransformRecorder>();
    }

    void Update()
    {
        if (State == TransformRecordingState.Recording)
        {
            framesRecorded++;
            timeRecorded += Time.deltaTime;
        }
    }

    public void StartRecording()
    {
        foreach (TransformRecorder tr in allrecordings)
            tr.StartRecording();

        State = TransformRecordingState.Recording;
    }

    public void PlayRecordedData()
    {
        if (!IsRecordingDataLoaded)
        {
            foreach (TransformRecorder tr in allrecordings)
                tr.LoadRecordedData();

            IsRecordingDataLoaded = true;
        }

        foreach (TransformRecorder tr in allrecordings)
            tr.IsPlaying = true;
    }

    public void PauseAllRecordings()
    {
        foreach (TransformRecorder tr in allrecordings)
            tr.Pause();
    }

    public void LoadRecordedData()
    {
        foreach (TransformRecorder tr in allrecordings)
            tr.LoadRecordedData();

        IsRecordingDataLoaded = true;
        loadedFrames = allrecordings[0].PositionData.Count;

        State = TransformRecordingState.DataLoaded;
    }

    public void StopRecordingAndSave()
    {
        foreach (TransformRecorder tr in allrecordings)
            tr.StopRecordingAndSave();
        
        State = TransformRecordingState.RecordingEnded;
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(TransformRecordingManager))]
class TransformRecordingManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        var recordingManager = (TransformRecordingManager)target;
        if (recordingManager == null) return;

        Undo.RecordObject(recordingManager, "Change TransformRecordingManager");

        switch (recordingManager.State)
        {
            case TransformRecordingState.Initialized:

                // show record or load recording data
                if (GUILayout.Button("Start Recording"))
                {
                    recordingManager.StartRecording();
                }

                if (GUILayout.Button("Load Recorded Data"))
                {
                    recordingManager.LoadRecordedData();
                }
                break;
            case TransformRecordingState.Recording:

                GUILayout.Label($"Recording... Frames Recorded: {recordingManager.framesRecorded}  |  Time Recorded: {recordingManager.timeRecorded}");

                if (GUILayout.Button("Stop Recording & Save To File"))
                {
                    recordingManager.StopRecordingAndSave();
                }
                break;

            case TransformRecordingState.RecordingEnded:
                GUILayout.Label($"Recorded {recordingManager.framesRecorded} frames!");
                GUILayout.Label($"Your recording data is saved on: {recordingManager.allrecordings[0].PathTransformData}");
                GUILayout.Label("End Playmode -> Click outside the unity editor window -> click inside unity editor window and hopa you have it");
                break;

            case TransformRecordingState.DataLoaded:
                GUILayout.Label($"Loaded {recordingManager.loadedFrames}  frames");

                if (GUILayout.Button("Play Recorded Data"))
                {
                    recordingManager.PlayRecordedData();
                }


                if (GUILayout.Button("Pause"))
                {
                    recordingManager.PauseAllRecordings();
                }
                
                GUILayout.Label("To record new data end Playmode, and start again. IT WILL OVERRIDE PREVIOUS DATA");

                break;
        }


    }
}
#endif

