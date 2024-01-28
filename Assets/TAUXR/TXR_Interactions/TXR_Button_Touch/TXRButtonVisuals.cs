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

    Tween _showTween;


    void Start()
    {
        CreateShowSequence();
    }

    void Update()
    {

    }

    private void CreateShowSequence()
    {
        Color backfaceColor = _backfaceColorActive;
        Color textColor = _text.color;

        _showTween.SetAutoKill(false);
        _showTween = DOVirtual.Float(0, 1, _showDuration, value =>
        {
            backfaceColor.a = value;
            _backface.FillColorStart = backfaceColor;
            _backface.FillColorEnd = backfaceColor;

            _strokeThickness = Mathf.Lerp(0, _strokeThickness, value);

            textColor.a = value;
            _text.color = textColor;
        });
        _showTween.SetEase(Ease.OutCubic);
        _showTween.Pause();


    }

}
