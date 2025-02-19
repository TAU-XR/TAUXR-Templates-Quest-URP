using System;
using UnityEngine;

public class SQISubmitButton : MonoBehaviour
{
    public TXRButton Button => _button;
    private TXRButton _button;

    public Action AnswerSubmitted;

    public void Init()
    {
        _button = GetComponent<TXRButton>();
        _button.Init();
        _button.Pressed.AddListener(() => AnswerSubmitted?.Invoke());
    }

    public void SetHidden(bool hidden)
    {
        if (hidden) _button.SetState(TXRButtonState.Hidden);
        else _button.SetState(TXRButtonState.Active);
    }
}