using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ButtonColliderType { Press, Hover}

public class ButtonFingerTouch : MonoBehaviour
{
    Transform activeToucher;
    public Transform buttonSurface;

    bool IsHovering = false;

    float HOVER_DISTANCE_MIN = .00069f;
    float HOVER_DISTANCE_MAX = .0028f;

    public float DistanceToucherFromButton => distanceToucherFromButtonClamped;
    public float distanceToucherFromButtonClamped;

    public UnityEvent OnHoverEnter, OnHoverExit;
    public UnityEvent OnButtonPress, OnButtonRelease;

    void Start()
    {
        
    }

    void Update()
    {
        if(IsHovering)
        {
            SignHover();
        }
    }

    private void ToucherHoverEnter(Transform toucher)
    {
        // ignore more than one toucher at a time.
        if (activeToucher != null) return;

        activeToucher = toucher;
        IsHovering = true;

        OnHoverEnter.Invoke();
    }

    private void ToucherHoverExit(Transform toucher)
    {
        // return if the exiting toucher is not the active toucher
        if (toucher != activeToucher) return;

        IsHovering = false;
        activeToucher = null;

        OnHoverExit.Invoke();
    }

    private void SignHover()
    {
        float distanceToucherFromButtom = (activeToucher.position - buttonSurface.position).sqrMagnitude;
        distanceToucherFromButtonClamped = 1 - ((distanceToucherFromButtom - HOVER_DISTANCE_MIN) / (HOVER_DISTANCE_MAX - HOVER_DISTANCE_MIN));
        distanceToucherFromButtonClamped  = Mathf.Clamp(distanceToucherFromButtonClamped, 0, 1);
    }

    private void ButtonPressed()
    {
        print("Button pressed");
        OnButtonPress.Invoke();
    }

    private void ButtonReleased()
    {
        print("Button released");
        OnButtonRelease.Invoke();
    }


    public void ButtonColliderEnter(ButtonColliderType colliderType, Collider detectedCollider)
    {
        // ignore colliders that are not touchers.
        if (!detectedCollider.CompareTag("Toucher"))
            return;

        switch(colliderType)
        {
            case ButtonColliderType.Press:
                ButtonPressed();
                break;

            case ButtonColliderType.Hover:
                ToucherHoverEnter(detectedCollider.transform);
                break;
        }
    }

    public void ButtonColliderExit(ButtonColliderType colliderType, Collider detectedCollider)
    {
        switch (colliderType)
        {
            case ButtonColliderType.Press:
                ButtonReleased();
                break;

            case ButtonColliderType.Hover:
                ToucherHoverExit(detectedCollider.transform);
                break;
        }
    }
}
