using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class OVRSkeletonPlayFromData : MonoBehaviour
{
    public OVRCustomSkeleton skeletonToPlayOn;
    public string DataPathFolderName;
    public int currentFrame = 0;
    public bool bPlay = false;
    public int loadBonesPerFrame = 1;
    int loadedBones = 0;
    string[] paths;

    public List<Vector3>[] BonesPositions;
    public List<Quaternion>[] BonesRotations;
    public List<Vector3>[] BonesScales;

    int frameCount = 0;
    List<Transform> skeletonBones;
    void Start()
    {
        StartCoroutine(LoadRecordedDataToLists());
        bPlay = false;
    }

    void Update()
    {
        if (bPlay)
            PlaySkeletonData();
    }

    IEnumerator LoadRecordedDataToLists()
    {
        // validate a data folder was entered.
        if (DataPathFolderName == null)
        {
            Debug.LogError("Recorded skeleton data path is empty. Please select your data folder");
            yield break;
        }

        // get to the skeleton data folder
        string SkeletonDataFolderPath = "Assets/Resources/SkeletonRecordings/" + DataPathFolderName + "/";
        string dir = Path.GetDirectoryName(SkeletonDataFolderPath);
        if (!Directory.Exists(dir))
            Debug.LogError("Recorded skeleton data not found -> check your path");

        // init list arrays to the size of bones.
        BonesPositions = new List<Vector3>[skeletonToPlayOn.Bones.Count];
        BonesRotations = new List<Quaternion>[skeletonToPlayOn.Bones.Count];
        BonesScales = new List<Vector3>[skeletonToPlayOn.Bones.Count];

        paths = new string[skeletonToPlayOn.Bones.Count];


        for (int boneIndex = 0; boneIndex < skeletonToPlayOn.Bones.Count; boneIndex++)
        {
            loadedBones++;
            // init all paths to their indexes
            paths[boneIndex] = SkeletonDataFolderPath + $"BoneRecording_{boneIndex}.csv";
            if (!File.Exists(paths[boneIndex]))
            {
                Debug.LogWarning($"Couldn't find data file BoneRecording_{boneIndex}. Could not load data for this bone");
                break;
            }

            // init a bone list
            BonesPositions[boneIndex] = new List<Vector3>();
            BonesRotations[boneIndex] = new List<Quaternion>();
            BonesScales[boneIndex] = new List<Vector3>();

            // load all bone data into a string array
            string[] allFile = File.ReadAllLines(paths[boneIndex]);
            string[] lineData;

            // extract bone data from string to its bone lists
            for (int frameIndex = 0; frameIndex < allFile.Length; frameIndex++)
            {
                lineData = allFile[frameIndex].Split(',');

                // for some reason some lines are not 10 in length, so ignore them for now
                if (lineData.Length != 10) break;

                Vector3 pos = new Vector3(float.Parse(lineData[0]), float.Parse(lineData[1]), float.Parse(lineData[2]));
                Quaternion rot = new Quaternion(float.Parse(lineData[3]), float.Parse(lineData[4]), float.Parse(lineData[5]), float.Parse(lineData[6]));
                Vector3 scl = new Vector3(1f, 1f, 1f);
                //Vector3 scl = new Vector3(float.Parse(lineData[7]), float.Parse(lineData[8]), float.Parse(lineData[9])); - for some reason it makes problems.

                BonesPositions[boneIndex].Add(pos);
                BonesRotations[boneIndex].Add(rot);
                BonesScales[boneIndex].Add(scl);
            }
            //yield return null;
            Debug.Log($" Total Bones Loaded: {loadedBones}. Loaded Bone_{boneIndex}.");
        }

        frameCount = BonesPositions[0].Count;
        Debug.Log($"Total Lists: {BonesPositions.Length}. Total Frames {BonesPositions[0].Count}. Successfully loaded recorded data to lists. ");
    }

    public void PlaySkeletonData()
    {
        if (currentFrame >= frameCount)
            return;

        for (int boneIndex = 0; boneIndex < skeletonToPlayOn.Bones.Count; boneIndex++)
        {
            skeletonToPlayOn.Bones[boneIndex].Transform.localPosition = BonesPositions[boneIndex][currentFrame];
            skeletonToPlayOn.Bones[boneIndex].Transform.localRotation = BonesRotations[boneIndex][currentFrame];
            skeletonToPlayOn.Bones[boneIndex].Transform.localScale = BonesScales[boneIndex][currentFrame];
        }

        currentFrame++;

    }
}