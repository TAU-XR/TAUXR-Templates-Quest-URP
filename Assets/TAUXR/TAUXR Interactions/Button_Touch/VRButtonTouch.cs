using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

// TODO: Only press from forward

public class VRButtonTouch : MonoBehaviour
{
    VRButtonState state;

    [SerializeField] private Transform buttonSurface;

    private Transform activeToucher;
    private List<Transform> touchers = new List<Transform>();

    private float HOVER_DISTANCE_MIN = .00069f;
    private float HOVER_DISTANCE_MAX = .0028f;

    private float distanceToucherFromButtonClamped;

    public Transform ActiveToucher => activeToucher;
    public float DistanceToucherFromButton => distanceToucherFromButtonClamped;

    [SerializeField] private ButtonColliderResponse ResponseHoverEnter;
    public UnityEvent HoverEnter;

    [SerializeField] private ButtonColliderResponse ResponseHoverExit;
    public UnityEvent HoverExit;

    [SerializeField] private ButtonColliderResponse ResponsePress;
    public UnityEvent Pressed;

    [SerializeField] private ButtonColliderResponse ResponseRelease;
    public UnityEvent Released;


    public AudioSource soundDisabled;
    public AudioSource soundActive;
    public AudioSource soundHoverEnter;
    public AudioSource soundHoverExit;
    public AudioSource soundPress;
    public AudioSource soundRelease;
    public Animator animator;

    private bool isPressed = false;
    private bool isHovered = false;

    void Start()
    {

    }

    void Update()
    {
        if (isHovered)
        {
            distanceToucherFromButtonClamped = GetToucherToButtonDistance(activeToucher.position, buttonSurface.position);
        }
    }

    private float GetToucherToButtonDistance(Vector3 toucherPosition, Vector3 buttonSurfacePosition)
    {
        float distanceToucherFromButtom = (toucherPosition - buttonSurfacePosition).sqrMagnitude;
        float distnaceClamped = 1 - ((distanceToucherFromButtom - HOVER_DISTANCE_MIN) / (HOVER_DISTANCE_MAX - HOVER_DISTANCE_MIN));
        return Mathf.Clamp01(distnaceClamped);
    }

    public void SetState(VRButtonState state)
    {
        switch (state)
        {
            case VRButtonState.Disabled:
                break;

            case VRButtonState.Active:
                break;

            case VRButtonState.Pressed:
                break;
        }
    }
    private void PlaySound(AudioSource sound)
    {
        if (sound == null) return;
        sound.Stop();
        sound.Play();
    }
    public void OnHoverEnter(Transform toucher)
    {
        var ShouldContinueAfterToucherEnter = HoverEnterToucherProcess(toucher);
        if (!ShouldContinueAfterToucherEnter) return;

        isHovered = true;

        DelegateInteralExtenralResponses(ResponseHoverEnter, OnHoverEnterInternal, HoverEnter);
    }

    private void DelegateInteralExtenralResponses(ButtonColliderResponse response, Action internalAction, UnityEvent externalEvent)
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

    private void OnHoverEnterInternal()
    {
        PlaySound(soundHoverEnter);
        animator.SetBool("IsHover", true);
    }

    public void OnHoverExit(Transform toucher)
    {
        // Catching extreme cases where toucher exit the hover collider without activating the press collider
        if (isPressed)
        {
            OnReleased(toucher);
        }

        var ShouldContinueAfterToucherExit = HoverExitToucherProcessing(toucher);
        if (!ShouldContinueAfterToucherExit) return;

        isHovered = false;
        activeToucher = null;

        DelegateInteralExtenralResponses(ResponseHoverExit, OnHoverExitInternal, HoverExit);
    }

    private bool HoverExitToucherProcessing(Transform toucher)
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

    private void OnHoverExitInternal()
    {
        PlaySound(soundHoverExit);
        animator.SetBool("IsHover", false);
    }

    public void OnPressed(Transform toucher)
    {
        if (toucher != activeToucher) return;

        DelegateInteralExtenralResponses(ResponsePress, OnPressedInternal, Pressed);
    }

    private void OnPressedInternal()
    {
        isPressed = true;
        PlaySound(soundPress);
        animator.SetBool("IsPressed", true);
    }

    public void OnReleased(Transform toucher)
    {
        if (toucher != activeToucher) return;

        DelegateInteralExtenralResponses(ResponseRelease, OnReleasedInternal, Released);
    }

    private void OnReleasedInternal()
    {
        isPressed = false;
        PlaySound(soundRelease);
        animator.SetBool("IsPressed", false);
    }

    public void ButtonColliderEnter(ButtonColliderType colliderType, Collider detectedCollider)
    {
        if (!detectedCollider.CompareTag("Toucher"))
            return;

        switch (colliderType)
        {
            case ButtonColliderType.Press:
                OnPressed(detectedCollider.transform);
                break;

            case ButtonColliderType.Hover:
                OnHoverEnter(detectedCollider.transform);
                break;
        }
    }

    public void ButtonColliderExit(ButtonColliderType colliderType, Collider detectedCollider)
    {
        if (!detectedCollider.CompareTag("Toucher"))
            return;

        switch (colliderType)
        {
            case ButtonColliderType.Press:
                OnReleased(detectedCollider.transform);
                break;

            case ButtonColliderType.Hover:
                OnHoverExit(detectedCollider.transform);
                break;
        }
    }


    // used for external scripts that want to manipulate buttons regardless of touchers.
    public void InvokeButtonEvent(ButtonEvent buttonEvent, ButtonColliderResponse response)
    {
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
}
public enum VRButtonState { Disabled, Active, Pressed }
public enum ButtonColliderResponse { Both, Internal, External, None }

public enum ButtonEvent { HoverEnter, HoverExit, Pressed, Released }