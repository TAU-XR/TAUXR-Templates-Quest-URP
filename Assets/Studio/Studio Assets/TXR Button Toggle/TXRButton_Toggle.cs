using System;
using UnityEngine;
using UnityEngine.Events;


public class TXRButton_Toggle : TXRButton
{
    public Action<TXRButton_Toggle> ToggledOn;
    public Action<TXRButton_Toggle> ToggledOff;
    public UnityEvent ToggleOn;
    public UnityEvent ToggleOff;
    public TXRButtonToggleState ToggleState;
    public ButtonColliderResponse StartingStateResponse;
    private TXRButtonToggleVisuals _toggleVisuals;

    public Color ToggledColor;

    public override void Init()
    {
        base.Init();
        _toggleVisuals = _visuals.GetComponent<TXRButtonToggleVisuals>();
        TriggerToggleEvent(ToggleState, StartingStateResponse);
    }

    public void TriggerToggleEvent(TXRButtonToggleState state, ButtonColliderResponse response)
    {
        ToggleState = state;
        UnityEvent toggleEvent = state == TXRButtonToggleState.On ? ToggleOn : ToggleOff;
        Action<TXRButton_Toggle> actionToInvoke = state == TXRButtonToggleState.On ? ToggledOn : ToggledOff;
        actionToInvoke?.Invoke(this);
        Action internalAction = OnReleasedInternal; // toggle is change only on  release

        DelegateInteralExtenralResponses(response, internalAction, toggleEvent);
    }

    public override void TriggerButtonEventFromInput(ButtonEvent buttonEvent)
    {
        if (!IsInteractable) return;

        switch (buttonEvent)
        {
            case ButtonEvent.HoverEnter:
                DelegateInteralExtenralResponses(ResponseHoverEnter, OnHoverEnterInternal, HoverEnter);
                break;

            case ButtonEvent.HoverExit:
                DelegateInteralExtenralResponses(ResponseHoverExit, OnHoverExitInternal, HoverExit);
                break;

            case ButtonEvent.Pressed:
                DelegateInteralExtenralResponses(ResponsePress, OnPressedInternal, Pressed);
                break;

            case ButtonEvent.Released:
                ToggleState = ToggleState == TXRButtonToggleState.On ? TXRButtonToggleState.Off : TXRButtonToggleState.On;

                UnityEvent toggleEvent = ToggleState == TXRButtonToggleState.On ? ToggleOn : ToggleOff;
                toggleEvent.Invoke();
                Action<TXRButton_Toggle> actionToInvoke = ToggleState == TXRButtonToggleState.On ? ToggledOn : ToggledOff;
                actionToInvoke?.Invoke(this);
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

public enum TXRButtonToggleState
{
    On,
    Off
}