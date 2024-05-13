using UnityEngine;
using DG.Tweening;
using TMPro;
using NaughtyAttributes;

public enum EButtonAnimationState { Show, Hide, Active, Disable, Hover, Press }
public class TXRButtonVisuals : MonoBehaviour
{
    private Shapes.Rectangle _backface;
    private Shapes.Rectangle _stroke;
    private TextMeshPro _text;
    private ButtonVisualsConfigurations _configurations;

    Sequence _backfaceColorSequence;
    Sequence _backfaceGradientSequence;
    Sequence _backfaceZValueSequence;
    Sequence _strokeThicknessSequence;

    Color _pressedColor;

    public void Init(TXRButtonReferences references)
    {
        _backface = references.Backface;
        _stroke = references.Stroke;
        _text = references.Text;
        _configurations = references.Configurations;

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
        SetBackfaceColor(_configurations.backfaceColorActive, _configurations.activeDuration);
        SetBackfaceZ(_configurations.backfaceZPositionActive);
        SetHoverGradient(false);
        SetStrokeThickness(_configurations.strokeThicknessActive);
    }

    public void Show()
    {
        Active();
    }

    public void Hide()
    {
        SetBackfaceColor(_configurations.backfaceColorHide, _configurations.hideDuration);
        SetHoverGradient(false);
        SetStrokeThickness(0);
    }

    public void Hover()
    {
        SetHoverGradient(true);
        SetBackfaceZ(_configurations.backfadeZPositionHover);
        SetStrokeThickness(_configurations.strokeThicknessHover);
    }

    public void Press()
    {
        SetBackfaceZ(_configurations.backfadeZPositionPress);
        SetHoverGradient(true);
        SetBackfaceColor(_pressedColor);
        SetStrokeThickness(_configurations.strokeThicknessPress);
    }

    public void Disabled()
    {
        SetHoverGradient(false);
        SetBackfaceColor(_configurations.backfaceColorDisabled);
        SetStrokeThickness(_configurations.strokeThicknessActive);
    }

    public void SetPressedColor(Color backfaceColor, float duration = 0.25f)
    {
        _pressedColor = backfaceColor;
        Press();
    }

    private void SetHoverGradient(bool isOn, float duration = 0.25f)
    {
        float gradientRadius = isOn ? _configurations.backfaceGradientRadiusHover : 0;

        _backfaceGradientSequence.Kill();

        _backfaceGradientSequence = DOTween.Sequence();
        _backfaceGradientSequence.Append(DOVirtual.Float(_backface.FillRadialRadius, gradientRadius, duration, t => { _backface.FillRadialRadius = t; }));
        _backfaceGradientSequence.Join(DOVirtual.Color(_backface.FillColorStart, _configurations.backfaceColorGradientHover, duration, t => { _backface.FillColorStart = t; }));
    }

    private void SetBackfaceZ(float zValue, float duration = 0.25f)
    {
        Vector3 backfaceLocalPosition = _backface.transform.localPosition;
        backfaceLocalPosition.z = zValue;

        _backfaceZValueSequence.Kill();
        _backfaceZValueSequence = DOTween.Sequence();
        _backfaceZValueSequence.Append(_backface.transform.DOLocalMove(backfaceLocalPosition, duration));
    }

    private void SetStrokeThickness(float thickness, float duration = 0.25f)
    {
        _strokeThicknessSequence.Kill();
        _strokeThicknessSequence = DOTween.Sequence();
        _strokeThicknessSequence.Append(DOVirtual.Float(_stroke.Thickness, thickness, duration, t => { _stroke.Thickness = t; }));
    }

    private void SetBackfaceColor(Color backfaceColor, float duration = 0.25f)
    {
        _backfaceColorSequence.Kill();
        _backfaceColorSequence = DOTween.Sequence();
        _backfaceColorSequence.Append(DOVirtual.Color(_backface.FillColorEnd, backfaceColor, duration, t => { _backface.FillColorEnd = t; }));
    }

}
