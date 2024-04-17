using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;

public class TextPopUp : MonoBehaviour
{
    [SerializeField] private TextMeshPro _textUI;
    [TextArea(1, 10)] [SerializeField] private string _text;
    [SerializeField] private TextAutoScaler _textAutoScaler;

    private void OnValidate()
    {
        if (Application.isPlaying || !_textAutoScaler.AutoScaleWhenChangingText)
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
        _textAutoScaler.Text = newText;
        _textAutoScaler.SetScale();
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
#endif
}