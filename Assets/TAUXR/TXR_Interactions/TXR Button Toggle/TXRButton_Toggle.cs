using System;
using UnityEngine.Events;


public class TXRButton_Toggle : TXRButton
{
    public UnityEvent ToggleOn;
    public UnityEvent ToggleOff;
    public TXRButtonToggleState ToggleState;
    public ButtonColliderResponse StartingStateResponse;
    private TXRButtonToggleVisuals _toggleVisuals;

    protected override void Init()
    {
        base.Init();
        _toggleVisuals = visuals.GetComponent<TXRButtonToggleVisuals>();
        TriggerToggleEvent(ToggleState, StartingStateResponse);
    }

    public void TriggerToggleEvent(TXRButtonToggleState state, ButtonColliderResponse response)
    {
        ToggleState = state;
        UnityEvent toggleEvent = state == TXRButtonToggleState.On ? ToggleOn : ToggleOff;
        Action internalAction = OnReleasedInternal; // toggle is change only on release

        DelegateInteralExtenralResponses(response, internalAction, toggleEvent);
    }

    public override void TriggerButtonEventFromInput(ButtonEvent buttonEvent)
    {
        if (State != ButtonState.Interactable) return;

        switch (buttonEvent)
        {
            case ButtonEvent.HoverEnter:
                DelegateInteralExtenralResponses(ResponseHoverEnter, OnHoverEnterInternal, HoverEnter);
                break;

            case ButtonEvent.HoverExit:
                DelegateInteralExtenralResponses(ResponseHoverExit, OnHoverExitInternal, HoverExit);
                break;

            case ButtonEvent.Pressed:
                ToggleState = ToggleState == TXRButtonToggleState.On ? TXRButtonToggleState.Off : TXRButtonToggleState.On;
                DelegateInteralExtenralResponses(ResponsePress, OnPressedInternal, Pressed);
                break;

            case ButtonEvent.Released:
                UnityEvent toggleEvent = ToggleState == TXRButtonToggleState.On ? ToggleOn : ToggleOff;
                toggleEvent.Invoke();
                DelegateInteralExtenralResponses(ResponseRelease, OnReleasedInternal, Released);
                break;
        }
    }

    protected override void OnReleasedInternal()
    {
        _toggleVisuals.SetToggleState(ToggleState == TXRButtonToggleState.On);
        base.OnReleasedInternal();
    }
}

public enum TXRButtonToggleState { On, Off }