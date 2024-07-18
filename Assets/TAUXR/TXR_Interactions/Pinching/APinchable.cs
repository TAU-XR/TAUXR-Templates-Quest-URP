using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class APinchable : MonoBehaviour, IComparable<APinchable>
{
    public PinchManager PinchingHandPinchManager { get; set; }

    public virtual float PinchExitThreshold => 0.97f;

    // public virtual int Priority => 0;
    public int Priority;

    protected int _numberOfPinchersInside = 0;
    protected AHoverEffect _hoverEffect;

    private void Awake()
    {
        _hoverEffect = GetComponent<AHoverEffect>();
        DoOnAwake();
    }

    protected virtual void DoOnAwake()
    {
    }

    public virtual void OnHoverEnter(PinchManager pinchManager)
    {
        _numberOfPinchersInside++;
    }

    public virtual void OnHoverStay(PinchManager pinchManager)
    {
        UpdateHoverEffectState(pinchManager, CanBePinched(pinchManager));
    }

    protected void UpdateHoverEffectState(PinchManager pinchManager, bool shouldEffectBeActive)
    {
        if (_hoverEffect != null)
        {
            _hoverEffect.UpdateHoverEffectState(pinchManager, shouldEffectBeActive);
        }
    }

    public virtual void OnHoverExit(PinchManager pinchManager)
    {
        _numberOfPinchersInside--;
        UpdateHoverEffectState(pinchManager, _numberOfPinchersInside > 0);
    }

    public virtual bool CanBePinched(PinchManager pinchManager)
    {
        return true;
    }

    public virtual void OnPinchEnter(PinchManager pinchManager)
    {
        PinchingHandPinchManager = pinchManager;
        pinchManager.PinchedObject = this;
    }

    public virtual void OnPinchExit()
    {
        PinchingHandPinchManager.PinchedObject = null;
        PinchingHandPinchManager = null;
    }

    public int CompareTo(APinchable other)
    {
        if (Equals(other))
        {
            //Duplicate object
            Debug.Log("Duplicate object");
            return 0;
        }

        //return 1 if other is greater, 0 if same, -1 if smaller
        int result = other.Priority.CompareTo(Priority);
        result = result == 0 ? 1 : result;
        return result;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        SortedSetElement other = (SortedSetElement)obj;
        return ReferenceEquals(this, other);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    protected bool IsOtherHand(PinchManager pinchManager)
    {
        return PinchingHandPinchManager != null && PinchingHandPinchManager != pinchManager;
    }
}