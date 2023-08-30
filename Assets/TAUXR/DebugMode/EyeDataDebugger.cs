using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeDataDebugger : MonoBehaviour
{
    [SerializeField] private Material _focusedObjectMaterial;
    [SerializeField] private Transform _eyeHitPositionSphere;
    [SerializeField] private GameObject _textPopUp;

    private GameObject _previousFocusedObject;
    private Material _previousFocusedObjectPreviousMaterial;

    public void DebugEyeData()
    {
        //TODO: refactor
        GameObject focusedObject = TXRPlayer.Instance.EyeTracker.FocusedObject.gameObject;

        if (focusedObject != null)
        {
            if (focusedObject.tag.Equals("PinchPoint") || focusedObject.tag.Equals("Toucher"))
            {
                return;
            }

            if (_previousFocusedObject == null)
            {
                //TODO: extract to method
                UpdateTextPopUp(focusedObject.transform);
                _previousFocusedObjectPreviousMaterial = focusedObject.GetComponent<MeshRenderer>().material;
                _previousFocusedObject = focusedObject.gameObject;
                focusedObject.GetComponent<MeshRenderer>().material = _focusedObjectMaterial;
                _eyeHitPositionSphere.gameObject.SetActive(true);
            }

            _eyeHitPositionSphere.position = TXRPlayer.Instance.EyeTracker.EyeGazeHitPosition;
        }
        else if (_previousFocusedObject != null && focusedObject == null)
        {
            RevertPreviousFocusedObject();
        }
    }

    private void UpdateTextPopUp(Transform focusedObject)
    {
        //TODO: Make it work with a non convex mesh collider
        _textPopUp.SetActive(true);
        Collider focusedObjectCollider = focusedObject.GetComponent<Collider>();
        _textPopUp.transform.position = new Vector3(
            focusedObjectCollider.ClosestPoint(TXRPlayer.Instance.EyeTracker.EyePosition).x,
            focusedObjectCollider.bounds.max.y + 0.1f,
            focusedObjectCollider.ClosestPoint(TXRPlayer.Instance.EyeTracker.EyePosition).z);
        _textPopUp.transform.LookAt(TXRPlayer.Instance.EyeTracker.EyePosition);
        _textPopUp.transform.eulerAngles = new Vector3(0,
            _textPopUp.transform.eulerAngles.y + 90, 0);
        _textPopUp.GetComponent<TextPopUp>().SetTextAndScale(focusedObject.name);
    }

    private void RevertPreviousFocusedObject()
    {
        _previousFocusedObject.GetComponent<MeshRenderer>().material = _previousFocusedObjectPreviousMaterial;
        _eyeHitPositionSphere.gameObject.SetActive(false);
        _previousFocusedObject = null;
    }

    public void RevertChanges()
    {
        if (_previousFocusedObject != null)
        {
            RevertPreviousFocusedObject();
        }

        _textPopUp.SetActive(false);
    }
}