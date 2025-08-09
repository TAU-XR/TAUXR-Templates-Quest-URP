using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class SQIAnswerButton : MonoBehaviour
{
    public bool IsCorrect;
    public string AnswerInfo;

    private TXRButton_Toggle _button;

    private Color _startingAnswerColor;
    private Color _correctAnswerColor;
    private Color _wrongAnswerColor;
    private bool _isFirstInit = true;

    public void Init(Color correctAnswerColor, Color wrongAnswerColor)
    {
        _button = GetComponent<TXRButton_Toggle>();
        _button.Init();

        _correctAnswerColor = correctAnswerColor;
        _wrongAnswerColor = wrongAnswerColor;
        if (_isFirstInit)
        {
            // save disabled initial color for answer reseting
            _startingAnswerColor = _button.GetColor(TXRButtonState.Disabled);
            _isFirstInit = false;
        }
    }

    public void ResetAnswer()
    {
        _button.TriggerToggleEvent(TXRButtonToggleState.Off, ButtonColliderResponse.Internal);
        _button.SetInteractable(true);
        _button.SetColor(TXRButtonState.Disabled, _startingAnswerColor);
        _button.SetState(TXRButtonState.Active);
    }

    public async UniTask SubmitAnswer()
    {
        _button.SetInteractable(false);

        Color answerColorAfterSubmission = IsCorrect ? _correctAnswerColor : _wrongAnswerColor;
        _button.SetColor(TXRButtonState.Disabled, answerColorAfterSubmission);

        _button.SetState(TXRButtonState.Disabled);
    }

    public void SetHidden(bool hidden)
    {
        if (hidden) _button.SetState(TXRButtonState.Hidden);
        else _button.SetState(TXRButtonState.Active);
    }
}