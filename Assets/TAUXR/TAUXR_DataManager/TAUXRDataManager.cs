using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TAUXRDataManager : MonoBehaviour
{
    #region Singelton Decleration
    private static TAUXRDataManager _instance;

    public static TAUXRDataManager Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion


    #region Events

    public ExampleEvent ExampleEvent;
    public CoinCollectedDataEventReporter CoinColldected;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void ConstructEvents()
    {
        ExampleEvent = new ExampleEvent();

        CoinColldected = new CoinCollectedDataEventReporter(TAUXRPlayer.Instance, gameManager);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SendEvent(DataEvent dataEvent)
    {

    }
}
