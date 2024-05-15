using System;
using UnityEngine;
using UnityEngine.Events;

// Toggle means- OnReleased is not called when finger is off the press colider -> it is called when the button is pressed again after it was already pressed.

/*TODO:
 * Fix hover visuals - no need to change _backfaceColor active.

*/
public class TXRButton_Toggle : TXRButton
{
  /*  public UnityEvent ToggleOn;
    public UnityEvent ToggleOff;
    public TXRButtonToggleState ToggleState;
    TXRButtonToggleVisuals _toggleVisuals;

    protected override void Start()
    {
        base.Start();
        ResponseRelease = ButtonColliderResponse.None;  // important line - we want no response on release. It should be controlled by the toggle.
        _toggleVisuals = GetComponent<TXRButtonToggleVisuals>();
    }

    public override void OnPressed(Transform toucher)
    {
        if (State != ButtonState.Interactable) return;
        if (toucher != ActiveToucher) return;

        if (!isPressed)     // get toggled on - call press animation without release.
        {
            DelegateInteralExtenralResponses(ResponsePress, OnPressedInternal, ToggleOn);
            ToggleState = TXRButtonToggleState.On;
        }
        else
        {                   // get toggled off - call release animation.
            DelegateInteralExtenralResponses(ButtonColliderResponse.Both, OnReleasedInternal, ToggleOff);
            ToggleState = TXRButtonToggleState.Off;
        }
    }

    // only change- do not change state to active.
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

        if (ToggleState == TXRButtonToggleState.On)
        {
            visuals.SetState(EButtonAnimationState.Press);
        }
        else
        {
            visuals.SetState(EButtonAnimationState.Active);
        }

    }

    protected override void OnHoverEnterInternal()
    {
        isHovered = true;
        PlaySound(soundHoverEnter);
        visuals.SetState(EButtonAnimationState.Hover);
    }

    public void TriggerToggleEvent(TXRButtonToggleState state, ButtonColliderResponse response)
    {
        UnityEvent toggleEvent = state == TXRButtonToggleState.On ? ToggleOn : ToggleOff;
        Action internalAction = state == TXRButtonToggleState.On ? OnPressedInternal : OnReleasedInternal;

        DelegateInteralExtenralResponses(response, internalAction, toggleEvent);
    }*/
}

public enum TXRButtonToggleState { On, Off }