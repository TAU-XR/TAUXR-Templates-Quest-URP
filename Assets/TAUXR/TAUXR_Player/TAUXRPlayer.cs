using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HandType { Left, Right, None, Any }
public class TAUXRPlayer : MonoBehaviour
{
    [SerializeField] private Transform playerHead;
    [SerializeField] private Transform rightHandAnchor;
    [SerializeField] private Transform leftHandAnchor;
    [SerializeField] private Transform rightEye;
    [SerializeField] private Transform leftEye;


    OVRHand ovrHandR, ovrHandL;
    OVRSkeleton skeletonR, skeletonL;

    private PinchPoint pinchPoincL, pinchPointR;
    private List<HandCollider> handCollidersL, handCollidersR;



    public Transform PlayerHead => playerHead;
    public Transform RightHand => rightHandAnchor;
    public Transform LeftHand => leftHandAnchor;
    public Transform RightEye => rightEye;
    public Transform LeftEye => leftEye;


    private static TAUXRPlayer _instance;
    public static TAUXRPlayer Instance { get { return _instance; } }

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

        ovrHandL = leftHandAnchor.GetComponentInChildren<OVRHand>();
        ovrHandR = rightHandAnchor.GetComponentInChildren<OVRHand>();

        skeletonL = ovrHandL.GetComponent<OVRSkeleton>();
        skeletonR = ovrHandR.GetComponent<OVRSkeleton>();

        InitHandColliders();
    }

    private void InitPinchPoints()
    {
        foreach (PinchPoint pp in GetComponentsInChildren<PinchPoint>())
        {
            if (pp.HandT == HandType.Right)
            {
                pinchPointR = pp;
                pinchPointR.Init(skeletonR);
            }
            else
            {
                pinchPoincL = pp;
                pinchPoincL.Init(skeletonL);
            }
        }
    }

    private void InitHandColliders()
    {
        handCollidersL = new List<HandCollider>();
        handCollidersR = new List<HandCollider>();

        foreach (HandCollider hc in GetComponentsInChildren<HandCollider>())
        {
            if (hc.HandT == HandType.Right)
            {
                hc.Init(skeletonR);
                handCollidersR.Add(hc);
            }
            else
            {
                hc.Init(skeletonL);
                handCollidersL.Add(hc);
            }
        }
    }

    void Start()
    {

    }

    void Update()
    {
        UpdateHand(HandType.Right);
        UpdateHand(HandType.Left);
    }

    public void UpdateHand(HandType type)
    {
        OVRSkeleton skeleton;
        //      OVRHand ovrHand;
        List<HandCollider> handColliders;
        //        PinchPoint pinchPoint;

        if (type == HandType.Left)
        {
            skeleton = skeletonL;
            //ovrHand = ovrHandL;
            handColliders = handCollidersL;
            //pinchPoint = pinchPoincL;
        }
        else
        {
            skeleton = skeletonR;
            //ovrHand = ovrHandR;
            handColliders = handCollidersR;
            //pinchPoint = pinchPointR;
        }

        if (skeleton.IsDataHighConfidence)
        {
            foreach (HandCollider hc in handColliders)
                hc.UpdateHandCollider();

            //pinchPoint.UpdatePinchPoint(ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Index));

        }
    }
}
