using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum HandType { Left, Right, None, Any }
public class TAUXRPlayer : MonoBehaviour
{
    [SerializeField] private Transform ovrRig;
    [SerializeField] private Transform playerHead;
    [SerializeField] private Transform rightHandAnchor;
    [SerializeField] private Transform leftHandAnchor;

    public bool IsEyeTrackingEnabled;
    public bool IsFaceTrackingEnabled;

    [Header("Eye Tracking")]
    [SerializeField] private Transform rightEye;
    [SerializeField] private Transform leftEye;
    [SerializeField] private float eyeRayMaxLength = 10000;
    private float EYETRACKINGCONFIDENCETHRESHOLD = .5f;
    private Vector3 NOTTRACKINGVECTORVALUE = new Vector3(-1f, -1f, -1f);


    private Transform focusedObject;
    private Vector3 eyeGazeHitPosition;

    private OVREyeGaze ovrEye;
    private OVRHand ovrHandR, ovrHandL;
    private OVRSkeleton skeletonR, skeletonL;

    private PinchPoint pinchPoincL, pinchPointR;
    private List<HandCollider> handCollidersL, handCollidersR;



    public Transform PlayerHead => playerHead;
    public Transform RightHand => rightHandAnchor;
    public Transform LeftHand => leftHandAnchor;

    public Transform RightEye => rightEye;
    public Transform LeftEye => leftEye;
    public Transform FocusedObject => focusedObject;
    public Vector3 EyeGazeHitPosition => eyeGazeHitPosition;

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

        if (rightEye.TryGetComponent(out OVREyeGaze e))
        {
            ovrEye = e;
        }
        focusedObject = null;
        eyeGazeHitPosition = NOTTRACKINGVECTORVALUE;

        if(IsFaceTrackingEnabled)
        {
            ovrRig.AddComponent<OVRFaceExpressions>();
        }
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

        if (IsEyeTrackingEnabled)
        {
            CalculateEyeParameters();
        }
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

    private void CalculateEyeParameters()
    {
        if (ovrEye == null) return;

        if (ovrEye.Confidence < EYETRACKINGCONFIDENCETHRESHOLD)
        {
            Debug.LogWarning("EyeTracking confidence value is low. Eyes are not tracked");
            focusedObject = null;
            eyeGazeHitPosition = NOTTRACKINGVECTORVALUE;

            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(rightEye.position, rightEye.forward, out hit, eyeRayMaxLength))
        {
            focusedObject = hit.transform;
            eyeGazeHitPosition = hit.point;
        }
        else
        {
            focusedObject = null;
            eyeGazeHitPosition = NOTTRACKINGVECTORVALUE;
        }

    }
}
