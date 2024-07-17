using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

public class DebugMode : MonoBehaviour
{
    private PinchingInputManager pinchingInputManager;

    private bool _waitingForPinchingInputsInARow;

    [Header("Debug mode state")] [SerializeField]
    private bool _inDebugMode;

    [SerializeField] private int _numberOfPinchesToEnterDebugMode = 3;

    [Header("Debug mode components")] [SerializeField]
    private bool _debugEyeData = true;

    [ShowIf("_debugEyeData")] [SerializeField]
    private EyeDataDebugger _eyeDataDebugger;

    [SerializeField] private bool _debugPlayerTransform = true;

    [ShowIf("_debugPlayerTransform")] [SerializeField]
    private PlayerTransformDebugger _playerTransformDebugger;

    private float _lastStateChangeTime;

    private const float MinimumTimeBetweenStateChanges = 1f;
    private bool _wasInDebugMode;


    private void Start()
    {
#if !UNITY_EDITOR
        _debugPlayerTransform = false;
#endif

        if (_debugEyeData)
        {
            _eyeDataDebugger.gameObject.SetActive(true);
        }

        pinchingInputManager = TXRPlayer.Instance.PinchingInputManager;
    }

    private void Update()
    {
        UpdateDebugModeState();

        bool leftDebugMode = _wasInDebugMode && !_inDebugMode;
        if (leftDebugMode)
        {
            _eyeDataDebugger.RevertChanges();
            _playerTransformDebugger.DebugPlayerTransform = false;
            _wasInDebugMode = false;
        }

        if (!_inDebugMode)
        {
            return;
        }

        UpdateDebugComponentsStates();
    }


    private void UpdateDebugModeState()
    {
        if (_lastStateChangeTime > Time.time - MinimumTimeBetweenStateChanges)
        {
            return;
        }

        if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift))
        {
            ToggleDebugModeState();
            return;
        }

        if (!pinchingInputManager.IsInputPressedThisFrame(HandType.Any) || _waitingForPinchingInputsInARow) return;

        _waitingForPinchingInputsInARow = true;
        HandType nextHand = pinchingInputManager.IsLeftHeld() ? HandType.Right : HandType.Left;
        pinchingInputManager.WaitForPressesInARow(_numberOfPinchesToEnterDebugMode, 1, ToggleDebugModeState,
            () => _waitingForPinchingInputsInARow = false, true, nextHand).Forget();
    }

    [Button]
    private void ToggleDebugModeState()
    {
        _lastStateChangeTime = Time.time;
        _waitingForPinchingInputsInARow = false;
        _wasInDebugMode = _inDebugMode;
        _inDebugMode = !_inDebugMode;

        string consoleMessage = _inDebugMode ? "Debug mode activated" : "Debug mode disabled";
        Debug.Log(consoleMessage);
    }

    private void UpdateDebugComponentsStates()
    {
        _eyeDataDebugger.DebugEyeData = _debugEyeData;
        _playerTransformDebugger.DebugPlayerTransform = _debugPlayerTransform;
    }
}