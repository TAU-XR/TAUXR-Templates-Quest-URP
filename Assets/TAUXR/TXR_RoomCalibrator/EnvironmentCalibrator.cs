using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class EnvironmentCalibrator : TXRSingleton<EnvironmentCalibrator>
{
    // gets 2 points on the room model.
    [SerializeField] private GameObject centerModel;
    [SerializeField] private Transform virtualReferencePoint1;
    [SerializeField] private Transform virtualReferencePoint2;
    [SerializeField] private GameObject calibrationMark;
    [SerializeField] private TXRButtonHold _btnConfirm;
    [SerializeField] private TXRButtonHold _btnRedo;
    [SerializeField] private GameObject _instructionsPinch;
    [SerializeField] private GameObject _instructionsMarkerPosition;
    [SerializeField] private GameObject _instructionsMarkerRotation;

    [SerializeField] private FollowTransform _playerMarkedPosition;  // a sphere following the exact position players mark when they pinch. Changes every frame according to players' hand.
    [SerializeField] private FollowTransform _rightHandInstructionsPivot;
    [SerializeField] private FollowTransform _leftHandInstructionsPivot;

    private float PINCH_HOLD_DURATION = 1f;

    // gets 2 points where player touches during the calibration action.
    private Vector3 realWorldReferencePoint1;
    private Vector3 realWorldReferencePoint2;

    private bool _shouldRedo, _shouldConfirm;
    private void Init()
    {
        centerModel.SetActive(false);

        // init buttons
        _btnConfirm.ButtonActivated.AddListener(OnConfirmCalibration);
        _btnRedo.ButtonActivated.AddListener(OnRedoCalibration);
        _btnConfirm.gameObject.SetActive(false);
        _btnRedo.gameObject.SetActive(false);

        // TODO: Apply offsets
        _playerMarkedPosition.Init(TXRPlayer.Instance.GetHandFingerCollider(HandType.Left, FingerType.Index));
        _rightHandInstructionsPivot.Init(TXRPlayer.Instance.RightHand);
        _leftHandInstructionsPivot.Init(TXRPlayer.Instance.LeftHand);

        // disable instructions
        _instructionsMarkerPosition.SetActive(false);
        _instructionsMarkerRotation.SetActive(false);
        _instructionsPinch.SetActive(false);

        _shouldConfirm = false;
        _shouldRedo = false;
    }


    public async UniTask CalibrateRoom()
    {
        Init();

        EnterCalibrationMode();

        // wait until getting 1st point
        await TXRPlayer.Instance.WaitForPinchHold(HandType.Right, PINCH_HOLD_DURATION, true);
        realWorldReferencePoint1 = _playerMarkedPosition.Position;
        Instantiate(calibrationMark, realWorldReferencePoint1, Quaternion.identity);

        DisplaySecondPointInstructions();

        // wait until getting 2st point
        await TXRPlayer.Instance.WaitForPinchHold(HandType.Right, PINCH_HOLD_DURATION, false);

        realWorldReferencePoint2 = _playerMarkedPosition.Position;
        Instantiate(calibrationMark, realWorldReferencePoint2, Quaternion.identity);

        // once we have 2 real world reference points - we can calibrate.
        AlignVirtualToPhysicalRoom();

        // show center model and disable passthrough
        centerModel.SetActive(true);

        DisplayConfirmationButtons();

        while (!_shouldRedo && !_shouldConfirm)
        {
            await UniTask.Yield();
        }
        if (_shouldRedo)
        {
            // we will call Calibrate Room again to redo the calibration. 
            _shouldRedo = false;
            await CalibrateRoom();
        }
        else
        {
            // if we got here it means _shouldConfirm = true and we can end the task.
            _shouldConfirm = false;
            EndCalibration();
        }
    }

    private void EnterCalibrationMode()
    {
        // hide all scene visuals ? how can I view only titles and get back to scene.
        // to begin with: activate only on experience start before first scene is loaded.

        // enable passthrough
        //TXRPlayer.Instance.SetPassthrough(true);

        // hide hands

        // display reference point in hand (so user have accurate place for point-markdown)
        _playerMarkedPosition.gameObject.SetActive(true);

        // display instruction text for position stage
        _instructionsMarkerPosition.SetActive(true);
        _instructionsPinch.SetActive(true);
    }

    private void DisplaySecondPointInstructions()
    {
        // hide previous instructions
        _instructionsMarkerPosition.SetActive(false);

        // display new instructions
        _instructionsMarkerRotation.SetActive(true);
    }

    private void DisplayConfirmationButtons()
    {
        // remove previous instructions
        _instructionsMarkerRotation.SetActive(false);
        _instructionsPinch.SetActive(false);

        // show buttons
        _btnConfirm.gameObject.SetActive(true);
        _btnRedo.gameObject.SetActive(true);
    }

    private void EndCalibration()
    {
        // hide marker
        _playerMarkedPosition.gameObject.SetActive(false);

        // hide buttons
        _btnConfirm.gameObject.SetActive(false);
        _btnRedo.gameObject.SetActive(false);
    }

    // called when the redo calibration hold button is touched
    private void OnRedoCalibration()
    {
        _shouldRedo = true;
    }

    // called when the confirm calibration hold button is touched
    private void OnConfirmCalibration()
    {
        _shouldConfirm = true;
    }

    public void AlignVirtualToPhysicalRoom()
    {
        // ignore differences on height
        realWorldReferencePoint2.y = realWorldReferencePoint1.y;

        Vector3 realDirection = (realWorldReferencePoint2 - realWorldReferencePoint1).normalized;
        Vector3 virtualDirection = (virtualReferencePoint2.position - virtualReferencePoint1.position).normalized;

        float angle = Vector3.Angle(virtualDirection, realDirection);
        Vector3 centerRotation = centerModel.transform.eulerAngles;
        centerRotation.y += angle;
        Vector3 positionOffset = realWorldReferencePoint1 - virtualReferencePoint1.position;

        centerModel.transform.eulerAngles = centerRotation;
        //Quaternion rotation = Quaternion.FromToRotation(virtualDirection, realDirection);

        //centerModel.transform.RotateAround(centerModel.transform.position, centerModel.transform.up, rotation.eulerAngles.y);


        centerModel.transform.position += positionOffset;

        /*// Calculate the directions in real world and virtual world
        Vector3 realDirection = (realWorldReferencePoint2 - realWorldReferencePoint1).normalized;
        Vector3 virtualDirection = (virtualReferencePoint2.position - virtualReferencePoint1.position).normalized;

        // Calculate the rotation required to align player's view with real world
        Quaternion rotation = Quaternion.FromToRotation(realDirection, virtualDirection);

        // Rotate the player around the first reference point
        TXRPlayer.Instance.transform.RotateAround(TXRPlayer.Instance.transform.position, TXRPlayer.Instance.transform.up, rotation.eulerAngles.y);

        // Calculate the position offset after rotation
        Vector3 positionOffset = virtualReferencePoint1.position - realWorldReferencePoint1;

        // Apply the position offset to the player
        TXRPlayer.Instance.transform.position += positionOffset;*/
    }
}
