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

    private void OnValidate()
    {
        if (Application.isPlaying || !_textPopUpScaler.AutoScaleWhenChangingText)
        {
            return;
        }

        SetTextAndAutoScale(_text);
    }

    public void SetText(string newText)
    {
        _textUI.text = newText;
    }

    public void SetTextAndAutoScale(string newText)
    {
        _textUI.text = newText;
        _textPopUpScaler.Text = newText;
        _textPopUpScaler.AutoScale();
    }

    public void SetTextAndScale(string newText, Vector2 textSize)
    {
        _textUI.text = newText;
        _textPopUpScaler.Text = newText;
        _textPopUpScaler.SetScale(textSize);
    }

#if UNITY_EDITOR
    [Button]
    public void GetTextFromComponent()
    {
        _text = _textUI.text;
    }

    [Button]
    public void SetTextAndAutoScale()
    {
        SetTextAndAutoScale(_text);
    }

    [Button]
    public void SetTextAndScale(Vector2 textSize)
    {
        SetTextAndScale(_text, textSize);
    }
#endif
}