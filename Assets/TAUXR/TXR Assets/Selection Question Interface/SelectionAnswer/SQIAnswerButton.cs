using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class SQIAnswerButton : MonoBehaviour
{
    public string Text;
    public bool IsCorrect;
    public string AnswerInfo;

    private TXRButton_Toggle _button;
    private TextMeshPro _text;

    private float _timeFromPressToDisable;

    private Color _startingActiveColor;
    private Color _correctAnswerColor;
    private Color _wrongAnswerColor;
    private float _durationFromSubmissionToDisable;
    public void Init(Color correctAnswerColor, Color wrongAnswerColor, float durationFromSubmissionToDisable)
    {
        _button = GetComponent<TXRButton_Toggle>();
        _button.Init();
        _startingActiveColor = _button.GetColor(TXRButtonState.Active);
        _text = GetComponentInChildren<TextMeshPro>();

        _correctAnswerColor = correctAnswerColor;
        _wrongAnswerColor = wrongAnswerColor;
        _durationFromSubmissionToDisable = durationFromSubmissionToDisable;
    }

    public void ResetAnswer()
    {
        _button.TriggerToggleEvent(TXRButtonToggleState.Off, ButtonColliderResponse.Internal);
        _button.SetColor(TXRButtonState.Active, _startingActiveColor);
        _button.SetInteractable(true);
        _button.SetState(TXRButtonState.Active);
    }

    public async UniTask SubmitAnswer()
    {
        _button.SetInteractable(false);

        Color answerColorAfterSubmission = IsCorrect ? _correctAnswerColor : _wrongAnswerColor;
        _button.SetColor(TXRButtonState.Active, answerColorAfterSubmission);

        await UniTask.Delay(TimeSpan.FromSeconds(_durationFromSubmissionToDisable));

        if (!IsCorrect)
        {
            _button.SetState(TXRButtonState.Disabled);
        }
    }

    public void SetHidden(bool hidden)
    {
        if (hidden) _button.SetState(TXRButtonState.Hidden);
        else _button.SetState(TXRButtonState.Active);
    }
}