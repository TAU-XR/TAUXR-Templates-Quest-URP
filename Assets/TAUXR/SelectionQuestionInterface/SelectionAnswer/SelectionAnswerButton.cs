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

    [SerializeField] private float _timeFromPressToDisable = 1;

    private SelectionAnswerButtonConfiguration _buttonConfiguration;

    private void Awake()
    {
        _button = GetComponent<TXRButton_Toggle>();
        _text = GetComponentInChildren<TextMeshPro>();
    }

    public void Init(SelectionAnswerData selectionAnswerData, SelectionAnswerButtonConfiguration buttonConfiguration)
    {
        _selectionAnswerData = selectionAnswerData;
        _text.text = _selectionAnswerData.Text;
        _button.SetState(ButtonState.Interactable);
        _buttonConfiguration = buttonConfiguration;
    }

    public async UniTask OnAnswerSubmitted()
    {
        _button.SetState(ButtonState.Frozen);

        _button.SetColor(EButtonAnimationState.Active, _buttonConfiguration.AnswerColorAfterSubmission);

        await UniTask.Delay(TimeSpan.FromSeconds(_timeFromPressToDisable));

        if (!_selectionAnswerData.IsCorrect)
        {
            _button.SetState(ButtonState.Disabled);
        }
    }
}