using System.Collections.Generic;
using UnityEngine;

public class DataEvent
{
    public string EventName => eventName;
    private readonly string eventName;

    public Dictionary<string, string> Fields => fields;
    private readonly Dictionary<string, string> fields;


    public DataEvent(string eventName)
    {
        this.eventName = eventName;
        fields = new Dictionary<string, string>();
        fields.Add("TimeFromAppLaunch", Time.time.ToString());
    }
}
public interface IEventReporter
{
    // add all relevant fields to dataEvent.
    void SetupDataEvent();

    // update dataEvent to reflect current variables values serialized to strings.
    void UpdateDataEvent();

    // tell TAUXRDataManager to write our dataEvent to file.
    void Report();
}

public class ExampleEvent : IEventReporter
{
    TAUXRDataManager dataManager = TAUXRDataManager.Instance;
    private DataEvent dataEvent;

    // include all event dependencies in its constructor function parameters.
    public ExampleEvent(/* declare all needed dependencies here */ )
    {
        // get all neccessary dependencies to reach desired data to write
        // i.e this.gameManager = gameManager;

        SetupDataEvent();
    }
    public void SetupDataEvent()
    {
        dataEvent = new DataEvent("ExampleEvent");

    }

    public void UpdateDataEvent()
    {

    }
    public void Report()
    {
        dataEvent.Fields["PlayerPosition"] = TAUXRPlayer.Instance.PlayerHead.position.ToString();
        dataManager.SendEvent(dataEvent);
    }
}

public class CoinCollectedDataEventReporter : IEventReporter
{
    TAUXRDataManager dataManager = TAUXRDataManager.Instance;
    private DataEvent dataEvent;

    TAUXRPlayer player;
    GameManager gameManager;
    public CoinCollectedDataEventReporter(TAUXRPlayer player, GameManager manager)
    {
        this.player = player;
        this.gameManager = manager;

        SetupDataEvent();
    }

    public  void SetupDataEvent()
    {
        dataEvent = new DataEvent("DataEvent_CoinCollected");
        dataEvent.Fields.Add("Trial_Time", "");
        dataEvent.Fields.Add("Coins_Collected", "");
        dataEvent.Fields.Add("PlayerPosition_X", "");
        dataEvent.Fields.Add("PlayerPosition_Y", "");
        dataEvent.Fields.Add("PlayerPosition_Z", "");
    }

    public void UpdateDataEvent()
    {
        dataEvent.Fields["Trial_Time"] = gameManager.TrialTime.ToString();
        dataEvent.Fields["Coins_Collected"] = gameManager.CoinsCollected.ToString();
        dataEvent.Fields["PlayerPosition_X"] = player.PlayerHead.position.x.ToString();
        dataEvent.Fields["PlayerPosition_Y"] = player.PlayerHead.position.y.ToString();
        dataEvent.Fields["PlayerPosition_Z"] = player.PlayerHead.position.z.ToString();

    }

    public void Report()
    {
        UpdateDataEvent();
        dataManager.SendEvent(dataEvent);
    }
}



