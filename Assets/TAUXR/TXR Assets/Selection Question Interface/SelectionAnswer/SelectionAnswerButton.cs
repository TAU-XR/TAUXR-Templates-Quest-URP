using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class SelectionAnswerButton : MonoBehaviour
{
    public SelectionAnswerData SelectionAnswerData => _selectionAnswerData;
    private SelectionAnswerData _selectionAnswerData;

    private TXRButton_Toggle _button;
    private TextMeshPro _text;

    private float _timeFromPressToDisable;

    private SelectionAnswerButtonConfiguration _buttonConfiguration;
    private Color _startingActiveColor;

    public void Init()
    {
        _button = GetComponent<TXRButton_Toggle>();
        _button.Init();
        _startingActiveColor = _button.GetColor(TXRButtonState.Active);
        _text = GetComponentInChildren<TextMeshPro>();
    }
    public void SetNewAnswer(SelectionAnswerData selectionAnswerData, SelectionAnswerButtonConfiguration buttonConfiguration)
    {
        _selectionAnswerData = selectionAnswerData;
        _text.text = _selectionAnswerData.Text;
        _buttonConfiguration = buttonConfiguration;
        _button.SetColor(TXRButtonState.Active, _startingActiveColor);
        _button.SetInteractable(true);
        _button.SetState(TXRButtonState.Active);
    }

    public async UniTask OnAnswerSubmitted()
    {
        _button.SetInteractable(false);

        _button.SetColor(TXRButtonState.Active, _buttonConfiguration.AnswerColorAfterSubmission);

        await UniTask.Delay(TimeSpan.FromSeconds(_buttonConfiguration.TimeFromSubmitToDisable));

        if (!_selectionAnswerData.IsCorrect)
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