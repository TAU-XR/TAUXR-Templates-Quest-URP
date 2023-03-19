using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCollider : MonoBehaviour
{
    public HandType HandT;

    OVRSkeleton ovrSkeleton;
    public int fingerIndex;


    private void Start()
    {
        if (HandT == HandType.Right)
        {
            ovrSkeleton = Referencer.Instance.PlayerOVRSkeletonRight;
        }
        else
        {
            ovrSkeleton = Referencer.Instance.PlayerOVRSkeletonLeft;
        }
    }


    private void Update()
    {
        if (ovrSkeleton.IsDataHighConfidence)
        {
            TrackPosition();
        }
    }

    private void TrackPosition()
    {
        transform.position = ovrSkeleton.Bones[fingerIndex].Transform.position;
        transform.rotation = ovrSkeleton.Bones[fingerIndex].Transform.rotation;
    }
}
