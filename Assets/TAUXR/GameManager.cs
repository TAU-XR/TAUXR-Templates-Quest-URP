
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class GameManager : TXRSingleton<GameManager>
{
    [SerializeField] private bool _shouldCalibrateEnvironment;
    private TXRSceneManager _sceneManager;
    private EnvironmentCalibrator _calibrator;

    private void Start()
    {
        _calibrator = EnvironmentCalibrator.Instance;

        _sceneManager = TXRSceneManager.Instance;
        _sceneManager.Init();

      //  StartTAUXRExperience().Forget();
    }

    private async UniTask StartTAUXRExperience()
    {
        // for testing: remove later
        await _calibrator.CalibrateRoom();


        // calibrate environment only when checked and not in editor
        if (!Application.isEditor && _shouldCalibrateEnvironment)
        {
            // trigger calibration on BaseScene
            await _calibrator.CalibrateRoom();
        }
        _sceneManager.Init();
    }

}
