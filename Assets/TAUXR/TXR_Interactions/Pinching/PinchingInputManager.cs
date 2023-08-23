using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;


public class PinchingInputManager : AInputManager
{
    private readonly TXRHand _handLeft;
    private readonly TXRHand _handRight;

    public PinchingInputManager(TXRHand handLeft, TXRHand handRight)
    {
        _handLeft = handLeft;
        _handRight = handRight;
    }

    protected override bool IsLeftHeld()
    {
        return _handLeft.PinchManager.IsPlayerPinchingThisFrame();
    }

    protected override bool IsRightHeld()
    {
        return _handRight.PinchManager.IsPlayerPinchingThisFrame();
    }
}