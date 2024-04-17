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


    public void Fade(bool fadeIn, bool useAnimation = true)
    {
        _fadeTween?.Kill();

        float targetAlpha = fadeIn ? 1f : 0f;
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

        //TODO: check if dotween also doesn't get to 1 similar to lerps.
    }

    private void ChangeScale(float backfaceX, float backfaceY)
    {
        _scaleTween?.Kill();

        float currentBackfaceX = _background.Width;
        float currentBackfaceY = _background.Height;

        _scaleTween = DOVirtual.Float(0, 1, _scaleChangeDuration, t =>
        {
            _background.Width = Mathf.Lerp(currentBackfaceX, backfaceX, t);
            _background.Height = Mathf.Lerp(currentBackfaceY, backfaceY, t);
        });
    }
}