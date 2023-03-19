using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Referencer : MonoBehaviour
{
    #region Singelton Decleration
    private static Referencer _instance;

    public static Referencer Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    [Header("Player")]
    public Transform OVRCameraRig;
    public Transform PlayerHead;
    public OVRSkeleton PlayerOVRSkeletonLeft;
    public OVRSkeleton PlayerOVRSkeletonRight;


}
