using UnityEngine;
using DG.Tweening;
using TMPro;

public class TXRButtonVisuals : MonoBehaviour
{
    [SerializeField] private Shapes.Rectangle _backface;
    [SerializeField] private Shapes.Rectangle _stroke;
    [SerializeField] private TextMeshPro _text;

    [Header("General")]
    [SerializeField] private Color _backfaceColorActive;
    [SerializeField] private Color _backfaceColorPressed;
    [SerializeField] private Color _backfaceColorInactive;
    [SerializeField] private Color _backfaceColorGradientHover;
    [SerializeField] private float _strokeThickness;



    [Header("Show Animation")]
    [SerializeField] private float _showDuration;
    [SerializeField] private float _hideDuration;

    Tween _showTween;
    Tween _hideTween;

    private void OnValidate()
    {
        _showTween.Kill();
        _hideTween.Kill();
        InitShowHideTweens();
    }

    void Start()
    {
        InitShowHideTweens();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Show();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Hide();
        }
    }

    public void Show()
    {
        if (_hideTween.IsPlaying())
        {
            _hideTween.Pause();
        }
        _showTween.Restart();
    }

    public void Hide()
    {
        if (_showTween.IsPlaying())
        {
            _showTween.Pause();
        }
        _hideTween.Restart();
    }

    private void InitShowHideTweens()
    {
        Color backfaceColor = _backfaceColorActive;
        Color textColor = _text.color;

        _showTween = DOVirtual.Float(0, 1, _showDuration, value =>
        {
            backfaceColor.a = value;
            _backface.FillColorStart = backfaceColor;
            _backface.FillColorEnd = backfaceColor;

            _stroke.Thickness = Mathf.Lerp(0, _strokeThickness, value);

            textColor.a = value;
            _text.color = textColor;
        });
        _showTween.SetAutoKill(false);
        _showTween.Pause();

        _hideTween = DOVirtual.Float(1, 0, _hideDuration, value =>
        {
            backfaceColor.a = value;
            _backface.FillColorStart = backfaceColor;
            _backface.FillColorEnd = backfaceColor;

            _stroke.Thickness = Mathf.Lerp(0, _strokeThickness, value);

            textColor.a = value;
            _text.color = textColor;
        });
        _hideTween.SetAutoKill(false);
        _hideTween.Pause();

    }

}
