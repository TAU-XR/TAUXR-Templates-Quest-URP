using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DebugMode : MonoBehaviour
{
    private PinchingInputManager pinchingInputManager;

    private bool _waitingForPinchingInputsInARow;
    [SerializeField] private bool _inDebugMode;
    [SerializeField] private int _numberOfPinchesToEnterDebugMode = 3;

    [SerializeField] private bool _debugEyeData = true;
    [SerializeField] private GameObject TextPopUpPrefab;

    private void Start()
    {
        pinchingInputManager = TXRPlayer.Instance.PinchingInputManager;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!pinchingInputManager.IsInputPressedThisFrame(HandType.Any) || _waitingForPinchingInputsInARow) return;
        _waitingForPinchingInputsInARow = true;
        HandType nextHand = pinchingInputManager.IsLeftHeld() ? HandType.Right : HandType.Left;
        pinchingInputManager.WaitForInputsInARow(_numberOfPinchesToEnterDebugMode, 1, ToggleDebugModeState,
            () => _waitingForPinchingInputsInARow = false, true, nextHand).Forget();
    }

    private void ToggleDebugModeState()
    {
        _waitingForPinchingInputsInARow = false;
        _inDebugMode = !_inDebugMode;
        Debug.Log("In debug mode = " + _inDebugMode);
    }
}