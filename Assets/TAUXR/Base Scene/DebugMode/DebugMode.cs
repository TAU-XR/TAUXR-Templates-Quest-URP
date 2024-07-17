using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

public class DebugMode : MonoBehaviour
{
    private PinchingInputManager pinchingInputManager;

    private bool _waitingForPinchingInputsInARow;
    [SerializeField] private bool _inDebugMode;
    [SerializeField] private int _numberOfPinchesToEnterDebugMode = 3;
    [SerializeField] private bool _debugEyeData = true;
    [SerializeField] private bool _debugMovement = true;
    [SerializeField] private EyeDataDebugger _eyeDataDebugger;
    private float _lastStateChangeTime;

    private const float MinimumTimeBetweenStateChanges = 1f;
    private bool _wasInDebugMode;


    private void Start()
    {
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
            _wasInDebugMode = false;
        }

        if (!_inDebugMode)
        {
            return;
        }

        if (_debugEyeData)
        {
            _eyeDataDebugger.DebugEyeData();
        }
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
}