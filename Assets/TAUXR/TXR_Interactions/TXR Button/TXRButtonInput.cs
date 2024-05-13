using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TXRButtonInput : MonoBehaviour
{
    public TXRButtonReferences references;
    public TXRButton _btn;
    public List<Transform> _touchers = new();
    public Transform _mainToucher;

    [Header("Configurations")]
    public float PressDistance;

    public bool isPressing = false;

    private void Update()
    {
        if (_mainToucher == null)
        {
            // hover exit
            _btn.OnHoverExit(_mainToucher);
            return;
        }

        float toucherDistance = Vector3.Distance(_mainToucher.position, references.ButtonSurface.position);
        if (toucherDistance > PressDistance)
        {
            if (isPressing)
            {
                // release
                isPressing = false;
                _btn.OnReleased(_mainToucher);
            }
            else
            {
                // hover enter
                _btn.OnHoverEnter(_mainToucher);
            }
        }
        else
        {
            isPressing = true;
            // press
            _btn.OnPressed(_mainToucher);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Toucher")) return;

        _touchers.Add(other.transform);

        if (_touchers.Count == 1)
            _mainToucher = _touchers[0];
    }

    private void OnTriggerExit(Collider other)
    {

    }

}

public class ButtonToucher
{
    public float DistanceToButton;
    private Transform _transform;
    public Vector3 Position => _transform.position;
    public ButtonToucher(Transform toucherTransform)
    {
        _transform = toucherTransform;
    }
}