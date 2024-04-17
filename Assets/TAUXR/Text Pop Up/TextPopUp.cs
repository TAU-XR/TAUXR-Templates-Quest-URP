using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;

public class TextPopUp : MonoBehaviour
{
    private const float DefaultTextWidth = 0.48f;
    private const float DefaultTextHeight = 0.08f;
    private const float DefaultTextFontSize = 0.3368976f;
    private const int NumberOfLettersUntilLineWrap = 32;
    private const int NumberOfLettersWhenScaleIsOne = 55;

    [SerializeField] private bool _useAnimation = true;

    [SerializeField] private Transform _background;
    [SerializeField] private TextMeshPro _textUI;

    [SerializeField] private float _fontSizeMultiplier = 1;
    [SerializeField] private Vector2 _layoutScale = new(1, 1);
    [TextArea(1, 10)] [SerializeField] private string _text;

    [SerializeField] private bool _setWidthOnly;
    [SerializeField] private bool _setHeightOnly;

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
        if (Application.isPlaying)
        {
            return;
        }

        SetTextAndScale(_text);
    }

    public void SetTextAndScale(string newText)
    {
        _textUI.text = newText;
        _textUI.fontSize = DefaultTextFontSize * _fontSizeMultiplier;
        Vector2 scale = GetScaleAmount();
        UpdateTextRect(scale);
        _background.localScale = new Vector3(1, scale.y, scale.x);
    }

    private void UpdateTextRect(Vector2 newScale)
    {
        _textUI.rectTransform.sizeDelta = new Vector2(DefaultTextWidth * newScale.x, DefaultTextHeight * newScale.y);
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

        newScaleY *= (1 + ((float)GetNumberOfExtraLineBreaks() / GetNumberOfWrappingLineBreaksInText(_text)) / 2);

        Vector2 newScale = new Vector2(newScaleX, newScaleY) * _layoutScale * _fontSizeMultiplier;

        return newScale;
    }

    private float GetSquareMetersScaleAmount()
    {
        return (float)_text.Length / NumberOfLettersWhenScaleIsOne;
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