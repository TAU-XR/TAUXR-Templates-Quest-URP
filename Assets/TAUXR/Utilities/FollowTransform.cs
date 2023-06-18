using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField] Vector3 _offset;
    void Start()
    {
        
    }

    void Update()
    {
        if(_target != null)
        {
            transform.position = _target.position + _offset;
        }
    }

    public void Init(Transform target, Vector3 offset = default(Vector3))
    {
        _target= target;
        _offset = offset;
    }

    public Vector3 Position => transform.position;
    public Quaternion Rotation => transform.rotation;
}
