using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCollider : MonoBehaviour
{
    public ButtonColliderType Type;
    public ButtonFingerTouch FatherButton;

    private void OnTriggerEnter(Collider other)
    {
        FatherButton.ButtonColliderEnter(Type, other);
    }

    private void OnTriggerExit(Collider other)
    {
        FatherButton.ButtonColliderExit(Type, other);
    }
}
