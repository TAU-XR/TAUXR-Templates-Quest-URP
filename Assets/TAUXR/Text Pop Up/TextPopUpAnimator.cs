using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Shapes;
using TMPro;

public class TextPopUpAnimator : MonoBehaviour
{
    private Tween _fadeTween;
    private Tween _scaleTween;

    private Rectangle _background;
    private TextMeshPro _textUI;

    [SerializeField] private float _fadeDuration = 0.5f;
    [SerializeField] private float _scaleChangeDuration = 0.5f;

    public void Init(Rectangle background, TextMeshPro textUI)
    {
        _background = background;
        _textUI = textUI;
    }

    public void SetAppearance(bool state, bool useAnimation = true)
    {
        _fadeTween?.Kill();

        float targetAlpha = state ? 1f : 0f;
        float currentAlpha = _textUI.color.a;
        float duration = useAnimation ? _fadeDuration : 0.01f;

        Color textColor = _textUI.color;
        Color backfaceColor = _background.Color;
        _fadeTween = DOVirtual.Float(currentAlpha, targetAlpha, duration, t =>
        {
            textColor.a = t;
            backfaceColor.a = t;

            _textUI.color = textColor;
            _background.Color = backfaceColor;
        });

        //TODO: check if Dotween also doesn't get to 1 similar to lerps.
    }

    public void SetScale(Vector2 newTextScale, Vector2 newBackgroundScale)
    {
        _scaleTween?.Kill();

        Vector2 currentBackgroundScale = new Vector2(_background.Width, _background.Height);
        Vector2 currentTextScale = new Vector2(_textUI.rectTransform.sizeDelta.x, _textUI.rectTransform.sizeDelta.y);

        _scaleTween = DOVirtual.Float(0, 1, _scaleChangeDuration, t =>
        {
            float textXScale = Mathf.Lerp(currentTextScale.x, newTextScale.x, t);
            float textYScale = Mathf.Lerp(currentTextScale.y, newTextScale.y, t);
            _textUI.rectTransform.sizeDelta = new Vector2(textXScale, textYScale);

            _background.Width = Mathf.Lerp(currentBackgroundScale.x, newBackgroundScale.x, t);
            _background.Height = Mathf.Lerp(currentBackgroundScale.y, newBackgroundScale.y, t);
        });
    }
}