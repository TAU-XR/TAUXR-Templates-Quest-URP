using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Shapes;
using UnityEngine;
using TMPro;

public class TextPopUp : MonoBehaviour
{
    [SerializeField] private TextPopUpReferences _textPopUpReferences;
    [SerializeField] private ETextPopUpState _startingState;
    [SerializeField] private float _fontSizeMultiplier = 1;
    [SerializeField] private Vector2 _backgroundPadding = new(0.3f, 0.1f);
    [TextArea(1, 10)] [SerializeField] private string _text;

    [SerializeField] private TextPopUpTextsConfigurationsScriptableObject _textConfigurations;

    private void Awake()
    {
        _textPopUpReferences.TextPopUpAnimator.Init(_textPopUpReferences);
    }

    private void Start()
    {
        SetStartingState();
    }

    private void SetStartingState()
    {
        switch (_startingState)
        {
            case ETextPopUpState.Active:
                _textPopUpReferences.TextPopUpAnimator.SetAppearance(true, false);
                break;
            case ETextPopUpState.Appear:
                _textPopUpReferences.TextPopUpAnimator.SetAppearance(false, false);
                _textPopUpReferences.TextPopUpAnimator.SetAppearance(true);
                break;
            case ETextPopUpState.Disabled:
                _textPopUpReferences.TextPopUpAnimator.SetAppearance(false, false);
                break;
        }
    }

    public void Show(bool useAnimation = true)
    {
        _textPopUpReferences.TextPopUpAnimator.SetAppearance(true, useAnimation);
    }

    public void Hide(bool useAnimation = true)
    {
        _textPopUpReferences.TextPopUpAnimator.SetAppearance(false, useAnimation);
    }

    public void SetText(string newText)
    {
        _textPopUpReferences.TextUI.text = newText;
    }

    public void SetScale(Vector2 textAreaSize, bool useAnimation = true)
    {
        if (useAnimation)
        {
            _textPopUpReferences.TextPopUpAnimator.SetScale(textAreaSize, textAreaSize + _backgroundPadding);
            return;
        }

        _textPopUpReferences.TextUI.rectTransform.sizeDelta = textAreaSize;
        _textPopUpReferences.Background.Width = textAreaSize.x + _backgroundPadding.x;
        _textPopUpReferences.Background.Height = textAreaSize.y + _backgroundPadding.y;
    }

    public void SetTextFromConfiguration(string textId, bool useAnimation = true)
    {
        TextPopUpTextConfiguration textConfiguration = _textConfigurations.GetTextConfiguration(textId);
        if (textConfiguration == null)
        {
            return;
        }

        SetText(textConfiguration.Text);
        SetScale(textConfiguration.TextRectSize, useAnimation);
    }

    public void SetLanguageToEnglish()
    {
        _textPopUpReferences.TextUI.isRightToLeftText = false;
        _textPopUpReferences.TextUI.alignment = TextAlignmentOptions.Left;
    }

    public void SetLanguageToHebrew()
    {
        _textPopUpReferences.TextUI.isRightToLeftText = true;
        _textPopUpReferences.TextUI.alignment = TextAlignmentOptions.Right;
    }
#if UNITY_EDITOR
    public void SetTextAndScale(Vector2 textAreaSize)
    {
        _textPopUpReferences.TextPopUpAnimator.Init(_textPopUpReferences);
        SetText(_text);
        SetScale(textAreaSize);
    }

    public void SetActiveState(bool newState)
    {
        _textPopUpReferences.TextPopUpAnimator.Init(_textPopUpReferences);
        _textPopUpReferences.TextPopUpAnimator.SetAppearance(newState, false);
    }
#endif
}