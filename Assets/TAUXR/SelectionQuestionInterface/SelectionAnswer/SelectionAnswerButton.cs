using System;
using System.Collections;
using System.Collections.Generic;
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

    private void Awake()
    {
        _button = GetComponent<TXRButton_Toggle>();
        _startingActiveColor = _button.GetColor(EButtonAnimationState.Active);
        _text = GetComponentInChildren<TextMeshPro>();
    }

    public void SetNewAnswer(SelectionAnswerData selectionAnswerData, SelectionAnswerButtonConfiguration buttonConfiguration)
    {
        _selectionAnswerData = selectionAnswerData;
        _text.text = _selectionAnswerData.Text;
        _buttonConfiguration = buttonConfiguration;
        _button.SetColor(EButtonAnimationState.Active, _startingActiveColor);
        _button.SetState(ButtonState.Interactable);
    }

    public async UniTask OnAnswerSubmitted()
    {
        _button.SetState(ButtonState.Frozen);

        _button.SetColor(EButtonAnimationState.Active, _buttonConfiguration.AnswerColorAfterSubmission);

        await UniTask.Delay(TimeSpan.FromSeconds(_buttonConfiguration.TimeFromPressToDisable));

        if (!_selectionAnswerData.IsCorrect)
        {
            _button.SetState(ButtonState.Disabled);
        }
    }
}