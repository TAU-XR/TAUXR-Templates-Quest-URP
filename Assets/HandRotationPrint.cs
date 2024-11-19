using System.Collections.Generic;
using UnityEngine;

public class HandRotationPrint : MonoBehaviour
{
    public TMPro.TextMeshPro columnsText;
    public TMPro.TextMeshPro dataText;

    Transform t;
    TXRPlayer player;
    List<string> columnNames = new List<string>();
    List<string> data = new List<string>();
    Transform Head;
    Transform RightHand;
    Transform LeftHand;
    Transform[] transformsToRecord;

    // Start is called before the first frame update
    void Start()
    {
        player = TXRPlayer.Instance;
        Head = player.PlayerHead;
        RightHand = player.RightHand;
        LeftHand = player.LeftHand;

        transformsToRecord = new Transform[] { Head, RightHand, LeftHand };


        foreach (Transform t in transformsToRecord)
        {

            columnNames.Add(t.name + "_Position_X");
            columnNames.Add(t.name + "_Height");
            columnNames.Add(t.name + "_Position_Z");
            columnNames.Add(t.name + "_Pitch");
            columnNames.Add(t.name + "_Yaw");
            columnNames.Add(t.name + "_Roll");
        }

        string columns = (string.Join("\n", columnNames));
        columnsText.text = columns;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Transform t in transformsToRecord)
        {

            data.Add(t.position.x.ToString("F0"));
            data.Add(t.position.y.ToString("F0"));
            data.Add(t.position.z.ToString("F0"));
            data.Add(t.eulerAngles.x.ToString("F0"));
            data.Add(t.eulerAngles.y.ToString("F0"));
            data.Add(t.eulerAngles.z.ToString("F0"));
        }

        string dataCombined = (string.Join("\n", data));
        dataText.text = dataCombined;
        data.Clear();
    }
}
