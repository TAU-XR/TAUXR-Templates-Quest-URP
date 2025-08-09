using System;
using UnityEngine;

public class TXRRadioButtonGroup : MonoBehaviour
{
    public TXRButton_Toggle[] Buttons => _buttons;
    [SerializeField] private TXRButton_Toggle[] _buttons;

    public int NumberOfButtons => _buttons.Length;
    public TXRButton_Toggle SelectedButton => _selectedButton;
    private TXRButton_Toggle _selectedButton;

    public Action OnButtonSelected;
    public Action OnButtonDeselected;
    private bool _didTriggerOnNoAnswerSelected = true;

    private void Update()
    {
        if (_selectedButton == null)
        {
            if (!_didTriggerOnNoAnswerSelected)
            {
                OnButtonDeselected?.Invoke();
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
        _buttons = GetComponentsInChildren<TXRButton_Toggle>();
    }

    private void OnDisable()
    {
        UnregisterButtonEvents();
    }

    private void RegisterButtonEvents()
    {
        foreach (TXRButton_Toggle button in _buttons)
        {
            if (button == null) continue;
            button.ToggledOn += OnButtonToggleOn;
            button.ToggledOff += OnButtonToggleOff;
        }
    }

    private void UnregisterButtonEvents()
    {
        foreach (TXRButton_Toggle button in _buttons)
        {
            if (button == null) continue;
            button.ToggledOn -= OnButtonToggleOn;
            button.ToggledOff -= OnButtonToggleOff;
        }
    }

    private void OnButtonToggleOn(TXRButton_Toggle button)
    {
        if (_selectedButton == button) return;

        // turn off selected button first.
        if (_selectedButton != null)
        {
            _selectedButton.TriggerToggleEvent(TXRButtonToggleState.Off, ButtonColliderResponse.Both);
        }

        Debug.Log("INVOKE SELECTED");
        _selectedButton = button;
        OnButtonSelected?.Invoke();
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