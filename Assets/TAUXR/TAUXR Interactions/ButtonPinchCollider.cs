using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPinchCollider : MonoBehaviour
{
    ButtonPinch fatherBtn;
    private void Awake()
    {
        fatherBtn = GetComponentInParent<ButtonPinch>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PinchPoint"))
            fatherBtn.OnPinchPointEnter(other.GetComponent<PinchPoint>());
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PinchPoint"))
            fatherBtn.OnPinchPointExit(other.GetComponent<PinchPoint>());
    }
}
