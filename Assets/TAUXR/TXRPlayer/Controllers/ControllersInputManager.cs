using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class ControllersInputManager : AInputManager
{
    protected override bool IsLeftHeld()
    {
        return OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > .7f;
    }

    protected override bool IsRightHeld()
    {
        return OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > .7f;
    }

    protected override void DoWhileHolding(float holdingDuration, float duration)
    {
        //Updating vibration
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
        if (IsInputPressedThisFrame(HandType.Left))
        {
            OVRInput.SetControllerVibration(vibrationStrength, vibrationStrength, OVRInput.Controller.LTouch);
        }

        if (IsInputPressedThisFrame(HandType.Right))
        {
            OVRInput.SetControllerVibration(vibrationStrength, vibrationStrength, OVRInput.Controller.RTouch);
        }
    }

    private static void ResetControllersVibration()
    {
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
    }

    protected override void DoOnHoldFinished()
    {
        ResetControllersVibration();
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