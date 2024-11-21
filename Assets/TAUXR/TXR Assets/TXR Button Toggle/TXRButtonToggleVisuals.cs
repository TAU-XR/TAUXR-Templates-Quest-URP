using OVRSimpleJSON;
using UnityEngine;

public class TXRButtonToggleVisuals : TXRButtonVisuals
{
    public bool IsToggleOn;


    public void SetToggleState(bool isOn)
    {
        /*Color activeColor = isOn ? _configurations.pressColor : _configurations.activeColor;
        SetBackfaceColor(TXRButtonState.Active, activeColor);
        IsToggleOn = isOn;*/
    }


   /* protected override void Active()
    {
        float backfaceZValue = IsToggleOn ? _backfaceZPositionActiveToggleOn : _configurations.backfaceZPositionActive;
        SetBackfaceColor(_activeColor, _configurations.activeDuration);
        SetBackfaceZ(backfaceZValue);
        SetHoverGradient(false);
        SetStrokeThickness(_configurations.strokeThicknessActive);
        SetTextOpacity(1);
    }

    protected override void Hover()
    {
        float backfaceZValue = IsToggleOn ? _backfaceZPositionHoverToggleOn : _configurations.backfadeZPositionHover;
        SetHoverGradient(true);
        SetBackfaceZ(backfaceZValue);
        SetStrokeThickness(_configurations.strokeThicknessHover);
    }*/
}