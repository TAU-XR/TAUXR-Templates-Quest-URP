using UnityEngine;
using Shapes;
using DG.Tweening;
public class PositioningHandleVisuals : MonoBehaviour
{
    public float _duration;

    [Header("UI Elements")]
    [SerializeField] Line _handle;
    [SerializeField] Line _background;

    [Header("UI States")]
    [SerializeField] Line _handleHover;
    [SerializeField] Line _handlePinched;
    [SerializeField] Line _handleActive;
    [SerializeField] Line _backgroundHover;
    [SerializeField] Line _backgroundPinched;
    [SerializeField] Line _backgroundActive;
    [SerializeField] GameObject _UIStatesParent;

    private Tween _backgroundTween;

    private void Awake()
    {
        _UIStatesParent.SetActive(false);
    }

    public void SetActive(bool withAnimation = true)
    {
        AnimateToState(_handleActive, _backgroundActive, withAnimation);
    }

    public void SetHover(bool withAnimation = true)
    {
        AnimateToState(_handleHover, _backgroundHover, withAnimation);
    }

    public void SetPinched(bool withAnimation = true)
    {
        AnimateToState(_handlePinched, _backgroundPinched, withAnimation);
    }

    private void AnimateToState(Line handleState, Line backgroundState, bool withAnimation = true)
    {
        _backgroundTween.Kill();
        float duration = withAnimation ? _duration : 0.01f;

        Vector3 handleStart = _handle.Start;
        Vector3 handleEnd = _handle.End;
        float handleThickness = _handle.Thickness;

        Color backgroundColor = _background.Color;

        _backgroundTween = DOVirtual.Float(0, 1, duration, t =>
        {
            _handle.Start = Vector3.Lerp(handleStart, handleState.Start, t);
            _handle.End = Vector3.Lerp(handleEnd, handleState.End, t);
            _handle.Thickness = Mathf.Lerp(handleThickness, handleState.Thickness, t);

            _background.Color = Color.Lerp(backgroundColor, backgroundState.Color, t);
        });

    }

}
