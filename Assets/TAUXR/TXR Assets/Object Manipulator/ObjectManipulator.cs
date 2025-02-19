using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ManipulatorState { Idle, Rotation, Scale }

public class ObjectManipulator : MonoBehaviour
{
    public ManipulatorState State;

    private const float BASE_ACTIVATION_DISTANCE = .05f; // Pinching should be done within this radius of object's center.
    private float _currentActivationDistance = BASE_ACTIVATION_DISTANCE; // Updates according to scale factor.

    private Vector3 _initialPincherToPivot;
    private Pincher _rightPincher;
    private Pincher _leftPincher;
    [SerializeField] private float _rotationLerpSpeed = 7f;
    [SerializeField] private float _scaleLerpSpeed = 7f;

    private Vector3 _scaleOnScaleStart = Vector3.one;
    private float _pinchersDistanceOnScaleStart = 0;
    public float _scaleMultiplier = 1f;
    public float MAXSCALE = 2f;
    public float MINSCALE = .5f;
    public float _distanceThreshold = .1f;

    private Quaternion _lastRotation = Quaternion.identity;
    private float _rotationSpeed = 0; // In angles

    private Pincher _rotationPincher;
    private Pincher _scalePincher;

    private bool _pincherInCollider = false;
    private bool _readyForPinch = false;

    [SerializeField] private float rotationEndStrengthThreshold = 0.98f; // Threshold for ending rotation

    private void OnTriggerEnter(Collider other)
    {
        Pincher pincher = other.GetComponent<Pincher>();
        if (pincher != null)
        {
            _pincherInCollider = true;
            _readyForPinch = pincher.Strength < 0.98f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Pincher pincher = other.GetComponent<Pincher>();
        if (pincher != null)
        {
            _pincherInCollider = false;
            _readyForPinch = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Pincher pincher = other.GetComponent<Pincher>();
        if (pincher != null)
        {
            _readyForPinch = pincher.Strength < 0.98f;
        }
    }

    void Start()
    {
        _rightPincher = TXRPlayer.Instance.HandRight.Pincher;
        _leftPincher = TXRPlayer.Instance.HandLeft.Pincher;

        State = ManipulatorState.Idle;
    }

    void Update()
    {
        switch (State)
        {
            case ManipulatorState.Idle:
                if (_rotationSpeed > 0)
                {
                    Decay();
                }
                WaitForRotationPincher();
                break;

            case ManipulatorState.Rotation:
                RotateTowardsPincher();
                EvaluateScaling();
                HandleRotationEnding();
                break;

            case ManipulatorState.Scale:
                Scale();
                HandleScaleEnding();
                break;
        }
    }

    private void Decay()
    {
        // Decay logic here
    }

    private void WaitForRotationPincher()
    {
        if (ShouldBecomeRotationPincher(_rightPincher))
        {
            SetRotationPincher(_rightPincher);
        }
        else if (ShouldBecomeRotationPincher(_leftPincher))
        {
            SetRotationPincher(_leftPincher);
        }
    }

    private void SetRotationPincher(Pincher targetRotationPincher)
    {
        _rotationPincher = targetRotationPincher;
        _scalePincher = _rotationPincher == _rightPincher ? _leftPincher : _rightPincher;
        ResetRotationPincher();
        State = ManipulatorState.Rotation;
    }

    private bool ShouldBecomeRotationPincher(Pincher pincher)
    {
        return pincher.Strength >= 1 && _readyForPinch;
    }

    private void ResetRotationPincher()
    {
        _initialPincherToPivot = (_rotationPincher.transform.position - transform.position).normalized;
        _initialPincherToPivot = transform.InverseTransformDirection(_initialPincherToPivot).normalized;
    }

    private void RotateTowardsPincher()
    {
        Vector3 currentPincherToPivot = (_rotationPincher.transform.position - transform.position).normalized;
        currentPincherToPivot = transform.InverseTransformDirection(currentPincherToPivot);

        Quaternion targetRotation = Quaternion.FromToRotation(_initialPincherToPivot, currentPincherToPivot);
        targetRotation = transform.localRotation * targetRotation;

        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, _rotationLerpSpeed * Time.deltaTime);
    }

    private void EvaluateScaling()
    {
        bool shouldStartScaling = _scalePincher.Strength >= 1 && (_scalePincher.transform.position - transform.position).sqrMagnitude <= _currentActivationDistance;
        if (shouldStartScaling)
        {
            _pinchersDistanceOnScaleStart = Vector3.Distance(_rightPincher.transform.position, _leftPincher.transform.position);
            _scaleOnScaleStart = transform.localScale;
            State = ManipulatorState.Scale;
        }
    }

    private void HandleRotationEnding()
    {
        if (_rotationPincher.Strength < rotationEndStrengthThreshold)
        {
            _rotationPincher = null;
            State = ManipulatorState.Idle;
        }
    }

    private void Scale()
    {
        float currentDistance = Vector3.Distance(_rightPincher.transform.position, _leftPincher.transform.position);
        float scaleMultiplier = currentDistance / _pinchersDistanceOnScaleStart * _scaleMultiplier;

        Vector3 targetScale = _scaleOnScaleStart * scaleMultiplier;
        targetScale = ClampVector(targetScale, MINSCALE, MAXSCALE);

        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, _scaleLerpSpeed * Time.deltaTime);
        float activationDistanceMultiplier = transform.localScale.x / 1; // Assuming start scale is 1, and that scale on all axis is the same.
        _currentActivationDistance = BASE_ACTIVATION_DISTANCE * activationDistanceMultiplier;
    }

    private Vector3 ClampVector(Vector3 v, float min, float max)
    {
        if (v.x < min)
        {
            v.x = min; v.y = min; v.z = min;
        }
        if (v.x > max)
        {
            v.x = max; v.y = max; v.z = max;
        }
        return v;
    }

    private void HandleScaleEnding()
    {
        Pincher nonPinchingPincher = GetNonPinchingPincher(_scalePincher, _rotationPincher);
        if (nonPinchingPincher != null)
        {
            SetRotationPincher(nonPinchingPincher);
        }
    }

    private Pincher GetNonPinchingPincher(Pincher pincher1, Pincher pincher2)
    {
        if (pincher1.Strength < 1) return pincher1;
        if (pincher2.Strength < 1) return pincher2;
        return null;
    }
}
