using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortedSetTests : MonoBehaviour
{
    private SortedSet<SortedSetElement> _sortedSet = new();

    void Start()
    {
        foreach (SortedSetElement pinchable in GetComponentsInChildren<SortedSetElement>())
        {
            _sortedSet.Add(pinchable);
        }

        foreach (SortedSetElement pinchable in GetComponentsInChildren<SortedSetElement>())
        {
            if (_sortedSet.Add(pinchable))
            {
                Debug.Log("Wrong!");
            }
        }


        foreach (SortedSetElement element in _sortedSet)
        {
            Debug.Log(element.Priority);
        }
    }
}