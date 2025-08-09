using OVRSimpleJSON;
using UnityEngine;

public class TXRButtonToggleVisuals : TXRButtonVisuals
{
    public bool IsToggleOn => _isToggleOn;
    private bool _isToggleOn;
    public Transform _toggleOnState;
    public Transform _toggleOffState;
    public Transform _hoverOffState;
    public Transform _hoverOnState;

    public void SetToggleState(bool isOn)
    {
        _isToggleOn = isOn;
        _activeState = isOn ? _toggleOnState : _toggleOffState;
        _hoverState = isOn ? _hoverOnState : _hoverOffState;
    }

    public override void SetAllStatesSizeFromMainUI()
    {
        Transform stateTransform = _activeState;
        for (int i = 0; i <= 8; i++)
        {
            switch (i)
            {
                case 0: stateTransform = _activeState; break;
                case 1: stateTransform = _disableState; break;
                case 2: stateTransform = _hoverState; break;
                case 3: stateTransform = _pressState; break;
                case 4: stateTransform = _hiddenState; break;
                case 5: stateTransform = _toggleOnState; break;
                case 6: stateTransform = _toggleOffState; break;
                case 7: stateTransform = _hoverOnState; break;
                case 8: stateTransform = _hoverOffState; break;
                default: stateTransform = _activeState; break;
            }
            SetStatesSizesFromMainUI(stateTransform);
        }
    }
}