using UnityEngine;
public class PointingUI : MonoBehaviour
{
    public Transform Background;
    public GameObject PointingPointPrefab;
    public GameObject PointerPrefab;

    private PointingPoint currentPointingPoint;
    private PointingPointer pointer;

    private Transform rightHand;
    private bool isCurrentlyPointing;

    void Start()
    {
        rightHand = TXRPlayer.Instance.RightHand;
        // Instantiate a pointer and get reference to its PointingPointer script
        pointer = CreatePointer(rightHand);

        // Create the pointing point initially and disable it
        GameObject pointingPointInstance = Instantiate(PointingPointPrefab);
        currentPointingPoint = pointingPointInstance.GetComponent<PointingPoint>();
        currentPointingPoint.SetAppear(false);
        isCurrentlyPointing = false;
    }

    void Update()
    {
        UpdatePointer(pointer);
    }

    void UpdatePointer(PointingPointer pointer)
    {
        if (pointer.IsPointingBackground(Background))
        {
            Vector3 hitPosition = pointer.GetHitPoint(Background);
            if (!isCurrentlyPointing)
            {
                currentPointingPoint.SetAppear(true);
                isCurrentlyPointing = true;
            }
            currentPointingPoint.UpdatePosition(hitPosition);
        }
        else
        {
            if (isCurrentlyPointing)
            {
                currentPointingPoint.SetAppear(false);
                isCurrentlyPointing = false;
            }
        }
    }

    PointingPointer CreatePointer(Transform parentTransform)
    {
        GameObject pointerInstance = Instantiate(PointerPrefab, parentTransform);
        return pointerInstance.GetComponent<PointingPointer>();
    }
}