using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class SelectionAnswerButton : MonoBehaviour
{
    public Action AnswerSelected;
    public Action AnswerDeselected;
    public SelectionAnswerData SelectionAnswerData => _selectionAnswerData;
    private SelectionAnswerData _selectionAnswerData;

    private TXRButton_Toggle _button;
    private TextMeshPro _text;

    [SerializeField] private float _timeFromPressToDisable = 1;

    private Color _startingPressColor;

    private SelectionAnswerButtonConfiguration _buttonConfiguration;

    private void Awake()
    {
        _button = GetComponent<TXRButton_Toggle>();
        _text = GetComponentInChildren<TextMeshPro>();
    }

    private void Start()
    {
        _button.ToggleOn.AddListener(OnAnswerSelected);
        _button.ToggleOff.AddListener(OnAnswerDeselected);
    }

    private void OnAnswerSelected()
    {
        AnswerSelected?.Invoke();
    }

    private void OnAnswerDeselected()
    {
        AnswerDeselected?.Invoke();
    }

    public void ManuallyDeselectAnswer()
    {
        _button.TriggerToggleEvent(TXRButtonToggleState.Off, ButtonColliderResponse.Both);
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

        _button.SetPressedColor(_buttonConfiguration.AnswerColorAfterSubmission);

        await UniTask.Delay(TimeSpan.FromSeconds(_timeFromPressToDisable));

        if (!_selectionAnswerData.IsCorrect)
        {
            _button.SetState(ButtonState.Disabled);
        }
    }
}