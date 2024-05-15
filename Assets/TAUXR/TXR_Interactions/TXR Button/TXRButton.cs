using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Events;

/*TODO
 * - Pass toucher transform in a more elegant way
 * - refactor input processing to another script and get rid of the hovera and press colliders
 * - easy change button text & color
 */

public class TXRButton : MonoBehaviour
{
    public ButtonState State => state;
    public bool ShouldPlaySounds = true;

    [SerializeField] private ButtonState state = ButtonState.Interactable;
    private ButtonState lastState;
    private Transform buttonSurface;

    public Transform activeToucher;
    private List<Transform> touchers = new List<Transform>();

    private float HOVER_DISTANCE_MIN = .00069f;
    private float HOVER_DISTANCE_MAX = .0028f;

    private float distanceToucherFromButtonClamped;

    public Transform ActiveToucher => activeToucher;
    public float DistanceToucherFromButton => distanceToucherFromButtonClamped;

    [SerializeField] protected ButtonColliderResponse ResponseHoverEnter;
    public UnityEvent HoverEnter;

    [SerializeField] protected ButtonColliderResponse ResponseHoverExit;
    public UnityEvent HoverExit;

    [SerializeField] protected ButtonColliderResponse ResponsePress;
    public UnityEvent Pressed;

    [SerializeField] protected ButtonColliderResponse ResponseRelease;
    public UnityEvent Released;


    protected AudioSource soundDisabled;
    protected AudioSource soundActive;
    protected AudioSource soundHoverEnter;
    protected AudioSource soundHoverExit;
    protected AudioSource soundPress;
    protected AudioSource soundRelease;

    protected TXRButtonInput input;

    protected TXRButtonVisuals visuals;

    public Action<Transform> PressTransform;
    public TXRButtonReferences References;
    protected virtual void Start()
    {
        Init();
    }

    protected void Init()
    {
        visuals = References.ButtonVisuals;
        visuals.Init(References);

        input = References.ButtonInput;
        input.Init(References);

        buttonSurface = References.ButtonSurface;
        soundPress = References.SoundPress;
        soundRelease = References.SoundRelease;

        lastState = state;
        SetState(state);
    }

    [Button]
    private void SetState()
    {
        SetState(state);
    }

    public void SetState(ButtonState state)
    {
        switch (state)
        {
            case ButtonState.Hidden:
                visuals.SetState(EButtonAnimationState.Hide);
                break;
            case ButtonState.Disabled:
                visuals.SetState(EButtonAnimationState.Disable);
                break;
            case ButtonState.Interactable:
                visuals.SetState(EButtonAnimationState.Active);
                break;
            case ButtonState.Frozen:
                break;
        }

        this.state = state;
    }

    // used for external scripts that want to manipulate buttons regardless of touchers.
    public virtual void TriggerButtonEvent(ButtonEvent buttonEvent, ButtonColliderResponse response)
    {
        switch (buttonEvent)
        {
            case ButtonEvent.HoverEnter:
                DelegateInteralExtenralResponses(response, OnHoverEnterInternal, HoverEnter);
                break;

            case ButtonEvent.HoverExit:
                DelegateInteralExtenralResponses(response, OnHoverExitInternal, HoverExit);
                break;

            case ButtonEvent.Pressed:
                DelegateInteralExtenralResponses(response, OnPressedInternal, Pressed);
                break;

            case ButtonEvent.Released:
                DelegateInteralExtenralResponses(response, OnReleasedInternal, Released);
                break;
        }
    }

    public void TriggerButtonEventFromInput(ButtonEvent buttonEvent)
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
                DelegateInteralExtenralResponses(ResponsePress, OnPressedInternal, Pressed);
                break;

            case ButtonEvent.Released:
                DelegateInteralExtenralResponses(ResponseRelease, OnReleasedInternal, Released);
                break;
        }
    }

    public void SetColor(EButtonAnimationState state, Color color, float duration = 0.25f)
    {
        visuals.SetColor(state, color, duration);
    }

    protected void PlaySound(AudioSource sound)
    {
        if (sound == null || !ShouldPlaySounds) return;
        sound.Stop();
        sound.Play();
    }

    protected void DelegateInteralExtenralResponses(ButtonColliderResponse response, Action internalAction, UnityEvent externalEvent)
    {
        switch (response)
        {
            case ButtonColliderResponse.None:
                break;
            case ButtonColliderResponse.Both:
                externalEvent.Invoke();
                internalAction();
                break;
            case ButtonColliderResponse.Internal:
                internalAction();
                break;
            case ButtonColliderResponse.External:
                externalEvent.Invoke();
                break;
        }
    }
    protected virtual void OnHoverEnterInternal()
    {
        PlaySound(soundHoverEnter);
        visuals.SetState(EButtonAnimationState.Hover);
    }
    protected virtual void OnHoverExitInternal()
    {
        PlaySound(soundHoverExit);
        visuals.SetState(EButtonAnimationState.Active);
    }
    protected virtual void OnPressedInternal()
    {
        PlaySound(soundPress);
        visuals.SetState(EButtonAnimationState.Press);
    }
    protected virtual void OnReleasedInternal()
    {
        PlaySound(soundRelease);
        visuals.SetState(EButtonAnimationState.Active);
    }
}

public enum ButtonColliderResponse { Both, Internal, External, None }
public enum ButtonEvent { HoverEnter, Pressed, Released, HoverExit }
public enum ButtonState { Hidden, Disabled, Interactable, Frozen }