using UnityEngine;
using Shapes;
using DG.Tweening;

public class PointingPoint : MonoBehaviour
{
    public GameObject Circle;
    public float lerpSpeed = 15.0f;
    public float appearanceDuration = 0.25f;
    public bool IsPinching => TXRPlayer.Instance.HandRight.Pincher.Strength == 1;
    public void SetAppear(bool state)
    {
        Circle.SetActive(state);
    }

    public void UpdatePosition(Vector3 targetPosition)
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, lerpSpeed * Time.deltaTime);
    }
}