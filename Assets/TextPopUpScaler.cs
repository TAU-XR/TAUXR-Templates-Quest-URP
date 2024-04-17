using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;

public class TextPopUpScaler : MonoBehaviour
{
    private const float DefaultTextFontSize = 0.34f;
    private const int ReferenceNumberOfLettersUntilLineWrap = 74;
    private const int ReferenceNumberOfLetters = 300;
    private const float ReferenceSquareMeters = 0.25f;
    private const float ReferenceLineHeight = 0.04f;

    [SerializeField] private Rectangle _background;
    [SerializeField] private TextMeshPro _textUI;

    [SerializeField] private float _fontSizeMultiplier = 1;
    [SerializeField] private float _layoutRatio = 5.5f;
    [SerializeField] private float _scaleFactor = 1;
    [SerializeField] private bool _extendWidthOnly;
    [SerializeField] private bool _extendHeightOnly;

    [SerializeField] private Vector2 _backgroundPadding = new(0.3f, 0.1f);
    public bool AutoScaleWhenChangingText => _autoScaleWhenChangingText;
    [SerializeField] private bool _autoScaleWhenChangingText = true;

    [HideInInspector] public string Text;

    public void SetScale(Vector2 textSize)
    {
        _textUI.rectTransform.sizeDelta = textSize;
        _background.Width = textSize.x + _backgroundPadding.x;
        _background.Height = textSize.y + _backgroundPadding.y;
    }

    public void AutoScale()
    {
        _textUI.fontSize = DefaultTextFontSize * _fontSizeMultiplier;

        float xScale = Mathf.Sqrt(_layoutRatio * ReferenceSquareMeters);

        float heightAdditionFromExtraLineBreaks = GetNumberOfExtraLineBreaks() * ReferenceLineHeight;
        float yScale = Mathf.Sqrt(1 / _layoutRatio * ReferenceSquareMeters) + heightAdditionFromExtraLineBreaks;

        Vector2 scale = new Vector2(xScale, yScale) * _scaleFactor * _fontSizeMultiplier * GetNumberOfLettersScalingFactor();

        _textUI.rectTransform.sizeDelta = scale;
        Vector2 backFaceSize = scale + _backgroundPadding;
        _background.Width = backFaceSize.x;
        _background.Height = backFaceSize.y;
    }

    private Vector2 GetNumberOfLettersScalingFactor()
    {
        float squareMetersScalingFactor = (float)Text.Length / ReferenceNumberOfLetters;
        float newScaleX = 1;
        float newScaleY = 1;

        if (_extendWidthOnly)
        {
            newScaleX = squareMetersScalingFactor;
        }
        else if (_extendHeightOnly)
        {
            newScaleY = squareMetersScalingFactor;
        }
        else
        {
            newScaleX = Mathf.Sqrt(squareMetersScalingFactor);
            newScaleY = Mathf.Sqrt(squareMetersScalingFactor);
        }

        Vector2 newScale = new(newScaleX, newScaleY);

        return newScale;
    }

    private int GetNumberOfExtraLineBreaks()
    {
        return GetNumberOfLineBreaks() - GetNumberOfWrappingLineBreaksInText(Text);
    }

    private int GetNumberOfLineBreaks()
    {
        string[] paragraphs = Text.Split("\n");
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
        int numberOfLettersUntilLineWrap = (int)(ReferenceNumberOfLettersUntilLineWrap * (float)Text.Length / ReferenceNumberOfLetters);
        return text.Length / numberOfLettersUntilLineWrap;
    }
}