using System;

/*
 * Known Issues:
 * - Does not work on multi pinchers - need to handle multiple pinching.
 * 
 * 
 */

public class PinchDetector : APinchable
{
    public Action<PinchManager> PinchEnter;
    public Action PinchExit;
    public Action<PinchManager> PinchHoverEnter;
    public Action<PinchManager> PinchHoverExit;

    public bool IsPinched = false;
    public bool IsHovered = false;
    public override void OnPinchEnter(PinchManager pinchManager)
    {
        base.OnPinchEnter(pinchManager);
        PinchEnter?.Invoke(pinchManager);
        IsPinched = true;
    }
    public override void OnPinchExit()
    {
        base.OnPinchExit();
        PinchExit?.Invoke();
        IsPinched = false;
    }

    public override void OnHoverEnter(PinchManager pinchManager)
    {
        base.OnHoverEnter(pinchManager);
        PinchHoverEnter?.Invoke(pinchManager);
        IsHovered = true;

    }

    public override void OnHoverExit(PinchManager pinchManager)
    {
        base.OnHoverExit(pinchManager);
        PinchHoverExit?.Invoke(pinchManager);
        IsHovered = false;
    }
}
