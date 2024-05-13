using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Toggle means- OnReleased is not called when finger is off the press colider -> it is called when the button is pressed again after it was already pressed.

/*TODO:
 * Fix hover visuals - no need to change _backfaceColor active.

*/
public class TXRButton_Toggle : TXRButton
{
    public UnityEvent ToggleOn;
    public UnityEvent ToggleOff;
    private bool _shouldToggleOff = false;

    protected override void Start()
    {
        base.Start();
        ResponseRelease = ButtonColliderResponse.None;  // important line - we want no response on release. It should be controlled by the toggle.
    }

    public override void OnPressed(Transform toucher)
    {
        if (State != ButtonState.Interactable) return;
        if (toucher != ActiveToucher) return;

        if (!isPressed)
        {
            DelegateInteralExtenralResponses(ResponsePress, OnPressedInternal, ToggleOn);
        }
        else
        {
            DelegateInteralExtenralResponses(ButtonColliderResponse.Both, OnReleasedInternal, ToggleOff);
        }
    }

    public override void OnHoverExit(Transform toucher)
    {
        if (State != ButtonState.Interactable) return;

        var ShouldContinueAfterToucherExit = HoverExitToucherProcessing(toucher);
        if (!ShouldContinueAfterToucherExit) return;

        DelegateInteralExtenralResponses(ResponseHoverExit, OnHoverExitInternal, HoverExit);

    }

    protected override void OnHoverExitInternal()
    {
        isHovered = false;
        activeToucher = null;
        PlaySound(soundHoverExit);
        //visuals.SetHoverGradient(false);

    }

    protected override void OnHoverEnterInternal()
    {
        isHovered = true;
        PlaySound(soundHoverEnter);
        visuals.Hover();
    }

    public void TriggerToggleEvent(TXRButtonToggleState state, ButtonColliderResponse response)
    {
        UnityEvent toggleEvent = state == TXRButtonToggleState.On ? ToggleOn : ToggleOff;
        Action internalAction = state == TXRButtonToggleState.On ? OnPressedInternal : OnReleasedInternal;

        DelegateInteralExtenralResponses(response, internalAction, ToggleOn);
    }
}

public enum TXRButtonToggleState { On, Off }