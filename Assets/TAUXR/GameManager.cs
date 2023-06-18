
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class GameManager : TAUXRSingleton<GameManager>
{
    [SerializeField] private bool _shouldCalibrateEnvironment;
    private TAUXRSceneManager _sceneManager;
    private EnvironmentCalibrator _calibrator;

    private void Start()
    {
        _calibrator = EnvironmentCalibrator.Instance;

        _sceneManager = TAUXRSceneManager.Instance;
        _sceneManager.Init();
    }

    private async UniTask StartTAUXRExperience()
    {
        // calibrate environment only when checked and not in editor
        if(!Application.isEditor && _shouldCalibrateEnvironment)
        {
            // trigger calibration on BaseScene
            await _calibrator.CalibrateRoom();
        }
        _sceneManager.Init();
    }

}
