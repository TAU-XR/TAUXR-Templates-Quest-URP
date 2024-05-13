using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class TXRButtonInput : MonoBehaviour
{
    public TXRButtonReferences references;
    public TXRButton _btn;
    public List<Transform> _touchers = new List<Transform>();
    public Transform _mainToucher;

    [Header("Configurations")]
    public float PressDistance;

    public bool isPressing = false;
    public bool isHovering = false;

    private void Update()
    {
        if (_mainToucher == null)
        {
            if (isHovering)
            {
                // hover exit
                _btn.TriggerButtonEvent(ButtonEvent.HoverExit);
                isHovering = false;
            }
            return;
        }

        float toucherDistance = Vector3.Distance(_mainToucher.position, references.ButtonSurface.position);
        if (toucherDistance > PressDistance)
        {
            if (isPressing)
            {
                // release
                isPressing = false;
                _btn.TriggerButtonEvent(ButtonEvent.Released);
            }
            else if (!isHovering)
            {
                // hover enter
                _btn.TriggerButtonEvent(ButtonEvent.HoverEnter);
                isHovering = true;
            }
        }
        else
        {
            if (!isPressing)
            {
                // press
                isPressing = true;
                //_btn.PressTransform.Invoke(_mainToucher);
                _btn.TriggerButtonEvent(ButtonEvent.Pressed);
            }
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
        if (!other.CompareTag("Toucher")) return;
        _touchers.Remove(other.transform);

        if (_touchers.Count > 0)
        {
            _mainToucher = _touchers.Last();
        }
        else
        {
            _mainToucher = null;
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
}