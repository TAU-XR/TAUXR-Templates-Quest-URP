using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PlacementPoint : MonoBehaviour
{
    public bool ContainsObject { get; private set; }

    public void PlaceObject(Transform objectToPlace)
    {
        objectToPlace.parent = transform;
        objectToPlace.position = transform.position;
        objectToPlace.rotation = transform.rotation;
        ContainsObject = true;
    }

    public void RemoveObject()
    {
        ContainsObject = false;
    }
}