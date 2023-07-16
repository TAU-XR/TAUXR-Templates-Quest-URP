
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : TXRSingleton<GameManager>
{
    [SerializeField] private bool _shouldCalibrateEnvironment;
    [SerializeField] private bool _shouldCalibrateOnEditor;
    private TXRSceneManager _sceneManager;
    private EnvironmentCalibrator _calibrator;

    private void Start()
    {
        _calibrator = EnvironmentCalibrator.Instance;
        _sceneManager = TXRSceneManager.Instance;

        StartTAUXRExperience().Forget();
    }

    private async UniTask StartTAUXRExperience()
    {
        // calibrate environment only when checked and not in editor
        if (!Application.isEditor && _shouldCalibrateEnvironment || _shouldCalibrateOnEditor && Application.isEditor)
        {
            // trigger calibration on BaseScene
            await _calibrator.CalibrateRoom();
        }

        _sceneManager.Init();
    }

}
