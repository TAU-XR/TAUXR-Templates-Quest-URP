using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TransformRecorder : MonoBehaviour
{
    public bool IsRecording;
    bool isFirstRecordingFrame = true;              // to catch the first frame recording.
    public bool IsPlaying;

    public List<Vector3> PositionData;
    public List<Quaternion> RotationData;
    public List<Vector3> ScaleData;

    public string PathTransformData;

    StreamWriter writer;

    public int playIndex = 0;
    private void Awake()
    {
        PathTransformData = $"Assets/Resources/TransformRecordingData/{gameObject.name}_TransformRecording.csv";

        IsRecording = false;
        // so it will never start recording from start and automatically erase last data. Recording should always be triggered manualy from the inspector while in play

      //  if (bRecord)
         //   writer = new StreamWriter(path, false);
    }

  
    private void LateUpdate()
    {
        if(IsRecording)
        {
            RecordDataToLists();
            WriteDataToFile();
        }

        if(IsPlaying)
        {
            Play();
        }
    }

    public void StartRecording()
    {
        if (isFirstRecordingFrame)
        {
            // initiate the new writer
            writer = new StreamWriter(PathTransformData, false);
            isFirstRecordingFrame = false;
        }
        
        IsRecording = true;
    }

    private void RecordDataToLists()
    {
        PositionData.Add(transform.position);
        RotationData.Add(transform.rotation);
        ScaleData.Add(transform.localScale);
    }

    // file will be saved this way: PositionX,PosY,PosZ,RotX,RotY,RotZ,RotW,ScaleX,ScaleY,ScaleZ
    private void WriteDataToFile()
    {
        Vector3 pos = PositionData[PositionData.Count - 1];
        Quaternion rot = RotationData[RotationData.Count - 1];
        Vector3 sca = ScaleData[ScaleData.Count - 1];

        writer.WriteLine($"{pos.x},{pos.y},{pos.z},{rot.x},{rot.y},{rot.z},{rot.w},{sca.x},{sca.y},{sca.z}");
    }

    public void LoadRecordedData()
    {
        string[] allFile = File.ReadAllLines(PathTransformData);
        string[] lineData;

        for (int i = 0; i < allFile.Length; i++)
        {
            lineData = allFile[i].Split(',');
          
            // for some reason some lines are not 10 in length, so ignore them for now
            if (lineData.Length != 10) break;
          
            Vector3 pos = new Vector3(float.Parse(lineData[0]), float.Parse(lineData[1]), float.Parse(lineData[2]));
            Quaternion rot = new Quaternion(float.Parse(lineData[3]), float.Parse(lineData[4]), float.Parse(lineData[5]), float.Parse(lineData[6]));
            Vector3 scl = new Vector3(float.Parse(lineData[7]), float.Parse(lineData[8]), float.Parse(lineData[9]));
           
            PositionData.Add(pos);
            RotationData.Add(rot);
            ScaleData.Add(scl);
        }
    }

    public void Play()
    {
        if (playIndex >= PositionData.Count) return;
        transform.position = PositionData[playIndex];
        transform.rotation = RotationData[playIndex];
        playIndex++;
    }

    public void Pause()
    {
        IsPlaying = !IsPlaying;
    }

    public void StopRecordingAndSave()
    {
        IsRecording = false;
        writer.Flush();
        writer.Close();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TransformRecorder))]
class TransformRecorderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GUILayout.Label("Make sure you have a TransformRecordingManager setup and you're ready to go!");
    }
}
#endif