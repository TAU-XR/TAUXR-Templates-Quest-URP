using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SQIRemoteSelectButton : MonoBehaviour
{
    [SerializeField] TXRButton_Toggle _targetAnswerButton;
    TXRButton _button;
    void Start()
    {
        _button = GetComponent<TXRButton>();
        _button.Released.AddListener(OnRelease);
    }

    private void OnRelease()
    {
        _targetAnswerButton.TriggerButtonEventFromInput(ButtonEvent.Released);
    }
}
