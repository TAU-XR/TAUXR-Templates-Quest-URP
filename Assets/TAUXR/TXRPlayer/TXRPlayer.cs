using Cysharp.Threading.Tasks;
using System;
using Unity.VisualScripting;
using UnityEngine;
using System.Threading;

public class TXRPlayer : TXRSingleton<TXRPlayer>
{
    [Header("Player Trackables")] [SerializeField]
    private Transform ovrRig;

    [SerializeField] private Transform playerHead;
    [SerializeField] private Transform rightHandAnchor;
    [SerializeField] private Transform leftHandAnchor;
    private OVRManager _ovrManager;
    public Transform PlayerHead => playerHead;
    public Transform RightHand => rightHandAnchor;
    public Transform LeftHand => leftHandAnchor;

    [Header("Hand Tracking")] public TXRHand HandLeft;
    public TXRHand HandRight;

    [Header("Eye Tracking")] public TXREyeTracker EyeTracker;
    public bool IsEyeTrackingEnabled;
    public Transform FocusedObject => EyeTracker.FocusedObject;
    public Vector3 EyeGazeHitPosition => EyeTracker.EyeGazeHitPosition;
    public Transform RightEye => EyeTracker.RightEye;
    public Transform LeftEye => EyeTracker.LeftEye;


    [Header("Face Tracking")] [SerializeField]
    public OVRFaceExpressions ovrFace;

    public OVRFaceExpressions OVRFace => ovrFace;
    public bool IsFaceTrackingEnabled;

    // Color overlay
    [SerializeField] private MeshRenderer colorOverlayMR;

    public TXRControllersInputManager TXRControllersInputManager;

    protected override void DoInAwake()
    {
        _ovrManager = GetComponentInChildren<OVRManager>();

        HandRight.Init();
        HandLeft.Init();

        TXRControllersInputManager = new TXRControllersInputManager();

        if (IsEyeTrackingEnabled)
            EyeTracker.Init();
        if (IsFaceTrackingEnabled)
        {
            ovrRig.AddComponent<OVRFaceExpressions>();
        }
    }


    void Update()
    {
        HandRight.UpdateHand();
        HandLeft.UpdateHand();

        if (IsEyeTrackingEnabled)
        {
            EyeTracker.UpdateEyeTracker();
        }
    }

    // covers player's view with color. 
    public async UniTask FadeToColor(Color targetColor, float duration)
    {
        if (duration == 0)
        {
            colorOverlayMR.material.color = targetColor;
            return;
        }

        Color currentColor = colorOverlayMR.material.color;
        if (currentColor == targetColor) return;

        float lerpTime = 0;
        while (lerpTime < duration)
        {
            lerpTime += Time.deltaTime;
            float t = lerpTime / duration;
            colorOverlayMR.material.color = Color.Lerp(currentColor, targetColor, t);

            await UniTask.Yield();
        }
    }

    public void RepositionPlayer(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }

    public void RecenterView()
    {
        // IMPLEMENT
    }

    public void CalibrateFloorHeight()
    {
        // IMPLEMENT
    }

    public void SetPassthrough(bool state)
    {
        _ovrManager.isInsightPassthroughEnabled = state;
    }

    #region Hands

    public bool IsPlayerPinchingThisFrame(HandType handType)
    {
        switch (handType)
        {
            case HandType.Left:
                return HandLeft.PinchManager.IsPlayerPinchingThisFrame();
            case HandType.Right:
                return HandRight.PinchManager.IsPlayerPinchingThisFrame();
            case HandType.Any:
                return HandRight.PinchManager.IsPlayerPinchingThisFrame() ||
                       HandLeft.PinchManager.IsPlayerPinchingThisFrame();
                ;
            case HandType.None:
                return false;
            default: return false;
        }
    }

    public async UniTask WaitForPinch(HandType handType)
    {
        await WaitForPinchExitIfPinching(handType);
        await UniTask.WaitUntil(() => IsPlayerPinchingThisFrame(handType));
    }

    private async UniTask WaitForPinchExitIfPinching(HandType handType)
    {
        if (IsPlayerPinchingThisFrame(handType))
        {
            await UniTask.WaitUntil(() => !IsPlayerPinchingThisFrame(handType));
        }
    }

    public async UniTask WaitForPinchHold(HandType handType, float duration, bool waitUntilRelease = false)
    {
        await WaitForPinchExitIfPinching(handType);

        float holdingDuration = 0;
        while (holdingDuration < duration)
        {
            if (IsPlayerPinchingThisFrame(handType))
            {
                holdingDuration += Time.deltaTime;
            }
            else
            {
                holdingDuration = 0;
            }

            await UniTask.Yield();
        }

        await UniTask.WaitUntil(() => !waitUntilRelease || !IsPlayerPinchingThisFrame(handType));
    }

    public Transform GetHandFingerCollider(HandType handType, FingerType fingerType)
    {
        switch (handType)
        {
            case HandType.Left:
                return HandLeft.GetFingerCollider(fingerType);
            case HandType.Right:
                return HandRight.GetFingerCollider(fingerType);
            case HandType.Any:
                return HandLeft.GetFingerCollider(fingerType);
            case HandType.None:
                return null;
            default: return null;
        }
    }

    #endregion
}