
using UnityEngine;

public class ExperimentCalibration : MonoBehaviour
{

    TXRPlayer player;
    private LineRenderer lineRenderer;
    public float rayLength;

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




}
