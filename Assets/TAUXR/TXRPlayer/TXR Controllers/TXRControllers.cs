using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class TXRControllers
{
    private bool _wasTriggerPressedLastFrame = false;
    public bool ReadyForNextTriggerPress = true;

    public CancellationTokenSource WaitForTriggerHoldCancellationTokenSource;

    public bool IsTriggerPressedThisFrame(HandType handType)
    {
        bool isLeftTriggerHeld = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > .7f;
        bool isRightTriggerHeld = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > .7f;

        switch (handType)
        {
            case HandType.Left:
                return isLeftTriggerHeld;
            case HandType.Right:
                return isRightTriggerHeld;
            case HandType.Any:
                return isLeftTriggerHeld || isRightTriggerHeld;
            case HandType.None:
                return false;
            default: return false;
        }
    }

    public async UniTask WaitForTriggerPress(HandType handType, Action callback = default)
    {
        await WaitForTriggerExitIfHolding(handType);
        UniTask.WaitUntil(() => IsTriggerPressedThisFrame(handType));
        callback?.Invoke();
    }

    public async UniTask WaitForTriggerHold(HandType handType, float duration,
        Action callback = default)
    {
        using (WaitForTriggerHoldCancellationTokenSource = new CancellationTokenSource())
        {
            float holdingDuration = 0;

            await WaitForTriggerExitIfHolding(handType);

            bool cancellationRequested = WaitForTriggerHoldCancellationTokenSource.IsCancellationRequested;
            while (holdingDuration < duration && !cancellationRequested)
            {
                holdingDuration = IsTriggerPressedThisFrame(handType)
                    ? holdingDuration + Time.deltaTime
                    : 0;

                UpdateControllersVibration(holdingDuration, duration);

                await UniTask.Yield();
            }

            if (!cancellationRequested)
            {
                callback?.Invoke();
            }
        }

        ResetControllersVibration();
    }

    private async UniTask WaitForTriggerExitIfHolding(HandType handType)
    {
        if (IsTriggerPressedThisFrame(handType))
        {
            await UniTask.WaitUntil(() => !IsTriggerPressedThisFrame(handType));
        }
    }

    private void UpdateControllersVibration(float holdingDuration, float duration)
    {
        if (holdingDuration > 0)
        {
            SetControllersVibrationOnHold(holdingDuration / duration);
        }
        else
        {
            ResetControllersVibration();
        }
    }

    private void SetControllersVibrationOnHold(float vibrationStrength)
    {
        if (IsTriggerPressedThisFrame(HandType.Left))
        {
            OVRInput.SetControllerVibration(vibrationStrength, vibrationStrength, OVRInput.Controller.LTouch);
        }

        if (IsTriggerPressedThisFrame(HandType.Right))
        {
            OVRInput.SetControllerVibration(vibrationStrength, vibrationStrength, OVRInput.Controller.RTouch);
        }
    }

    private static void ResetControllersVibration()
    {
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
    }

    public async UniTask SetControllersVibration(float vibrationStrength, float duration)
    {
        OVRInput.SetControllerVibration(vibrationStrength, vibrationStrength, OVRInput.Controller.RTouch);
        OVRInput.SetControllerVibration(vibrationStrength, vibrationStrength, OVRInput.Controller.LTouch);

        await UniTask.Delay(TimeSpan.FromSeconds(duration));

        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
    }
}