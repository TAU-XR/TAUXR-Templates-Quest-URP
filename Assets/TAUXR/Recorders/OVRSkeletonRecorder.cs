using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/*
 * Create a file for each bone formatted as Frame,PosX,PosY,PosZ,RotX,RotY,RotZ,Rotw,SclX,SclY,SclZ
 * Load files, maybe across several frames.
 * Affect skeleton bones by their files.
 */
public class OVRSkeletonRecorder : MonoBehaviour
{
    public OVRCustomSkeleton skeletonToRecord;

    public bool IsRecording = false;

    StreamWriter[] writers;
    string[] paths;

    void Start()
    {
        Debug.LogWarning("Skeleton Recording is active -> Recording Files Initiated");

        InitRecording();

        // to force manual recording
        IsRecording = false;
    }

    void LateUpdate()
    {
        if (IsRecording)
        {
            RecordSkeletonData();
        }
    }

    public void InitRecording()
    {
        if (skeletonToRecord == null)
        {
            Debug.LogWarning("No skeleton found -> Can't record");
            return;
        }

        // create a new folder for recording files
        string basePath = $"Assets/Resources/SkeletonRecordings/SkeletonRecorededData_{System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}/";
        string dir = Path.GetDirectoryName(basePath);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        // init arrays
        int bonesCount = skeletonToRecord.CustomBones.Count;
        writers = new StreamWriter[bonesCount];
        paths = new string[bonesCount];

        //create file for each bone
        for (int i = 0; i < skeletonToRecord.CustomBones.Count; i++)
        {
            paths[i] = basePath + $"/BoneRecording_{i}.csv";
            writers[i] = new StreamWriter(paths[i], false);
        }
    }

    public void RecordSkeletonData()
    {
        if (skeletonToRecord.IsDataHighConfidence)
        {
            for (int i = 0; i < skeletonToRecord.CustomBones.Count; i++)
            {
                Vector3 pos = skeletonToRecord.Bones[i].Transform.localPosition;
                Quaternion rot = skeletonToRecord.Bones[i].Transform.localRotation;
                Vector3 scl = skeletonToRecord.Bones[i].Transform.localScale;

                writers[i].WriteLine($"{pos.x},{pos.y},{pos.z},{rot.x},{rot.y},{rot.z},{rot.w},{scl.x},{scl.y},{scl.z}");
            }
        }
    }

    private void OnApplicationQuit()
    {
        foreach (StreamWriter w in writers)
        {
            w.Flush();
            w.Close();
        }
    }
}