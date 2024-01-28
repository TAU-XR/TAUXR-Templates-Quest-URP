using UnityEngine;

// Determines the player's position and orientation in scenes.
public class PlayerScenePositioner : MonoBehaviour
{
    private void Awake()
    {
        // destroy the reference objcets when loading the scene
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
