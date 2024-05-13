using UnityEngine;
using DG.Tweening;
using TMPro;
using NaughtyAttributes;

public enum EButtonAnimationState { Show, Hide, Active, Disable, Hover, Press }
public class TXRButtonVisuals : MonoBehaviour
{
    [SerializeField] private Shapes.Rectangle _backface;
    [SerializeField] private Shapes.Rectangle _stroke;
    [SerializeField] private TextMeshPro _text;
    [SerializeField] private ButtonVisualsConfigurations _configurations;
    EButtonAnimationState _state;
    Sequence _animationSequence;
    Sequence _backfaceColorSequence;
    Sequence _hoverSequence;

    Color _pressedColor;

    public void Init()
    {
        _pressedColor = _configurations.backfaceColorPress;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Active();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Hide();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Hover();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Press();
        }
    }

    public void Active()
    {
        // color - active.
        // press - off
        _state = EButtonAnimationState.Active;
        _animationSequence.Kill();
        InitSequence(_configurations.backfaceColorActive, _configurations.backfaceColorActive, _configurations.backfaceGradientActive, _configurations.backfadeZPositionActive, _configurations.strokeThicknessActive, _configurations.activeDuration);
        _animationSequence.Restart();
    }

    public void Show()
    {
        // active
        // hover - off
        Active();
    }

    public void Hide()
    {
        // active.
        // hover - off
        // color - transparent.
        // set stroke transparent.
        _state = EButtonAnimationState.Hide;
        _animationSequence.Kill();
        InitSequence(_configurations.backfaceColorHide, _configurations.backfaceColorHide, _configurations.backfaceGradientActive, _configurations.backfadeZPositionActive, 0, _configurations.hideDuration);
        _animationSequence.Restart();
    }

    public void Hover()
    {
        // hover - on
        _state = EButtonAnimationState.Hover;
        _animationSequence.Kill();
        InitSequence(_configurations.backfaceColorGradientHover, _configurations.backfaceColorActive, _configurations.backfaceGradientHover, _configurations.backfadeZPositionHover, _configurations.strokeThicknessHover, _configurations.hoverDuration);
        _animationSequence.Restart();
    }

    public void Press()
    {
        // hover - on
        // press - on
        // color - press
        _state = EButtonAnimationState.Press;
        _animationSequence.Kill();
        InitSequence(_configurations.backfaceColorPress, _pressedColor, _configurations.backfaceGradientActive, _configurations.backfadeZPositionPress, _configurations.strokeThicknessPress, _configurations.pressDuration);
        _animationSequence.Restart();
    }

    public void Disabled()
    {
        // hover - off
        // color - disabled
        _state = EButtonAnimationState.Disable;
        _animationSequence.Kill();
        InitSequence(_configurations.backfaceColorDisabled, _configurations.backfaceColorDisabled, _configurations.backfaceGradientActive, 0, _configurations.strokeThicknessActive, _configurations.activeDuration);
        _animationSequence.Restart();
    }

    private void InitSequence(Color backfaceFillStart, Color backfaceFillEnd, float backfaceGradientRadius, float backfaceZOffset, float strokeThickness, float duration)
    {
        _animationSequence = DOTween.Sequence();
        _animationSequence.Append(DOVirtual.Color(_backface.FillColorStart, backfaceFillStart, duration, t => { _backface.FillColorStart = t; }));
        _animationSequence.Join(DOVirtual.Color(_backface.FillColorEnd, backfaceFillEnd, duration, t => { _backface.FillColorEnd = t; }));
        _animationSequence.Join(DOVirtual.Float(_backface.FillRadialRadius, backfaceGradientRadius, duration, t => { _backface.FillRadialRadius = t; }));

        Vector3 backfaceLocalPosition = _backface.transform.localPosition;
        backfaceLocalPosition.z = backfaceZOffset;
        _animationSequence.Join(_backface.transform.DOLocalMove(backfaceLocalPosition, duration));

        _animationSequence.Join(DOVirtual.Float(_stroke.Thickness, strokeThickness, duration, t => { _stroke.Thickness = t; }));

        _animationSequence.SetAutoKill(false);
        _animationSequence.Pause();
    }

    public void SetPressedColor(Color backfaceColor, float duration = 0.25f)
    {
        _pressedColor = backfaceColor;
        Press();

        /* 
          for future refactor

        _backfaceColorSequence.Kill();

        // Backface color fill end is the actual color. Fill start affects hover gradient.
        _backfaceColorSequence = DOTween.Sequence();
        _backfaceColorSequence.Append(DOVirtual.Color(_backface.FillColorEnd, backfaceColor, duration, t => { _backface.FillColorEnd = t; }));
        */
    }

    private void SetHover(bool isOn)
    {
        // Apply gradient by changing the backface gradient and fill start color. Then affect stroke position.
        SetBackfaceGradient(_configurations.backfaceColorGradientHover, _configurations.backfaceGradientHover, _configurations.hoverDuration);
    }

    private void SetPressed(bool isOn)
    {

    }
    private void SetBackfaceColor(Color gradientColor, Color backfaceColor, float duration)
    {
        _backfaceColorSequence = DOTween.Sequence();
        _backfaceColorSequence.Append(DOVirtual.Color(_backface.FillColorEnd, backfaceColor, duration, t => { _backface.FillColorEnd = t; }));
    }
    private void SetBackfaceGradient(Color gradientColor, float gradientRadius, float duration = .25f)
    {
        _hoverSequence.Kill();

        _hoverSequence = DOTween.Sequence();
        _hoverSequence.Append(DOVirtual.Float(_backface.FillRadialRadius, gradientRadius, duration, t => { _backface.FillRadialRadius = t; }));
        _hoverSequence.Join(DOVirtual.Color(_backface.FillColorStart, gradientColor, duration, t => { _backface.FillColorStart = t; }));
    }

}
