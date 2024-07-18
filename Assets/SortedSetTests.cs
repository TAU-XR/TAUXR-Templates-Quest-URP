using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortedSetTests : MonoBehaviour
{
    private SortedSet<SortedSetElement> _sortedSet = new();

    void Start()
    {
        bool added;
        foreach (SortedSetElement pinchable in GetComponentsInChildren<SortedSetElement>())
        {
            added = _sortedSet.Add(pinchable);
            Debug.Log(added);
            added = _sortedSet.Add(pinchable);
            Debug.Log(added);
            added = _sortedSet.Add(pinchable);
            Debug.Log(added);
            added = _sortedSet.Add(pinchable);
            Debug.Log(added);
            added = _sortedSet.Add(pinchable);
            Debug.Log(added);
            added = _sortedSet.Add(pinchable);
            Debug.Log(added);
        }

        foreach (SortedSetElement pinchable in GetComponentsInChildren<SortedSetElement>())
        {
            added = _sortedSet.Add(pinchable);
            Debug.Log(added);
            added = _sortedSet.Add(pinchable);
            Debug.Log(added);
            added = _sortedSet.Add(pinchable);
            Debug.Log(added);
            added = _sortedSet.Add(pinchable);
            Debug.Log(added);
            added = _sortedSet.Add(pinchable);
            Debug.Log(added);
            added = _sortedSet.Add(pinchable);
            Debug.Log(added);
        }

        foreach (SortedSetElement element in _sortedSet)
        {
            Debug.Log(element.Priority);
        }
    }
}