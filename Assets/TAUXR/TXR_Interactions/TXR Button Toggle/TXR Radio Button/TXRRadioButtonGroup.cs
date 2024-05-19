using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TXRRadioButtonGroup : MonoBehaviour
{
    [SerializeField] private TXRButton_Radio[] _radioButtons;

    [HideInInspector] public TXRButton_Radio SelectedRadioButton;

    public void Reset()
    {
        SelectedRadioButton = null;
    }

    private void OnEnable()
    {
        RegisterButtonEvents();
    }

    private void OnDisable()
    {
        UnregisterButtonEvents();
    }

    public void RegisterButtonEvents()
    {
        foreach (TXRButton_Radio radioButton in _radioButtons)
        {
            radioButton.ButtonSelected += () => OnButtonSelected(radioButton);
            radioButton.ButtonDeselected += () => OnButtonDeselected(radioButton);
        }
    }

    public void UnregisterButtonEvents()
    {
        foreach (TXRButton_Radio radioButton in _radioButtons)
        {
            radioButton.ButtonSelected -= () => OnButtonSelected(radioButton);
            radioButton.ButtonDeselected -= () => OnButtonDeselected(radioButton);
        }
    }


    private void OnButtonSelected(TXRButton_Radio radioButton)
    {
        if (SelectedRadioButton == radioButton)
        {
            return;
        }

        if (SelectedRadioButton != null)
        {
            SelectedRadioButton.ManuallyDeselectAnswer();
        }

        SelectedRadioButton = radioButton;
    }

    private void OnButtonDeselected(TXRButton_Radio radioButton)
    {
        if (SelectedRadioButton == radioButton)
        {
            SelectedRadioButton = null;
        }
    }
}