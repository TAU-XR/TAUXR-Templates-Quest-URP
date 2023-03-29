using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TAUXRDataEventReporter : MonoBehaviour
{

    public ExampleEvent ExampleEvent;

    void Start()
    {
        // inject dependencies here
        ExampleEvent = new ExampleEvent();
    }

    void Update()
    {

    }
}
