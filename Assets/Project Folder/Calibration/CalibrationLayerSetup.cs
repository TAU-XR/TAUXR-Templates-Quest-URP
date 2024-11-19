using UnityEditor;
using UnityEngine;

public class CalibrationLayerSetup : MonoBehaviour
{
    public static string layerName = "Calibration";
    static int layerIndex = 30;

    [InitializeOnLoadMethod]
    static void EnsureCalibrationLayer()
    {

        if (LayerMask.NameToLayer(layerName) == -1)
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layersProp = tagManager.FindProperty("layers");

            if (layersProp != null && layersProp.GetArrayElementAtIndex(layerIndex).stringValue == "")
            {
                layersProp.GetArrayElementAtIndex(layerIndex).stringValue = layerName;
                tagManager.ApplyModifiedProperties();
                Debug.Log("Calibration layer added!");
            }
        }
    }

    public static void SetCalibrationLayerRecursively(GameObject obj)
    {
        int layerIndex = LayerMask.NameToLayer(layerName);
        if (layerIndex == -1)
        {
            Debug.Log("Trying to assign calibration layer but it doesnt exist");
        }
        // Set the object's layer
        obj.layer = layerIndex;

        // Recursively set the layer for each child object
        foreach (Transform child in obj.transform)
        {
            SetCalibrationLayerRecursively(child.gameObject);
        }
    }
}
