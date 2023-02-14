using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HandType { Left, Right, None, Any }
public class TAUXRPlayer : MonoBehaviour
{
    public OVRHand ovrHandR, ovrHandL;
    OVRSkeleton skeletonR, skeletonL;

    private PinchPoint pinchPoincL, pinchPointR;
    private List<HandCollider> handCollidersL, handCollidersR;
    private void Awake()
    {
        skeletonL = ovrHandL.GetComponent<OVRSkeleton>();
        skeletonR = ovrHandR.GetComponent<OVRSkeleton>();

        handCollidersL = new List<HandCollider>();
        handCollidersR = new List<HandCollider>();

        // init pinch points
        foreach (PinchPoint pp in GetComponentsInChildren<PinchPoint>())
        {
            if(pp.HandT == HandType.Right)
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

        // init hand colliders
        foreach (HandCollider hc in GetComponentsInChildren<HandCollider>())
        {
            if(hc.HandT == HandType.Right)
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
        OVRHand ovrHand;
        List<HandCollider> handColliders;
        PinchPoint pinchPoint;

        if(type == HandType.Left)
        {
            skeleton = skeletonL;
            ovrHand = ovrHandL;
            handColliders = handCollidersL;
            pinchPoint = pinchPoincL;
        }
        else
        {
            skeleton = skeletonR;
            ovrHand = ovrHandR;
            handColliders = handCollidersR;
            pinchPoint = pinchPointR;
        }

        if(skeleton.IsDataHighConfidence)
        {
            foreach (HandCollider hc in handColliders)
                hc.UpdateHandCollider();

            pinchPoint.UpdatePinchPoint(ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Index));
            
        }
    }
}
