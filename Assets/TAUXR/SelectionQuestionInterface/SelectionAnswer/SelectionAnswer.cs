using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class SelectionAnswer : MonoBehaviour
{
    public Action AnswerSelected;
    public SelectionAnswerData SelectionAnswerData => _selectionAnswerData;
    private SelectionAnswerData _selectionAnswerData;

    private TXRButton_Toggle _button;
    private TextMeshPro _text;

    private void Awake()
    {
        _button = GetComponent<TXRButton_Toggle>();
        _text = GetComponentInChildren<TextMeshPro>();
    }

    private void Start()
    {
        _button.ToggleOn.AddListener(OnAnswerSelected);
    }

    private void OnAnswerSelected()
    {
        AnswerSelected?.Invoke();
    }

    public void Init(SelectionAnswerData selectionAnswerData)
    {
        _selectionAnswerData = selectionAnswerData;
        _text.text = _selectionAnswerData.Text;
    }

    public async UniTask OnAnswerSubmitted()
    {
        _button.SetState(ButtonState.Frozen);
        //Color button - green for correct, red for wrong
        await UniTask.Delay(TimeSpan.FromSeconds(2));

        //If wrong answer
        _button.SetState(ButtonState.Disabled);
    }
}