using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;

public class TextPopUp : MonoBehaviour
{
    [SerializeField] private ETextPopUpState _startingState;
    [SerializeField] private Rectangle _background;
    [SerializeField] private TextMeshPro _textUI;
    [TextArea(1, 10)] [SerializeField] private string _text;
    [SerializeField] private TextPopUpScaler _textPopUpScaler;
    [SerializeField] private TextPopUpAnimator _textPopUpAnimator;

    private void Start()
    {
        _textPopUpScaler.Init(_background, _textUI, _textPopUpAnimator);
        _textPopUpAnimator.Init(_background, _textUI);
    }

    public void SetText(string newText)
    {
        _textUI.text = newText;
    }

    public void SetTextAndAutoScale(string newText, bool useAnimation = true)
    {
        _textUI.text = newText;
        _textPopUpScaler.Text = newText;
        _textPopUpScaler.AutoScale(useAnimation);
    }

    public void SetTextAndScale(string newText, Vector2 textSize, bool useAnimation = true)
    {
        _textUI.text = newText;
        _textPopUpScaler.Text = newText;
        _textPopUpScaler.SetScale(textSize, useAnimation);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying || !_textPopUpScaler.AutoScaleWhenChangingTextInInspector)
        {
            return;
        }

        SetTextAndAutoScale();
    }

    public void GetTextFromComponent()
    {
        _text = _textUI.text;
    }

    public void SetTextAndAutoScale()
    {
        _textPopUpScaler.Init(_background, _textUI, _textPopUpAnimator);
        _textPopUpAnimator.Init(_background, _textUI);
        SetTextAndAutoScale(_text, false);
    }

    public void SetTextAndScale(Vector2 textSize)
    {
        _textPopUpScaler.Init(_background, _textUI, _textPopUpAnimator);
        _textPopUpAnimator.Init(_background, _textUI);
        SetTextAndScale(_text, textSize, false);
    }
#endif
}