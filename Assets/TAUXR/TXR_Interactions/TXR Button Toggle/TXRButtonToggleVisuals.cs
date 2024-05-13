using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TXRButtonToggleVisuals : TXRButtonVisuals
{
    TXRButton_Toggle btnToggle;
    public override void Init(TXRButtonReferences references)
    {
        base.Init(references);
        btnToggle = GetComponent<TXRButton_Toggle>();
    }
    protected override void Hover()
    {
        SetHoverGradient(true);
        SetStrokeThickness(_configurations.strokeThicknessHover);

        if (btnToggle.ToggleState == TXRButtonToggleState.Off)
        {
            SetBackfaceZ(_configurations.backfadeZPositionHover);
        }
    }
 
}
