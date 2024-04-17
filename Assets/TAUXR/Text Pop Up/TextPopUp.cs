using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;

public class TextPopUp : MonoBehaviour
{
    private const float DefaultTextFontSize = 0.34f;
    private const int ReferenceNumberOfLettersUntilLineWrap = 74;
    private const int ReferenceNumberOfLetters = 300;
    private const float ReferenceSquareMeters = 0.25f;
    private const float ReferenceLineHeight = 0.04f;

    [SerializeField] private bool _useAnimation = true;
    [SerializeField] private Rectangle _background;
    [SerializeField] private TextMeshPro _textUI;

    [SerializeField] private float _fontSizeMultiplier = 1;
    [SerializeField] private float _layoutRatio = 5.5f;
    [SerializeField] private float _scaleFactor = 1;
    [SerializeField] private bool _extendWidthOnly;
    [SerializeField] private bool _extendHeightOnly;

    [TextArea(1, 10)] [SerializeField] private string _text;

    private Vector2 _scale;
    [SerializeField] private Vector2 _backgroundPadding = new(0.3f, 0.1f);
    [SerializeField] private bool _autoScaleWhenChangingText = true;

    private void OnValidate()
    {
        if (Application.isPlaying || !_autoScaleWhenChangingText)
        {
            return;
        }

        SetTextAndScale(_text);
    }

    public void SetText(string newText)
    {
        _textUI.text = newText;
    }


    public void SetTextAndScale(string newText)
    {
        _textUI.text = newText;
        _textUI.fontSize = DefaultTextFontSize * _fontSizeMultiplier;

        float xScale = Mathf.Sqrt(_layoutRatio * ReferenceSquareMeters);
        float yScale = Mathf.Sqrt(1 / _layoutRatio * ReferenceSquareMeters) + GetNumberOfExtraLineBreaks() * ReferenceLineHeight;
        _scale = new Vector2(xScale, yScale) * _scaleFactor * GetLayoutScalingFactor();

        _textUI.rectTransform.sizeDelta = _scale;
        Vector2 backFaceSize = _scale + _backgroundPadding;
        _background.Width = backFaceSize.x;
        _background.Height = backFaceSize.y;
    }

    private Vector2 GetLayoutScalingFactor()
    {
        float scalingFactor = (float)_text.Length / ReferenceNumberOfLetters;
        float newScaleX;
        float newScaleY;

        if (_extendWidthOnly)
        {
            newScaleX = scalingFactor;
            newScaleY = 1;
        }
        else if (_extendHeightOnly)
        {
            newScaleY = scalingFactor;
            newScaleX = 1;
        }
        else
        {
            newScaleX = Mathf.Sqrt(scalingFactor);
            newScaleY = Mathf.Sqrt(scalingFactor);
        }

        Vector2 newScale = new Vector2(newScaleX, newScaleY) * _fontSizeMultiplier;

        return newScale;
    }

    private int GetNumberOfExtraLineBreaks()
    {
        return GetNumberOfLineBreaks() - GetNumberOfWrappingLineBreaksInText(_text);
    }

    private int GetNumberOfLineBreaks()
    {
        string[] paragraphs = _text.Split("\n");
        int manualLineBreaks = paragraphs.Length - 1;
        int wrappingLineBreaksInParagraph = 0;
        foreach (string paragraph in paragraphs)
        {
            wrappingLineBreaksInParagraph += GetNumberOfWrappingLineBreaksInText(paragraph);
        }

        return manualLineBreaks + wrappingLineBreaksInParagraph;
    }

    private int GetNumberOfWrappingLineBreaksInText(string text)
    {
        return text.Length / (int)(ReferenceNumberOfLettersUntilLineWrap * (float)_text.Length / ReferenceNumberOfLetters);
    }


#if UNITY_EDITOR
    [Button]
    public void GetTextFromComponent()
    {
        _text = _textUI.text;
    }

    [Button]
    public void SetTextAndScale()
    {
        SetTextAndScale(_text);
    }

    [Button]
    public void DebugNumberOfLetters()
    {
        Debug.Log(_textUI.text.Length);
    }

    [Button]
    public void ResetScale()
    {
        _textUI.rectTransform.sizeDelta = new Vector2(0.48f, 0.08f);
        _background.transform.localScale = Vector3.one;
    }

#endif
}