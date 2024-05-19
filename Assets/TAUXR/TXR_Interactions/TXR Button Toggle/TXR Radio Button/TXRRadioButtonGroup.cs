using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TXRRadioButtonGroup : MonoBehaviour
{
    public TXRButton_Radio[] Buttons => _buttons;
    [SerializeField] private TXRButton_Radio[] _buttons;

    public TXRButton_Radio SelectedButton => _selectedButton;
    private TXRButton_Radio _selectedButton;

    public void Reset()
    {
        _selectedButton = null;
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
        foreach (TXRButton_Radio button in _buttons)
        {
            button.ButtonSelected += () => OnButtonSelected(button);
            button.ButtonDeselected += () => OnButtonDeselected(button);
        }
    }

    private void UnregisterButtonEvents()
    {
        foreach (TXRButton_Radio button in _buttons)
        {
            button.ButtonSelected -= () => OnButtonSelected(button);
            button.ButtonDeselected -= () => OnButtonDeselected(button);
        }
    }


    private void OnButtonSelected(TXRButton_Radio button)
    {
        if (_selectedButton == button)
        {
            return;
        }

        if (_selectedButton != null)
        {
            _selectedButton.ManuallyDeselect();
        }

        _selectedButton = button;
    }

    private void OnButtonDeselected(TXRButton_Radio button)
    {
        if (_selectedButton == button)
        {
            _selectedButton = null;
        }
    }
}