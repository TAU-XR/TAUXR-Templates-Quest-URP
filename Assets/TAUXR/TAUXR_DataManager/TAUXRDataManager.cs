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


    DataEventWriter eventWriter;

    #region Events

    //public ExampleEvent ExampleEvent;
    public WriteNoteDataEvent WriteNoteDataEvent;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        eventWriter = new DataEventWriter();
        ConstructEvents();
    }

    private void ConstructEvents()
    {
      //  ExampleEvent = new ExampleEvent();
        WriteNoteDataEvent = new WriteNoteDataEvent();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    public void SendEvent(DataEvent dataEvent)
    {
        eventWriter.WriteDataEventToFile(dataEvent);
    }

    IEnumerator test()
    {
        WriteNoteDataEvent.ReportNote("Lets see if that works");
        yield return new WaitForSeconds(1);
        WriteNoteDataEvent.ReportNote("maybe it does");
        yield return new WaitForSeconds(1.2f);
        WriteNoteDataEvent.ReportNote("yalla oved");


    }
}
