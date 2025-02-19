using UnityEngine;

public class PointingPointer : MonoBehaviour
{
    public Transform Pointer;

    public bool IsPointingBackground(Transform background)
    {
        if (Pointer == null) return false;

        RaycastHit hit;
        Ray pointerRay = new Ray(Pointer.position, Pointer.forward);
        if (Physics.Raycast(pointerRay, out hit, Mathf.Infinity, LayerMask.GetMask("PUIBackground")))
        {
            Debug.Log("Wallak  " + hit.transform.name);

            return hit.transform == background;
        }
        Debug.Log("Wallak No hit");
        return false;
    }

    public Vector3 GetHitPoint(Transform background)
    {
        if (IsPointingBackground(background))
        {
            RaycastHit hit;
            Ray pointerRay = new Ray(Pointer.position, Pointer.forward);
            if (Physics.Raycast(pointerRay, out hit, Mathf.Infinity, LayerMask.GetMask("PUIBackground")))
            {
                return hit.point;
            }
        }
        return Vector3.zero;
    }
}
