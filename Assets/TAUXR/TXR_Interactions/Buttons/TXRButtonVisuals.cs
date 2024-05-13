using UnityEngine;
using DG.Tweening;
using TMPro;

public enum EButtonAnimationState { Show, Hide, Active, Disable, Hover, Press }
public class TXRButtonVisuals : MonoBehaviour
{
    [SerializeField] private Shapes.Rectangle _backface;
    [SerializeField] private Shapes.Rectangle _stroke;
    [SerializeField] private TextMeshPro _text;
    [SerializeField] private ButtonVisualsConfigurations _configurations;

    Sequence _animationSequence;
    Sequence _backfaceColorSequence;
    Sequence _backfaceGradientSequence;

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
        _animationSequence.Kill();
        InitSequence(_configurations.backfaceColorActive, _configurations.backfaceColorActive, _configurations.backfaceGradientActive, _configurations.backfadeZPositionActive, _configurations.strokeThicknessActive, _configurations.activeDuration);
        _animationSequence.Restart();
    }

    public void Show()
    {
        Active();
    }

    public void Hide()
    {
        _animationSequence.Kill();
        InitSequence(_configurations.backfaceColorHide, _configurations.backfaceColorHide, _configurations.backfaceGradientActive, _configurations.backfadeZPositionActive, 0, _configurations.hideDuration);
        _animationSequence.Restart();
    }

    public void Hover()
    {
        _animationSequence.Kill();
        InitSequence(_configurations.backfaceColorGradientHover, _configurations.backfaceColorActive, _configurations.backfaceGradientHover, _configurations.backfadeZPositionHover, _configurations.strokeThicknessHover, _configurations.hoverDuration);
        _animationSequence.Restart();
    }

    public void Press()
    {
        _animationSequence.Kill();
        InitSequence(_configurations.backfaceColorPress, _configurations.backfaceColorPress, _configurations.backfaceGradientActive, _configurations.backfadeZPositionPress, _configurations.strokeThicknessPress, _configurations.pressDuration);
        _animationSequence.Restart();
    }

    public void Disabled()
    {
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

   /* public void SetColor(Color backfaceColor)
    {

    }

    private void ApplyHoverGradient()
    {

    }

    private void SetBackfaceColor(Color backfaceFillStart, Color backfaceFillEnd, float duration)
    {
        _backfaceColorSequence = DOTween.Sequence();
        _backfaceColorSequence.Append(DOVirtual.Color(_backface.FillColorStart, backfaceFillStart, duration, t => { _backface.FillColorStart = t; }));
        _backfaceColorSequence.Join(DOVirtual.Color(_backface.FillColorEnd, backfaceFillEnd, duration, t => { _backface.FillColorEnd = t; }));
    }
    private void SetBackfaceGradient(float backfaceGradientRadius, float duration)
    {
        _backfaceGradientSequence = DOTween.Sequence();
        _backfaceGradientSequence.Append(DOVirtual.Float(_backface.FillRadialRadius, backfaceGradientRadius, duration, t => { _backface.FillRadialRadius = t; }));
        _backfaceColorSequence.Join(DOVirtual.Color(_backface.FillColorEnd, backfaceFillEnd, duration, t => { _backface.FillColorEnd = t; }));
    }*/

}
