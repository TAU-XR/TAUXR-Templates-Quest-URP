using UnityEngine;

public class ChangeModelColorWhenInPlace : MonoBehaviour
{
    private Collider _targetCollider;
    private Material _materialToChange; // Assign the material using the custom shader
    private Renderer _renderer;
    private MaterialPropertyBlock _propertyBlock;
    public Color colorWhenInside = Color.blue;
    public Color colorWhenOutside = Color.green;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _materialToChange = _renderer.material;
        _propertyBlock = new MaterialPropertyBlock();
        _targetCollider = GetComponentInChildren<Collider>();
    }
    private void Update()
    {
        if (IsAnythingInside(_targetCollider))
        {
            //renderer.material.SetColor("_Fresnel_Color", colorWhenInside);
            _materialToChange.SetColor("_Fresnel_Color", colorWhenInside);
            GetComponent<Renderer>().material = _materialToChange;
        }
        else
        {
            _renderer.material.SetColor("_Fresnel_Color", colorWhenOutside);
            //materialToChange.SetColor("_Fresnel_Color", colorWhenOutside);
        }

    }

    private bool IsAnythingInside(Collider collider)
    {
        Bounds bounds = collider.bounds;
        Collider[] overlappingColliders = Physics.OverlapBox(bounds.center, bounds.extents, Quaternion.identity);
        return overlappingColliders.Length > 1;
    }

}
