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

    protected TXRButtonVisuals visuals;
    protected bool isPressed = false;
    protected bool isHovered = false;

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

        buttonSurface = References.ButtonSurface;
        soundPress = References.SoundPress;
        soundRelease = References.SoundRelease;

        lastState = state;
        SetState(state);
    }

    void Update()
    {
        if (lastState != state)
        {
            SetState(state);
            lastState = state;
        }

        if (state != ButtonState.Interactable) return;


        if (isHovered)
        {
            distanceToucherFromButtonClamped = GetToucherToButtonDistance(activeToucher.position, buttonSurface.position);
        }
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

    public void SetColor(EButtonAnimationState state, Color color, float duration = 0.25f)
    {
        visuals.SetColor(state, color, duration);
    }

    public void SetDefaultColor(EButtonAnimationState state)
    {
        Color color = References.Configurations.activeColor;

        switch (state)
        {
            case EButtonAnimationState.Active:
                color = References.Configurations.activeColor;
                break;
            case EButtonAnimationState.Press:
                color = References.Configurations.pressColor;
                break;
            case EButtonAnimationState.Disable:
                color = References.Configurations.disableColor;
                break;
        }

        visuals.SetColor(state, color);
    }


    // TODO: Move to interactor
    private float GetToucherToButtonDistance(Vector3 toucherPosition, Vector3 buttonSurfacePosition)
    {
        float distanceToucherFromButtom = (toucherPosition - buttonSurfacePosition).sqrMagnitude;
        float distnaceClamped = 1 - ((distanceToucherFromButtom - HOVER_DISTANCE_MIN) / (HOVER_DISTANCE_MAX - HOVER_DISTANCE_MIN));
        return Mathf.Clamp01(distnaceClamped);
    }
    protected void PlaySound(AudioSource sound)
    {
        if (sound == null || !ShouldPlaySounds) return;
        sound.Stop();
        sound.Play();
    }

    // Called from Hover Collider on its public UnityEvent
    public virtual void OnHoverEnter(Transform toucher)
    {
        if (state != ButtonState.Interactable) return;

        var ShouldContinueAfterToucherEnter = HoverEnterToucherProcess(toucher);
        if (!ShouldContinueAfterToucherEnter) return;

        DelegateInteralExtenralResponses(ResponseHoverEnter, OnHoverEnterInternal, HoverEnter);
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

    private bool HoverEnterToucherProcess(Transform toucher)
    {
        if (touchers.Count == 0)
        {
            touchers.Add(toucher);
            activeToucher = toucher;
            return true;
        }
        else
        {
            touchers.Add(toucher);
            return false;
        }
    }

    protected virtual void OnHoverEnterInternal()
    {
        isHovered = true;
        PlaySound(soundHoverEnter);
        visuals.SetState(EButtonAnimationState.Hover);
    }

    // called from button collider

    public virtual void OnHoverExit(Transform toucher)
    {
        if (state != ButtonState.Interactable) return;
        print("HOVER EXIT");
        // Catching extreme cases where toucher exit the hover collider without activating the press collider
        if (isPressed)
        {
            OnReleased(toucher);
        }

        var ShouldContinueAfterToucherExit = HoverExitToucherProcessing(toucher);
        if (!ShouldContinueAfterToucherExit) return;

        DelegateInteralExtenralResponses(ResponseHoverExit, OnHoverExitInternal, HoverExit);
    }

    protected bool HoverExitToucherProcessing(Transform toucher)
    {
        touchers.Remove(toucher);
        if (toucher != activeToucher) return false;
        else
        {
            if (touchers.Count > 0)
            {
                activeToucher = touchers.Last();
                return false;
            }
            else
            {
                activeToucher = null;
            }
        }
        return true;
    }

    protected virtual void OnHoverExitInternal()
    {
        isHovered = false;
        activeToucher = null;
        PlaySound(soundHoverExit);
        visuals.SetState(EButtonAnimationState.Active);
    }

    // called from the UnityEvent on the press collider
    public virtual void OnPressed(Transform toucher)
    {
        if (state != ButtonState.Interactable) return;

        if (toucher != activeToucher) return;

        PressTransform?.Invoke(toucher);
        DelegateInteralExtenralResponses(ResponsePress, OnPressedInternal, Pressed);
    }

    protected virtual void OnPressedInternal()
    {
        isPressed = true;
        PlaySound(soundPress);
        visuals.SetState(EButtonAnimationState.Press);
    }

    // called from the UnityEvent on the press collider
    public virtual void OnReleased(Transform toucher)
    {
        if (state != ButtonState.Interactable) return;

        if (toucher != activeToucher) return;

        DelegateInteralExtenralResponses(ResponseRelease, OnReleasedInternal, Released);
    }

    protected virtual void OnReleasedInternal()
    {
        isPressed = false;
        PlaySound(soundRelease);
        visuals.SetState(EButtonAnimationState.Active);
    }

    // added as a quick fix for bug where title on sun nav wouldn't clear active toucher after touch (probably because it moves immediately to keyboard position.
    public void ClearActiveToucher()
    {
        OnHoverExitInternal();
        touchers.Clear();
    }
}

public enum ButtonColliderResponse { Both, Internal, External, None }
public enum ButtonEvent { HoverEnter, Pressed, Released, HoverExit }
public enum ButtonState { Hidden, Disabled, Interactable, Frozen }