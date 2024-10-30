using System;
using UnityEngine;

public class TXRRadioButtonGroup : MonoBehaviour
{
    public TXRButton_Toggle[] Buttons => _buttons;
    [SerializeField] private TXRButton_Toggle[] _buttons;

    public TXRButton_Toggle SelectedButton => _selectedButton;
    private TXRButton_Toggle _selectedButton;

    public Action OnAnswerSelected;
    public Action OnAnswerDeselected;
    private bool _didTriggerOnNoAnswerSelected = true;

    private void Update()
    {
        if (_selectedButton == null)
        {
            if (!_didTriggerOnNoAnswerSelected)
            {
                OnAnswerDeselected?.Invoke();
                _didTriggerOnNoAnswerSelected = true;
            }
        }
    }
    public void Reset()
    {
        if (_selectedButton != null)
        {
            _selectedButton.TriggerToggleEvent(TXRButtonToggleState.Off, ButtonColliderResponse.Both);
            _selectedButton = null;
        }
    }

    private void OnEnable()
    {
        RegisterButtonEvents();
    }

    private void OnDisable()
    {
        UnregisterButtonEvents();
    }

    private void RegisterButtonEvents()
    {
        foreach (TXRButton_Toggle button in _buttons)
        {
            button.ToggledOn += () => OnButtonToggleOn(button);
            button.ToggledOff += () => OnButtonToggleOff(button);
        }
    }

    private void UnregisterButtonEvents()
    {
        foreach (TXRButton_Toggle button in _buttons)
        {
            button.ToggledOn -= () => OnButtonToggleOn(button);
            button.ToggledOff -= () => OnButtonToggleOff(button);
        }
    }

    private void OnButtonToggleOn(TXRButton_Toggle button)
    {
        if (_selectedButton == button)
        {
            return;
        }

        if (_selectedButton != null)
        {
            _selectedButton.TriggerToggleEvent(TXRButtonToggleState.Off, ButtonColliderResponse.Both);
        }
        else
        {
            OnAnswerSelected?.Invoke();
        }

        _selectedButton = button;
        _didTriggerOnNoAnswerSelected = false;
    }

    private void OnButtonToggleOff(TXRButton_Toggle button)
    {
        if (_selectedButton == button)
        {
            _selectedButton = null;
        }
    }

    public void SelectButton(TXRButton_Toggle button)
    {
        button.TriggerToggleEvent(TXRButtonToggleState.On, ButtonColliderResponse.Both);
    }
}