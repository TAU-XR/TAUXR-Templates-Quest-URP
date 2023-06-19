using UnityEngine;

public class Pincher : MonoBehaviour
{
    public float Strength => _pinchStrength;


    public HandType HandT;
    private OVRSkeleton _ovrSkeleton;
    private int INDEX_I = 20, THUMB_I = 19;
    private float _pinchStrength;

    private float _pinchDistance;
    private float PINCH_MAX_DISTANCE = .01f;
    private float PINCH_MIN_DISTANCE = .0006f;

    // visuals - will be moved to shader
    private float _pinchSignLerpSpeed = 12f;
    private Material _pinchMaterial;
    [SerializeField] public Color _colorFullPinch;
    [SerializeField] public Color _colorNoPinch;
    private float _targetAlpha = 0;

    public void Init(OVRSkeleton ovrSkeleton)
    {
        _ovrSkeleton = ovrSkeleton;
        _pinchMaterial = GetComponent<MeshRenderer>().material;
        _colorFullPinch = _pinchMaterial.color;
    }

    public void UpdatePincher()
    {
        // set pinch position
        Vector3 indexPos = _ovrSkeleton.Bones[INDEX_I].Transform.position;
        Vector3 thumbPos = _ovrSkeleton.Bones[THUMB_I].Transform.position;
        Vector3 pincherTargetPosition = (thumbPos + indexPos) / 2;
        transform.position = pincherTargetPosition;

        // set pinch strength based on finger distance
        _pinchDistance = (indexPos - thumbPos).sqrMagnitude;
        _pinchStrength = Mathf.InverseLerp(PINCH_MAX_DISTANCE, PINCH_MIN_DISTANCE, _pinchDistance);

        SignPinchStrength(_pinchStrength);
    }

    private void SignPinchStrength(float pinchStr)
    {
        _targetAlpha = Mathf.Lerp(_targetAlpha, pinchStr, _pinchSignLerpSpeed * Time.deltaTime);

        _pinchMaterial.color = Color.Lerp(_colorNoPinch, _colorFullPinch, _targetAlpha);
    }
}
