using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonPinch : MonoBehaviour
{
    public TextMeshPro btnText;
    PinchPoint activePinchPoint;
    bool isPinching;
    void Start()
    {
        btnText.text = "Pinch Button";    
    }

    void Update()
    {
        if(activePinchPoint!=null)
        {
            if (activePinchPoint.IsPinching())
                btnText.text = "Pinch Detected!";
            else
                btnText.text = "Hand in Place";
        }
        else
            btnText.text = "Waiting for hand";

    }

    public void PinchStarted()
    {

    }

    public void PinchEnded()
    {

    }

    public void OnPinchPointEnter(PinchPoint pp)
    {
        if (activePinchPoint == null)
            activePinchPoint = pp;

    }

    public void OnPinchPointExit(PinchPoint pp)
    {
        if (activePinchPoint == pp)
            activePinchPoint = null;

    }
}
