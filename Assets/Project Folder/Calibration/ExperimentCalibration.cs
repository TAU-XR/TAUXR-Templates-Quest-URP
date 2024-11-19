
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ExperimentCalibration : TXRSingleton<ExperimentCalibration>
{

    TXRPlayer player;
    private LineRenderer lineRenderer;
    public float rayLength;
    public TextMeshPro instructionsText;
    public TXRButton approveButton;

    void Start()
    {
        player = TXRPlayer.Instance;

        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("No LineRenderer found on the object.");
            return;
        }


        lineRenderer.positionCount = 2;
        lineRenderer.SetWidth(0.02f, 0.02f);

        instructionsText.gameObject.SetActive(false);
        CalibrationLayerSetup.SetCalibrationLayerRecursively(gameObject);
    }

    void Update()
    {
        // Draw a line from the player's head forward to help with recenter
        Transform PlayerHead = player.PlayerHead.transform;
        Vector3 p1 = PlayerHead.position;
        Vector3 p2 = PlayerHead.position + PlayerHead.forward.normalized * rayLength;

        Vector3 perpendicularDirection = Vector3.Cross(p2 - p1, -PlayerHead.right).normalized;
        if (perpendicularDirection == Vector3.zero)
        {
            perpendicularDirection = Vector3.Cross(p2 - p1, -PlayerHead.forward).normalized;
        }
        perpendicularDirection *= 0.5f;

        Vector3 p3 = p2 + perpendicularDirection;


        lineRenderer.SetPosition(0, p2);
        lineRenderer.SetPosition(1, p3);


    }

    public async UniTask WaitForCalibration()
    {
        HideScene();
        await ShowInstructions();
        await WaitForApproveButtonPress();
        ShowScene();
        TXRDataManager.Instance.LogLineToFile("Calibration approved");
        //should destroy or hide?
        Destroy(gameObject);
    }

    private void HideScene()
    {
        // Define the target layer name
        string calibrationLayerName = CalibrationLayerSetup.layerName;

        // Get the layer index for the calibration layer
        int calibrationLayerIndex = LayerMask.NameToLayer(calibrationLayerName);

        // Check if the layer exists
        if (calibrationLayerIndex == -1)
        {
            Debug.LogError($"Layer '{calibrationLayerName}' does not exist. Please add it first.");
            return;
        }

        // Create a mask for only the calibration layer
        int calibrationLayerMask = 1 << calibrationLayerIndex;

        // Set the camera's culling mask to only render the calibration layer
        Camera.main.cullingMask = calibrationLayerMask;
    }

    private void ShowScene()
    {
        // Set the camera's culling mask to render all layers
        Camera.main.cullingMask = -1;
    }

    private async UniTask ShowInstructions()
    {
        instructionsText.gameObject.SetActive(true);
        await UniTask.Delay(300);
        instructionsText.gameObject.SetActive(false);
    }

    private async UniTask WaitForApproveButtonPress()
    {
        await approveButton.WaitForButtonPress();
    }


}
