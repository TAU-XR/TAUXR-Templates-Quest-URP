using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using TMPro;

public class TXRButtonInput : MonoBehaviour
{
    public TXRButtonReferences references;
    public TXRButton _btn;
    public List<Transform> _touchers = new List<Transform>();
    public Transform _mainToucher;
    [Header("Configurations")]
    public float PressDistance = .01f;

    public bool isPressing = false;
    public bool isHovering = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Toucher")) return;

        _touchers.Add(other.transform);

        if (_touchers.Count == 1)
            _mainToucher = _touchers[0];
    }

    private void Update()
    {
        if (_mainToucher == null)  // no toucher around
        {
            if (isHovering)         // Hover Exit
            {
                print("HOVER EXIT");
                _btn.TriggerButtonEvent(ButtonEvent.HoverExit);
                isHovering = false;
            }
            return;
        }

        Transform buttonSurface = references.ButtonSurface;
        Vector3 closestPointOnBtn = GetClosestPointOnSurface(_mainToucher.position, buttonSurface, references.Backface.Width, references.Backface.Height);

        Vector3 toucherToBtn = _mainToucher.transform.position - closestPointOnBtn;
        float toucherDistance = toucherToBtn.magnitude;
        float dot = Vector3.Dot(toucherToBtn.normalized, references.ButtonSurface.forward);
        bool isToucherInFront = Vector3.Dot(toucherToBtn.normalized, references.ButtonSurface.forward) > 0;

        if (toucherDistance < PressDistance)
        {
            if (!isPressing && isHovering)        // Press
            {
                print("PRESS");
                isPressing = true;
                //_btn.PressTransform.Invoke(_mainToucher);
                _btn.TriggerButtonEvent(ButtonEvent.Pressed);
            }
        }
        else
        {
            if (isPressing)         // Release
            {
                if (!isToucherInFront) return;  // prevent press release when pressing the btn too "deep"

                print("RELEASE");
                isPressing = false;
                isHovering = false; // reset hovering to get hover enter after release
                _btn.TriggerButtonEvent(ButtonEvent.Released);
            }
            else if (!isHovering)       // Hover Enter
            {
                if (!isToucherInFront) return;  // prevent button activation from behind
                print("ENTER");
                _btn.TriggerButtonEvent(ButtonEvent.HoverEnter);
                isHovering = true;
            }
        }

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

    private Vector3 GetClosestPointOnSurface(Vector3 target, Transform pivot, float width, float height)
    {
        Vector3 localSpaceTargetPosition = pivot.InverseTransformPoint(target);

        float closestX = Mathf.Clamp(localSpaceTargetPosition.x, -width / 2f, width / 2f);
        float closestY = Mathf.Clamp(localSpaceTargetPosition.y, -height / 2f, height / 2f);
        float closestZ = 0;

        Vector3 localClosestPoint = new Vector3(closestX, closestY, closestZ);
        return pivot.TransformPoint(localClosestPoint);
    }
}