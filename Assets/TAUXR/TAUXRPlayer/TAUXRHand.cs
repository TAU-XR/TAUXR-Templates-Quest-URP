using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TAUXRHand : MonoBehaviour
{
    public HandType HandType;
    [SerializeField] private OVRHand _ovrHand;
    [SerializeField] private OVRSkeleton _ovrSkeleton;
    [SerializeField] private Pincher _pincher;
    [SerializeField] private List<HandCollider> _handColliders;

    private float PINCH_EXIT_THRESHOLD = .97f;
    private float PINCH_ENTER_THRESHOLD = .99f;

    bool _isPinching = false;
    public Action PinchEnter, PinchExit;

    private void Update()
    {
        UpdateHand();
    }
    public void Init()
    {
        _handColliders = GetComponentsInChildren<HandCollider>().ToList();
        foreach (HandCollider hc in _handColliders)
        {
            hc.Init(_ovrSkeleton);
        }

        _pincher.Init(_ovrSkeleton);
    }

    public void UpdateHand()
    {
        if (_ovrSkeleton.IsDataHighConfidence)
        {
            // update hand colliders
            foreach (HandCollider hc in _handColliders)
            {
                hc.UpdateHandCollider();
            }

            // update pincher
            _pincher.UpdatePincher();
            HandlePinchEvents();

        }
    }

    private void HandlePinchEvents()
    {

        if (!_isPinching)
        {
            if (_pincher.Strength > PINCH_ENTER_THRESHOLD)
            {
                Debug.Log("PINCH ENTER");
                PinchEnter?.Invoke();
                _isPinching = true;
            }

        }
        else
        {
            if (_pincher.Strength < PINCH_EXIT_THRESHOLD)
            {
                Debug.Log("PINCH EXIT");
                PinchExit?.Invoke();
                _isPinching = false;
            }
        }
    }

    public bool IsPlayerPinchingThisFrame()
    {
        return _isPinching;
    }

    public async UniTask WaitForPinchHold(float duration)
    {
        float holdingDuration = 0;
        while (holdingDuration < duration)
        {
            if (IsPlayerPinchingThisFrame())
            {
                holdingDuration += Time.deltaTime;
            }
            else
            {
                holdingDuration = 0;
            }

            await UniTask.Yield();
        }
    }


}

