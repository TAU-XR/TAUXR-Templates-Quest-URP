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
    private const int NumberOfLettersUntilLineWrap = 60;
    private const int ReferenceNumberOfLetters = 300;
    private const float ReferenceSquareMeters = 0.2f;

    [SerializeField] private bool _useAnimation = true;

    [SerializeField] private Rectangle _background;
    [SerializeField] private TextMeshPro _textUI;

    [SerializeField] private float _fontSizeMultiplier = 1;
    [SerializeField] private float _layoutRatio = 6;

    [TextArea(1, 10)] [SerializeField] private string _text;

    [SerializeField] private bool _setWidthOnly;
    [SerializeField] private bool _setHeightOnly;

    private Vector2 _layout;
    [SerializeField] private Vector2 _backFacePadding = new Vector2(0.2f, 0.04f);
    [SerializeField] private bool _useOnValidate = true;

    private void OnEnable()
    {
        if (!_useAnimation)
        {
            GetComponent<Animator>().Play("Birth", -1, 1);
        }
    }

    public void SetText(string newText)
    {
        _textUI.text = newText;
    }

    private void OnValidate()
    {
        if (Application.isPlaying || !_useOnValidate)
        {
            return;
        }

        _layout = new Vector2(Mathf.Sqrt(_layoutRatio * ReferenceSquareMeters), Mathf.Sqrt(1 / _layoutRatio * ReferenceSquareMeters));
        SetTextAndScale(_text);
    }

    public void SetTextAndScale(string newText)
    {
        _textUI.text = newText;
        _textUI.fontSize = DefaultTextFontSize * _fontSizeMultiplier;
        Vector2 scale = GetScaleAmount();
        UpdateTextRect(scale);
        Vector2 backFaceSize = _layout * scale + _backFacePadding;
        _background.Width = backFaceSize.x;
        _background.Height = backFaceSize.y;
    }

    private void UpdateTextRect(Vector2 scale)
    {
        _textUI.rectTransform.sizeDelta = _layout * scale;
    }

    private Vector2 GetScaleAmount()
    {
        float squareMetersScaleAmount = GetSquareMetersScaleAmount();
        float newScaleX;
        float newScaleY;

        if (_setWidthOnly)
        {
            newScaleX = squareMetersScaleAmount;
            newScaleY = 1;
        }
        else if (_setHeightOnly)
        {
            newScaleY = squareMetersScaleAmount;
            newScaleX = 1;
        }
        else
        {
            newScaleX = Mathf.Sqrt(squareMetersScaleAmount);
            newScaleY = Mathf.Sqrt(squareMetersScaleAmount);
        }

        //Increase the y scale according to the amount of extra line breaks.
        newScaleY *= (1 + (float)GetNumberOfLineBreaks() / GetNumberOfWrappingLineBreaksInText(_text)) / 2;

        Vector2 newScale = new Vector2(newScaleX, newScaleY) * _fontSizeMultiplier;

        return newScale;
    }

    private float GetSquareMetersScaleAmount()
    {
        return (float)_text.Length / ReferenceNumberOfLetters;
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
        return text.Length / (int)(NumberOfLettersUntilLineWrap * GetSquareMetersScaleAmount());
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